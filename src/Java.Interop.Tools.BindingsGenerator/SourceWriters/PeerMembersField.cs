using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

public class PeerMembersField : FieldWriter
{
	// static readonly JniPeerMembers _members = new XAPeerMembers ("android/provider/ContactsContract$AggregationExceptions", typeof (AggregationExceptions));
	public PeerMembersField (string rawJniType, string declaringType, bool isInterface)
	{
		Name = "_members";
		Type = new TypeReferenceWriter ("JniPeerMembers");

		IsPrivate = isInterface;
		IsStatic = true;
		IsReadonly = true;

		Value = $"new JniPeerMembers (\"{rawJniType}\", typeof ({declaringType}){(isInterface ? ", isInterface: true" : string.Empty)})";
	}

	public static PeerMembersField Create (TypeDefinition type, GeneratorSettings settings)
	{
		// TODO: Handle generics correctly
		var t = type.HasGenericParameters ? "Java.Lang.Object" : type.GetManagedName (settings);
		return new PeerMembersField (type.FullNameGenericsErased.Replace ('.', '/'), t, type.IsInterface);
	}
}

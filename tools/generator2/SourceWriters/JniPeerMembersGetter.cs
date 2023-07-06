using Xamarin.SourceWriter;

namespace generator2;

public class JniPeerMembersGetter : PropertyWriter
{
	// [DebuggerBrowsable (DebuggerBrowsableState.Never)]
	// [EditorBrowsable (EditorBrowsableState.Never)]
	// public override global::Java.Interop.JniPeerMembers JniPeerMembers {
	//   get { return _members; }
	// }
	public JniPeerMembersGetter ()
	{
		Name = "JniPeerMembers";
		PropertyType = new TypeReferenceWriter ("global::Java.Interop.JniPeerMembers");
		IsPublic = true;
		IsOverride = true;
		UseGetterExpressionBody = true;

		Attributes.Add (new DebuggerBrowsableAttributeWriter ());
		Attributes.Add (new EditorBrowsableAttributeWriter ());

		HasGet = true;
		GetBody.Add ("_members;");
	}
}

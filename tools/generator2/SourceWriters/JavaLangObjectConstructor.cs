using Javil;
using Xamarin.SourceWriter;

namespace generator2;

public class JavaLangObjectConstructor : ConstructorWriter
{
	public JavaLangObjectConstructor (TypeDefinition klass)
	{
		Name = klass.GetName ();

		if (klass.IsFinal)
			IsInternal = true;
		else
			IsProtected = true;

		Parameters.Add (new MethodParameterWriter ("reference", new TypeReferenceWriter ("ref JniObjectReference")));
		Parameters.Add (new MethodParameterWriter ("options", new TypeReferenceWriter ("JniObjectReferenceOptions")));

		BaseCall = "base (ref reference, options)";
	}
}

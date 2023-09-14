using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

public class JavaLangObjectConstructor : ConstructorWriter
{
	public JavaLangObjectConstructor (TypeDefinition klass, GeneratorSettings settings)
	{
		Name = klass.GetManagedName (settings);

		if (klass.IsFinal)
			IsInternal = true;
		else
			IsProtected = true;

		Parameters.Add (new MethodParameterWriter ("reference", new TypeReferenceWriter ("ref JniObjectReference")));
		Parameters.Add (new MethodParameterWriter ("options", new TypeReferenceWriter ("JniObjectReferenceOptions")));

		BaseCall = "base (ref reference, options)";
	}
}

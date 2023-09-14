using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class GenericClassAlternative : ClassWriter
{
	public static GenericClassAlternative Create (TypeDefinition type, GeneratorSettings settings)
	{
		var t = new GenericClassAlternative {
			Name = type.GetManagedName (settings),
			IsAbstract = true
		};

		if (type.IsPublic)
			t.IsPublic = true;
		else if (type.IsProtected)
			t.IsProtected = true;

		return t;
	}
}
using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class GenericInterfaceAlternative : InterfaceWriter
{
	public static GenericInterfaceAlternative Create (TypeDefinition type, GeneratorSettings settings)
	{
		var t = new GenericInterfaceAlternative {
			Name = type.GetManagedName (settings),
			IsPublic = true
		};

		return t;
	}
}

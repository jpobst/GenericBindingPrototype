using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator;

class GenericInterfaceAlternative : InterfaceWriter
{
	public static GenericInterfaceAlternative Create (TypeDefinition type)
	{
		var t = new GenericInterfaceAlternative {
			Name = type.GetName (),
			IsPublic = true
		};

		return t;
	}
}

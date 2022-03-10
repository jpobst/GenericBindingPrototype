using Javil;
using Xamarin.SourceWriter;

namespace generator2;

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

using Javil;
using Xamarin.SourceWriter;

namespace generator2;

class GenericClassAlternative : ClassWriter
{
	public static GenericClassAlternative Create (TypeDefinition type)
	{
		var t = new GenericClassAlternative {
			Name = type.GetName (),
			IsAbstract = true
		};

		if (type.IsPublic)
			t.IsPublic = true;
		else if (type.IsProtected)
			t.IsProtected = true;

		return t;
	}
}

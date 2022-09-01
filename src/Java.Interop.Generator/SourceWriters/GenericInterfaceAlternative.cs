using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator;

class GenericInterfaceAlternative : InterfaceWriter, IManagedTypeModel
{
	public ManagedNamespaceModel? Namespace { get; set; }

	public static GenericInterfaceAlternative Create (TypeDefinition type)
	{
		var t = new GenericInterfaceAlternative {
			Name = type.GetName (),
			IsPublic = true
		};

		return t;
	}

	public void PopulateMembers ()
	{
		foreach (var nested in NestedTypes.OfType<IManagedTypeModel> ())
			nested.PopulateMembers ();
	}
}

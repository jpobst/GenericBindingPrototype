using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator;

class GenericClassAlternative : ClassWriter, IManagedTypeModel
{
	public ManagedNamespaceModel? Namespace { get; set; }

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

	public void PopulateMembers ()
	{
		foreach (var nested in NestedTypes.OfType<IManagedTypeModel> ())
			nested.PopulateMembers ();
	}
}

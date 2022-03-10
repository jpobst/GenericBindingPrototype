using Javil;
using Xamarin.SourceWriter;

namespace generator2;

class BoundInterface : InterfaceWriter
{
	public GenericInterfaceAlternative? GenericInterfaceAlternative { get; set; }

	public static BoundInterface Create (TypeDefinition type)
	{
		var t = new BoundInterface () {
			Name = type.GetName (),
			IsPublic = true
		};

		foreach (var tp in type.GenericParameters)
			t.GenericParameters.Add (tp.Name);

		foreach (var iface in type.ImplementedInterfaces)
			t.Implements.Add (FormatExtensions.FormatTypeReference (iface.InterfaceType));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => f.IsConstant && (f.IsPublic || f.IsProtected)))
			t.Fields.Add (BoundField.Create (field));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => !f.IsConstant && (f.IsPublic || f.IsProtected)))
			t.Properties.Add (BoundFieldAsProperty.Create (field));

		// TODO: Methods

		if (type.HasGenericParameters)
			t.GenericInterfaceAlternative = GenericInterfaceAlternative.Create (type);

		return t;
	}
}

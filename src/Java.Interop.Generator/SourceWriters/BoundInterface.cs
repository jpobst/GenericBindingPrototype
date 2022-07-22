using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator;

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

		// TODO:
		// Abstract classes must declare all interface methods
		// Covariant return types do not work from interface -> class
		// Clean up this Where () clause
		// Default interface methods
		// Static interface methods
		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (f => (f.IsAbstract && !f.IsStatic) && f.Name != "values" && !f.IsBridge && !f.IsConstructor && !f.IsDefaultInterfaceMethod))
			if (BoundInterfaceMethod.Create (method, type) is MethodWriter m)
				t.Methods.Add (m);

		// Default interface methods
		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (f => f.IsDefaultInterfaceMethod && !f.IsBridge))
			if (BoundMethod.Create (method, type) is MethodWriter m)
				t.Methods.Add (m);

		if (type.HasGenericParameters)
			t.GenericInterfaceAlternative = GenericInterfaceAlternative.Create (type);

		return t;
	}
}

using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator;

class BoundInterface : InterfaceWriter, IManagedTypeModel
{
	public TypeDefinition JavaType { get; set; }
	public GenericInterfaceAlternative? GenericInterfaceAlternative { get; set; }
	public ManagedNamespaceModel? Namespace { get; set; }

	BoundInterface (TypeDefinition javaType)
	{
		JavaType = javaType;
	}

	public static BoundInterface Create (TypeDefinition type)
	{
		var t = new BoundInterface (type) {
			Name = type.GetName (),
			IsPublic = true
		};

		foreach (var tp in type.GenericParameters)
			t.GenericParameters.Add (tp.Name);

		foreach (var iface in type.ImplementedInterfaces)
			t.Implements.Add (FormatExtensions.FormatTypeReference (iface.InterfaceType));

		if (type.HasGenericParameters)
			t.GenericInterfaceAlternative = GenericInterfaceAlternative.Create (type);

		type.SetManagedTypeModel (t);

		return t;
	}

	public void PopulateMembers ()
	{
		foreach (var field in JavaType.Fields.OfType<FieldDefinition> ().Where (f => f.IsConstant && (f.IsPublic || f.IsProtected)))
			Fields.Add (BoundField.Create (field));

		foreach (var field in JavaType.Fields.OfType<FieldDefinition> ().Where (f => !f.IsConstant && (f.IsPublic || f.IsProtected)))
			Properties.Add (BoundFieldAsProperty.Create (field));

		// TODO:
		// Abstract classes must declare all interface methods
		// Covariant return types do not work from interface -> class
		// Clean up this Where () clause
		// Default interface methods
		// Static interface methods
		foreach (var method in JavaType.Methods.OfType<MethodDefinition> ().Where (f => (f.IsAbstract && !f.IsStatic) && f.Name != "values" && !f.IsBridge && !f.IsConstructor && !f.IsDefaultInterfaceMethod))
			if (BoundInterfaceMethod.Create (method, JavaType) is MethodWriter m)
				Methods.Add (m);

		// Default interface methods
		foreach (var method in JavaType.Methods.OfType<MethodDefinition> ().Where (f => f.IsDefaultInterfaceMethod && !f.IsBridge))
			if (BoundMethod.Create (method, JavaType) is MethodWriter m)
				Methods.Add (m);

		foreach (var nested in NestedTypes.OfType<IManagedTypeModel> ())
			nested.PopulateMembers ();
	}
}

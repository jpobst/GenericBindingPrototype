using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator;

class BoundClass : ClassWriter
{
	public GenericClassAlternative? NonGenericClass { get; set; }

	public static BoundClass Create (TypeDefinition type)
	{
		var t = new BoundClass () {
			Name = type.GetName ()
		};

		if (type.IsPublic)
			t.IsPublic = true;
		else if (type.IsProtected)
			t.IsProtected = true;

		t.IsAbstract = type.IsAbstract;
		t.IsSealed = type.IsFinal;

		foreach (var tp in type.GenericParameters)
			t.GenericParameters.Add (tp.Name);

		t.Inherits = FormatExtensions.FormatTypeReference (type.BaseType);

		// Create a BoundGenericClass that inherits this interface
		if (type.HasGenericParameters)
			t.NonGenericClass = GenericClassAlternative.Create (type);

		t.Attributes.Add (JavaSignatureAttributeWriter.Create (type));

		foreach (var iface in type.ImplementedInterfaces)
			t.Implements.Add (FormatExtensions.FormatTypeReference (iface.InterfaceType));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => f.IsConstant && (f.IsPublic || f.IsProtected)))
			t.Fields.Add (BoundField.Create (field));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => !f.IsConstant && (f.IsPublic || f.IsProtected)))
			t.Properties.Add (BoundFieldAsProperty.Create (field));

		// TODO: Clean up this Where () clause
		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (f => (f.IsPublic || f.IsProtected) && f.Name != "values" && !f.IsBridge && !f.IsConstructor))
			if (BoundMethod.Create (method, type) is MethodWriter m)
				t.Methods.Add (m);

		return t;
	}
}

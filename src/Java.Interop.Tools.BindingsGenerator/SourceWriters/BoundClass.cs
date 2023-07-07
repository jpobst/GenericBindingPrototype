using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class BoundClass : ClassWriter
{
	public static BoundClass Create (TypeDefinition type)
	{
		var t = new BoundClass () {
			Name = type.GetName (),
			IsPartial = true
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

		if (type.HasGenericParameters)
			foreach (var gp in type.GenericParameters) {
				if (gp.InterfaceBounds is not null)
					foreach (var tr in gp.InterfaceBounds)
						t.GenericConstraints.Add (new GenericConstraintModel (gp.Name, FormatExtensions.FormatTypeReference (tr)));
			}

		// Binding infrastructure
		//t.Attributes.Add (JniTypeSignatureAttributeWriter.Create (type));
		//t.Fields.Add (PeerMembersField.Create (type));
		//t.Properties.Add (new JniPeerMembersGetter ());
		t.Constructors.Add (new JavaLangObjectConstructor (type));

		// Public API
		foreach (var iface in type.ImplementedInterfaces)
			t.Implements.Add (FormatExtensions.FormatTypeReference (iface.InterfaceType));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => f.ShouldBindFieldAsConstant ()))
			t.Fields.Add (BoundField.Create (field));

		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (m => m.ShouldBindMethodAsConstructor ()))
			if (BoundConstructor.Create (method, type) is ConstructorWriter m)
				t.Constructors.Add (m);

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => f.ShouldBindFieldAsProperty ()))
			t.Properties.Add (BoundFieldAsProperty.Create (field));

		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (m => m.ShouldBindMethod ())) {
			// The normal bound method
			if (BoundMethod.Create (method, type) is MethodWriter m)
				t.Methods.Add (m);

			// Possibly one or more covariant interface bridge methods
			foreach (var method_declaration in method.GetCovariantInterfaceMethod ())
				if (CovariantReturnMethodBridge.Create (method, method_declaration.Item1, method_declaration.Item2, type) is MethodWriter cm)
					t.Methods.Add (cm);
		}

		return t;
	}
}

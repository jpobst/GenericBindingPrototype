using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class BoundClass : ClassWriter
{
	public static BoundClass Create (TypeDefinition type, GeneratorSettings settings)
	{
		var t = new BoundClass () {
			Namespace = type.GetManagedNamespace (settings),
			Name = type.GetManagedName (settings),
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

		t.Inherits = FormatExtensions.FormatTypeReference (type.BaseType, settings);

		if (type.HasGenericParameters)
			foreach (var gp in type.GenericParameters) {
				if (gp.InterfaceBounds is not null)
					foreach (var tr in gp.InterfaceBounds)
						t.GenericConstraints.Add (new GenericConstraintModel (gp.Name, FormatExtensions.FormatTypeReference (tr, settings)));
			}

		// Binding infrastructure
		if (!settings.GenerateStubsOnly) {
			//t.Attributes.Add (JniTypeSignatureAttributeWriter.Create (type));
			//t.Fields.Add (PeerMembersField.Create (type));
			//t.Properties.Add (new JniPeerMembersGetter ());
		}

		t.Constructors.Add (new JavaLangObjectConstructor (type, settings));

		// Public API
		foreach (var iface in type.ImplementedInterfaces)
			t.Implements.Add (FormatExtensions.FormatTypeReference (iface.InterfaceType, settings));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => f.ShouldBindFieldAsConstant ()))
			t.Fields.Add (BoundField.Create (field, settings));

		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (m => m.ShouldBindMethodAsConstructor ()))
			if (BoundConstructor.Create (method, type, settings) is ConstructorWriter m)
				t.Constructors.Add (m);

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => f.ShouldBindFieldAsProperty ()))
			t.Properties.Add (BoundFieldAsProperty.Create (field, settings));

		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (m => m.ShouldBindMethod ())) {
			// The normal bound method
			if (BoundMethod.Create (method, type, settings) is MethodWriter m)
				t.Methods.Add (m);

			// Possibly one or more covariant interface bridge methods
			foreach (var method_declaration in method.GetCovariantInterfaceMethods ())
				if (CovariantReturnMethodBridge.Create (method, method_declaration.Item1, method_declaration.Item2, type, settings) is MethodWriter cm)
					t.Methods.Add (cm);
		}

		return t;
	}
}

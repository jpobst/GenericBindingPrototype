using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class BoundInterface : InterfaceWriter
{
	public static BoundInterface Create (TypeDefinition type, GeneratorSettings settings)
	{
		var t = new BoundInterface () {
			Namespace = type.GetManagedNamespace (settings),
			Name = type.GetManagedName (settings),
			IsPublic = true
		};

		foreach (var tp in type.GenericParameters)
			t.GenericParameters.Add (tp.Name);

		foreach (var iface in type.ImplementedInterfaces)
			t.Implements.Add (FormatExtensions.FormatTypeReference (iface.InterfaceType, settings));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => f.ShouldBindFieldAsConstant ()))
			t.Fields.Add (BoundField.Create (field, settings));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => f.ShouldBindFieldAsProperty ()))
			t.Properties.Add (BoundFieldAsProperty.Create (field, settings));

		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (m => m.ShouldBindInterfaceMethodAsDeclaration ()))
			if (BoundInterfaceMethodDeclaration.Create (method, type, settings) is MethodWriter m)
				t.Methods.Add (m);

		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (m => m.ShouldBindInterfaceMethodAsImplementation ()))
			if (BoundMethod.Create (method, type, settings) is MethodWriter m)
				t.Methods.Add (m);

		return t;
	}
}

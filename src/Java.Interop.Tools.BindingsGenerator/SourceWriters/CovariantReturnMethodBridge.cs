using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class CovariantReturnMethodBridge
{
	public static BoundMethod? Create (MethodDefinition method, ImplementedInterface iface, MethodDefinition methodDeclaration, TypeDefinition type, GeneratorSettings settings)
	{
		var mapping = new GenericParameterMapping ();

		if (type.ImplementedInterfaces.Any (ii => ii.InterfaceType.FullName == iface.InterfaceType.FullName))
			mapping.AddMappingFromTypeReference (iface.InterfaceType);
		else
			mapping.AddMappingToImplementedInterface (type, iface);

		var return_type = FormatExtensions.FormatTypeReference (mapping.GetMappedReference (methodDeclaration.ReturnType), settings, true);

		var m = new BoundMethod {
			Name = method.GetManagedGenericName (settings),
			ExplicitInterfaceImplementation = FormatExtensions.FormatTypeReference (mapping.GetMappedReference (iface.InterfaceType), settings),
			ReturnType = new TypeReferenceWriter (return_type)
		};

		m.Comments.Add ("// Bridge method to support covariant return type of interface method declaration");

		if (method.HasParameters)
			foreach (var p in method.Parameters)
				m.Parameters.Add (new MethodParameterWriter (p.GetManagedName (settings), new TypeReferenceWriter (FormatExtensions.FormatTypeReference (p.ParameterType, settings))));

		m.Body.Add ($"return ({return_type})this.{m.Name} ({string.Join (", ", m.Parameters.Select (p => p.Name))});");

		return m;
	}
}

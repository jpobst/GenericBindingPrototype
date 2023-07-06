using Javil;
using Xamarin.SourceWriter;

namespace generator2;

class CovariantReturnMethodBridge
{
	public static BoundMethod? Create (MethodDefinition method, ImplementedInterface iface, MethodDefinition methodDeclaration, TypeDefinition type)
	{
		var mapping = new GenericParameterMapping ();

		if (type.ImplementedInterfaces.Any (ii => ii.InterfaceType.FullName == iface.InterfaceType.FullName))
			mapping.AddMappingFromTypeReference (iface.InterfaceType);
		else
			mapping.AddMappingToImplementedInterface (type, iface);

		var return_type = FormatExtensions.FormatTypeReference (mapping.GetMappedReference (methodDeclaration.ReturnType), true);

		var m = new BoundMethod {
			Name = method.GetName (),
			ExplicitInterfaceImplementation = FormatExtensions.FormatTypeReference (mapping.GetMappedReference (iface.InterfaceType)),
			ReturnType = new TypeReferenceWriter (return_type)
		};

		m.Comments.Add ("// Bridge method to support covariant return type of interface method declaration");

		if (method.HasParameters)
			foreach (var p in method.Parameters)
				m.Parameters.Add (new MethodParameterWriter (p.GetName (), new TypeReferenceWriter (FormatExtensions.FormatTypeReference (p.ParameterType))));

		m.Body.Add ($"return ({return_type})this.{m.Name} ({string.Join (", ", m.Parameters.Select (p => p.Name))});");

		return m;
	}
}

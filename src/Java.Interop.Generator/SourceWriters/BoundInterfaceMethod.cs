using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator;

class BoundInterfaceMethod : MethodWriter
{
	// Currently this is just interface method declarations, DIM not supported
	public static BoundInterfaceMethod? Create (MethodDefinition method, TypeDefinition type)
	{
		var m = new BoundInterfaceMethod {
			Name = method.GetName (),
			ReturnType = new TypeReferenceWriter (FormatExtensions.FormatTypeReference (method.ReturnType)),
			IsDeclaration = true
		};

		if (method.HasParameters)
			foreach (var p in method.Parameters)
				m.Parameters.Add (new MethodParameterWriter (p.GetName (), new TypeReferenceWriter (FormatExtensions.FormatTypeReference (p.ParameterType))));

		return m;
	}
}

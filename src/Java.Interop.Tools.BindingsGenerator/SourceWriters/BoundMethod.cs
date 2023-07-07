using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class BoundMethod : MethodWriter
{
	public static BoundMethod? Create (MethodDefinition method, TypeDefinition type)
	{
		var m = new BoundMethod {
			Name = method.GetName ()
		};

		var base_method = method.FindDeclaredBaseMethodOrDefault ();
		var effective_method = method;
		var effective_return_type = method.ReturnType;

		if (base_method is not null && !method.IsStatic) {
			m.IsOverride = true;
			effective_method = base_method;

			if (method.ReturnType.IsArray && !effective_method.ReturnType.IsArray)
				effective_return_type = effective_method.ReturnType;
			else if ((method.ReturnType as GenericInstanceType)?.GenericArguments.Any () == true && method.ReturnType.FullName != effective_method.ReturnType.FullName && !((effective_method.ReturnType as GenericInstanceType)?.GenericArguments.Any () == true))
				effective_return_type = effective_method.ReturnType;
		}

		if (type.IsAbstract && method.IsAbstract && m.IsOverride)
			return null;

		if (effective_method.IsPublic)
			m.IsPublic = true;

		if (effective_method.IsProtected)
			m.IsProtected = true;

		if (!method.IsFinal && !method.IsStatic && !type.IsFinal && !m.IsOverride)
			m.IsVirtual = true;

		if (!m.IsPublic && !m.IsProtected)
			return null;

		m.IsStatic = method.IsStatic;
		m.IsAbstract = method.IsAbstract;

		// Don't emit these modifiers for DIM
		if (type.IsInterface && method.IsDefaultInterfaceMethod ()) {
			m.IsPublic = false;
			m.IsProtected = false;
			m.IsVirtual = false;
			m.IsAbstract = false;
		}

		if (!m.IsOverride && method.HasGenericParameters)
			foreach (var gp in method.GenericParameters) {
				if (gp.InterfaceBounds is not null)
					foreach (var tr in gp.InterfaceBounds)
						m.GenericConstraints.Add (new GenericConstraintModel (gp.Name, FormatExtensions.FormatTypeReference (tr)));
			}

		if (method.GetExplicitInterface () is ImplementedInterface explicit_interface) {
			var mapping = new GenericParameterMapping ();

			if (type.ImplementedInterfaces.Any (ii => ii.InterfaceType.FullName == explicit_interface.InterfaceType.FullName))
				mapping.AddMappingFromTypeReference (explicit_interface.InterfaceType);
			else
				mapping.AddMappingToImplementedInterface (type, explicit_interface);

			m.ExplicitInterfaceImplementation = FormatExtensions.FormatTypeReference (mapping.GetMappedReference (explicit_interface.InterfaceType));
		}

		m.ReturnType = new TypeReferenceWriter (FormatExtensions.FormatTypeReference (effective_return_type));

		if (method.HasParameters)
			foreach (var p in method.Parameters)
				m.Parameters.Add (new MethodParameterWriter (p.GetName (), new TypeReferenceWriter (FormatExtensions.FormatTypeReference (p.ParameterType))));

		if (!m.IsAbstract)
			m.Body.Add ("throw new global::System.NotImplementedException ();");

		return m;
	}
}

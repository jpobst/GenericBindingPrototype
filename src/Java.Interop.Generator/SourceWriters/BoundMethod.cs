using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator;

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

		m.ReturnType = new TypeReferenceWriter (FormatExtensions.FormatTypeReference (effective_return_type));
		m.ExplicitInterfaceImplementation = method.GetExplicitInterface ();

		if (m.ExplicitInterfaceImplementation.HasValue () && method.IsDefaultInterfaceMethod) {
			m.IsPublic = false;
			m.IsVirtual = false;
		}

		if (method.HasParameters)
			foreach (var p in method.Parameters)
				m.Parameters.Add (new MethodParameterWriter (p.GetName (), new TypeReferenceWriter (FormatExtensions.FormatTypeReference (p.ParameterType))));

		if (!m.IsAbstract)
			m.Body.Add ("throw new NotImplementedException ();");

		return m;
	}
}

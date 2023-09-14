using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

static class TypeFixupExtensions
{
	internal static string EscapeMethodName (MethodDefinition method, GeneratorSettings settings)
	{
		// We need to ensure with escape generic names that are keywords
		// if generic parameters are removed, like "void lock<T> (...)"
		if (CSharpFacts.IsReservedKeyword (method.GetCapitalizedMethodName (settings)))
			return "@" + method.GetCapitalizedMethodName (settings);

		return method.GetCapitalizedMethodName (settings);
	}

	internal static string EscapeMethodGenericName (MethodDefinition method, GeneratorSettings settings)
	{
		// We need to ensure with escape generic names that are keywords
		// if generic parameters are removed, like "void lock<T> (...)"
		if (CSharpFacts.IsReservedKeyword (method.GetCapitalizedMethodName (settings)))
			return "@" + method.GetCapitalizedMethodGenericName (settings);

		return method.GetCapitalizedMethodGenericName (settings);
	}

	public static string EscapeIdentifier (string name)
	{
		return CSharpFacts.IsReservedKeyword (name) ? "@" + name : name;
	}

	public static string GetCapitalizedMethodName (this MethodDefinition method, GeneratorSettings settings)
	{
		return settings.MethodCapitalizationStrategy switch {
			MethodCapitalizationStrategy.PascalCase => method.Name.Capitalize (),
			_ => method.Name
		};
	}

	public static string GetCapitalizedMethodGenericName (this MethodDefinition method, GeneratorSettings settings)
	{
		return settings.MethodCapitalizationStrategy switch {
			MethodCapitalizationStrategy.PascalCase => method.GenericName.Capitalize (),
			_ => method.GenericName
		};
	}

	public static void SetMethodIsOverride (this TypeWriter type, string typeName, string method)
	{
		if (type.Name != typeName)
			return;

		if (type.Methods.FirstOrDefault (m => m.Name == method) is MethodWriter m)
			m.IsOverride = true;
	}

	public static void SetMethodIsProtected (this TypeWriter type, string typeName, string method)
	{
		if (type.Name != typeName)
			return;

		if (type.Methods.FirstOrDefault (m => m.Name == method) is MethodWriter m) {
			m.IsPublic = false;
			m.IsProtected = true;
		}
	}

	public static void SetMethodReturnType (this TypeWriter type, string typeName, string methodName, string returnType)
	{
		if (type.Name != typeName)
			return;

		if (type.Methods.FirstOrDefault (m => m.Name == methodName) is MethodWriter m)
			m.ReturnType.Name = returnType;
	}

	public static void SetTypeIsPublic (this TypeWriter type, string typeName)
	{
		if (type.Name != typeName)
			return;

		type.IsPublic = true;
	}

	public static bool RenameField (this ContainerDefinition container, string typeName, string fieldName, string value)
	{
		var type = container.FindType (typeName);

		if (type is null)
			return false;

		if (type.Fields.FirstOrDefault (p => p.Name == fieldName) is FieldDefinition p) {
			p.SetManagedName (value, true);
			return true;
		}

		return false;
	}

	public static bool RenameType (this ContainerDefinition container, string typeName, string newName)
	{
		var type = container.FindType (typeName);

		if (type is null)
			return false;

		type.SetManagedName (newName);

		return false;
	}

	public static bool HideType (this ContainerDefinition container, string typeName)
	{
		var type = container.FindType (typeName);

		if (type is null)
			return false;

		type.IsPublic = false;
		type.IsProtected = false;
		type.IsPrivate = true;

		return false;
	}

	public static IEnumerable<TypeReference> GetReferencedTypes (this TypeReference type)
	{
		if (type is GenericInstanceType gt)
			foreach (var t in gt.GenericArguments)
				foreach (var rt in t.GetReferencedTypes ())
					yield return rt;

		if (type is not WildcardType && type.Name != "**")
			yield return type;
	}

	public static void AddMappingToImplementedInterface (this GenericParameterMapping mapping, TypeDefinition type, ImplementedInterface iface)
	{
		var stack = new Stack<TypeReference> ();

		if (AddMappingToImplementedInterfaceCore (stack, type, iface)) {
			foreach (var tr in stack.Reverse ())
				mapping.AddMappingFromTypeReference (tr);
		}
	}

	static bool AddMappingToImplementedInterfaceCore (Stack<TypeReference> mapping, TypeDefinition type, ImplementedInterface iface)
	{
		// If we found what we're looking for, return
		if (type.FullNameGenericsErased == iface.InterfaceType.FullNameGenericsErased)
			return true;

		foreach (var i in type.ImplementedInterfaces) {
			mapping.Push (i.InterfaceType);

			if (AddMappingToImplementedInterfaceCore (mapping, i.InterfaceType.Resolve ()!, iface))
				return true;

			mapping.Pop ();
		}

		return false;
	}
}

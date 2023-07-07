using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

static class TypeFixupExtensions
{
	public static void SetNamespace (this TypeReference type, string ns)
	{
		type.CustomData ["managedNamespace"] = ns;
	}

	public static string GetNamespace (this TypeReference type)
	{
		if (type.CustomData.TryGetValue ("managedNamespace", out var data) && data is string ns)
			return ns;

		ns = type.Namespace;

		if (ns == "java.lang.module")
			ns = "Java.Lang.Modules";
		if (ns == "sun.text.normalizer")
			ns = "Sun.Text.Normalizers";

		return string.Join ('.', ns.Split ('.').Select (s => s.Capitalize ()));
	}

	public static void SetName (this ICustomDataProvider member, string name)
	{
		member.CustomData ["managedName"] = name;
	}

	public static string GetName (this IMemberDefinition member)
	{
		if (member.CustomData.TryGetValue ("managedName", out var data) && data is string name)
			return name;

		if (member is MethodDefinition method)
			return EscapeMethodName (method);

		return EscapeIdentifier (member.Name);
	}

	public static string GetName (this ParameterDefinition member)
	{
		if (member.CustomData.TryGetValue ("managedName", out var data) && data is string name)
			return name;

		return EscapeIdentifier (member.Name);
	}

	public static void SetExplicitInterface (this MethodDefinition method, ImplementedInterface iface)
	{
		method.CustomData ["explicitInterface"] = iface;
	}

	public static ImplementedInterface? GetExplicitInterface (this MethodDefinition method)
	{
		if (method.CustomData.TryGetValue ("explicitInterface", out var data) && data is ImplementedInterface tr)
			return tr;

		return null;
	}

	public static void SetCovariantInterfaceMethod (this MethodDefinition method, ImplementedInterface iface, MethodDefinition interfaceMethod)
	{
		if (!method.CustomData.TryGetValue ("covariantInterfaceMethods", out var list)) {
			list = new List<(ImplementedInterface, MethodDefinition)> ();
			method.CustomData.Add ("covariantInterfaceMethods", list);
		}

		var typed_list = (List<(ImplementedInterface, MethodDefinition)>) list;

		// Don't allow duplicates
		if (!typed_list.Any (m => m.Item2.GetDescriptor () == interfaceMethod.GetDescriptor ()))
			typed_list.Add ((iface, interfaceMethod));
	}

	public static IEnumerable<(ImplementedInterface, MethodDefinition)> GetCovariantInterfaceMethod (this MethodDefinition method)
	{
		if (method.CustomData.TryGetValue ("covariantInterfaceMethods", out var data) && data is List<(ImplementedInterface, MethodDefinition)> list)
			return list;

		return Array.Empty<(ImplementedInterface, MethodDefinition)> ();
	}

	private static string EscapeMethodName (MethodDefinition method)
	{
		// We need to ensure with escape generic names that are keywords
		// if generic parameters are removed, like "void lock<T> (...)"
		if (EscapeIdentifier (method.Name).StartsWith ('@'))
			return "@" + method.GenericName;

		return method.GenericName;
	}

	public static string EscapeIdentifier (string name)
	{
		return CSharpFacts.IsReservedKeyword (name) ? "@" + name : name;
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
			p.SetName (value);
			return true;
		}

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

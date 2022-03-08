using Javil;
using Xamarin.SourceWriter;

namespace generator2;

static class TypeFixupExtensions
{
	public static void SetNamespace (this TypeReference type, string ns)
	{
		type.CustomData ["managedNamespace"] = ns;
	}

	public static string GetNamespace (this TypeReference type)
	{
		if (type.CustomData.TryGetValue ("managedNamespace", out var ns))
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
		if (member.CustomData.TryGetValue ("managedName", out var name))
			return name;

		if (member is MethodDefinition method)
			return EscapeMethodName (method);

		return EscapeIdentifier (member.Name);
	}

	public static string GetName (this ParameterDefinition member)
	{
		if (member.CustomData.TryGetValue ("managedName", out var name))
			return name;

		return EscapeIdentifier (member.Name);
	}

	private static string EscapeMethodName (MethodDefinition method)
	{
		// We need to ensure with escape generic names that are keywords
		// if generic parameters are removed, like "void lock<T> (...)"
		if (EscapeIdentifier (method.Name).StartsWith ('@'))
			return "@" + method.GenericName;

		return method.GenericName;
	}

	private static string EscapeIdentifier (string name)
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
}

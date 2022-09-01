using System.Diagnostics.Metrics;
using System.Xml.Linq;
using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator;

public static class TypeFixupExtensions
{
	public static void SetNamespace (this TypeReference type, string ns)
	{
		type.CustomData ["managedNamespace"] = ns;
	}

	public static string GetNamespace (this TypeReference type)
	{
		if (type.CustomData.TryGetValue ("managedNamespace", out var ns_obj) && ns_obj is string ns)
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

	public static void SetExplicitInterface (this ICustomDataProvider member, string name)
	{
		member.CustomData ["explicitInterface"] = name;
	}

	public static void SetExplicitInterface (this ICustomDataProvider member, TypeDefinition iface)
	{
		member.CustomData ["explicitInterface"] = iface;
	}

	public static void SetManagedReturnType (this ICustomDataProvider member, TypeDefinition type)
	{
		member.CustomData ["managedReturn"] = type;
	}

	public static void SetManagedTypeModel (this TypeReference type, TypeWriter model)
	{
		type.CustomData ["managedTypeModel"] = model;
	}

	public static TypeWriter? GetManagedTypeModel (this TypeReference type)
	{
		if (type.CustomData.TryGetValue ("managedTypeModel", out var model) && model is TypeWriter tw)
			return tw;

		return null;
	}

	public static void SetManagedTypeDefinition (this TypeReference type, ManagedTypeDefinition model)
	{
		type.CustomData ["managedTypeDefinition"] = model;
	}

	public static ManagedTypeDefinition? GetManagedTypeDefinition (this TypeReference type)
	{
		if (type.CustomData.TryGetValue ("managedTypeDefinition", out var model) && model is ManagedTypeDefinition tw)
			return tw;

		return null;
	}

	public static string GetName (this IMemberDefinition member)
	{
		if (member.CustomData.TryGetValue ("managedName", out var obj) && obj is string name)
			return name;

		if (member is MethodDefinition method)
			return EscapeMethodName (method);

		return EscapeIdentifier (member.Name);
	}

	public static string GetName (this ParameterDefinition member)
	{
		if (member.CustomData.TryGetValue ("managedName", out var obj) && obj is string name)
			return name;

		return EscapeIdentifier (member.Name);
	}

	// TODO: This should probably handle managedName
	public static string? GetExplicitInterface (this IMemberDefinition member)
	{
		if (!member.CustomData.TryGetValue ("explicitInterface", out var iface_object))
			return null;

		if (iface_object is string iface_name)
			return iface_name;

		if (iface_object is TypeDefinition iface)
			return $"global::{iface.GetNamespace ()}.{iface.GenericNestedName.Replace ('$', '.')}"; ;

		return null;
	}

	// TODO: This should probably handle managedName
	public static TypeReference? GetManagedReturnType (this MethodDefinition member)
	{
		if (!member.CustomData.TryGetValue ("managedReturn", out var return_object))
			return member.ReturnType;

		//if (return_object is string return_name)
		//	return return_name;

		if (return_object is TypeDefinition ret)
			return ret;

		return null;
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

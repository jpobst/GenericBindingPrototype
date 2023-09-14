using System.Diagnostics.CodeAnalysis;
using Javil;

namespace Java.Interop.Tools.BindingsGenerator;

static class CustomDataExtensions
{
	// This class holds methods for all of the "fixups" done against the Java model,
	// which are stored in the model's CustomData. This is what we called "metadata"
	// in the original generator.
	// "Explicit" data refers to something the user told us to use. It always takes
	// precedence over "Implicit" data, which is something that we calculated.
	const string CovariantInterfaceMethods = nameof (CovariantInterfaceMethods);
	const string ExplicitExplicitInterface = nameof (ExplicitExplicitInterface);
	const string ImplicitExplicitInterface = nameof (ImplicitExplicitInterface);
	const string ExplicitManagedNamespace = nameof (ExplicitManagedNamespace);
	const string ImplicitManagedNamespace = nameof (ImplicitManagedNamespace);
	const string ExplicitManagedName = nameof (ExplicitManagedName);
	const string ImplicitManagedName = nameof (ImplicitManagedName);

	// Covariant interface methods cannot be set explicitly
	public static void SetCovariantInterfaceMethod (this MethodDefinition method, ImplementedInterface iface, MethodDefinition interfaceMethod)
	{
		if (!method.TryGetValue<List<(ImplementedInterface, MethodDefinition)>> (CovariantInterfaceMethods, out var list)) {
			list = new List<(ImplementedInterface, MethodDefinition)> ();
			method.CustomData.Add (CovariantInterfaceMethods, list);
		}

		// Don't allow duplicates
		if (!list.Any (m => m.Item2.GetDescriptor () == interfaceMethod.GetDescriptor ()))
			list.Add ((iface, interfaceMethod));
	}

	public static IEnumerable<(ImplementedInterface, MethodDefinition)> GetCovariantInterfaceMethods (this MethodDefinition method)
	{
		if (method.TryGetValue<List<(ImplementedInterface, MethodDefinition)>> (CovariantInterfaceMethods, out var list))
			return list;

		return Array.Empty<(ImplementedInterface, MethodDefinition)> ();
	}

	// Users will set this as a string
	public static void SetExplicitInterface (this MethodDefinition method, string iface)
	{
		method.CustomData [ExplicitExplicitInterface] = iface;
	}

	// Internally we set this as an ImplementedInterface type
	public static void SetExplicitInterface (this MethodDefinition method, ImplementedInterface iface)
	{
		method.CustomData [ImplicitExplicitInterface] = iface;
	}

	// Will return string for an explictly set value
	// Will return ImplementedInterface for an implicitly set value
	public static object? GetExplicitInterface (this MethodDefinition method)
	{
		// Check explicit first
		if (method.TryGetValue<string> (ExplicitExplicitInterface, out var explicit_interface))
			return explicit_interface;

		// Check implicit next
		if (method.TryGetValue<ImplementedInterface> (ImplicitExplicitInterface, out var implicit_interface))
			return implicit_interface;

		return null;
	}

	// Technically this is only looking for one we set, not one a user set
	public static bool IsExplicitInterface (this MethodDefinition method)
	{
		return method.GetExplicitInterface () is ImplementedInterface;
	}

	public static void SetNamespace (this TypeReference type, string name, bool isExplicit = false)
	{
		type.CustomData [isExplicit ? ExplicitManagedNamespace : ImplicitManagedNamespace] = name;
	}

	public static string GetManagedNamespace (this TypeDefinition type, GeneratorSettings settings)
	{
		// Check explicit first
		if (type.TryGetValue<string> (ExplicitManagedName, out var explicit_name))
			return explicit_name;

		// Check implicit next
		if (type.TryGetValue<string> (ImplicitManagedName, out var implicit_name))
			return implicit_name;

		var ns = type.Namespace;

		// TODO: Should not be hardcoded
		if (ns == "java.lang.module")
			ns = "Java.Lang.Modules";
		if (ns == "sun.text.normalizer")
			ns = "Sun.Text.Normalizers";

		return string.Join ('.', ns.Split ('.').Select (s => s.Capitalize ()));
	}

	public static void SetManagedName (this IMemberDefinition method, string name, bool isExplicit = false)
	{
		method.CustomData [isExplicit ? ExplicitManagedName : ImplicitManagedName] = name;
	}

	public static string GetManagedName (this MethodDefinition method, GeneratorSettings settings)
	{
		// Check explicit first
		if (method.TryGetValue<string> (ExplicitManagedName, out var explicit_name))
			return explicit_name;

		// Check implicit next
		if (method.TryGetValue<string> (ImplicitManagedName, out var implicit_name))
			return implicit_name;

		// Create name from Java name
		return TypeFixupExtensions.EscapeMethodName (method, settings);
	}

	public static string GetManagedGenericName (this MethodDefinition method, GeneratorSettings settings)
	{
		// Check explicit first
		if (method.TryGetValue<string> (ExplicitManagedName, out var explicit_name))
			return explicit_name + method.FormatGenericArguments ();

		// Check implicit next
		if (method.TryGetValue<string> (ImplicitManagedName, out var implicit_name))
			return implicit_name + method.FormatGenericArguments ();

		// Create name from Java name
		return TypeFixupExtensions.EscapeMethodGenericName (method, settings);
	}

	public static string GetManagedName (this FieldDefinition field, GeneratorSettings settings)
	{
		// Check explicit first
		if (field.TryGetValue<string> (ExplicitManagedName, out var explicit_name))
			return explicit_name;

		// Check implicit next
		if (field.TryGetValue<string> (ImplicitManagedName, out var implicit_name))
			return implicit_name;

		// Create name from Java name
		return TypeFixupExtensions.EscapeIdentifier (field.Name);
	}

	public static string GetManagedName (this TypeDefinition type, GeneratorSettings settings)
	{
		// Check explicit first
		if (type.TryGetValue<string> (ExplicitManagedName, out var explicit_name))
			return explicit_name;

		// Check implicit next
		if (type.TryGetValue<string> (ImplicitManagedName, out var implicit_name))
			return implicit_name;

		// Create name from Java name
		if (type.IsInterface) {
			var prefix = type.Name.Length <= 2 || type.Name [0] != 'I' || !char.IsUpper (type.Name [1]) ? "I" : "";
			return TypeFixupExtensions.EscapeIdentifier (prefix + type.Name);
		}

		return TypeFixupExtensions.EscapeIdentifier (type.Name);
	}

	public static string GetManagedName (this ParameterDefinition paramater, GeneratorSettings settings)
	{
		// Check explicit first
		if (paramater.TryGetValue<string> (ExplicitManagedName, out var explicit_name))
			return explicit_name;

		// Check implicit next
		if (paramater.TryGetValue<string> (ImplicitManagedName, out var implicit_name))
			return implicit_name;

		// Create name from Java name
		return TypeFixupExtensions.EscapeIdentifier (paramater.Name);
	}

	static bool TryGetValue<T> (this ICustomDataProvider member, string key, [NotNullWhen (true)] out T? value)
	{
		if (member.CustomData.TryGetValue (key, out var data) && data is T value_t) {
			value = value_t;
			return true;
		}

		value = default;
		return false;
	}
}

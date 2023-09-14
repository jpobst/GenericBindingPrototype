using Javil;

namespace Java.Interop.Tools.BindingsGenerator;

// Automatically rename fields and methods that have name collisions.
static class TypeMemberNameCollisionFixups
{
	public static void Run (ContainerDefinition container, GeneratorSettings settings)
	{
		if (!settings.FixNestedTypeNameCollisions)
			return;

		foreach (var type in container.Types)
			FixType (type, settings);
	}

	static void FixType (TypeDefinition type, GeneratorSettings settings)
	{
		if (!type.IsPublicApi ())
			return;

		// Type names get precedence
		var used_names = type.NestedTypes.Select (nt => nt.GetManagedName (settings)).Distinct ().ToList ();

		// Additionally, members cannot match the enclosing type name
		used_names.Add (type.GetManagedName (settings));

		// Rename any methods that conflict with type names
		foreach (var method in type.Methods.Where (m => used_names.Contains (m.GetManagedName (settings))))
			method.SetManagedName (method.GetManagedName (settings) + "_");

		used_names.AddRange (type.Methods.Select (nt => nt.GetManagedName (settings)).Distinct ());

		// Rename any fields that conflict with type or method names
		foreach (var field in type.Fields.Where (m => used_names.Contains (m.GetManagedName (settings))))
			field.SetManagedName (field.GetManagedName (settings) + "_");

		foreach (var nested in type.NestedTypes)
			FixType (nested, settings);
	}
}

using Javil;

namespace Java.Interop.Tools.BindingsGenerator;

// Java allows public methods to override protected methods.
// C# does not, so we mark the base method as public..
static class ProtectedBaseMethodFixup
{
	public static void Run (ContainerDefinition container, GeneratorSettings settings)
	{
		foreach (var type in container.Types)
			FixType (type, settings);
	}

	static void FixType (TypeDefinition type, GeneratorSettings settings)
	{
		if (!type.IsPublicApi ())
			return;

		foreach (var method in type.Methods)
			FixMethod (method, settings);

		foreach (var nested in type.NestedTypes)
			FixType (nested, settings);
	}

	static void FixMethod (MethodDefinition method, GeneratorSettings settings)
	{
		if (!method.IsPublic)
			return;

		var base_method = method.FindDeclaredBaseMethodOrDefault ();

		if (base_method is null)
			return;

		if (base_method.IsProtected) {
			base_method.IsPublic = true;
			base_method.IsProtected = false;
		}

		method.SetManagedName (base_method.GetManagedName (settings));
	}
}

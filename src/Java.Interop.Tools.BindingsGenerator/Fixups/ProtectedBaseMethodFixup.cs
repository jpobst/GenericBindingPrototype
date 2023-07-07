using Javil;

namespace Java.Interop.Tools.BindingsGenerator;

// Java allows public methods to override protected methods.
// C# does not, so we mark the base method as public..
static class ProtectedBaseMethodFixup
{
	public static void Run (ContainerDefinition container)
	{
		foreach (var type in container.Types)
			FixType (type);
	}

	static void FixType (TypeDefinition type)
	{
		if (!(type.IsPublic || type.IsProtected))
			return;

		foreach (var method in type.Methods)
			FixMethod (method);

		foreach (var nested in type.NestedTypes)
			FixType (nested);
	}

	static void FixMethod (MethodDefinition method)
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
	}
}

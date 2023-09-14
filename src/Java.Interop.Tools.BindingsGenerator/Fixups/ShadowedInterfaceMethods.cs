using Javil;

namespace Java.Interop.Tools.BindingsGenerator;

// Java interfaces often declare methods that are the same as a method inherited from an implemented interface.
// Example:
//   public interface ICloseable : IAutoCloseable 
//   {
// 	void close ();
//   }
//
//   public interface IAutoCloseable 
//   {
// 	void close ();
//   }
// This is a CS0108 warning in C#, so we remove the unneeded method declaration.
//   CS0108 'ICloseable.close()' hides inherited member 'IAutoCloseable.close()'. Use the new keyword if hiding was intended.
static class ShadowedInterfaceMethods
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

		foreach (var nested in type.NestedTypes)
			FixType (nested, settings);

		if (!type.IsInterface)
			return;

		foreach (var method in type.Methods.ToList ())
			if (ShouldRemoveMethod (method))
				type.Methods.Remove (method);
	}

	static bool ShouldRemoveMethod (MethodDefinition method)
	{
		if (!method.IsPublic || method.IsDefaultInterfaceMethod ())
			return false;
		if (method?.DeclaringType?.TryResolve (out var resolved) == true)
			if (resolved.TryFindDeclarationForInterfaceMethod (method, out var a, out var b))
				return true;

		return false;
	}
}

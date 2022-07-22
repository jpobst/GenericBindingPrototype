using Javil;

namespace Java.Interop.Generator;

// When a default interface method provides an implementation to an interface method
// on a base interface, it has to be explicitly stated and cannot have accessibility modifiers.
// Example:
//   interface IA { void M (); }
//   interface IB : IA { void IA.M () { } }
static class DefaultInterfaceMembersOverrides
{
	public static void Run (ContainerDefinition container)
	{
		// Run against all types we are going to generate
		foreach (var type in container.Types)
			CheckInterfaceType (type);
	}

	static void CheckInterfaceType (TypeDefinition type)
	{
		if (!type.IsPublic && !type.IsProtected)
			return;

		// Need to recurse through nested types
		foreach (var nested in type.NestedTypes)
			CheckInterfaceType (nested);

		// Nothing to do if this isn't an interface
		if (!type.IsInterface)
			return;

		var default_methods = type.Methods.Where (m => m.IsDefaultInterfaceMethod);

		if (!default_methods.Any ())
			return;

		// Get all methods defined on interfaces this interface implements
		var inherited_methods = type.GetInheritedInterfaceMethods ().ToList ();

		if (!inherited_methods.Any ())
			return;

		foreach (var dim in default_methods) {
			if (inherited_methods.FirstOrDefault (m => dim.AreMethodsCompatible (m)) is MethodDefinition md)
				FixupDefaultInterfaceMethod (dim, md);
		}
	}

	static void FixupDefaultInterfaceMethod (MethodDefinition dim, MethodDefinition method)
	{
		dim.SetExplicitInterface (method.DeclaringType?.Resolve ()!);
		//dim.IsPublic = false;
	}
}

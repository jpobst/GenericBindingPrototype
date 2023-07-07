using System.Reflection;
using System;
using Javil;

namespace Java.Interop.Tools.BindingsGenerator;

// This fixes several issues:
// - Abstract classes must declare abstract methods for all needed interface members
// - Covariant return types for implemented interface methods
static class ImplementedInterfaceFixup
{
	public static void Run (ContainerDefinition container)
	{
		foreach (var type in container.Types.Where (t => t.IsClass))
			FixType (type);
	}

	private static void FixType (TypeDefinition type)
	{
		if (!type.IsPublicApi ())
			return;

		foreach (var iface in type.ImplementedInterfaces)
			CheckImplementedInterface (type, iface);

		foreach (var nested in type.NestedTypes)
			FixType (nested);
	}

	static void CheckImplementedInterface (TypeDefinition type, ImplementedInterface iface)
	{
		var ifa = iface.InterfaceType.Resolve ()!;

		foreach (var method in ifa.Methods.Where (m => !m.IsStatic))
			CheckMethod (type, iface, method);

		// Recurse into implemented interfaces
		foreach (var implemented in ifa.ImplementedInterfaces)
			CheckImplementedInterface (type, implemented);
	}

	static void CheckMethod (TypeDefinition type, ImplementedInterface iface, MethodDefinition method)
	{
		var implementation = type.FindImplementedInterfaceOrDefault (iface, method);

		if (implementation is not null) {
			// If the implementation has a covariant return type, mark it so we can
			// generate a bridge method later
			if (!method.IsDefaultInterfaceMethod () && implementation.IsMethodCovariantReturn (method))
				implementation.SetCovariantInterfaceMethod (iface, method);

			return;
		}

		// If the type is abstract, it doesn't have to provide an implementation of the method,
		// but it does have to specify an abstract declaration of the method. We're going to
		// inject a Java version of one.
		if (type.IsAbstract) {
			// Don't worry about DIM
			if (method.IsDefaultInterfaceMethod ())
				return;

			// TODO: Need to fix generics
			var new_method = method.Clone (type);
			new_method.IsAbstract = true;

			type.Methods.Add (new_method);
		}
	}
}

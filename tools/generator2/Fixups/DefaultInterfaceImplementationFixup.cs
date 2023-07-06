using System.Reflection;
using System;
using Javil;

namespace generator2;

// - Default interface methods implementing a base interface method must be explicitly declared
static class DefaultInterfaceImplementationFixup
{
	public static void Run (ContainerDefinition container)
	{
		foreach (var type in container.Types)
			FixType (type);
	}

	private static void FixType (TypeDefinition type)
	{
		if (!type.IsPublicApi ())
			return;

		if (type.IsInterface)
			foreach (var method in type.Methods.Where (m => m.IsDefaultInterfaceMethod ())) {
				if (type.TryFindDeclarationMethodIsProvidingImplementationFor (method, out var ii, out var m) && !method.IsMethodCovariantReturn (m))
					method.SetExplicitInterface (ii);
			}

		foreach (var nested in type.NestedTypes)
			FixType (nested);
	}
}

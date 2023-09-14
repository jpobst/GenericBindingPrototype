using Javil;

namespace Java.Interop.Tools.BindingsGenerator;

// Default interface methods implementing a base interface method must be explicitly declared
// Example:
//   interface Foo {
//     void Baz ();
//   }
//   interface Bar : Foo {
//     void Baz () { }
//   }
// Produces:
//   CS0108 'Bar.Baz()' hides inherited member 'Foo.Baz()'. Use the new keyword if hiding was intended.
// Instead we need:
//   interface Bar : Foo {
//     void Foo.Baz () { }
//   }
static class DefaultInterfaceImplementationFixup
{
	public static void Run (ContainerDefinition container)
	{
		foreach (var type in container.Types)
			SetExplicitInterfaces (type);

		while (true) {
			var changes_made = false;

			foreach (var type in container.Types)
				changes_made |= FixInterfaceChain (type);

			if (!changes_made)
				break;
		}
	}

	private static void SetExplicitInterfaces (TypeDefinition type)
	{
		if (!type.IsPublicApi ())
			return;

		if (type.IsInterface)
			foreach (var method in type.Methods.Where (m => m.IsDefaultInterfaceMethod ())) {
				if (type.TryFindDeclarationMethodIsProvidingImplementationFor (method, out var ii, out var m) && !method.IsMethodCovariantReturn (m))
					method.SetExplicitInterface (ii);
			}

		foreach (var nested in type.NestedTypes)
			SetExplicitInterfaces (nested);
	}

	private static bool FixInterfaceChain (TypeDefinition type)
	{
		// A wrinkle is if we have:
		//   interface Foo {
		//     void Baz ();
		//   }
		//   interface Bar : Foo {
		//     void Foo.Baz () { }
		//   }
		//   interface Bar2 : Bar {
		//     void Bar.Baz () { }
		//   }
		// This causes:
		// CS0539 'Bar.Baz()' in explicit interface declaration is not found among members of the interface that can be implemented.
		// Instead it has to be:
		//   interface Bar2 : Bar {
		//     void Foo.Baz () { }
		//   }
		if (!type.IsPublicApi ())
			return false;

		// This has to be done recursively. We are being lazy here and just doing it repeatedly until
		// changes are no longer being made. Finding the proper base in one pass would be an optimization.
		var changes_made = false;

		if (type.IsInterface)
			foreach (var method in type.Methods.Where (m => m.IsDefaultInterfaceMethod ())) {
				if (type.TryFindDeclarationMethodIsProvidingImplementationFor (method, out var ii, out var m) && !method.IsMethodCovariantReturn (m) && m.GetExplicitInterface () is ImplementedInterface ii2 && method.GetExplicitInterface () is ImplementedInterface ii3 && ii3.InterfaceType.FullName != ii2.InterfaceType.FullName) {
					method.SetExplicitInterface (ii2);
					changes_made = true;
				}
			}

		foreach (var nested in type.NestedTypes)
			changes_made |= FixInterfaceChain (nested);

		return changes_made;
	}
}

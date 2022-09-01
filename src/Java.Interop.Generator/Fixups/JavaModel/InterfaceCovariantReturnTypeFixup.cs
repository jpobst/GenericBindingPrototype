using Javil;

namespace Java.Interop.Generator;

// Java allows classes to change the return type of interface methods they implement.
// C# does not, so we have to change the method return type to match the interface method.

// Example:

// public interface Foo
// {
// 	 object Do ();
// }

// public class Bar : Foo
// {
// 	 public string Do () => null;
// }

// CS0738 'Bar' does not implement interface member 'Foo.Do()'. 'Bar.Do()' cannot implement
// 'Foo.Do()' because it does not have the matching return type of 'object'.

public static class InterfaceCovariantReturnTypeFixup
{
	public static void Run (ContainerDefinition container)
	{
		// We have to check interface as well because they may have nested classes
		foreach (var type in container.Types)
			CheckClassType (type);
	}

	static void CheckClassType (TypeDefinition type)
	{
		if (!type.IsVisible ())
			return;

		// Need to recurse through nested types
		foreach (var nested in type.NestedTypes)
			CheckClassType (nested);

		// Nothing to do if this isn't an class
		if (!type.IsClass)
			return;

		// Get all possible interface methods
		var interface_methods = type.GetInheritedInterfaceMethods ().ToList ();

		foreach (var method in type.Methods) {
			if (interface_methods.FirstOrDefault (m => method.AreMethodsCompatible (m)) is MethodDefinition base_method)
				if (base_method.ReturnType?.Resolve () is TypeDefinition td)
					method.SetManagedReturnType (td);
		}
	}
}

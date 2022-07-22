using Javil;

namespace Java.Interop.Generator;

// Java doesn't require abstract classes to declare a method for all interface
// methods.  C# does, so we need to inject abstract methods where they are missing.
public static class AbstractClassMustImplementInterfaceMethodsFixup
{
	public static void Run (ContainerDefinition container)
	{
		// Run against all types we are going to generate
		foreach (var type in container.Types)
			CheckAbstractType (type);
	}

	static void CheckAbstractType (TypeDefinition type)
	{
		if (!type.IsPublic && !type.IsProtected)
			return;

		// Need to recurse through nested types
		foreach (var nested in type.NestedTypes)
			CheckAbstractType (nested);

		// Nothing to do if this isn't an abstract class
		if (!type.IsAbstract || type.IsInterface)
			return;

		var implemented_methods = type.GetImplementedMethods ().ToList ();

		foreach (var iface in type.GetImplementedInterfacesThatMustBeImplemented ())
			CheckImplementedInterface (type, iface, implemented_methods);
	}

	static void CheckImplementedInterface (TypeDefinition type, TypeDefinition iface, List<MethodDefinition> implementedMethod)
	{
		foreach (var method in iface.Methods.Where (m => !m.IsStatic && !m.IsDefaultInterfaceMethod)) {
			if (!implementedMethod.Any (m => method.AreMethodsCompatible (m)))
				implementedMethod.Add (AddAbstractMethod (type, method, iface));
		}
	}

	static MethodDefinition AddAbstractMethod (TypeDefinition type, MethodDefinition method, TypeDefinition iface)
	{
		var clone = method.CloneAsAbstract (type);

		type.Methods.Add (clone);

		return clone;
	}
}

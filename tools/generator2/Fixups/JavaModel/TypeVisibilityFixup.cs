using Javil;

namespace generator2;

// Java allows non-public types to be used as parmeters/return types/etc of
// public methods.  C# does not, so we expose the needed type (and its base
// types) as public.
static class TypeVisibilityFixup
{
	public static void Run (ContainerDefinition container)
	{
		// Some non-public types that are used by public types
		foreach (var type in container.Types.Where (t => t.IsPublic || t.IsProtected))
			FixVisibleType (type);
	}

	private static void FixVisibleType (TypeDefinition type)
	{
		if (!(type.IsPublic || type.IsProtected))
			return;

		//var base_type = type.BaseType?.Resolve ();

		FixTypeVisibility (type.BaseType?.Resolve (), type.IsPublic);

		foreach (var implements in type.ImplementedInterfaces)
			FixTypeVisibility (implements.InterfaceType.Resolve (), type.IsPublic);

		foreach (var method in type.Methods) {
			FixTypeVisibility (method.ReturnType.Resolve (), method.IsPublic);

			foreach (var p in method.Parameters)
				FixTypeVisibility (p.ParameterType.Resolve (), method.IsPublic);
		}

		//while (base_type is not null && base_type.FullName != "Java.Lang.Object") {
		//    if (type.IsPublic && !base_type.IsPublic)
		//        base_type.IsPublic = true;
		//    else if (type.IsProtected && !(base_type.IsPublic || base_type.IsProtected))
		//        base_type.IsProtected = true;

		//    base_type = base_type?.BaseType?.Resolve ();
		//}

		foreach (var nested in type.NestedTypes)
			FixVisibleType (nested);
	}

	private static void FixTypeVisibility (TypeDefinition? type, bool makePublic)
	{
		while (type is not null && type.FullName != "Java.Lang.Object") {
			if (char.IsDigit (type.Name [0]))
				return;

			if (!type.IsPublic)
				type.IsPublic = true;
			//else if (!(type.IsPublic || type.IsProtected))
			//    type.IsProtected = true;

			type = type?.BaseType?.Resolve ();
		}
	}
}

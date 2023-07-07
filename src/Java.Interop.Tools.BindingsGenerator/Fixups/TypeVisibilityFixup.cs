using Javil;

namespace Java.Interop.Tools.BindingsGenerator;

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

        //if (type.BaseType is TypeReference base_type)
        //    foreach (var t in base_type.GetReferencedTypes ())
        //        FixTypeVisibility (t.Resolve (), type.IsPublic);

        FixTypeVisibility (type.BaseType, type.IsPublic || type.IsProtected);

		foreach (var implements in type.ImplementedInterfaces)
			FixTypeVisibility (implements.InterfaceType, type.IsPublic || type.IsProtected);

        foreach (var method in type.Methods.Where (m => m.IsPublic || m.IsProtected)) {
            FixTypeVisibility (method.ReturnType, method.IsPublic || method.IsProtected);

            foreach (var p in method.Parameters)
                FixTypeVisibility (p.ParameterType, method.IsPublic || method.IsProtected);
        }

        foreach (var method in type.Fields.Where (m => m.IsPublic || m.IsProtected)) {
            FixTypeVisibility (method.FieldType, method.IsPublic || method.IsProtected);
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

        //if (type.DeclaringType?.Resolve () is TypeDefinition parent)
        FixTypeVisibility (type.DeclaringType, type.IsPublic || type.IsProtected);
	}

    private static void FixTypeVisibility (TypeReference? type, bool makePublic)
    {
        if (!makePublic || type is null || char.IsDigit (type.Name[0]))
            return;

        foreach (var rt in type.GetReferencedTypes ()) {
            if (rt.Resolve () is TypeDefinition resolved && !resolved.IsPublic) {
                resolved.IsPublic = true;
                FixVisibleType (resolved);

                if (resolved.FullName != "Java.Lang.Object")
                    FixTypeVisibility (rt, makePublic);
            }
        }
    }

    private static void FixTypeVisibility (TypeDefinition? type, bool makePublic)
	{
        if (!makePublic)
            return;

		while (type is not null && type.FullName != "Java.Lang.Object") {
			if (char.IsDigit (type.Name [0]))
				return;

            if (!type.IsPublic) {
                type.IsPublic = true;
            }
			//else if (!(type.IsPublic || type.IsProtected))
			//    type.IsProtected = true;

			type = type?.BaseType?.Resolve ();
		}
	}
}

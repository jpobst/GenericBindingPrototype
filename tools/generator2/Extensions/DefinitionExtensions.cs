using Javil;

namespace generator2;

public static class DefinitionExtensions
{
	public static bool IsDefaultInterfaceMethod (this MethodDefinition method) => !method.IsAbstract && !method.IsStatic;

	public static bool IsPublicApi (this TypeDefinition type) => type.IsPublic || type.IsProtected;

	public static bool IsPublicApi (this FieldDefinition field) => field.IsPublic || field.IsProtected;

	public static bool IsPublicApi (this MethodDefinition method) => (method.IsPublic || method.IsProtected) && !method.IsBridge && !method.IsSynthetic;

	public static bool ShouldBindFieldAsConstant (this FieldDefinition field) => field.IsPublicApi () && field.IsConstant;

	public static bool ShouldBindFieldAsProperty (this FieldDefinition field) => field.IsPublicApi () && !field.IsConstant;

	public static bool ShouldBindInterfaceMethodAsDeclaration (this MethodDefinition method) => method.IsPublicApi () && !method.IsStatic && !method.IsDefaultInterfaceMethod ();

	public static bool ShouldBindInterfaceMethodAsImplementation (this MethodDefinition method) => method.IsPublicApi () && (method.IsStatic || method.IsDefaultInterfaceMethod ());

	public static bool ShouldBindMethod (this MethodDefinition method) => method.IsPublicApi () && !method.IsBridge && !method.IsConstructor;

	public static bool ShouldBindMethodAsConstructor (this MethodDefinition method) => method.IsPublicApi () && !method.IsBridge && method.IsConstructor;

	public static MethodDefinition Clone (this MethodDefinition method, TypeDefinition declaringType)
	{
		var m = new MethodDefinition (method.Name, method.ReturnType, declaringType) {
			IsPublic = method.IsPublic,
			IsPrivate = method.IsPrivate,
			IsProtected = method.IsProtected,
			IsStatic = method.IsStatic,
			IsFinal = method.IsFinal,
			IsSynchronized = method.IsSynchronized,
			IsBridge = method.IsBridge,
			IsVarargs = method.IsVarargs,
			IsNative = method.IsNative,
			IsAbstract = method.IsAbstract,
			IsStrict = method.IsStrict,
			IsSynthetic = method.IsSynthetic,
			IsDeprecated = method.IsDeprecated,
			ReturnTypeNullability = method.ReturnTypeNullability
		};

		// TODO: Need to clone collection objects
		if (method.HasCheckedExceptions)
			foreach (var c in method.CheckedExceptions)
				m.CheckedExceptions.Add (c);

		if (method.HasGenericParameters)
			foreach (var g in method.GenericParameters)
				m.GenericParameters.Add (g);

		if (method.HasParameters)
			foreach (var p in method.Parameters)
				m.Parameters.Add (p);

		return m;
	}
}

using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator;

public class BindingsWriter
{
	private readonly string output_dir;

	public BindingsWriter (string outputDir)
	{
		output_dir = outputDir;
	}

	public void WriteProject (ContainerDefinition container)
	{
		MicrosoftAndroidFixups.ApplyContainerFixups (container);
		TypeVisibilityFixup.Run (container);
		InterfaceCovariantReturnTypeFixups.Run (container);
		AbstractClassMustImplementInterfaceMethodsFixup.Run (container);
		DefaultInterfaceMembersOverrides.Run (container);

		//Parallel.ForEach (container.Types.Where (t => t.IsPublic || t.IsProtected).ToList (), (type) => { WriteType (type); });

		foreach (var type in container.Types.Where (t => t.IsPublic || t.IsProtected))
			WriteType (type);
	}

	private void WriteType (TypeDefinition type)
	{
		using var writer = new CodeWriter (Path.Combine (output_dir, type.GetNamespace () + "." + type.Name + ".cs"));

		writer.WriteLine ($"namespace {type.GetNamespace ()};");
		writer.WriteLine ();

		foreach (var t in CreateType (type)) {
			MicrosoftAndroidFixups.ApplyTypeFixups (t);
			JavaBaseFixups.ApplyTypeFixups (t);
			t.Write (writer);
		}
	}

	private IEnumerable<TypeWriter> CreateType (TypeDefinition type)
	{
		if (type.IsInterface) {
			foreach (var i in CreateInterface (type))
				yield return i;
			yield break;
		}

		foreach (var c in CreateClass (type))
			yield return c;
	}

	private IEnumerable<ClassWriter> CreateClass (TypeDefinition type)
	{
		var klass = BoundClass.Create (type);

		// We always nest types in the non-generic interface
		ClassWriter nest_parent = klass.NonGenericClass is null ? klass : klass.NonGenericClass;

		foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
			nest_parent.NestedTypes.AddRange (CreateType (nested));

		yield return klass;

		if (klass.NonGenericClass is not null)
			yield return klass.NonGenericClass;
	}

	private IEnumerable<InterfaceWriter> CreateInterface (TypeDefinition type)
	{
		var iface = BoundInterface.Create (type);

		// We always nest types in the non-generic interface
		InterfaceWriter nest_parent = iface.GenericInterfaceAlternative is null ? iface : iface.GenericInterfaceAlternative;

		foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
			nest_parent.NestedTypes.AddRange (CreateType (nested));

		yield return iface;

		if (iface.GenericInterfaceAlternative is GenericInterfaceAlternative gia)
			yield return gia;
	}
}

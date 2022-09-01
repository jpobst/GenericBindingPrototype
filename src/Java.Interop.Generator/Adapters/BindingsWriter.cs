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
		InterfaceCovariantReturnTypeFixup.Run (container);
		AbstractClassMustImplementInterfaceMethodsFixup.Run (container);
		DefaultInterfaceMembersExplicitInterfaceDeclarationFixup.Run (container);

		// Building the managed model is a two-step process. This is because we need *all*
		// type definition objects created before we can resolve things like base types to type references.
		var m = new ManagedProjectReferenceModel ();

		// Create the bare-bones type model
		foreach (var type in container.Types.Where (t => t.IsPublic || t.IsProtected))
			m.AddTypes (type.GetNamespace (), CreateManagedType (type));

		// Perform fixups
		MoveTypesNestedInGenericTypesFixup.Fixup (m);

		// Populate the rest of the type model, resolving types involved in base types, implemented interfaces, etc.
		foreach (var type in m.GetAllTypes ())
			type.Populate ();

		MissedGenericTypesFixup.Fixup (m);

		// Populate the type members
		foreach (var type in m.GetAllTypes ())
			type.PopulateMembers ();

		// Write generated source to files
		foreach (var type in m.GetAllTypes ())
			WriteManagedType (type);

		return;
		// Create the managed model
		var model = new ManagedProjectModel ();

		foreach (var type in container.Types.Where (t => t.IsPublic || t.IsProtected))
			model.AddTypes (type.GetNamespace (), CreateType (type).ToArray ());

		foreach (var type in model.GetAllTypes ().OfType<IManagedTypeModel> ())
			type.PopulateMembers ();

		// Apply managed fixups
		foreach (var type in model.GetAllTypes ()) {
			MicrosoftAndroidFixups.ApplyTypeFixups (type);
			JavaBaseFixups.ApplyTypeFixups (type);
		}

		// Write generated source to files
		foreach (var type in model.GetAllTypes ())
			WriteType (type);

		//Parallel.ForEach (container.Types.Where (t => t.IsPublic || t.IsProtected).ToList (), (type) => { WriteType (type); });
		//foreach (var type in container.Types.Where (t => t.IsPublic || t.IsProtected))
		//WriteType (type);
	}


	public ManagedTypeDefinition CreateManagedType (TypeDefinition type, ManagedTypeDefinition? declaringType = null)
	{
		if (type.IsInterface)
			return CreateManagedInterface (type, declaringType);

		return CreateManagedClass (type, declaringType);
	}


	ManagedTypeDefinition CreateManagedClass (TypeDefinition type, ManagedTypeDefinition? declaringType = null)
	{
		var td = new ManagedTypeDefinition (type.GetName (), declaringType, type);

		foreach (var gp in type.GenericParameters)
			td.GenericParameters.Add (new ManagedGenericParameter (gp.Name, td));

		foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
			td.NestedTypes.Add (CreateManagedType (nested, td));

		return td;
		//var klass = BoundClass.Create (type);

			//// We always nest types in the non-generic interface
			//ClassWriter nest_parent = klass.NonGenericClass is null ? klass : klass.NonGenericClass;

			//foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
			//	nest_parent.NestedTypes.AddRange (CreateType (nested));

			//yield return klass;

			//if (klass.NonGenericClass is not null)
			//	yield return klass.NonGenericClass;
	}

	ManagedTypeDefinition CreateManagedInterface (TypeDefinition type, ManagedTypeDefinition? declaringType = null)
	{
		var td = new ManagedTypeDefinition (type.GetName (), declaringType, type);

		foreach (var gp in type.GenericParameters)
			td.GenericParameters.Add (new ManagedGenericParameter (gp.Name, td));

		foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
			td.NestedTypes.Add (CreateManagedType (nested, td));

		return td;
		//var iface = BoundInterface.Create (type);

		//// We always nest types in the non-generic interface
		//InterfaceWriter nest_parent = iface.GenericInterfaceAlternative is null ? iface : iface.GenericInterfaceAlternative;

		//foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
		//	nest_parent.NestedTypes.AddRange (CreateType (nested));

		//yield return iface;

		//if (iface.GenericInterfaceAlternative is GenericInterfaceAlternative gia)
		//	yield return gia;
	}

	void WriteType (TypeDefinition type)
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

	void WriteManagedType (ManagedTypeDefinition type)
	{
		using var writer = new CodeWriter (Path.Combine (output_dir, type.Namespace + "." + type.ArityName + ".cs"));

		writer.WriteLine ($"namespace {type.Namespace};");
		writer.WriteLine ();

		ManagedClassWriterAdapter.Create (type).Write (writer);
		//foreach (var t in CreateType (type)) {
		//	MicrosoftAndroidFixups.ApplyTypeFixups (t);
		//	JavaBaseFixups.ApplyTypeFixups (t);
		//	t.Write (writer);
		//}
	}

	void WriteType (TypeWriter type)
	{
		using var writer = new CodeWriter (Path.Combine (output_dir, GetFileName (type)));

		writer.WriteLine ($"namespace {type.GetNamespace ()};");
		writer.WriteLine ();

		type.Write (writer);
	}

	static string GetFileName (TypeWriter type)
	{
		var name = type.GetNamespace () + "." + type.Name;

		if (type.GenericParameters.Any ())
			name += "`" + type.GenericParameters.Count.ToString ();

		name += ".cs";

		return name;
	}

	public IEnumerable<TypeWriter> CreateType (TypeDefinition type)
	{
		if (type.IsInterface) {
			foreach (var i in CreateInterface (type))
				yield return i;

			yield break;
		}

		foreach (var c in CreateClass (type))
			yield return c;
	}

	IEnumerable<ClassWriter> CreateClass (TypeDefinition type)
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

	IEnumerable<InterfaceWriter> CreateInterface (TypeDefinition type)
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

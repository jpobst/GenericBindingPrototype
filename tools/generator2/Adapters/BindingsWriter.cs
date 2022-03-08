using Javil;
using Xamarin.SourceWriter;

namespace generator2;

internal class BindingsWriter
{
	private string output_dir;

	public BindingsWriter (string outputDir)
	{
		output_dir = outputDir;
	}

	public void WriteProject (ContainerDefinition container)
	{
		MicrosoftAndroidFixups.ApplyContainerFixups (container);
		TypeVisibilityFixup.Run (container);

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
		var t = new ClassWriter {
			Name = type.GetName ()
		};

		if (type.IsPublic)
			t.IsPublic = true;
		else if (type.IsProtected)
			t.IsProtected = true;

		t.IsAbstract = type.IsAbstract;
		t.IsSealed = type.IsFinal;

		t.Inherits = FormatExtensions.FormatTypeReference (type.BaseType);

		if (type.HasGenericParameters)
			foreach (var tp in type.GenericParameters)
				t.GenericParameters.Add (tp.Name);

		foreach (var iface in type.ImplementedInterfaces)
			t.Implements.Add (FormatExtensions.FormatTypeReference (iface.InterfaceType));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => f.IsConstant && (f.IsPublic || f.IsProtected)))
			t.Fields.Add (CreateField (field));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => !f.IsConstant && (f.IsPublic || f.IsProtected)))
			t.Properties.Add (CreateFieldAsProperty (field));

		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (f => (f.IsPublic || f.IsProtected) && f.Name != "values" && !f.IsBridge && !f.IsConstructor))
			if (CreateMethod (method, type) is MethodWriter m)
				t.Methods.Add (m);

		if (type.HasGenericParameters && type.NestedTypes.Where (t => t.IsPublic || t.IsProtected).Any ()) {
			var non_generic = new ClassWriter {
				Name = type.Name,
				IsAbstract = true
			};

			if (type.IsPublic)
				non_generic.IsPublic = true;
			else if (type.IsProtected)
				non_generic.IsProtected = true;

			foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
				non_generic.NestedTypes.AddRange (CreateType (nested));

			yield return non_generic;
		} else {
			foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
				t.NestedTypes.AddRange (CreateType (nested));
		}

		yield return t;
	}

	private IEnumerable<InterfaceWriter> CreateInterface (TypeDefinition type)
	{
		var t = new InterfaceWriter {
			Name = type.GetName (),
			IsPublic = true
		};

		if (type.HasGenericParameters)
			foreach (var tp in type.GenericParameters)
				t.GenericParameters.Add (tp.Name);

		foreach (var iface in type.ImplementedInterfaces)
			t.Implements.Add (FormatExtensions.FormatTypeReference (iface.InterfaceType));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => f.IsConstant && (f.IsPublic || f.IsProtected)))
			t.Fields.Add (CreateField (field));

		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => !f.IsConstant && (f.IsPublic || f.IsProtected)))
			t.Properties.Add (CreateFieldAsProperty (field));

		if (type.HasGenericParameters && type.NestedTypes.Where (t => t.IsPublic || t.IsProtected).Any ()) {
			var non_generic = new InterfaceWriter {
				Name = type.Name,
				IsPublic = true
			};

			foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
				non_generic.NestedTypes.AddRange (CreateType (nested));

			yield return non_generic;
		} else {
			foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
				t.NestedTypes.AddRange (CreateType (nested));
		}

		yield return t;
	}

	private FieldWriter CreateField (FieldDefinition field)
	{
		var f = new FieldWriter {
			Name = field.GetName ()
		};

		if (field.IsPublic)
			f.IsPublic = true;

		if (field.IsProtected)
			f.IsProtected = true;

		f.IsConst = field.IsConstant;
		f.IsStatic = field.IsStatic && !field.IsConstant;

		f.Type = new TypeReferenceWriter (FormatExtensions.FormatTypeReference (field.FieldType));

		if (field.IsConstant)
			f.Value = FormatExtensions.SerializeConstantValue (field.Value, field.FieldType.Name);

		return f;
	}

	private PropertyWriter CreateFieldAsProperty (FieldDefinition field)
	{
		var p = new PropertyWriter {
			Name = field.GetName ()
		};

		if (field.IsPublic)
			p.IsPublic = true;

		if (field.IsProtected)
			p.IsProtected = true;

		p.IsStatic = field.IsStatic && !field.IsConstant;

		p.PropertyType = new TypeReferenceWriter (FormatExtensions.FormatTypeReference (field.FieldType));

		p.HasGet = true;
		p.HasSet = true;

		p.GetBody.Add ("throw new NotImplementedException ();");

		return p;
	}

	private MethodWriter? CreateMethod (MethodDefinition method, TypeDefinition type)
	{
		var m = new MethodWriter {
			Name = method.GetName ()
		};

		var base_method = method.FindDeclaredBaseMethodOrDefault ();
		var effective_method = method;
		var effective_return_type = method.ReturnType;

		if (base_method is not null && !method.IsStatic) {
			m.IsOverride = true;
			effective_method = base_method;

			if (method.ReturnType.IsArray && !effective_method.ReturnType.IsArray)
				effective_return_type = effective_method.ReturnType;
			else if ((method.ReturnType as GenericInstanceType)?.GenericArguments.Any () == true && method.ReturnType.FullName != effective_method.ReturnType.FullName && !((effective_method.ReturnType as GenericInstanceType)?.GenericArguments.Any () == true))
				effective_return_type = effective_method.ReturnType;

		}

		if (type.IsAbstract && method.IsAbstract && m.IsOverride)
			return null;

		if (effective_method.IsPublic)
			m.IsPublic = true;

		if (effective_method.IsProtected)
			m.IsProtected = true;

		if (!method.IsFinal && !method.IsStatic && !type.IsFinal && !m.IsOverride)
			m.IsVirtual = true;

		if (!m.IsPublic && !m.IsProtected)
			return null;

		m.IsStatic = method.IsStatic;
		m.IsAbstract = method.IsAbstract;

		m.ReturnType = new TypeReferenceWriter (FormatExtensions.FormatTypeReference (effective_return_type));

		if (method.HasParameters)
			foreach (var p in method.Parameters)
				m.Parameters.Add (new MethodParameterWriter (p.GetName (), new TypeReferenceWriter (FormatExtensions.FormatTypeReference (p.ParameterType))));

		if (!m.IsAbstract)
			m.Body.Add ("throw new NotImplementedException ();");

		return m;
	}
}

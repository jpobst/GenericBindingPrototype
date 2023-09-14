using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class BoundConstructor : ConstructorWriter
{
	private GeneratorSettings settings;

	public bool IsNonStaticNestedType { get; set; }
	public required string JniSignature { get; set; }

	public BoundConstructor (GeneratorSettings settings)
	{
		this.settings = settings;
	}

	public static BoundConstructor? Create (MethodDefinition method, TypeDefinition type, GeneratorSettings settings)
	{
		var ctor = new BoundConstructor (settings) {
			Name = type.GetManagedName (settings),
			IsUnsafe = true,
			// not a beautiful way to check static type, yes :|
			IsNonStaticNestedType = type.IsNested && !(type.IsAbstract && type.IsFinal),
			JniSignature = method.GetDescriptorGenericsErased (),
			settings = settings
		};

		if (method.IsPublic)
			ctor.IsPublic = true;

		if (method.IsProtected)
			ctor.IsProtected = true;

		// If 'elem' is a constructor for a non-static nested type, then
		// the type of the containing class must be inserted as the first argument
		if (ctor.IsNonStaticNestedType)
			ctor.Parameters.Add (new MethodParameterWriter (type.GetManagedName (settings), new TypeReferenceWriter (FormatExtensions.FormatTypeReference (type.DeclaringType, settings))));

		if (method.HasParameters)
			foreach (var p in method.Parameters)
				ctor.Parameters.Add (new MethodParameterWriter (p.GetManagedName (settings), new TypeReferenceWriter (FormatExtensions.FormatTypeReference (p.ParameterType, settings))));

		//if (!settings.GenerateStubsOnly)
			ctor.BaseCall = $"base (ref *InvalidJniObjectReference, JniObjectReferenceOptions.None)";

		return ctor;
	}

	protected override void WriteBody (CodeWriter writer)
	{
		if (settings.GenerateStubsOnly)
			return;

		var id_type = IsNonStaticNestedType ? "var" : "const string";
		var id = JniSignature;// IsNonStaticNestedType
			    //? "(" + constructor.Parameters.GetJniNestedDerivedSignature (opt) + ")V"
			    //: constructor.JniSignature;

		//writer.WriteLine ($"{id_type} __id = \"{id}\";");
		writer.WriteLine ();

		writer.WriteLine ($"if (PeerReference.IsValid)");
		writer.WriteIndentedLine ("return;");

		writer.WriteLine ();

		//foreach (var prep in constructor.Parameters.GetCallPrep (opt))
		//	writer.WriteLine (prep);

		writer.WriteLine ("try {");
		writer.Indent ();

		//WriteParamterListCallArgs (writer, constructor.Parameters, false, opt);

		//writer.WriteLine ("var __r = _members.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (){0});", constructor.Parameters.GetCallArgs (opt, invoker: false));
		//writer.WriteLine ("Construct (ref __r, JniObjectReferenceOptions.CopyAndDispose);");
		//writer.WriteLine ("_members.InstanceMethods.FinishCreateInstance (__id, this{0});", constructor.Parameters.GetCallArgs (opt, invoker: false));
		writer.Unindent ();

		writer.WriteLine ("} finally {");

		writer.Indent ();
		//var call_cleanup = constructor.Parameters.GetCallCleanup (opt);

		//foreach (string cleanup in call_cleanup)
		//	writer.WriteLine (cleanup);

		//foreach (var p in constructor.Parameters.Where (para => para.ShouldGenerateKeepAlive ()))
		//	writer.WriteLine ($"global::System.GC.KeepAlive ({opt.GetSafeIdentifier (p.Name)});");

		writer.Unindent ();

		writer.WriteLine ("}");
	}
}

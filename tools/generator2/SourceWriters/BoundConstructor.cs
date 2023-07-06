using Javil;
using Xamarin.SourceWriter;

namespace generator2;

class BoundConstructor : ConstructorWriter
{
	public bool IsNonStaticNestedType { get; set; }
	public required string JniSignature { get; set; }

	public static BoundConstructor? Create (MethodDefinition method, TypeDefinition type)
	{
		var ctor = new BoundConstructor {
			Name = type.GetName (),
			IsUnsafe = true,
			// not a beautiful way to check static type, yes :|
			IsNonStaticNestedType = type.IsNested && !(type.IsAbstract && type.IsFinal),
			JniSignature = method.GetDescriptorGenericsErased (),
		};

		if (method.IsPublic)
			ctor.IsPublic = true;

		if (method.IsProtected)
			ctor.IsProtected = true;

		// If 'elem' is a constructor for a non-static nested type, then
		// the type of the containing class must be inserted as the first argument
		if (ctor.IsNonStaticNestedType)
			ctor.Parameters.Add (new MethodParameterWriter (type.GetName (), new TypeReferenceWriter (FormatExtensions.FormatTypeReference (type.DeclaringType))));

		if (method.HasParameters)
			foreach (var p in method.Parameters)
				ctor.Parameters.Add (new MethodParameterWriter (p.GetName (), new TypeReferenceWriter (FormatExtensions.FormatTypeReference (p.ParameterType))));

		ctor.BaseCall = $"base (ref *InvalidJniObjectReference, JniObjectReferenceOptions.None)";

		return ctor;
	}

	protected override void WriteBody (CodeWriter writer)
	{
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

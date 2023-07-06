using Javil;
using Xamarin.SourceWriter;

namespace generator2;

// These are fixups specific to 'java.base.jmod'. Eventually we would either handle them
// automatically or they would be fixed with metadata.
static class JavaBaseFixups
{
	public static void ApplyContainerFixups (ContainerDefinition container)
	{
		// Some name collisions
		container.RenameField ("sun.nio.cs.DoubleByte.Encoder", "sgp", "sgpField");
		container.RenameField ("sun.security.ssl.SSLLogger", "isOn", "isOnField");

		// Some super tricky generics interface implementations (covariant)
		container.HideType ("java.util.concurrent.ConcurrentSkipListMap");
		container.HideType ("java.util.concurrent.ConcurrentSkipListSet");
		//container.HideType ("java.util.Spliterator$OfDouble");
	}

	public static void ApplyTypeFixups (TypeWriter type)
	{
		// TODO: This should be more specific (include namespace and method parameters)
		// Generic covariant return types
		type.SetMethodReturnType ("SSLServerCertStore", "engineGetCertificates", "global::Java.Util.Collection<global::Java.Security.Cert.Certificate>");
		type.SetMethodReturnType ("SSLServerCertStore", "engineGetCRLs", "global::Java.Util.Collection<global::Java.Security.Cert.CRL>");
		type.SetMethodReturnType ("Sub", "key", "global::Java.Lang.Object");
		type.SetMethodReturnType ("X509CertPath", "getCertificates", "global::Java.Util.List<global::Java.Security.Cert.Certificate>");


		// Missed overrides
		//type.SetMethodIsOverride ("BasicInterpreter", "copyOperation");
		//type.SetMethodIsOverride ("BasicInterpreter", "unaryOperation");
		//type.SetMethodIsOverride ("BasicInterpreter", "binaryOperation");
		//type.SetMethodIsOverride ("BasicInterpreter", "ternaryOperation");
		//type.SetMethodIsOverride ("BasicInterpreter", "naryOperation");
		//type.SetMethodIsOverride ("BasicInterpreter", "returnOperation");
		//type.SetMethodIsOverride ("BasicInterpreter", "merge");

		//type.SetMethodIsOverride ("SourceInterpreter", "copyOperation");
		//type.SetMethodIsOverride ("SourceInterpreter", "unaryOperation");
		//type.SetMethodIsOverride ("SourceInterpreter", "binaryOperation");
		//type.SetMethodIsOverride ("SourceInterpreter", "ternaryOperation");
		//type.SetMethodIsOverride ("SourceInterpreter", "naryOperation");
		//type.SetMethodIsOverride ("SourceInterpreter", "returnOperation");
		//type.SetMethodIsOverride ("SourceInterpreter", "merge");

		//type.SetMethodIsOverride ("Completion", "setRawResult");
		//type.SetMethodIsProtected ("Completion", "setRawResult");

		foreach (var nested in type.NestedTypes)
			ApplyTypeFixups (nested);
	}
}

using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

// These are fixups specific to 'java.base.jmod'. Eventually we would either handle them
// automatically or they would be fixed with metadata.
static class JavaBaseFixups
{
	public static void ApplyContainerFixups (ContainerDefinition container)
	{
		// Some name collisions
		container.RenameField ("sun.nio.cs.DoubleByte.Encoder", "sgp", "sgpField");
		container.RenameField ("sun.security.ssl.SSLLogger", "isOn", "isOnField");

		container.RenameType ("java.util.concurrent.locks.ReentrantReadWriteLock$WriteLock", "ReentrantWriteLock");
		container.RenameType ("java.util.concurrent.locks.ReentrantReadWriteLock$ReadLock", "ReentrantReadLock");

		// Some super tricky generics interface implementations (covariant)
		container.HideType ("java.util.concurrent.ConcurrentSkipListMap");
		container.HideType ("java.util.concurrent.ConcurrentSkipListSet");
		//container.HideType ("java.util.Spliterator$OfDouble");
	}

	public static void ApplyTypeFixups (IEnumerable<TypeWriter> types)
	{
		foreach (var type in types) {
			// TODO: This should be more specific (include namespace and method parameters)
			// Generic covariant return types
			type.SetMethodReturnType ("SSLServerCertStore", "engineGetCertificates", "global::Java.Util.ICollection<global::Java.Security.Cert.Certificate>");
			type.SetMethodReturnType ("SSLServerCertStore", "engineGetCRLs", "global::Java.Util.ICollection<global::Java.Security.Cert.CRL>");
			type.SetMethodReturnType ("Sub", "key", "global::Java.Lang.Object");
			type.SetMethodReturnType ("X509CertPath", "getCertificates", "global::Java.Util.IList<global::Java.Security.Cert.Certificate>");

			ApplyTypeFixups (type.NestedTypes);
		}
	}
}

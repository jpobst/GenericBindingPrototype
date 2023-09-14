using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

// These are fixups specific to 'android.jar'. Eventually we would either handle them
// automatically or they would be fixed with metadata.
static class MicrosoftAndroidFixups
{
	public static void ApplyContainerFixups (ContainerDefinition container)
	{
		// Some name collisions
		container.RenameField ("java.io.ByteArrayInputStream", "mark", "markField");
		container.RenameField ("java.util.Calendar", "isSet", "isSetField");
	}

	public static void ApplyTypeFixups (IEnumerable<TypeWriter> types)
	{
		foreach (var type in types) {
			// Unsupported covariant return types
			type.SetMethodReturnType ("Constructor", "getDeclaringClass", "Java.Lang.Class<Java.Lang.Object>");
			type.SetMethodReturnType ("JarFile", "entries", "Java.Util.IEnumeration<Java.Util.Zip.ZipEntry>");
			type.SetMethodReturnType ("JarFile", "stream", "Java.Util.Stream.IStream<Java.Util.Zip.ZipEntry>");

			ApplyTypeFixups (type.NestedTypes);
		}
	}
}

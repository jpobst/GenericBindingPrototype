using Javil;
using Xamarin.SourceWriter;

namespace generator2;

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

	public static void ApplyTypeFixups (TypeWriter type)
	{
		// Unsupported covariant return types
		type.SetMethodReturnType ("Constructor", "getDeclaringClass", "Java.Lang.Class<Java.Lang.Object>");
		type.SetMethodReturnType ("JarFile", "entries", "Java.Util.Enumeration<Java.Util.Zip.ZipEntry>");
		type.SetMethodReturnType ("JarFile", "stream", "Java.Util.Stream.Stream<Java.Util.Zip.ZipEntry>");

		// Missed overrides
		type.SetMethodIsOverride ("AbsSpinner", "setAdapter");
		type.SetMethodIsOverride ("AbsListView", "setAdapter");
		type.SetMethodIsOverride ("AdapterViewAnimator", "setAdapter");
		type.SetMethodIsOverride ("CountedCompleter", "setRawResult");
		type.SetMethodIsOverride ("RecursiveAction", "setRawResult");

		foreach (var nested in type.NestedTypes)
			ApplyTypeFixups (nested);
	}
}

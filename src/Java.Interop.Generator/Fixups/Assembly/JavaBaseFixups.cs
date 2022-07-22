using Xamarin.SourceWriter;

namespace Java.Interop.Generator;

// These are fixups specific to 'java.base.jmod'. Eventually we would either handle them
// automatically or they would be fixed with metadata.
static class JavaBaseFixups
{
	public static void ApplyTypeFixups (TypeWriter type)
	{
		// Missed overrides
		type.SetMethodIsOverride ("BasicInterpreter", "copyOperation");
		type.SetMethodIsOverride ("BasicInterpreter", "unaryOperation");
		type.SetMethodIsOverride ("BasicInterpreter", "binaryOperation");
		type.SetMethodIsOverride ("BasicInterpreter", "ternaryOperation");
		type.SetMethodIsOverride ("BasicInterpreter", "naryOperation");
		type.SetMethodIsOverride ("BasicInterpreter", "returnOperation");
		type.SetMethodIsOverride ("BasicInterpreter", "merge");

		type.SetMethodIsOverride ("SourceInterpreter", "copyOperation");
		type.SetMethodIsOverride ("SourceInterpreter", "unaryOperation");
		type.SetMethodIsOverride ("SourceInterpreter", "binaryOperation");
		type.SetMethodIsOverride ("SourceInterpreter", "ternaryOperation");
		type.SetMethodIsOverride ("SourceInterpreter", "naryOperation");
		type.SetMethodIsOverride ("SourceInterpreter", "returnOperation");
		type.SetMethodIsOverride ("SourceInterpreter", "merge");

		type.SetMethodIsOverride ("Completion", "setRawResult");
		type.SetMethodIsProtected ("Completion", "setRawResult");

		foreach (var nested in type.NestedTypes)
			ApplyTypeFixups (nested);
	}
}

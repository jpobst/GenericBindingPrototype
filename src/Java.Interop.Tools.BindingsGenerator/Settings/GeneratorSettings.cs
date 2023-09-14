namespace Java.Interop.Tools.BindingsGenerator;

public class GeneratorSettings
{
	/// <summary>
	/// The .jar/.jmod/.xml file to generate bindings for.
	/// </summary>
	public string InputFile { get; set; } = string.Empty;

	/// <summary>
	/// The directory to place the generated C# files.
	/// </summary>
	public string OutputDirectory { get; set; } = string.Empty;

	/// <summary>
	/// Only generate API stubs, does not generate method bodies.
	/// </summary>
	public bool GenerateStubsOnly { get; set; }

	/// <summary>
	/// Strategy to use for method capitalization.
	/// </summary>
	public MethodCapitalizationStrategy MethodCapitalizationStrategy { get; set; } = MethodCapitalizationStrategy.None;

	/// <summary>
	/// Rename nested types that have the same name as a method.
	/// </summary>
	public bool FixNestedTypeNameCollisions { get; set; } = false;

	/// <summary>
	/// Whether to annotate types with nullable reference type annotations.
	/// </summary>
	public bool UseNullableReferenceTypes { get; set; } = true;

	/// <summary>
	/// Specifies whether timing information should be logged.
	/// </summary>
	public bool LogTimingInformation { get; set; } = true;

	internal IDisposable LogTiming (string message) =>
		new TimingLogger (message, LogTimingInformation);
}

using System;
using System.CommandLine;
using System.Diagnostics;
using Javil;

namespace generator2;

public class Program
{
	public static async Task Main (params string [] args)
	{
		var root_command = new RootCommand ("Utility for creating a C# binding of a Java library.");

		var input_option = new Option<string> ("--input", "The .jar or .jmod to bind.") { IsRequired = true };
		var output_option = new Option<string> ("--output", "Output directory for generated source.") { IsRequired = true };

		root_command.Add (input_option);
		root_command.Add (output_option);

		root_command.SetHandler (
			GenerateBinding,
			input_option, output_option
		);

		await root_command.InvokeAsync (args);
	}

	public static void GenerateBinding (string jar, string outputDir)
	{
		// Start from a clean output directory
		if (Directory.Exists (outputDir))
			Directory.Delete (outputDir, true);

		Directory.CreateDirectory (outputDir);

		var sw = Stopwatch.StartNew ();

		// Read the jar
		var container = ContainerDefinition.ReadContainer (jar);

		Console.WriteLine ($"Parse: {sw.ElapsedMilliseconds}ms");
		sw.Restart ();

		// Generate the binding
		var pw = new BindingsWriter (outputDir);
		pw.WriteProject (container);

		Console.WriteLine ($"Generate: {sw.ElapsedMilliseconds}ms");
	}
}

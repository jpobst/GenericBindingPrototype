using System.Diagnostics;
using Java.Interop.Generator;
using Javil;

namespace generator2;

public class Program
{
	public static void Main ()
	{
		// Get from 'tools/generator2/bin/Debug/net6.0' back to the project root
		var repo_root = Path.Combine ("..", "..", "..", "..", "..");

		var jar_file = Path.Combine (repo_root, "lib", "android.jar");
		var output_dir = Path.Combine (repo_root, "output", "Microsoft.Android", "generated");

		GenerateBinding (jar_file, output_dir);
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
		var bw = new BindingsWriter (outputDir);
		bw.WriteProject (container);

		Console.WriteLine ($"Generate: {sw.ElapsedMilliseconds}ms");
	}
}

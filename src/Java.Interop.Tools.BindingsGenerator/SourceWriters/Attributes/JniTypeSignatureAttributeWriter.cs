using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

// [global::Java.Interop.JniTypeSignature ("java/io/File", GenerateJavaPeer=false)]
class JniTypeSignatureAttributeWriter : AttributeWriter
{
	public string Signature { get; }
	public bool GenerateJavaPeer { get; }

	public JniTypeSignatureAttributeWriter (string signature, bool generateJavaPeer)
	{
		Signature = signature;
		GenerateJavaPeer = generateJavaPeer;
	}

	public override void WriteAttribute (CodeWriter writer)
	{
		writer.WriteLine ($"[global::Java.Interop.JniTypeSignature (\"{Signature}\", GenerateJavaPeer={GenerateJavaPeer.ToString ().ToLowerInvariant ()})]");
	}

	public static JniTypeSignatureAttributeWriter Create (TypeDefinition type) => new JniTypeSignatureAttributeWriter (type.FullNameGenericsErased.Replace ('.', '/'), false);
}

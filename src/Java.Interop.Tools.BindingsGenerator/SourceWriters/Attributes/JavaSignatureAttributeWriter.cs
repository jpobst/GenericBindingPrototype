using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class JavaSignatureAttributeWriter : AttributeWriter
{
	public string Signature { get; }

	public JavaSignatureAttributeWriter (string signature)
	{
		Signature = signature;
	}

	public override void WriteAttribute (CodeWriter writer)
	{
		writer.WriteLine ($"[global::Microsoft.Android.JavaSignature (\"{Signature}\")]");
	}

	public static JavaSignatureAttributeWriter Create (TypeDefinition type) => new JavaSignatureAttributeWriter (type.JniFullName);
}

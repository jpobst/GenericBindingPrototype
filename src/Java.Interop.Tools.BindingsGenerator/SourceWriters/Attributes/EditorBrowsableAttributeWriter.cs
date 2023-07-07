using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

public class EditorBrowsableAttributeWriter : AttributeWriter
{
	public override void WriteAttribute (CodeWriter writer)
	{
		writer.WriteLine ("[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]");
	}
}

using Xamarin.SourceWriter;

namespace generator2;

public class DebuggerBrowsableAttributeWriter : AttributeWriter
{
	public override void WriteAttribute (CodeWriter writer)
	{
		writer.WriteLine ("[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]");
	}
}

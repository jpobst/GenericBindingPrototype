using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

public class DebuggerBrowsableAttributeWriter : AttributeWriter
{
	public override void WriteAttribute (CodeWriter writer)
	{
		writer.WriteLine ("[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]");
	}
}

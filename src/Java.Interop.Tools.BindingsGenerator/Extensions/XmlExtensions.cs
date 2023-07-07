using System.Xml;

namespace Java.Interop.Tools.BindingsGenerator;

static class XmlExtensions
{
	public static void WriteAttributeStringIf (this XmlWriter writer, bool condition, string localName, string? value)
	{
		if (condition)
			writer.WriteAttributeString (localName, value);
	}
}

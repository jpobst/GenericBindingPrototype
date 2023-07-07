namespace Java.Interop.Tools.BindingsGenerator;

static class CSharpFacts
{
	// Note this list must be sorted.
	private static string [] reserved_keywords = new [] {
	    "abstract", "as", "base", "bool", "break", "byte", "callback", "case", "catch", "char", "checked", "class", "const",
	    "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false",
	    "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal",
	    "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private",
	    "protected", "public", "readonly", "ref", "remove", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
	    "struct", "switch", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
	    "using", "virtual", "void", "volatile", "where", "while",
	};

	public static bool IsReservedKeyword (string s) => Array.BinarySearch (reserved_keywords, s) >= 0;
}

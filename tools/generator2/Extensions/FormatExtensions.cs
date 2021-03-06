using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Javil;

namespace generator2;

static class FormatExtensions
{
	public static string? SerializeConstantValue (object? value, string typeName)
	{
		if (value is null)
			return null;

		if (value is double doubleItem) {
			if (double.IsNaN (doubleItem))
				return "(0.0 / 0.0)";
			else if (double.IsNegativeInfinity (doubleItem))
				return "(-1.0 / 0.0)";
			else if (double.IsPositiveInfinity (doubleItem))
				return "(1.0 / 0.0)";
			else
				return doubleItem.ToString ("G17", CultureInfo.InvariantCulture);
		}

		if (value is float floatItem) {
			if (double.IsNaN (floatItem))
				return "(0.0f / 0.0f)";
			else if (double.IsNegativeInfinity (floatItem))
				return "(-1.0f / 0.0f)";
			else if (double.IsPositiveInfinity (floatItem))
				return "(1.0f / 0.0f)";
			else
				return floatItem.ToString ("G9", CultureInfo.InvariantCulture) + "F";
		}

		if (value is long)
			return value.ToString ();

		if (value is bool boolItem)
			return boolItem ? bool.TrueString.ToLower () : bool.FalseString.ToLower ();

		if (value is int intItem)
			return intItem.ToString ();

		if (value is string stringItem)
			return '"' + EscapeLiteral (stringItem) + '"';

		if (value is char charItem)
			return "(char)" + (int) charItem;

		throw new InvalidOperationException ("Unable to get value for: " + value);
	}

	public static string EscapeLiteral (string value)
	{
		bool fixup = false;
		for (int i = 0; i < value.Length; ++i) {
			var c = value [i];
			if (c < 0x20 || c > 0xff || c == '\\' || c == '"') {
				fixup = true;
				break;
			}
		}
		if (fixup) {
			var sb = new StringBuilder ();
			for (int i = 0; i < value.Length; ++i) {
				var c = value [i];
				if (c == '\\') {
					sb.Append (@"\\");
					continue;
				}
				if (c == '"') {
					sb.Append ("\\\"");
					continue;
				}
				if (c < 0x20 || c > 0xff) {
					sb.Append ("\\u").AppendFormat ("{0:x4}", (int) c);
					continue;
				}
				sb.Append (c);
			}
			value = sb.ToString ();
		}
		return value;
	}

	[return: NotNullIfNotNull ("type")]
	public static string? FormatTypeReference (TypeReference? type, bool isDeclaring = false)
	{
		if (type is null)
			return null;

		var name = type.Name;

		switch (type.FullName) {
			case "boolean":
				return "bool";
			case "boolean[]":
				return "bool[]";
			case "byte":
				return "sbyte";
			case "java.lang.String":
				return "string";
			case "**":
				return "global::Java.Lang.Object, global::Java.Lang.Object";
			case "*":
			case "?":
				return "global::Java.Lang.Object";
		}

		if (type.HasGenericParameters) {
			name += "<";
			name += string.Join (", ", type.GenericParameters.Select (gp => FormatTypeReference (gp)));
			name += ">";
		}

		if (type is GenericInstanceType git && git.GenericArguments.Any ()) {
			name += "<";
			name += string.Join (", ", git.GenericArguments.Select (gp => FormatTypeReference (gp)));
			name += ">";
		}

		// Fix generic types used without type arguments, like "Java.Lang.Class"
		// needs to be "Java.Lang.Class<Java.Lang.Object>".
		if (!name.Contains ('<') && !isDeclaring) {
			var is_array = name.EndsWith ("[]");

			if (is_array)
				name = name.Substring (0, name.Length - 2);

			var resolved = type.Resolve ();

			if (resolved != null && resolved.HasGenericParameters) {
				name += "<";

				var objs = new List<string> ();

				for (var i = 0; i < resolved.GenericParameters.Count; i++)
					objs.Add ("global::Java.Lang.Object");

				name += string.Join (", ", objs);
				name += ">";
			}

			if (is_array)
				name += "[]";
		}

		if (type.DeclaringType is not null)
			return FormatTypeReference (type.DeclaringType, true) + "." + name;

		if (type.Namespace.HasValue () && !isDeclaring)
			return $"global::{type.GetNamespace ()}.{name}";

		if (type.Namespace.HasValue ())
			return $"global::{type.GetNamespace ()}.{name}";

		return name;
	}
}

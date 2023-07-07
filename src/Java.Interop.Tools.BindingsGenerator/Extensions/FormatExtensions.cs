using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Javil;

namespace Java.Interop.Tools.BindingsGenerator;

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

	[return: NotNullIfNotNull (nameof (type))]
	public static string? FormatTypeReference (TypeReference? type, bool fixNestedGenerics = false, bool isDeclaringType = false)
	{
		if (type is null)
			return null;

		var name = type.Name;

		// Handle more arrays?
		switch (type.FullName) {
			case "boolean":
				return "bool";
			case "boolean[]":
				return "bool[]";
			case "boolean[][]":
				return "bool[][]";
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
			name += string.Join (", ", type.GenericParameters.Select (gp => fixNestedGenerics ? "global::Java.Lang.Object" : FormatTypeReference (gp)));
			name += ">";
		}

		var resolved = type.Resolve ();

		if (type is GenericInstanceType git && git.GenericArguments.Any ()) {
			name += "<";
			name += string.Join (", ", git.GenericArguments.Select (gp => FormatGenericArgument (resolved, gp)));
			name += ">";
		}

		// Fix generic types used without type arguments, like "Java.Lang.Class"
		// needs to be "Java.Lang.Class<Java.Lang.Object>".
		if (!name.Contains ('<')) {
			var is_array = name.EndsWith ("[]");

			if (is_array)
				name = name.Substring (0, name.Length - 2);

			//var resolved = type.Resolve ();

			if (resolved != null && resolved.HasGenericParameters) {
				name += "<";

				var objs = new List<string> ();

				foreach (var gp in resolved.GenericParameters)
					objs.Add (FormatExtensions.FormatTypeReference (gp.InterfaceBounds?.FirstOrDefault ()) ?? "global::Java.Lang.Object");
				//for (var i = 0; i < resolved.GenericParameters.Count; i++)
				//	objs.Add ("global::Java.Lang.Object");

				name += string.Join (", ", objs);
				name += ">";
			}

			if (is_array)
				name += "[]";
		}

		if (type.DeclaringType is not null)
			return FormatTypeReference (type.DeclaringType, fixNestedGenerics, true) + "." + name;

		if (type.Namespace.HasValue ())
			return $"global::{type.GetNamespace ()}.{name}";

		return name;
	}

	static string FormatGenericArgument (TypeDefinition? type, TypeReference ga)
	{
		if (ga.FullName != "?")
			return FormatTypeReference (ga);

		// Handle a parameter like android.widget.AdapterView<?>
		// where the AdapterView type is defined with a generic constraint:
		// android.widget.AdapterView<T extends android.widget.Adapter>
		if (type?.GenericParameters.FirstOrDefault ()?.InterfaceBounds?.FirstOrDefault () is TypeReference tr)
			return FormatTypeReference (tr);

		return "global::Java.Lang.Object";
	}
}

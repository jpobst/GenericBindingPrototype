using System.Globalization;
using System.Text;
using System.Xml;
using Javil;
using Javil.Attributes;

namespace Java.Interop.Generator;

// Outputs Java container in format compatible with class-parse
public static class BytecodeXmlWriter
{
	public static void Write (ContainerDefinition container, string filename)
	{
		var settings = new XmlWriterSettings {
			Indent = true,
			OmitXmlDeclaration = true,
			NewLineOnAttributes = true,
		};

		using var writer = XmlWriter.Create (filename, settings);

		writer.WriteStartElement ("api");

		foreach (var pkg in container.Types.GroupBy (t => t.Namespace).OrderBy (p => p.Key, StringComparer.OrdinalIgnoreCase)) {
			writer.WriteStartElement ("package");
			writer.WriteAttributeString ("name", pkg.Key);
			writer.WriteAttributeString ("jni-name", pkg.Key?.Replace ('.', '/'));

			foreach (var type in pkg.OrderBy (t => t.Name, StringComparer.OrdinalIgnoreCase))
				WriteType (writer, type);

			writer.WriteEndElement (); // package
		}

		writer.WriteEndElement ();  // api
	}

	private static void WriteType (XmlWriter writer, TypeDefinition type)
	{
		writer.WriteStartElement (type.IsInterface ? "interface" : "class");

		writer.WriteAttributeString ("abstract", type.IsAbstract.ToString ().ToLowerInvariant ());
		writer.WriteAttributeString ("deprecated", type.IsDeprecated ? "deprecated" : "not deprecated");

		if (type.Attributes.OfType<EnclosingMethodAttribute> ().FirstOrDefault () is EnclosingMethodAttribute enc) {
			writer.WriteAttributeString ("enclosing-method-jni-type", $"L{enc.Class};");

			if (enc.Method is ConstantNameAndTypeItem m) {
				writer.WriteAttributeString ("enclosing-method-name", m.Name);
				writer.WriteAttributeString ("enclosing-method-signature", m.Descriptor);
			}
		}

		if (!type.IsInterface && type.BaseType is not null) {
			writer.WriteAttributeString ("jni-extends", type.BaseType.JniFullNameGenericsErased);
			writer.WriteAttributeString ("extends", type.BaseType.FullNameGenericsErased.Replace ('$', '.'));
			writer.WriteAttributeString ("extends-generic-aware", type.BaseType.FullName.Replace ('$', '.'));
		}

		writer.WriteAttributeString ("final", type.IsFinal.ToString ().ToLowerInvariant ());
		writer.WriteAttributeString ("name", type.FullName.LastSubset ('.').Replace ('$', '.'));
		writer.WriteAttributeString ("jni-signature", type.JniFullName);
		writer.WriteAttributeStringIf (type.SourceFileName.HasValue (), "source-file-name", type.SourceFileName);

		writer.WriteAttributeString ("static", type.IsStatic.ToString ().ToLowerInvariant ());
		writer.WriteAttributeString ("visibility", type.IsPublic ? "public" : type.IsProtected ? "protected" : type.IsPrivate ? "private" : "");

		WriteGenericParamaters (writer, type);

		foreach (var iface in type.ImplementedInterfaces) {
			writer.WriteStartElement ("implements");

			writer.WriteAttributeString ("name", iface.InterfaceType.FullNameGenericsErased.Replace ('$', '.'));
			writer.WriteAttributeString ("name-generic-aware", iface.InterfaceType.FullName.Replace ('$', '.'));
			writer.WriteAttributeString ("jni-type", iface.InterfaceType.JniFullName);

			writer.WriteEndElement ();  // implements
		}

		WriteConstructors (writer, type);
		WriteMethods (writer, type);
		WriteFields (writer, type);

		writer.WriteEndElement ();  // class/interface

		foreach (var nested in type.NestedTypes.OrderBy (t => t.Name, StringComparer.OrdinalIgnoreCase))
			WriteType (writer, nested);
	}

	private static void WriteGenericParamaters (XmlWriter writer, IGenericParameterProvider provider)
	{
		if (provider.HasGenericParameters) {
			writer.WriteStartElement ("typeParameters");

			foreach (var tp in provider.GenericParameters) {
				writer.WriteStartElement ("typeParameter");
				writer.WriteAttributeString ("name", tp.Name);
				writer.WriteAttributeString ("jni-classBound", tp.ClassBounds?.JniFullName);
				writer.WriteAttributeString ("classBound", tp.ClassBounds?.FullName.Replace ('$', '.')); // Replace is for compatibility for nested types, does not seem like a good idea

				if (tp.InterfaceBounds.Any ()) {
					writer.WriteAttributeString ("interfaceBounds", string.Join (':', tp.InterfaceBounds.Select (ib => ib.FullName.Replace ('$', '.')))); // Replace is for compatibility for nested types, does not seem like a good idea
					writer.WriteAttributeString ("jni-interfaceBounds", string.Join (':', tp.InterfaceBounds.Select (ib => ib.JniFullName)));
				}

				writer.WriteEndElement ();  // typeParameter
			}

			writer.WriteEndElement ();  // typeParameters
		}
	}

	private static void WriteFields (XmlWriter writer, TypeDefinition type)
	{
		foreach (var field in type.Fields.OfType<FieldDefinition> ().Where (f => f.IsPublic || f.IsProtected).OrderBy (n => n.Name, StringComparer.OrdinalIgnoreCase)) {
			writer.WriteStartElement ("field");

			writer.WriteAttributeString ("deprecated", field.IsDeprecated ? "deprecated" : "not deprecated");
			writer.WriteAttributeString ("final", field.IsFinal.ToString ().ToLowerInvariant ());
			writer.WriteAttributeString ("name", field.Name);
			writer.WriteAttributeString ("static", field.IsStatic.ToString ().ToLowerInvariant ());
			writer.WriteAttributeString ("synthetic", field.IsSythetic.ToString ().ToLowerInvariant ());
			writer.WriteAttributeString ("transient", field.IsTransient.ToString ().ToLowerInvariant ());

			var field_type = field.FieldType;

			if (field_type is GenericParameter)
				field_type = TypeReference.CreateFromSignature ("Ljava/lang/Object;", type.Container);

			writer.WriteAttributeString ("type", field_type.FullNameGenericsErased.Replace ('$', '.'));
			writer.WriteAttributeString ("type-generic-aware", field.FieldType.FullName.Replace ('$', '.'));
			writer.WriteAttributeString ("jni-signature", field_type.JniFullNameGenericsErased);

			if (field.Nullability == Nullability.NotNull)
				writer.WriteAttributeString ("not-null", "true");

			if (SerializeConstantValue (field.Value) is string s)
				writer.WriteAttributeString ("value", s);

			writer.WriteAttributeString ("visibility", field.IsPublic ? "public" : field.IsProtected ? "protected" : null);
			writer.WriteAttributeString ("volatile", field.IsVolatile.ToString ().ToLowerInvariant ());

			writer.WriteEndElement ();  // field
		}
	}

	private static void WriteConstructors (XmlWriter writer, TypeDefinition type)
	{
		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (m => m.IsConstructor && (m.IsPublic || m.IsProtected)).OrderBy (m => m.Name + m.GetDescriptor (), StringComparer.OrdinalIgnoreCase))
			WriteMethod (writer, method);
	}

	private static void WriteMethods (XmlWriter writer, TypeDefinition type)
	{
		foreach (var method in type.Methods.OfType<MethodDefinition> ().Where (m => !m.IsConstructor && (m.IsPublic || m.IsProtected)).OrderBy (m => m.Name + m.GetDescriptor (), StringComparer.OrdinalIgnoreCase))
			WriteMethod (writer, method);
	}

	private static void WriteMethod (XmlWriter writer, MethodDefinition method)
	{
		writer.WriteStartElement (method.IsConstructor ? "constructor" : "method");

		writer.WriteAttributeStringIf (!method.IsConstructor, "abstract", method.IsAbstract.ToString ().ToLowerInvariant ());
		writer.WriteAttributeString ("deprecated", method.IsDeprecated ? "deprecated" : "not deprecated");
		writer.WriteAttributeString ("final", method.IsFinal.ToString ().ToLowerInvariant ());
		writer.WriteAttributeString ("name", method.IsConstructor ? method.DeclaringType?.NestedName.Replace ('$', '.') : method.Name);
		writer.WriteAttributeStringIf (!method.IsConstructor, "native", method.IsNative.ToString ().ToLowerInvariant ());
		writer.WriteAttributeStringIf (!method.IsConstructor, "return", SignatureToGenericJavaTypeName (method.ReturnType.JniFullName));// method.ReturnType.FullName.Replace ('$', '.')); ;// ;// ;
		writer.WriteAttributeStringIf (!method.IsConstructor, "jni-return", method.ReturnType.JniFullName);
		writer.WriteAttributeString ("static", method.IsStatic.ToString ().ToLowerInvariant ());
		writer.WriteAttributeStringIf (!method.IsConstructor, "synchronized", method.IsSynchronized.ToString ().ToLowerInvariant ());
		writer.WriteAttributeString ("visibility", method.IsPublic ? "public" : method.IsProtected ? "protected" : null);
		writer.WriteAttributeString ("bridge", method.IsBridge.ToString ().ToLowerInvariant ());
		writer.WriteAttributeString ("synthetic", method.IsSynthetic.ToString ().ToLowerInvariant ());
		writer.WriteAttributeString ("jni-signature", method.GetDescriptor ());
		writer.WriteAttributeStringIf (method.ReturnTypeNullability == Nullability.NotNull, "return-not-null", "true");
		WriteGenericParamaters (writer, method);

		for (var i = 0; i < method.Parameters.Count; i++) {
			var p = method.Parameters [i];
			var genericType = p.ParameterType.JniFullName;
			var is_vararg_array = method.IsVarargs && i == (method.Parameters.Count - 1);

			// Remove array specifier
			if (is_vararg_array)
				genericType = genericType.Substring (1);

			genericType = SignatureToGenericJavaTypeName (genericType);

			if (is_vararg_array)
				genericType += "...";

			writer.WriteStartElement ("parameter");
			writer.WriteAttributeString ("name", p.Name);
			writer.WriteAttributeString ("type", genericType);
			writer.WriteAttributeString ("jni-type", p.ParameterType.JniFullName);
			writer.WriteAttributeStringIf (p.Nullability == Nullability.NotNull, "not-null", "true");
			writer.WriteEndElement ();  // parameter
		}

		if (method.HasCheckedExceptions)
			foreach (var ex in method.CheckedExceptions) {
				writer.WriteStartElement ("exception");
				writer.WriteAttributeString ("name", ex.Type.GetDescriptor (method.GetGenericParametersInScope ()) [1..^1]);
				writer.WriteAttributeString ("type", ex.Type.GetDescriptor (method.GetGenericParametersInScope ()) [1..^1].Replace ('/', '.').Replace ('$', '.')); //ex.Type.FullNameGenericsErased.Replace ('$', '.'));
				writer.WriteAttributeString ("type-generic-aware", ex.Type.FullName.Replace ('$', '.'));
				writer.WriteEndElement ();  // exception
			}

		writer.WriteEndElement ();  // constructor / method
	}

	private static string? SerializeConstantValue (object? value)
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
				return floatItem.ToString ("G9", CultureInfo.InvariantCulture);
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
			return ((int) charItem).ToString ();

		throw new InvalidOperationException ("Unable to get value for: " + value);
	}

	static string EscapeLiteral (string value)
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

	static string SignatureToGenericJavaTypeName (string value)
	{
		if (string.IsNullOrEmpty (value))
			return string.Empty;

		var index = 0;
		var type = new StringBuilder ();

		return AppendGenericTypeNameFromSignature (type, value, ref index).ToString ();
	}

	static StringBuilder AppendGenericTypeNameFromSignature (StringBuilder typeBuilder, string value, ref int index)
	{
		var array = GetArraySuffix (value, ref index);
		var builtin = GetBuiltinName (value, ref index);

		if (builtin != null)
			return typeBuilder.Append (builtin).Append (array);

		switch (value [index]) {
			case 'L':
				index++;
				var depth = 0;
				while (index < value.Length) {
					var c = value [index++];
					if (depth == 0 && c == ';')
						break;

					if (c == '<') {
						depth++;
						typeBuilder.Append ("<");
						AppendGenericTypeNameFromSignature (typeBuilder, value, ref index);
					} else if (c == '>') {
						typeBuilder.Append (">");
						depth--;
					} else if (depth > 0) {
						index--;
						typeBuilder.Append (", ");
						AppendGenericTypeNameFromSignature (typeBuilder, value, ref index);
					} else if (c == '/' || c == '$') {
						typeBuilder.Append ('.');
					} else {
						typeBuilder.Append (c);
					}
				}
				return typeBuilder.Append (array);
			case 'T':
				index++;
				typeBuilder.Append (ExtractIdentifier (value, ref index));
				index++;    // consume ';'
				return typeBuilder.Append (array);
			case '*':
				index++;
				return typeBuilder.Append ("?");
			case '+':
				index++;
				typeBuilder.Append ("? extends ");
				return AppendGenericTypeNameFromSignature (typeBuilder, value, ref index);
			case '-':
				index++;
				typeBuilder.Append ("? super ");
				return AppendGenericTypeNameFromSignature (typeBuilder, value, ref index);
		}
		typeBuilder.Append ("/* should not be reached */").Append (value.Substring (index));
		index = value.Length;
		return typeBuilder;
	}

	static string? GetArraySuffix (string value, ref int index)
	{
		var o = index;

		while (value [index] == '[')
			index++;

		if (o == index)
			return null;

		return string.Join ("", Enumerable.Repeat ("[]", index - o));
	}

	static string? GetBuiltinName (string value, ref int index)
	{
		switch (value [index]) {
			case 'B': index++; return "byte";
			case 'C': index++; return "char";
			case 'D': index++; return "double";
			case 'F': index++; return "float";
			case 'I': index++; return "int";
			case 'J': index++; return "long";
			case 'S': index++; return "short";
			case 'V': index++; return "void";
			case 'Z': index++; return "boolean";
		}

		return null;
	}

	static string ExtractIdentifier (string signature, ref int index)
	{
		var s = index;
		var e = s + 1;

		while (e < signature.Length && IsUnqualifiedChar (signature [e]) && signature [e] != ':')
			e++;

		index = e;

		return signature.Substring (s, e - s);
	}

	// http://docs.oracle.com/javase/specs/jvms/se7/html/jvms-4.html#jvms-4.2.2
	static bool IsUnqualifiedChar (char c)
	{
		return c switch {
			'.' or ';' or '[' or '/' => false,
			_ => true,
		};
	}
}

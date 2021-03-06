using System;
using System.IO;

namespace Xamarin.SourceWriter
{
    public class CodeWriter : IDisposable
	{
		TextWriter stream;
		bool owns_stream;
		int indent;
		bool need_indent = true;
		string base_indent;

		public CodeWriter (string filename)
		{
			stream = File.CreateText (filename);
			owns_stream = true;
		}

		public CodeWriter (TextWriter streamWriter, string baseIndent = "")
		{
			stream = streamWriter;
			base_indent = baseIndent;
		}

		public void Write (string value)
		{
			WriteIndent ();
			stream.Write (value);
		}

		public void WriteLine ()
		{
			stream.WriteLine ();
			need_indent = true;
		}

		public void WriteLine (string value)
		{
			if (value?.Length > 0)
				WriteIndent ();

			stream.WriteLine (value);
			need_indent = true;
		}

		public void WriteLine (string format, params object[] args)
		{
			if (format?.Length > 0)
				WriteIndent ();

			stream.WriteLine (format, args);
			need_indent = true;
		}

		public void WriteLineNoIndent (string value)
		{
			stream.WriteLine (value);
			need_indent = true;
		}

		public void Indent (int count = 1) => indent += count;
		public void Unindent (int count = 1) => indent -= count;

		private void WriteIndent ()
		{
			if (!need_indent)
				return;

			stream.Write (base_indent + new string ('\t', indent));

			need_indent = false;
		}

		public void Dispose ()
		{
			if (owns_stream)
				stream?.Dispose ();
		}
	}
}

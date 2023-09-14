using System.Diagnostics;

namespace Java.Interop.Tools.BindingsGenerator;

class TimingLogger : IDisposable
{
	readonly Stopwatch? sw;
	readonly bool should_log;
	readonly string message;

	public TimingLogger (string message, bool log = true)
	{
		this.message = message;
		should_log = log;

		if (should_log)
			sw = Stopwatch.StartNew ();
	}

	public void Dispose ()
	{
		if (should_log)
			Console.WriteLine ($"{message} - {sw!.ElapsedMilliseconds}ms");
	}
}

using System.Collections.Generic;

namespace Xamarin.SourceWriter
{
    public interface ITakeParameters
	{
		List<MethodParameterWriter> Parameters { get; }
	}
}

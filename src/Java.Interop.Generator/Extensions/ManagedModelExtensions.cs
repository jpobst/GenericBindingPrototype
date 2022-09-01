using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Generator
{
	static class ManagedModelExtensions
	{
		public static string ChopArity (this string type)
		{
			var index = type.IndexOf ('<');

			if (index > 0)
				return type.Substring (0, index);

			return type;
		}
	}
}

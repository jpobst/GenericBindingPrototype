using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Generator
{
	public class ManagedGenericParameter : ManagedTypeReference
	{
		public override string Namespace => string.Empty;

		public ManagedGenericParameter (string name, ManagedTypeReference? declaringType = null) : base (string.Empty, name, declaringType)
		{
		}
	}
}

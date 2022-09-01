using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Generator
{
	public class ManagedWildcardType : ManagedTypeReference
	{
		public override string Namespace => string.Empty;

		public ManagedWildcardType (ManagedTypeReference? declaringType = null) : base (string.Empty, "?", declaringType)
		{
		}
	}
}

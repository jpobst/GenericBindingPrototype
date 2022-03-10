using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Android
{
	[System.AttributeUsage (AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	sealed class JavaSignatureAttribute : Attribute
	{
		public string Signature { get; }

		public JavaSignatureAttribute (string signature)
		{
			Signature = signature;
		}
	}
}

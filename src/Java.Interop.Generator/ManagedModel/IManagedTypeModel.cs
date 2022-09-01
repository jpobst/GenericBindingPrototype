using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Generator
{
	public interface IManagedTypeModel
	{
		ManagedNamespaceModel? Namespace { get; set; }
		void PopulateMembers ();
	}
}

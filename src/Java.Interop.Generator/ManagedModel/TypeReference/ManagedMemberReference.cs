using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Javil;

namespace Java.Interop.Generator
{
	public abstract class ManagedMemberReference
	{
		public virtual string Name { get; }
		public abstract string FullName { get; }
		public virtual ManagedMemberReference? DeclaringType { get; set; }
		public virtual string GenericName => Name;
		public abstract string FullGenericName { get; }

		protected ManagedMemberReference (string name, ManagedMemberReference? declaringType)
		{
			Name = name;
			DeclaringType = declaringType;
		}

		public ManagedTypeDefinition? Resolve ()
		{
			return ResolveDefinition ();
		}

		protected abstract ManagedTypeDefinition? ResolveDefinition ();
	}
}

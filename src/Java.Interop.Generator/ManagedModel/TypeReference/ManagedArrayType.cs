using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Generator
{
	public class ManagedArrayType : ManagedTypeReference
	{
		public int Rank { get; }
		public ManagedTypeReference ElementType { get; }

		public ManagedArrayType (ManagedTypeReference element, int rank) : base (string.Empty, element.Name, element.DeclaringType)
		{
			ElementType = element;
			Rank = rank;
		}

		public override string Namespace => ElementType.Namespace;

		public override string Name => ElementType.Name + "[]".Repeat (Rank);

		public override string GenericName => ElementType.GenericName + "[]".Repeat (Rank);

		public override string FullName => ElementType.FullName + "[]".Repeat (Rank);

		public override string FullGenericName => ElementType.FullGenericName + "[]".Repeat (Rank);
	}
}

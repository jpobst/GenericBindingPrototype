using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Generator
{
	public class ManagedGenericInstanceType : ManagedTypeReference
	{
		public ManagedTypeDefinition GenericType { get; set; }
		public Collection<ManagedTypeReference> GenericArguments { get; } = new Collection<ManagedTypeReference> ();

		public ManagedGenericInstanceType (ManagedTypeDefinition genericType) : base (string.Empty, genericType.Name, genericType.DeclaringType)
		{
			GenericType = genericType;
		}

		public override string Namespace => GenericType.Namespace;

		public override string GenericName {
			get {
				var sb = new StringBuilder ();

				sb.Append (Name);
				sb.Append ('<');
				sb.Append (string.Join (", ", GenericArguments.Select (a => a.FullGenericName)));
				sb.Append ('>');

				return sb.ToString ();
			}
		}
	}
}

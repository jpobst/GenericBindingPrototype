using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Javil;

namespace Java.Interop.Generator
{
	public class ManagedFieldDefinition : ManagedMemberReference
	{
		public bool IsPublic { get; set; }
		public bool IsStatic { get; set; }
		public bool IsProtected { get; set; }
		public bool IsConstant { get; set; }
		public ManagedTypeReference Type { get; set; }
		public string? Value { get; set; }

		public override string FullName => Name;
		public override string FullGenericName => Name;

		public ManagedFieldDefinition (string name, ManagedTypeReference fieldType, ManagedTypeReference? declaringType) : base (name, declaringType)
		{
			Type = fieldType;
		}

		public static ManagedFieldDefinition Create (FieldDefinition field, ManagedTypeReference? declaringType)
		{
			var f = new ManagedFieldDefinition (field.GetName (), ManagedTypeReference.ConvertTypeReference (field.FieldType), declaringType) {
				IsPublic = field.IsPublic,
				IsProtected = field.IsProtected,
				IsStatic = field.IsStatic,
				IsConstant = field.IsConstant
			};

			if (f.IsConstant)
				f.Value = FormatExtensions.SerializeConstantValue (field.Value, field.FieldType.Name);

			return f;
		}

		protected override ManagedTypeDefinition? ResolveDefinition ()
		{
			throw new NotImplementedException ();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator
{
	public class ManagedFieldWriterAdapter : FieldWriter
	{
		public ManagedFieldWriterAdapter (ManagedFieldDefinition field)
		{
			// TODO: Handle field as property
			Name = field.Name;

			if (field.IsPublic)
				IsPublic = true;

			if (field.IsProtected)
				IsProtected = true;

			IsConst = field.IsConstant;
			IsStatic = field.IsStatic && !field.IsConstant;

			Type = new TypeReferenceWriter (field.Type.FullGenericName);

			if (field.IsConstant)
				Value = field.Value;
		}
	}
}

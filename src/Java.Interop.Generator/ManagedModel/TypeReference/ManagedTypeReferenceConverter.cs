using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator
{
	public class ManagedTypeReferenceConverter
	{
		ContainerDefinition Container { get; }
		ManagedProjectModel Model { get; }

		public ManagedTypeReferenceConverter (ContainerDefinition container, ManagedProjectModel model)
		{
			Container = container;
			Model = model;
		}

		public ManagedTypeReference? Convert (TypeReference type)
		{
			var resolved = type.Resolve ();

			//if (resolved is TypeDefinition td) {
			//	return td.GetManagedTypeModel ();
			//}

			return null;
		}
	}
}

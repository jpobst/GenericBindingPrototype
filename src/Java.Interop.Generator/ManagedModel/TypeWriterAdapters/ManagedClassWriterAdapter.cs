using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator
{
	public class ManagedClassWriterAdapter : ClassWriter
	{
		public ManagedClassWriterAdapter (ManagedTypeDefinition type)
		{
			Name = type.Name;

			if (type.IsPublic)
				IsPublic = true;
			else if (type.IsProtected)
				IsProtected = true;

			IsAbstract = type.IsAbstract;
			IsSealed = type.IsSealed;

			foreach (var tp in type.GenericParameters)
				GenericParameters.Add (tp.Name);

			if (type.FullName != "Java.Lang.Object") {
				Inherits = type.BaseType?.FullGenericName ?? string.Empty;
				Inherits = Inherits.Replace ("?", "Java.Lang.Object");  // TODO: Cheating
			}

			foreach (var iface in type.ImplementedInterfaces)
				Implements.Add (iface.FullGenericName);

			foreach (var nested in type.NestedTypes)
				NestedTypes.Add (ManagedClassWriterAdapter.Create (nested));

			foreach (var fields in type.Fields)
				Fields.Add (new ManagedFieldWriterAdapter (fields));
		}

		public static TypeWriter Create (ManagedTypeDefinition type)
		{
			if (type.IsInterface)
				return new ManagedInterfaceWriterAdapter (type);

			return new ManagedClassWriterAdapter (type);
		}
	}
}

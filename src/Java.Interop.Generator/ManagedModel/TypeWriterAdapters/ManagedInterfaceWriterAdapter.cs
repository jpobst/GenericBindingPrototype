using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator
{
	public class ManagedInterfaceWriterAdapter : InterfaceWriter
	{
		public ManagedInterfaceWriterAdapter (ManagedTypeDefinition type)
		{
			Name = type.Name;

			if (type.IsPublic)
				IsPublic = true;
			else if (type.IsProtected)
				IsProtected = true;

			//IsAbstract = type.IsAbstract;
			IsSealed = type.IsSealed;

			foreach (var tp in type.GenericParameters)
				GenericParameters.Add (tp.Name);

			//if (type.FullName != "Java.Lang.Object")
			//	Inherits = "Java.Lang.Object";

			foreach (var nested in type.NestedTypes)
				NestedTypes.Add (ManagedClassWriterAdapter.Create (nested));
		}
	}
}

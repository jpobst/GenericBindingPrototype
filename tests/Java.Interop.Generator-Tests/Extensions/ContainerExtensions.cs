using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Javil;

namespace Java.Interop.Generator_Tests
{
	public static class ContainerExtensions
	{
		public static TypeDefinition GetJavaLangObject (this ContainerDefinition container) => container.FindType ("java.lang.Object")!;
		public static TypeDefinition GetVoid (this ContainerDefinition container) => container.FindType ("void")!;

		public static TypeDefinition AddClass (this ContainerDefinition container, string @namespace, string name, TypeDefinition? baseType = null, TypeDefinition? declaringType = null)
		{
			var td = new TypeDefinition (@namespace, name, declaringType, container, baseType ?? container.GetJavaLangObject ()) {
				IsPublic = true
			};

			return container.AddType (td);
		}

		public static TypeDefinition AddInterface (this ContainerDefinition container, string @namespace, string name, TypeDefinition? declaringType = null)
		{
			var td = new TypeDefinition (@namespace, name, declaringType, container, null) {
				IsPublic = true
			};

			return container.AddType (td);
		}
	}
}

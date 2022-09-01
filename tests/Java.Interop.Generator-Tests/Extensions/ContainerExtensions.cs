using System;
using System.Collections.Generic;
using System.ComponentModel;
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

		public static ContainerDefinition CreateContainer ()
		{
			var container = new ContainerDefinition ("dummy.jar");

			container.AddClass ("java.lang", "Object");

			return container;
		}

		public static TypeDefinition AddClass (this ContainerDefinition container, string @namespace, string name, TypeDefinition? baseType = null, TypeDefinition? declaringType = null, bool isAbstract = false)
		{
			var td = new TypeDefinition (@namespace, name, declaringType, container, baseType ?? container.GetJavaLangObject ()) {
				IsPublic = true,
				IsAbstract = isAbstract
			};

			return container.AddType (td);
		}

		public static TypeDefinition AddClass (this TypeDefinition parent, string @namespace, string name, TypeDefinition? baseType = null, TypeDefinition? declaringType = null, bool isAbstract = false)
		{
			var td = new TypeDefinition (@namespace, name, declaringType, parent.Container, baseType ?? parent.Container.GetJavaLangObject ()) {
				IsPublic = true,
				IsAbstract = isAbstract
			};

			parent.NestedTypes.Add (td);

			return td;
		}

		public static TypeDefinition AddInterface (this ContainerDefinition container, string @namespace, string name, TypeDefinition? declaringType = null)
		{
			var td = new TypeDefinition (@namespace, name, declaringType, container, null) {
				IsPublic = true,
				IsInterface = true
			};

			return container.AddType (td);
		}

		public static MethodDefinition AddMethod (this TypeDefinition type, string name, TypeDefinition? returnType = null, bool isAbstract = false, params TypeDefinition []? parameters)
		{
			returnType ??= type.Container.GetVoid ();

			var method = new MethodDefinition (name, returnType, type) {
				IsAbstract = isAbstract
			};

			if (parameters is not null) {
				var index = 0;

				foreach (var p in parameters)
					method.Parameters.Add (new ParameterDefinition (method, $"p{index}", p, index++, Nullability.Oblivous));
			}

			type.Methods.Add (method);

			return method;
		}

		public static GenericParameter AddGenericParameter (this TypeDefinition type, string name)
		{
			var gp = new GenericParameter (name, type.Container, type);

			type.GenericParameters.Add (gp);

			return gp;
		}
	}
}

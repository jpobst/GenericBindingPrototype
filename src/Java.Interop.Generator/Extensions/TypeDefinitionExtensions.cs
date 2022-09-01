using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator
{
	public static class TypeDefinitionExtensions
	{
		/// <summary>
		/// These are implemented interfaces that are not inherited from classes.  This is used to ensure
		/// an abstract class provides an instance method or abstract method for every interface method.
		/// </summary>
		public static IEnumerable<TypeDefinition> GetImplementedInterfacesThatMustBeImplemented (this TypeDefinition type)
		{
			// We do not need to recurse into base classes because they will already
			// have fully fulfilled their interface requirements.
			var results = new List<TypeDefinition> ();

			foreach (var iface in type.ImplementedInterfaces)
				AddImplementedInterfacesOfInterface (results, iface);

			return results.Distinct ();
		}

		static void AddImplementedInterfacesOfInterface (List<TypeDefinition> interfaces, ImplementedInterface iface)
		{
			if (iface.InterfaceType.Resolve () is not TypeDefinition td)
				return;

			interfaces.Add (td);

			if (iface.InterfaceType.Resolve () is TypeDefinition interface_type)
				foreach (var iface2 in interface_type.ImplementedInterfaces)
					AddImplementedInterfacesOfInterface (interfaces, iface2);
		}

		/// <summary>
		/// Returns all methods implemented by this type and its base types.
		/// </summary>
		public static IEnumerable<MethodDefinition> GetImplementedMethods (this TypeDefinition type)
		{
			foreach (var method in type.Methods.Where (m => !m.IsConstructor))
				yield return method;

			if (type.BaseType?.Resolve () is TypeDefinition td)
				foreach (var method in td.GetImplementedMethods ())
					yield return method;
		}

		/// <summary>
		/// Returns all methods defined on interfaces implemented by this type.
		/// </summary>
		public static IEnumerable<MethodDefinition> GetInheritedInterfaceMethods (this TypeDefinition type)
		{
			var results = new List<MethodDefinition> ();

			foreach (var iface in type.GetImplementedInterfacesThatMustBeImplemented ())
				results.AddRange (iface.Methods.Where (m => !m.IsConstructor));

			return results;
		}

		/// <summary>
		/// Returns if class is "visible" (public or protected).
		/// </summary>
		public static bool IsVisible (this TypeDefinition type) => type.IsPublic || type.IsProtected;

		public static string? GetNamespace (this TypeWriter type)
		{
			if (type is not IManagedTypeModel model)
				throw new Exception ();

			if (type.ParentType is not null)
				return type.ParentType.GetNamespace ();

			return model.Namespace?.Name;
		}
	}
}

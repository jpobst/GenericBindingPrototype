using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Generator
{
	// Sometimes a generic type is referenced as a non-generic (erased) type.
	// We need to fill in any missing generic parameters with JLO.
	// example:
	//   public class ArgbEvaluator : Java.Lang.Object, Android.Animation.TypeEvaluator<T> { ... }
	public static class MissedGenericTypesFixup
	{
		public static void Fixup (ManagedProjectReferenceModel project)
		{
			foreach (var type in project.GetAllTypes ().ToArray ())
				FixType (type);
		}

		static void FixType (ManagedTypeDefinition type)
		{
			// TODO: Inefficient
			// TODO: Other type references (base type, etc)
			type.BaseType = FixTypeReference (type.BaseType);

			if (type.ImplementedInterfaces.Count > 0) {
				var fixed_interfaces = type.ImplementedInterfaces.Select (i => FixTypeReference (i)).ToArray ();
				type.ImplementedInterfaces.Clear ();

				foreach (var iface in fixed_interfaces)
					if (iface is not null)
						type.ImplementedInterfaces.Add (iface);
			}

			foreach (var field in type.Fields)
				field.Type = FixTypeReference (field.Type);

			foreach (var child in type.NestedTypes)
				FixType (child);
		}

		static ManagedTypeReference? FixTypeReference (ManagedTypeReference? type)
		{
			if (type is ManagedTypeDefinition def && def.HasGenericParameters) {
				var new_ref = new ManagedGenericInstanceType (def);
				var jlo = def.Container?.FindType ("Java.Lang.Object");

				if (jlo is null)
					throw new Exception ();

				for (var i = 0; i < def.GenericParameters.Count; i++)
					new_ref.GenericArguments.Add (jlo);

				return new_ref;
			}

			return type;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Java.Interop.Generator
{
	public static class MoveTypesNestedInGenericTypesFixup
	{
		public static void Fixup (ManagedProjectReferenceModel project)
		{
			foreach (var type in project.GetAllTypes ().ToArray ())
				FixType (type);
		}

		static void FixType (ManagedTypeDefinition type)
		{
			// If we don't have nested types, nothing to do
			if (type.NestedTypes.Count == 0)
				return;

			if (type.HasGenericParameters) {
				var non_generic_type = new ManagedTypeDefinition (type.Name, type.DeclaringType, null) {
					IsPublic = true,
					IsSealed = true
				};

				if (type.DeclaringType is ManagedTypeDefinition declaring)
					declaring.NestedTypes.Add (non_generic_type);
				else
					type.Container?.AddTypes (type.Namespace, non_generic_type);

				// Move all nested children to new non-generic type
				var children = type.NestedTypes.ToArray ();
				type.NestedTypes.Clear ();
				non_generic_type.NestedTypes.AddRange (children);

				foreach (var nested in children)
					nested.DeclaringType = non_generic_type;

				// Recurse
				foreach (var child in children)
					FixType (child);
			}
		}
	}
}

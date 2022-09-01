using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator
{
	public class ManagedProjectModel
	{
		public NamespaceCollection Namespaces { get; } = new ();

		public void AddTypes (string ns, params TypeWriter[] types)
		{
			if (!Namespaces.TryGetValue (ns, out var model)) {
				model = new ManagedNamespaceModel (ns);
				Namespaces.Add (model);
			}

			model.Types.AddRange (types);
		}

		public IEnumerable<TypeWriter> GetAllTypes (bool includeNested = false)
		{
			return Namespaces.SelectMany (ns => ns.Types);
		}

		public class NamespaceCollection : KeyedCollection<string, ManagedNamespaceModel>
		{
			protected override string GetKeyForItem (ManagedNamespaceModel item) => item.Name;
		}
	}

	public class ManagedProjectReferenceModel
	{
		public ManagedNamespaceCollection Namespaces { get; } = new ();

		public void AddTypes (string ns, params ManagedTypeDefinition [] types)
		{
			if (!Namespaces.TryGetValue (ns, out var model)) {
				model = new ManagedNamespaceReferenceModel (ns, this);
				Namespaces.Add (model);
			}

			model.Types.AddRange (types);
		}

		public IEnumerable<ManagedTypeDefinition> GetAllTypes (bool includeNested = false)
		{
			return Namespaces.SelectMany (ns => ns.Types);
		}

		public ManagedTypeDefinition? FindType (string name)
		{
			// TODO: Inefficient
			return GetAllTypes (true).FirstOrDefault (t => t.FullName == name);
		}

		public class ManagedNamespaceCollection : KeyedCollection<string, ManagedNamespaceReferenceModel>
		{
			protected override string GetKeyForItem (ManagedNamespaceReferenceModel item) => item.Name;
		}
	}

}

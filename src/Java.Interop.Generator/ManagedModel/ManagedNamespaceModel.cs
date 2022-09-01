using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.SourceWriter;

namespace Java.Interop.Generator
{
	public class ManagedNamespaceModel
	{
		public string Name { get; set; }

		public ObservableCollection<TypeWriter> Types { get; } = new ();

		public ManagedNamespaceModel (string name)
		{
			Name = name;

			Types.CollectionChanged += Types_CollectionChanged;
		}

		private void Types_CollectionChanged (object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
				foreach (var type in e.NewItems.OfType<IManagedTypeModel> ())
					type.Namespace = this;
		}
	}

	public class ManagedNamespaceReferenceModel
	{
		public string Name { get; set; }

		public ObservableCollection<ManagedTypeDefinition> Types { get; } = new ();

		public ManagedProjectReferenceModel Container { get; }

		public ManagedNamespaceReferenceModel (string name, ManagedProjectReferenceModel container)
		{
			Name = name;
			Container = container;

			Types.CollectionChanged += Types_CollectionChanged;
		}

		private void Types_CollectionChanged (object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
				foreach (var type in e.NewItems.OfType<ManagedTypeDefinition> ())
					type.DeclaringNamespace = this;
		}
	}
}

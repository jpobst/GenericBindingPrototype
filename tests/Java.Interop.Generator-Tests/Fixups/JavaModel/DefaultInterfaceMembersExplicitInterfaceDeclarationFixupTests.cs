using Java.Interop.Generator;
using Javil;

namespace Java.Interop.Generator_Tests
{
	public class DefaultInterfaceMembersExplicitInterfaceDeclarationFixupTests
	{
		[Test]
		public void FixupNonGenericInterface ()
		{
			var container = ContainerExtensions.CreateContainer ();

			var base_iface = container.AddInterface ("xamarin.test", "MyBaseInterface");
			base_iface.AddMethod ("doThing", isAbstract: true);

			var inherited_iface = container.AddInterface ("xamarin.test", "MyInheritedInterface");
			inherited_iface.ImplementedInterfaces.Add (new ImplementedInterface (base_iface));
			var method = inherited_iface.AddMethod ("doThing");

			DefaultInterfaceMembersExplicitInterfaceDeclarationFixup.Run (container);

			// Ensure that MyInheritedInterface.doThing () was marked with an explicit interface declaration
			Assert.That (method.GetExplicitInterface (), Is.EqualTo ("global::Xamarin.Test.MyBaseInterface"));
		}

		[Test]
		public void FixupGenericInterface ()
		{
			var container = ContainerExtensions.CreateContainer ();

			var base_iface = container.AddInterface ("xamarin.test", "MyMap");
			base_iface.AddGenericParameter ("K");
			base_iface.AddGenericParameter ("V");

			base_iface.AddMethod ("doThing", isAbstract: true);

			var inherited_iface = container.AddInterface ("xamarin.test", "MyInheritedInterface");
			inherited_iface.AddGenericParameter ("K");
			inherited_iface.AddGenericParameter ("V");
			inherited_iface.ImplementedInterfaces.Add (new ImplementedInterface (base_iface));

			var method = inherited_iface.AddMethod ("doThing");

			DefaultInterfaceMembersExplicitInterfaceDeclarationFixup.Run (container);

			// Ensure that MyInheritedInterface.doThing () was marked with an explicit interface declaration
			Assert.That (method.GetExplicitInterface (), Is.EqualTo ("global::Xamarin.Test.MyMap<K, V>"));
		}
	}
}

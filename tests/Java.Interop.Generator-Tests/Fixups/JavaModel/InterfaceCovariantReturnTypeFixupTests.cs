using Java.Interop.Generator;
using Javil;

namespace Java.Interop.Generator_Tests
{
	public class InterfaceCovariantReturnTypeFixupTests
	{
		[Test]
		public void FixupCovariantReturnOnImplementedInterfaceMethod ()
		{
			var container = ContainerExtensions.CreateContainer ();

			// Need a type that inherits JLO
			var derived_type = container.AddClass ("java.lang", "MyObject");

			// Need an interface method that returns JLO
			var iface = container.AddInterface ("xamarin.test", "MyInterface");
			iface.AddMethod ("doThing", container.GetJavaLangObject (), isAbstract: true);

			// Need a class that implements the method but returns "derived_type" instead of JLO
			var klass = container.AddClass ("xamarin.test", "MyClass");
			klass.ImplementedInterfaces.Add (new ImplementedInterface (iface));
			var dothing = klass.AddMethod ("doThing", derived_type);

			InterfaceCovariantReturnTypeFixup.Run (container);

			// Ensure that the doThing return type was fixed to JLO
			Assert.That (dothing.GetManagedReturnType ()!.FullName, Is.EqualTo ("java.lang.Object"));
		}
	}
}

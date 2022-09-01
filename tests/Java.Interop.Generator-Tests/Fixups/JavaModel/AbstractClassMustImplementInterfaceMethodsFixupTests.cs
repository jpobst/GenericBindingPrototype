using Java.Interop.Generator;
using Javil;

namespace Java.Interop.Generator_Tests
{
	public class AbstractClassMustImplementInterfaceMethodsFixupTests
	{
		[Test]
		public void FixupAbstractClass ()
		{
			var container = ContainerExtensions.CreateContainer ();

			var iface = container.AddInterface ("xamarin.test", "MyAbstractInterface");
			iface.AddMethod ("doThing", isAbstract: true);

			var klass = container.AddClass ("xamarin.test", "MyAbstractClass", isAbstract: true);
			klass.ImplementedInterfaces.Add (new ImplementedInterface (iface));

			AbstractClassMustImplementInterfaceMethodsFixup.Run (container);

			// Ensure that an abstract MyAbstractClass.doThing () was inserted
			Assert.Multiple (() => {
				Assert.That (klass.Methods, Has.Count.EqualTo (1));
				Assert.That (klass.Methods [0].IsAbstract, Is.True);
				Assert.That (klass.Methods [0].Name, Is.EqualTo ("doThing"));
			});
		}
	}
}

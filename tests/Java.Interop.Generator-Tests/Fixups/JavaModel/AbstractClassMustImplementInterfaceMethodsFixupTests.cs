using Java.Interop.Generator;
using Javil;

namespace Java.Interop.Generator_Tests
{
	public class AbstractClassMustImplementInterfaceMethodsFixupTests
	{
		[Test]
		public void FixupAbstractClass ()
		{
			var container = CreateTestContainer ();

			var iface = container.AddInterface ("xamarin.test", "MyAbstractInterface");
			iface.Methods.Add (new MethodDefinition ("doThing", container.GetVoid (), iface) { IsAbstract = true });

			var klass = container.AddClass ("xamarin.test", "MyAbstractClass");
			klass.IsAbstract = true;
			klass.ImplementedInterfaces.Add (new ImplementedInterface (iface));

			AbstractClassMustImplementInterfaceMethodsFixup.Run (container);

			// Ensure that an abstract MyAbstractClass.doThing () was inserted
			Assert.That (klass.Methods, Has.Count.EqualTo (1));
			Assert.That (klass.Methods [0].IsAbstract, Is.True);
			Assert.That (klass.Methods [0].Name, Is.EqualTo ("doThing"));
		}

		static ContainerDefinition CreateTestContainer ()
		{
			var container = new ContainerDefinition ("dummy.jar");

			container.AddType (new TypeDefinition ("java.lang", "Object", null, container, null));

			return container;
		}
	}
}

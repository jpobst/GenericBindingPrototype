using Java.Interop.Generator;
using Javil;

namespace Java.Interop.Generator_Tests
{
	public class ManagedTypeReferenceConverterTests
	{
		[Test]
		public void SimpleClass ()
		{
			var container = ContainerExtensions.CreateContainer ();
			var writer = new BindingsWriter ("");
			var klass = container.AddClass ("xamarin.test", "MyClass");

			var model = new ManagedProjectModel ();
			model.AddTypes ("Xamarin.Test", writer.CreateType (klass).ToArray ());

			var converter = new ManagedTypeReferenceConverter (container, model);

			var result = converter.Convert (klass);

			Assert.That (result, Is.Not.Null);

			// Ensure that an abstract MyAbstractClass.doThing () was inserted
			Assert.Multiple (() => {
				Assert.That (result.GetNamespace (), Is.EqualTo ("Xamarin.Test"));
				Assert.That (result.NestedName, Is.EqualTo ("MyClass"));
			});
		}

		[Test]
		public void NestedClass ()
		{
			var container = ContainerExtensions.CreateContainer ();
			var writer = new BindingsWriter ("");
			var parent = container.AddClass ("xamarin.test", "MyParentClass");
			var klass = parent.AddClass ("xamarin.test", "MyClass");

			var model = new ManagedProjectModel ();
			model.AddTypes ("Xamarin.Test", writer.CreateType (parent).ToArray ());

			var converter = new ManagedTypeReferenceConverter (container, model);

			var result = converter.Convert (klass);

			Assert.That (result, Is.Not.Null);

			// Ensure that an abstract MyAbstractClass.doThing () was inserted
			Assert.Multiple (() => {
				Assert.That (result.GetNamespace (), Is.EqualTo ("Xamarin.Test"));
				Assert.That (result.NestedName, Is.EqualTo ("MyParentClass.MyClass"));
			});
		}
	}
}

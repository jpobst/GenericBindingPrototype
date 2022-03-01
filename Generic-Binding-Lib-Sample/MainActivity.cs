using Example;

namespace Generic_Binding_Lib_Sample
{
	[Activity (Label = "@string/app_name", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle? savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_main);

			TestGenericInterface ();
			TestPerformance ();
		}

		public void TestGenericInterface ()
		{
			// Create generic binding class
			var list = new MyCustomList<Android.Graphics.Point> ();
			var java_list_invoker = new CustomListConsumer (list);

			// Create and add some objects
			var p1 = new Android.Graphics.Point (10, 15);
			var p2 = new Android.Graphics.Point (30, 45);

			java_list_invoker.Add (p1);
			java_list_invoker.Add (p2);

			// Retrieve objects
			var point1 = java_list_invoker.Get (0);
			var point2 = java_list_invoker.Get (1);

			Console.WriteLine (point1);
			Console.WriteLine (point2);

			// Test collection objects
			java_list_invoker.AddAll (new [] { new Android.Graphics.Point (100, 150), new Android.Graphics.Point (300, 450) });

			Console.WriteLine (java_list_invoker.Get (2));
			Console.WriteLine (java_list_invoker.Get (3));
		}

		// C# class that implements a Java generic interface (ICustomList)
		public class MyCustomList<T> : Java.Lang.Object, ICustomList<T> where T : Java.Lang.Object
		{
			readonly List<T> list = new List<T> ();

			public bool Add (T p0)
			{
				list.Add (p0);
				return true;
			}

			public bool AddAll (ICollection<T> p0)
			{
				list.AddRange (p0);
				return true;
			}

			public T Get (int p0)
			{
				return list [p0];
			}
		}

		void TestPerformance ()
		{
			var my_class = new Android.Graphics.Point (10, 15);

			var non_generic = new MyErasedGenericType ();
			var generic = new MyGenericType<Android.Graphics.Point> ();

			var sw = System.Diagnostics.Stopwatch.StartNew ();
			non_generic.TestPerformance (my_class, 100000);
			var t1 = $"erased: {sw.ElapsedMilliseconds}ms";

			sw = System.Diagnostics.Stopwatch.StartNew ();
			generic.TestPerformance (my_class, 100000);
			var t2 = $"generic: {sw.ElapsedMilliseconds}ms";

			System.Diagnostics.Debug.WriteLine (t1);
			System.Diagnostics.Debug.WriteLine (t2);

			var tv = FindViewById<TextView> (Resource.Id.textView1);

			tv.Text = t1 + System.Environment.NewLine + t2;
		}

		public class MyErasedGenericType : ErasedGenericType
		{
			public override void PerformanceMethod (Java.Lang.Object p0)
			{
			}
		}

		public class MyGenericType<T> : GenericType<T> where T : Java.Lang.Object
		{
			public override void PerformanceMethod (T p0)
			{
			}
		}
	}
}

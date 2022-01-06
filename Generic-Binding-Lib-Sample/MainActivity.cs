using System.Diagnostics;
using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Example;
using Java.Lang;

namespace Generic_Binding_Lib_Sample
{
	[Activity (Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			Xamarin.Essentials.Platform.Init (this, savedInstanceState);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_main);

			var my_class = new MyClass ();

			var non_generic = new MyErasedGenericType ();
			var generic = new MyGenericType<MyClass> ();

			var sw = Stopwatch.StartNew ();
			non_generic.TestPerformance (my_class, 100000);
			var t1 = $"erased: {sw.ElapsedMilliseconds}ms";

			sw = Stopwatch.StartNew ();
			generic.TestPerformance (my_class, 100000);
			var t2 = $"generic: {sw.ElapsedMilliseconds}ms";

			System.Diagnostics.Debug.WriteLine (t1);
			System.Diagnostics.Debug.WriteLine (t2);
		}

		public override void OnRequestPermissionsResult (int requestCode, string [] permissions, [GeneratedEnum] Android.Content.PM.Permission [] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult (requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult (requestCode, permissions, grantResults);
		}

		public class MyClass : Java.Lang.Object
		{

		}

		public class MyErasedGenericType : ErasedGenericType
		{
			public override void PerformanceMethod (Object p0)
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

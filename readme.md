# POC for binding Java generics as C# generics

Today, we allow Java to invoke into a C# type by using a static method in the C# type which we can call with reflection.

For example, given:

```java
public class ArrayList
{
	public virtual void Add (Object obj) { ... }
}
```

We can give Java a static function delegate to call which can invoke the method on the instance:

```csharp
public class ArrayList
{
	public virtual void Add (Object obj) { ... }

	public static void n_Add (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
	{
		var __this = global::Java.Lang.Object.GetObject<global::ArrayList> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
		var p0 = global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
		__this.Add (p0);
	}
}
```

However if `ArrayList` was generic, you cannot call it without knowing what `T` is, which you cannot know in a `static` method.

```csharp
public class ArrayList<T>
{
	public static void n_Add (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
	{
		// What goes here? ------------------------------------------------˅
		var __this = global::Java.Lang.Object.GetObject<global::ArrayList<???> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
		...
	}
}
```

You also can't call the method in the first place with reflection, because you need to provide `T` to invoke the method:

```csharp
public class Test<T>
{
	public static void DoThing () { }
}

var type = Assembly.GetExecutingAssembly ().GetType ("Test`1");
var method = type.GetMethod ("DoThing", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
method.Invoke (null, null);
```

produces:
```
System.InvalidOperationException: Late bound operations cannot be performed on types or methods for which ContainsGenericParameters is true.
```

This POC proposes having both a `static` invoker AND an `instance` invoker:
- Java calls the static invoker
- The static invoker calls the instance invoker
- The instance invoker calls the desired method

The instance invoker method is defined on a non-generic "Invoker" interface so it can be called without needing generic type information,
but is implemented as an instance method in the generic bound class which does have access to generic type information needed to call the user's method.

This extra level of indirection incurs a ~2%-3% performance penalty when marshaling methods that use generic types:
https://github.com/xamarin/java.interop/issues/918#issuecomment-1006950735

## Binding a generic Java class example

Full sample:
- [Java Type](https://github.com/jpobst/GenericBindingPrototype/blob/main/java/GenericType.java)
- [C# Bound Type](https://github.com/jpobst/GenericBindingPrototype/blob/main/Generic-Binding-Lib/Additions/Example.GenericType.cs)
- [Usage](https://github.com/jpobst/GenericBindingPrototype/blob/main/Generic-Binding-Lib-Sample/MainActivity.cs#L77-L98)

```csharp
public interface IArrayListInvoker : IJavaObject, Java.Interop.IJavaPeerable
{
	void InvokeAdd (Object obj);
}

[global::Android.Runtime.Register ("java/util/ArrayList", DoNotGenerateAcw=true)]
public class ArrayList<T> : IArrayListInvoker
{
	// User's method
	[Register ("add", "(Ljava/lang/Object;)Z", "GetAdd_Ljava_lang_Object_Handler")]
	public virtual void Add (T obj) { ... }

	// Static invoker
	public static void n_Add (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
	{
		// Cast as non-generic IArrayListInvoker type
		var __this = global::Java.Lang.Object.GetObject<global::IArrayListInvoker> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
		var p0 = global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
		__this.InvokeAdd (p0);
	}

	// Instance invoker
	public void IArrayListInvoker.InvokeAdd (Object obj) => Add (obj.JavaCast<T> ());
}
```

## Binding a generic Java interface example

Full sample:
- [Java Type](https://github.com/jpobst/GenericBindingPrototype/blob/main/java/CustomList.java)
- [C# Bound Type](https://github.com/jpobst/GenericBindingPrototype/blob/main/Generic-Binding-Lib/Additions/Example.ICustomList.cs)
- [Usage](https://github.com/jpobst/GenericBindingPrototype/blob/main/Generic-Binding-Lib-Sample/MainActivity.cs#L27-L52)

Binding a generic Java interface is a bit more challenging, but we can (ab)use default interface
members in order to accomplish the same "static invoker" -> "instance invoker" strategy.

**Generic Java interface**

```java
public interface CustomList<T> {
	public abstract boolean add (T obj);
}
```

**C# binding**

```csharp
public interface ICustomListInterfaceInvoker : IJavaObject, Java.Interop.IJavaPeerable
{
	bool InvokeAdd (global::Java.Lang.Object obj);
}

[Register ("example/CustomList", "", "Example.ICustomListInvoker")]
public partial interface ICustomList<T> : IJavaObject, Java.Interop.IJavaPeerable, ICustomListInterfaceInvoker where T : global::Java.Lang.Object
{
	// User's method
	[Register ("add", "(Ljava/lang/Object;)Z", "GetAdd_Ljava_lang_Object_Handler:Example.ICustomListInvoker, Generic-Binding-Lib")]
	bool Add (T p0);

	// Instance invoker (DIM)
	bool ICustomListInterfaceInvoker.InvokeAdd (global::Java.Lang.Object obj) => Add (obj.JavaCast<T> ());
}

[global::Android.Runtime.Register ("example/CustomList", DoNotGenerateAcw=true)]
internal partial class ICustomListInvoker : global::Java.Lang.Object
{
	static Delegate cb_add_Ljava_lang_Object_;

	static Delegate GetAdd_Ljava_lang_Object_Handler ()
		=> cb_add_Ljava_lang_Object_ ??= JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_Z) n_Add_Ljava_lang_Object_);

	// Static invoker
	static bool n_Add_Ljava_lang_Object_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
	{
		// Cast as non-generic ICustomListInterfaceInvoker type
		var __this = global::Java.Lang.Object.GetObject<global::Example.ICustomListInterfaceInvoker> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
		var p0 = global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
		return __this.InvokeAdd (p0);
	}
}
```

using System;
using System.Collections.Generic;
using System.Linq;
using Android.Runtime;

namespace Example
{
	// Metadata.xml XPath interface reference: path="/api/package[@name='example']/interface[@name='CustomList']"
	[Register ("example/CustomList", "", "Example.ICustomListInvoker")]
	[global::Java.Interop.JavaTypeParameters (new string [] { "E" })]
	public partial interface ICustomList<T> : IJavaObject, Java.Interop.IJavaPeerable, ICustomListInterfaceInvoker where T : global::Java.Lang.Object
	{
		// Metadata.xml XPath method reference: path="/api/package[@name='example']/interface[@name='CustomList']/method[@name='add' and count(parameter)=1 and parameter[1][@type='E']]"
		[Register ("add", "(Ljava/lang/Object;)Z", "GetAdd_Ljava_lang_Object_Handler:Example.ICustomListInvoker, Generic-Binding-Lib")]
		bool Add (T p0);

		// Metadata.xml XPath method reference: path="/api/package[@name='example']/interface[@name='CustomList']/method[@name='addAll' and count(parameter)=1 and parameter[1][@type='java.util.Collection&lt;? extends E&gt;']]"
		[Register ("addAll", "(Ljava/util/Collection;)Z", "GetAddAll_Ljava_util_Collection_Handler:Example.ICustomListInvoker, Generic-Binding-Lib")]
		bool AddAll (global::System.Collections.Generic.ICollection<T> p0);

		// Metadata.xml XPath method reference: path="/api/package[@name='example']/interface[@name='CustomList']/method[@name='get' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("get", "(I)Ljava/lang/Object;", "GetGet_IHandler:Example.ICustomListInvoker, Generic-Binding-Lib")]
		T Get (int p0);

		bool ICustomListInterfaceInvoker.InvokeAdd (global::Java.Lang.Object obj)
			=> Add (obj.JavaCast<T> ());

		bool ICustomListInterfaceInvoker.InvokeAddAll (global::System.Collections.ICollection obj)
			=> AddAll (obj.OfType<Java.Lang.Object> ().Select (o => o.JavaCast<T> ()).ToArray ());

		global::Java.Lang.Object ICustomListInterfaceInvoker.InvokeGet (int p0)
			=> Get (p0);
	}

	public interface ICustomListInterfaceInvoker : IJavaObject, Java.Interop.IJavaPeerable
	{
		bool InvokeAdd (global::Java.Lang.Object obj);

		bool InvokeAddAll (global::System.Collections.ICollection obj);

		global::Java.Lang.Object InvokeGet (int p0);
	}

	[global::Android.Runtime.Register ("example/CustomList", DoNotGenerateAcw = true)]
	internal partial class ICustomListInvoker : global::Java.Lang.Object
	{
		static readonly Java.Interop.JniPeerMembers _members = new XAPeerMembers ("example/CustomList", typeof (ICustomListInvoker));

		static IntPtr java_class_ref {
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		IntPtr class_ref;

		public static ICustomListInterfaceInvoker GetObject (IntPtr handle, JniHandleOwnership transfer)
		{
			return global::Java.Lang.Object.GetObject<ICustomListInterfaceInvoker> (handle, transfer);
		}

		static IntPtr Validate (IntPtr handle)
		{
			if (!JNIEnv.IsInstanceOf (handle, java_class_ref))
				throw new InvalidCastException ($"Unable to convert instance of type '{JNIEnv.GetClassNameFromInstance (handle)}' to type 'example.CustomList'.");
			return handle;
		}

		protected override void Dispose (bool disposing)
		{
			if (this.class_ref != IntPtr.Zero)
				JNIEnv.DeleteGlobalRef (this.class_ref);
			this.class_ref = IntPtr.Zero;
			base.Dispose (disposing);
		}

		public ICustomListInvoker (IntPtr handle, JniHandleOwnership transfer) : base (Validate (handle), transfer)
		{
			IntPtr local_ref = JNIEnv.GetObjectClass (((global::Java.Lang.Object) this).Handle);
			this.class_ref = JNIEnv.NewGlobalRef (local_ref);
			JNIEnv.DeleteLocalRef (local_ref);
		}

		static Delegate cb_add_Ljava_lang_Object_;
#pragma warning disable 0169
		static Delegate GetAdd_Ljava_lang_Object_Handler ()
		{
			if (cb_add_Ljava_lang_Object_ == null)
				cb_add_Ljava_lang_Object_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_Z) n_Add_Ljava_lang_Object_);
			return cb_add_Ljava_lang_Object_;
		}

		static bool n_Add_Ljava_lang_Object_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Example.ICustomListInterfaceInvoker> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
			return __this.InvokeAdd (p0);
		}
#pragma warning restore 0169

		IntPtr id_add_Ljava_lang_Object_;
		public unsafe bool InvokeAdd (global::Java.Lang.Object p0)
		{
			if (id_add_Ljava_lang_Object_ == IntPtr.Zero)
				id_add_Ljava_lang_Object_ = JNIEnv.GetMethodID (class_ref, "add", "(Ljava/lang/Object;)Z");
			IntPtr native_p0 = JNIEnv.ToLocalJniHandle (p0);
			JValue* __args = stackalloc JValue [1];
			__args [0] = new JValue (native_p0);
			var __ret = JNIEnv.CallBooleanMethod (((global::Java.Lang.Object) this).Handle, id_add_Ljava_lang_Object_, __args);
			JNIEnv.DeleteLocalRef (native_p0);
			return __ret;
		}

		static Delegate cb_addAll_Ljava_util_Collection_;
#pragma warning disable 0169
		static Delegate GetAddAll_Ljava_util_Collection_Handler ()
		{
			if (cb_addAll_Ljava_util_Collection_ == null)
				cb_addAll_Ljava_util_Collection_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_Z) n_AddAll_Ljava_util_Collection_);
			return cb_addAll_Ljava_util_Collection_;
		}

		static bool n_AddAll_Ljava_util_Collection_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Example.ICustomListInterfaceInvoker> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Android.Runtime.JavaCollection.FromJniHandle (native_p0, JniHandleOwnership.DoNotTransfer);
			bool __ret = __this.InvokeAddAll (p0);
			return __ret;
		}
#pragma warning restore 0169

		IntPtr id_addAll_Ljava_util_Collection_;
		public unsafe bool InvokeAddAll (global::System.Collections.ICollection p0)
		{
			if (id_addAll_Ljava_util_Collection_ == IntPtr.Zero)
				id_addAll_Ljava_util_Collection_ = JNIEnv.GetMethodID (class_ref, "addAll", "(Ljava/util/Collection;)Z");
			IntPtr native_p0 = global::Android.Runtime.JavaCollection.ToLocalJniHandle (p0);
			JValue* __args = stackalloc JValue [1];
			__args [0] = new JValue (native_p0);
			var __ret = JNIEnv.CallBooleanMethod (((global::Java.Lang.Object) this).Handle, id_addAll_Ljava_util_Collection_, __args);
			JNIEnv.DeleteLocalRef (native_p0);
			return __ret;
		}

		static Delegate cb_get_I;
#pragma warning disable 0169
		static Delegate GetGet_IHandler ()
		{
			if (cb_get_I == null)
				cb_get_I = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPI_L) n_Get_I);
			return cb_get_I;
		}

		static IntPtr n_Get_I (IntPtr jnienv, IntPtr native__this, int p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Example.ICustomListInterfaceInvoker> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return JNIEnv.ToLocalJniHandle (__this.InvokeGet (p0));
		}
#pragma warning restore 0169

		IntPtr id_get_I;
		public unsafe global::Java.Lang.Object InvokeGet (int p0)
		{
			if (id_get_I == IntPtr.Zero)
				id_get_I = JNIEnv.GetMethodID (class_ref, "get", "(I)Ljava/lang/Object;");
			JValue* __args = stackalloc JValue [1];
			__args [0] = new JValue (p0);
			return (global::Java.Lang.Object) global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (JNIEnv.CallObjectMethod (((global::Java.Lang.Object) this).Handle, id_get_I, __args), JniHandleOwnership.TransferLocalRef);
		}

	}
}

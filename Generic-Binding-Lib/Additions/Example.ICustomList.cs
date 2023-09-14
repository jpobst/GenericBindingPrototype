using System;
using System.Collections.Generic;
using System.Linq;
using Android.Runtime;

namespace Example
{
	// Metadata.xml XPath interface reference: path="/api/package[@name='example']/interface[@name='CustomList']"
//	[Register ("example/CustomList", "", "Example.ICustomListInvoker", DoNotGenerateAcw = true)]
//	[global::Java.Interop.JavaTypeParameters (new string [] { "E" })]
	public partial interface ICustomList<T> : IJavaObject, Java.Interop.IJavaPeerable, global::jniabi.example._JniabiCustomList
		where T : global::Java.Lang.Object
	{
		// Metadata.xml XPath method reference: path="/api/package[@name='example']/interface[@name='CustomList']/method[@name='add' and count(parameter)=1 and parameter[1][@type='E']]"
		[Register ("add", "(Ljava/lang/Object;)Z", "GetAdd_Ljava_lang_Object_Handler:jniabi.example._JniabiCustomList, Generic-Binding-Lib")]
		bool Add (T? p0);

		// Metadata.xml XPath method reference: path="/api/package[@name='example']/interface[@name='CustomList']/method[@name='addAll' and count(parameter)=1 and parameter[1][@type='java.util.Collection&lt;? extends E&gt;']]"
		[Register ("addAll", "(Ljava/util/Collection;)Z", "GetAddAll_Ljava_util_Collection_Handler:jniabi.example._JniabiCustomList, Generic-Binding-Lib")]
		bool AddAll (global::System.Collections.Generic.ICollection<T>? p0);

		// Metadata.xml XPath method reference: path="/api/package[@name='example']/interface[@name='CustomList']/method[@name='get' and count(parameter)=1 and parameter[1][@type='int']]"
		[Register ("get", "(I)Ljava/lang/Object;", "GetGet_IHandler:jniabi.example._JniabiCustomList, Generic-Binding-Lib")]
		T? Get (int p0);

		bool global::jniabi.example._JniabiCustomList.add (global::Java.Interop.JniObjectReference native_obj)
		{
			var obj = global::Java.Lang.Object.GetObject<T> (native_obj.Handle, JniHandleOwnership.DoNotTransfer);
			return Add (obj);
		}

		bool global::jniabi.example._JniabiCustomList.addAll (global::Java.Interop.JniObjectReference native_c)
		{
			var c = JavaCollection<T>.FromJniHandle (native_c.Handle, JniHandleOwnership.DoNotRegister);
			return AddAll (c);
		}

		global::Java.Interop.JniObjectReference global::jniabi.example._JniabiCustomList.get (int p0)
		{
			var r = Get (p0);
			return new global::Java.Interop.JniObjectReference (JNIEnv.ToLocalJniHandle (r), Java.Interop.JniObjectReferenceType.Local);
		}
	}

	[global::Android.Runtime.Register ("example/CustomList", DoNotGenerateAcw = true)]
	internal partial class ICustomListInvoker : global::Java.Lang.Object, global::jniabi.example._JniabiCustomList
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

		public static global::jniabi.example._JniabiCustomList? GetObject (IntPtr handle, JniHandleOwnership transfer)
		{
			return global::Java.Lang.Object.GetObject<global::jniabi.example._JniabiCustomList> (handle, transfer);
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

		public unsafe bool add (global::Java.Interop.JniObjectReference p0)
		{
			return global::jniabi.example.JniabiCustomList.add (this, p0);
		}

		public unsafe bool addAll (global::Java.Interop.JniObjectReference p0)
		{
			return global::jniabi.example.JniabiCustomList.addAll (this, p0);
		}

		public unsafe global::Java.Interop.JniObjectReference get (int p0)
		{
			return global::jniabi.example.JniabiCustomList.get (this, p0);
		}
	}
}

namespace jniabi.example {
	using System.ComponentModel;
	using Java.Interop;

	[EditorBrowsable (EditorBrowsableState.Never)]
	public static partial class JniabiCustomList {

		public static JniPeerMembers _class => _JniabiCustomList._class;

		public static unsafe bool add (IJavaPeerable self, JniObjectReference e)
		{
			const string __id = "add.(Ljava/lang/Object;)V";
			try {
				Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue [1];
				__args [0] = new Java.Interop.JniArgumentValue (e);
				return _class.InstanceMethods.InvokeAbstractBooleanMethod (__id, self, __args);
			} finally {
				global::System.GC.KeepAlive (self);
			}
		}

		public static unsafe JniObjectReference get(IJavaPeerable self, int index)
		{
			const string __id = "get.(I)Ljava/lang/Object;";
			try {
				Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue [1];
				__args [0] = new Java.Interop.JniArgumentValue (index);
				return _class.InstanceMethods.InvokeAbstractObjectMethod (__id, self, __args);
			} finally {
				global::System.GC.KeepAlive (self);
			}
		}

		public static unsafe bool addAll(IJavaPeerable self, JniObjectReference c)
		{
			const string __id = "addAll.(Ljava/util/Collection;)V";
			try {
				Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue [1];
				__args [0] = new Java.Interop.JniArgumentValue (c);
				return _class.InstanceMethods.InvokeAbstractBooleanMethod (__id, self, __args);
			} finally {
				global::System.GC.KeepAlive (self);
			}
		}
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Register (JniTypeName, DoNotGenerateAcw = true)]
	public partial interface _JniabiCustomList : IJavaObject, IJavaPeerable {
		public const string JniTypeName = "example/CustomList";
		public static readonly Java.Interop.JniPeerMembers _class = new XAPeerMembers (JniTypeName, typeof (_JniabiCustomList), isInterface: true);

		[Register ("add", "(Ljava/lang/Object;)Z", "GetAdd_Ljava_lang_Object_Handler:jniabi.example._JniabiCustomList, Generic-Binding-Lib")]
		public bool add(JniObjectReference e);
		[Register ("get", "(I)Ljava/lang/Object;", "GetGet_IHandler:jniabi.example._JniabiCustomList, Generic-Binding-Lib")]
		public JniObjectReference get(int index);
		[Register ("addAll", "(Ljava/util/Collection;)Z", "GetAddAll_Ljava_util_Collection_Handler:jniabi.example._JniabiCustomList, Generic-Binding-Lib")]
		public bool addAll(JniObjectReference p0);


		private static Delegate? cb_add_Ljava_lang_Object_;
#pragma warning disable 0169
		private static Delegate GetAdd_Ljava_lang_Object_Handler ()
		{
			if (cb_add_Ljava_lang_Object_ == null)
				cb_add_Ljava_lang_Object_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_Z) n_Add_Ljava_lang_Object_);
			return cb_add_Ljava_lang_Object_;
		}

		private static bool n_Add_Ljava_lang_Object_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<_JniabiCustomList> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			try {
				var p0 = new JniObjectReference(native_p0);
				return __this!.add (p0);
			}
			finally {
				GC.KeepAlive (__this);
			}
		}
#pragma warning restore 0169

		private static Delegate? cb_addAll_Ljava_util_Collection_;
#pragma warning disable 0169
		private static Delegate GetAddAll_Ljava_util_Collection_Handler ()
		{
			if (cb_addAll_Ljava_util_Collection_ == null)
				cb_addAll_Ljava_util_Collection_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_Z) n_AddAll_Ljava_util_Collection_);
			return cb_addAll_Ljava_util_Collection_;
		}

		private static bool n_AddAll_Ljava_util_Collection_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<_JniabiCustomList> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			try {
				var p0 = new JniObjectReference(native_p0);
				return __this!.addAll (p0);
			}
			finally {
				GC.KeepAlive (__this);
			}
		}
#pragma warning restore 0169

		private static Delegate? cb_get_I;
#pragma warning disable 0169
		private static Delegate GetGet_IHandler ()
		{
			if (cb_get_I == null)
				cb_get_I = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPI_L) n_Get_I);
			return cb_get_I;
		}

		private static IntPtr n_Get_I (IntPtr jnienv, IntPtr native__this, int p0)
		{
			var __this = global::Java.Lang.Object.GetObject<_JniabiCustomList> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			try {
				return __this!.get (p0).Handle;
			}
			finally {
				GC.KeepAlive (__this);
			}
		}
#pragma warning restore 0169
	}
}

using System;
using Android.Runtime;
using Java.Interop;

namespace Example {

	// Metadata.xml XPath class reference: path="/api/package[@name='example']/class[@name='ErasedGenericType']"
	[global::Android.Runtime.Register ("example/ErasedGenericType", DoNotGenerateAcw=true)]
	[global::Java.Interop.JavaTypeParameters (new string [] {"T"})]
	public partial class ErasedGenericType : global::Java.Lang.Object {
		static readonly JniPeerMembers _members = new XAPeerMembers ("example/ErasedGenericType", typeof (ErasedGenericType));

		internal static IntPtr class_ref {
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
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		protected ErasedGenericType (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer)
		{
		}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='example']/class[@name='ErasedGenericType']/constructor[@name='ErasedGenericType' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe ErasedGenericType () : base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			const string __id = "()V";

			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				var __r = _members.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (), null);
				SetHandle (__r.Handle, JniHandleOwnership.TransferLocalRef);
				_members.InstanceMethods.FinishCreateInstance (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_PerformanceMethod_Ljava_lang_Object_;
#pragma warning disable 0169
		static Delegate GetPerformanceMethod_Ljava_lang_Object_Handler ()
		{
			if (cb_PerformanceMethod_Ljava_lang_Object_ == null)
				cb_PerformanceMethod_Ljava_lang_Object_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_V) n_PerformanceMethod_Ljava_lang_Object_);
			return cb_PerformanceMethod_Ljava_lang_Object_;
		}

		static void n_PerformanceMethod_Ljava_lang_Object_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Example.ErasedGenericType> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
			__this.PerformanceMethod (p0);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='example']/class[@name='ErasedGenericType']/method[@name='PerformanceMethod' and count(parameter)=1 and parameter[1][@type='T']]"
		[Register ("PerformanceMethod", "(Ljava/lang/Object;)V", "GetPerformanceMethod_Ljava_lang_Object_Handler")]
		public virtual unsafe void PerformanceMethod (global::Java.Lang.Object p0)
		{
			const string __id = "PerformanceMethod.(Ljava/lang/Object;)V";
			IntPtr native_p0 = JNIEnv.ToLocalJniHandle (p0);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (native_p0);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
				JNIEnv.DeleteLocalRef (native_p0);
				global::System.GC.KeepAlive (p0);
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='example']/class[@name='ErasedGenericType']/method[@name='TestPerformance' and count(parameter)=2 and parameter[1][@type='T'] and parameter[2][@type='int']]"
		[Register ("TestPerformance", "(Ljava/lang/Object;I)V", "")]
		public unsafe void TestPerformance (global::Java.Lang.Object p0, int p1)
		{
			const string __id = "TestPerformance.(Ljava/lang/Object;I)V";
			IntPtr native_p0 = JNIEnv.ToLocalJniHandle (p0);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue (native_p0);
				__args [1] = new JniArgumentValue (p1);
				_members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
				JNIEnv.DeleteLocalRef (native_p0);
				global::System.GC.KeepAlive (p0);
			}
		}

	}
}

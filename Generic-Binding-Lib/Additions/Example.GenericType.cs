using System;
using Android.Runtime;

internal delegate void _JniMarshal_PPL_V (IntPtr jnienv, IntPtr klass, IntPtr p0);
internal delegate void _JniMarshal_PPLI_V (IntPtr jnienv, IntPtr klass, IntPtr p0, int p1);

namespace Example
{
	public interface IGenericTypeInvoker : IJavaObject, Java.Interop.IJavaPeerable
	{
		static readonly Java.Interop.JniPeerMembers _members = new XAPeerMembers ("example/GenericType", typeof (IGenericTypeInvoker));

		void InvokePerformanceMethod (global::Java.Lang.Object obj);

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
			var __this = global::Java.Lang.Object.GetObject<global::Example.IGenericTypeInvoker> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (native_p0, JniHandleOwnership.DoNotTransfer);
			__this.InvokePerformanceMethod (p0);
		}
#pragma warning restore 0169
	}

	[global::Android.Runtime.Register ("example/GenericType", "", "Example.IGenericTypeInvoker", DoNotGenerateAcw = true)]
	public partial class GenericType<T> : global::Java.Lang.Object, IGenericTypeInvoker where T : global::Java.Lang.Object
	{
		[Register ("PerformanceMethod", "(Ljava/lang/Object;)V", "GetPerformanceMethod_Ljava_lang_Object_Handler:Example.IGenericTypeInvoker, Generic-Binding-Lib, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")]
		public virtual unsafe void PerformanceMethod (T p0) { }

		public void InvokePerformanceMethod (global::Java.Lang.Object obj) => PerformanceMethod (obj.JavaCast<T> ());

		internal static IntPtr class_ref {
			get { return IGenericTypeInvoker._members.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return IGenericTypeInvoker._members; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override IntPtr ThresholdClass {
			get { return IGenericTypeInvoker._members.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override global::System.Type ThresholdType {
			get { return IGenericTypeInvoker._members.ManagedPeerType; }
		}

		protected GenericType (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer)
		{
		}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='example']/class[@name='GenericType']/constructor[@name='GenericType' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe GenericType () : base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			const string __id = "()V";

			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				var __r = IGenericTypeInvoker._members.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (), null);
				SetHandle (__r.Handle, JniHandleOwnership.TransferLocalRef);
				IGenericTypeInvoker._members.InstanceMethods.FinishCreateInstance (__id, this, null);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='example']/class[@name='GenericType']/method[@name='TestPerformance' and count(parameter)=2 and parameter[1][@type='T'] and parameter[2][@type='int']]"
		[Register ("TestPerformance", "(Ljava/lang/Object;I)V", "")]
		public unsafe void TestPerformance (T p0, int p1)
		{
			const string __id = "TestPerformance.(Ljava/lang/Object;I)V";
			IntPtr native_p0 = JNIEnv.ToLocalJniHandle (p0);
			try {
				Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue [2];
				__args [0] = new Java.Interop.JniArgumentValue (native_p0);
				__args [1] = new Java.Interop.JniArgumentValue (p1);
				IGenericTypeInvoker._members.InstanceMethods.InvokeNonvirtualVoidMethod (__id, this, __args);
			} finally {
				JNIEnv.DeleteLocalRef (native_p0);
				global::System.GC.KeepAlive (p0);
			}
		}
	}
}

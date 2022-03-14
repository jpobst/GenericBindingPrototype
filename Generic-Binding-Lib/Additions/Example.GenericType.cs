using System;
using Android.Runtime;

internal delegate void _JniMarshal_PPL_V (IntPtr jnienv, IntPtr klass, IntPtr p0);
internal delegate void _JniMarshal_PPLI_V (IntPtr jnienv, IntPtr klass, IntPtr p0, int p1);
internal delegate bool _JniMarshal_PPL_Z (IntPtr jnienv, IntPtr klass, IntPtr p0);
internal delegate IntPtr _JniMarshal_PPI_L (IntPtr jnienv, IntPtr klass, int p0);

namespace Example
{
	[global::Android.Runtime.Register ("example/GenericType", "", "Example.IGenericTypeInvoker", DoNotGenerateAcw = true)]
	public partial class GenericType : global::Java.Lang.Object, global::jniabi.example._JniabiGenericType
	{
		[Register ("PerformanceMethod", "(Ljava/lang/Object;)V", "GetPerformanceMethod_Ljava_lang_Object_Handler:jniabi.example._JniabiGenericType, Generic-Binding-Lib")]
		public virtual unsafe void PerformanceMethod (Java.Lang.Object? p0)
		{
			jniabi.example.JniabiGenericType.PerformanceMethod (this, p0?.PeerReference ?? default);
		}

		void global::jniabi.example._JniabiGenericType.PerformanceMethod (global::Java.Interop.JniObjectReference native_p0)
		{
			var p0 = global::Java.Lang.Object.GetObject<Java.Lang.Object> (native_p0.Handle, JniHandleOwnership.DoNotTransfer);
			PerformanceMethod (p0);
		}

		internal static IntPtr class_ref {
			get { return global::jniabi.example.JniabiGenericType._class.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return global::jniabi.example.JniabiGenericType._class; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override IntPtr ThresholdClass {
			get { return global::jniabi.example.JniabiGenericType._class.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override global::System.Type ThresholdType {
			get { return typeof (GenericType); }
		}

		protected GenericType (IntPtr javaReference, JniHandleOwnership transfer)
			: base (javaReference, transfer)
		{
		}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='example']/class[@name='GenericType']/constructor[@name='GenericType' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe GenericType ()
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			const string __id = "()V";

			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				var __r = global::jniabi.example.JniabiGenericType._class.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (), null);
				SetHandle (__r.Handle, JniHandleOwnership.TransferLocalRef);
				global::jniabi.example.JniabiGenericType._class.InstanceMethods.FinishCreateInstance (__id, this, null);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='example']/class[@name='GenericType']/method[@name='TestPerformance' and count(parameter)=2 and parameter[1][@type='T'] and parameter[2][@type='int']]"
		[Register ("TestPerformance", "(Ljava/lang/Object;I)V", "")]
		public unsafe void TestPerformance (Java.Lang.Object? p0, int p1)
		{
			try {
				global::jniabi.example.JniabiGenericType.TestPerformance (this, p0?.PeerReference ?? default, p1);
			}
			finally {
				GC.KeepAlive (p0);
			}
		}
	}

	[global::Android.Runtime.Register ("example/GenericType", "", "Example.IGenericTypeInvoker", DoNotGenerateAcw = true)]
	public partial class GenericType<T> : global::Java.Lang.Object, global::jniabi.example._JniabiGenericType
		where T : global::Java.Lang.Object
	{
		[Register ("PerformanceMethod", "(Ljava/lang/Object;)V", "GetPerformanceMethod_Ljava_lang_Object_Handler:jniabi.example._JniabiGenericType, Generic-Binding-Lib")]
		public virtual unsafe void PerformanceMethod (T? p0)
		{
			jniabi.example.JniabiGenericType.PerformanceMethod (this, p0?.PeerReference ?? default);
		}

		void global::jniabi.example._JniabiGenericType.PerformanceMethod (global::Java.Interop.JniObjectReference native_p0)
		{
			var p0 = global::Java.Lang.Object.GetObject<T> (native_p0.Handle, JniHandleOwnership.DoNotTransfer);
			PerformanceMethod (p0);
		}

		internal static IntPtr class_ref {
			get { return global::jniabi.example.JniabiGenericType._class.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return global::jniabi.example.JniabiGenericType._class; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override IntPtr ThresholdClass {
			get { return global::jniabi.example.JniabiGenericType._class.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override global::System.Type ThresholdType {
			get { return typeof (GenericType<T>); }
		}

		protected GenericType (IntPtr javaReference, JniHandleOwnership transfer)
			: base (javaReference, transfer)
		{
		}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='example']/class[@name='GenericType']/constructor[@name='GenericType' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe GenericType ()
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			const string __id = "()V";

			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				var __r = global::jniabi.example.JniabiGenericType._class.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (), null);
				SetHandle (__r.Handle, JniHandleOwnership.TransferLocalRef);
				global::jniabi.example.JniabiGenericType._class.InstanceMethods.FinishCreateInstance (__id, this, null);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='example']/class[@name='GenericType']/method[@name='TestPerformance' and count(parameter)=2 and parameter[1][@type='T'] and parameter[2][@type='int']]"
		[Register ("TestPerformance", "(Ljava/lang/Object;I)V", "")]
		public unsafe void TestPerformance (T p0, int p1)
		{
			try {
				global::jniabi.example.JniabiGenericType.TestPerformance (this, p0.PeerReference, p1);
			}
			finally {
				GC.KeepAlive (p0);
			}
		}
	}
}

namespace jniabi.example {
	using System.ComponentModel;
	using Java.Interop;

	[EditorBrowsable (EditorBrowsableState.Never)]
	public static partial class JniabiGenericType {

		public static JniPeerMembers _class => _JniabiGenericType._class;

		public static unsafe void PerformanceMethod (IJavaPeerable self, JniObjectReference obj)
		{
			const string __id = "PerformanceMethod.(Ljava/lang/Object;)V";
			try {
				Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue [1];
				__args [0] = new Java.Interop.JniArgumentValue (obj);
				_JniabiGenericType._class.InstanceMethods.InvokeVirtualVoidMethod (__id, self, __args);
			} finally {
				global::System.GC.KeepAlive (self);
			}
		}

		public static unsafe void TestPerformance (IJavaPeerable self, JniObjectReference p0, int p1)
		{
			const string __id = "TestPerformance.(Ljava/lang/Object;I)V";
			try {
				Java.Interop.JniArgumentValue* __args = stackalloc Java.Interop.JniArgumentValue [2];
				__args [0] = new Java.Interop.JniArgumentValue (p0);
				__args [1] = new Java.Interop.JniArgumentValue (p1);
				_JniabiGenericType._class.InstanceMethods.InvokeNonvirtualVoidMethod (__id, self, __args);
			} finally {
				global::System.GC.KeepAlive (self);
			}
		}
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Register (JniTypeName)]
	public interface _JniabiGenericType : IJavaObject, global::Java.Interop.IJavaPeerable {

		public const string JniTypeName = "example/GenericType";
		public static readonly global::Java.Interop.JniPeerMembers _class = new XAPeerMembers (JniTypeName, typeof (_JniabiGenericType));

		void PerformanceMethod (JniObjectReference obj);

		private static Delegate? cb_PerformanceMethod_Ljava_lang_Object_;
#pragma warning disable 0169
		private static Delegate GetPerformanceMethod_Ljava_lang_Object_Handler ()
		{
			if (cb_PerformanceMethod_Ljava_lang_Object_ == null)
				cb_PerformanceMethod_Ljava_lang_Object_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_V) n_PerformanceMethod_Ljava_lang_Object_);
			return cb_PerformanceMethod_Ljava_lang_Object_;
		}

		private static void n_PerformanceMethod_Ljava_lang_Object_ (IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
		{
			var __this = global::Java.Lang.Object.GetObject<_JniabiGenericType> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var p0 = new JniObjectReference (native_p0);
			__this!.PerformanceMethod (p0);
		}
#pragma warning restore 0169
	}
}

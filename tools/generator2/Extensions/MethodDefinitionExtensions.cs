using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Javil;

namespace generator2
{
	public static class MethodDefinitionExtensions
	{
		/// <summary>
		/// Creates a clone of the specified method which the specified type as parent.
		/// This is generally used to inject abstract types where C# requires them but Java doesn't.
		/// </summary>
		public static MethodDefinition CloneAsAbstract (this MethodDefinition method, TypeReference parent)
		{
			var result = new MethodDefinition (method.Name, method.ReturnType, parent) {
				IsStatic = method.IsStatic,
				IsFinal = method.IsFinal,
				IsPublic = method.IsPublic,
				IsProtected = method.IsProtected,
				IsPrivate = method.IsPrivate,
				IsAbstract = true,
				IsSynthetic = method.IsSynthetic,
				IsBridge = method.IsBridge,
				IsVarargs = method.IsVarargs,
				IsSynchronized = method.IsSynchronized,
				IsNative = method.IsNative,
				IsStrict = method.IsStrict,
				IsDeprecated = method.IsDeprecated,
				ReturnTypeNullability = method.ReturnTypeNullability
			};

			// TODO: These probably need to be cloned
			foreach (var tp in method.GenericParameters)
				result.GenericParameters.Add (tp);

			foreach (var p in method.Parameters)
				result.Parameters.Add (new ParameterDefinition (method, p.Name, p.ParameterType, p.Index, p.Nullability));

			// TODO: These probably need to be cloned
			foreach (var t in method.CheckedExceptions)
				result.CheckedExceptions.Add (t);

			return result;
		}
	}
}

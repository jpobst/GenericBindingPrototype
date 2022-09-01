using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Javil;

namespace Java.Interop.Generator
{
	public class ManagedTypeReference : ManagedMemberReference
	{
		private Collection<ManagedGenericParameter>? generic_parameters;

		public virtual string Namespace { get; }

		public Collection<ManagedGenericParameter> GenericParameters => generic_parameters ??= new Collection<ManagedGenericParameter> ();
		public bool HasGenericParameters => generic_parameters?.Any () == true;

		public ManagedTypeReference (string @namespace, string name, ManagedMemberReference? declaringType) : base (name, declaringType)
		{
			Namespace = @namespace;
		}

		static ManagedTypeReference [] primitive_types = new [] {
			new ManagedTypeReference (string.Empty, "string", null),
			new ManagedTypeReference (string.Empty, "sbyte", null),
			new ManagedTypeReference (string.Empty, "char", null),
			new ManagedTypeReference (string.Empty, "double", null),
			new ManagedTypeReference (string.Empty, "float", null),
			new ManagedTypeReference (string.Empty, "int", null),
			new ManagedTypeReference (string.Empty, "long", null),
			new ManagedTypeReference (string.Empty, "short", null),
			new ManagedTypeReference (string.Empty, "void", null),
			new ManagedTypeReference (string.Empty, "bool", null)
		};

		public override string FullName {
			get {
				if (DeclaringType is null)
					return Namespace.HasValue () ? $"{Namespace}.{Name}" : Name;

				return $"{DeclaringType.FullName}.{Name}";
			}
		}

		public override string FullGenericName {
			get {
				if (DeclaringType is null)
					return Namespace.HasValue () ? $"{Namespace}.{GenericName}" : GenericName;

				return $"{DeclaringType.FullGenericName}.{GenericName}";
			}
		}

		protected override ManagedTypeDefinition? ResolveDefinition ()
		{
			if (this is ManagedTypeDefinition td)
				return td;

			return null;
		}

		public override string GenericName {
			get {
				if (!HasGenericParameters)
					return base.Name;

				return $"{base.Name}<{string.Join (", ", GenericParameters.Select (gp => gp.Name))}>";
			}
		}

		public static ManagedTypeReference? ConvertTypeReference (TypeReference? type)
		{
			if (type is null)
				return null;

			var primitive_name = type.FullName.Replace ("boolean", "bool").Replace ("byte", "sbyte").Replace ("java.lang.String", "string");

			if (primitive_types.FirstOrDefault (p => p.FullName == primitive_name) is ManagedTypeReference primitive)
				return primitive;

			if (type.GetType ().FullName == "Javil.TypeReference") {
				var java_type = type.Resolve ();
				var managed_type = java_type?.GetManagedTypeDefinition ();

				return managed_type;
			}

			if (type is GenericParameter gp)
				return new ManagedGenericParameter (gp.Name, ConvertTypeReference (gp.DeclaringType));

			if (type is WildcardType wt)
				return new ManagedWildcardType ();

			if (type is GenericInstanceType gi) {
				var java_type = type.Resolve ();

				if (java_type?.GetManagedTypeDefinition () is null)
					throw new Exception ();

				var managed_type = new ManagedGenericInstanceType (java_type?.GetManagedTypeDefinition ());

				foreach (var ga in gi.GenericArguments)
					if (ConvertTypeReference (ga) is ManagedTypeReference managed_ga)
						managed_type.GenericArguments.Add (managed_ga);

				if (managed_type.GenericArguments.Count == 0)
					Console.WriteLine ();
				return managed_type;
			}

			if (type is ArrayType a)
				if (ConvertTypeReference (a.ElementType) is ManagedTypeReference managed_a)
					return new ManagedArrayType (managed_a, a.Rank);

			return null;
		}
	}
}

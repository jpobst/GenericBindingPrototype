using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Javil;

namespace Java.Interop.Generator
{
	public class ManagedTypeDefinition : ManagedTypeReference
	{
		public ManagedNamespaceReferenceModel? DeclaringNamespace { get; set; }

		public List<ManagedTypeDefinition> NestedTypes { get; } = new List<ManagedTypeDefinition> ();

		public TypeDefinition? JavaType { get; }

		public ManagedTypeReference? BaseType { get; set; }

		public List<ManagedTypeReference> ImplementedInterfaces { get; } = new List<ManagedTypeReference> ();

		public List<ManagedFieldDefinition> Fields { get; } = new List<ManagedFieldDefinition> ();

		public ManagedTypeDefinition (string name, ManagedMemberReference? declaringType, TypeDefinition javaType) : base ("", name.ChopArity (), declaringType)
		{
			JavaType = javaType;
			JavaType?.SetManagedTypeDefinition (this);
		}

		public void Populate ()
		{
			foreach (var nested in NestedTypes)
				nested.Populate ();

			// TODO
			if (JavaType is null)
				return;

			IsInterface = JavaType.IsInterface;
			IsStatic = JavaType.IsStatic;
			IsPublic = JavaType.IsPublic;
			IsProtected = JavaType.IsProtected;
			IsPrivate = JavaType.IsPrivate;
			IsAbstract = JavaType.IsAbstract;
			IsSealed = JavaType.IsFinal;
			IsDeprecated = JavaType.IsDeprecated;

			if (ManagedTypeReference.ConvertTypeReference (JavaType.BaseType) is ManagedTypeReference mtr)
				BaseType = mtr;
			else
				Console.WriteLine ();

			foreach (var iface in JavaType.ImplementedInterfaces)
				if (ConvertTypeReference (iface.InterfaceType) is ManagedTypeReference mtr_iface)
					ImplementedInterfaces.Add (mtr_iface);
				else
					Console.WriteLine ();
		}

		public void PopulateMembers ()
		{
			// TODO
			if (JavaType is null)
				return;

			// Fields
			foreach (var field in JavaType.Fields.OfType<FieldDefinition> ().Where (f => f.IsPublic || f.IsProtected))
				Fields.Add (ManagedFieldDefinition.Create (field, this));

			foreach (var nested in NestedTypes)
				nested.PopulateMembers ();
		}

		public bool IsClass => !IsInterface;

		public bool IsInterface { get; set; }

		public bool IsStatic { get; set; }

		public bool IsPublic { get; set; }

		public bool IsProtected { get; set; }

		public bool IsPrivate { get; set; }

		public bool IsAbstract { get; set; }

		public bool IsSealed { get; set; }

		public bool IsNested => DeclaringType is not null;

		public bool IsDeprecated { get; set; }

		public override string Namespace {
			get {
				if (DeclaringType is null)
					return DeclaringNamespace?.Name ?? string.Empty;

				return DeclaringType.Resolve ()?.Namespace ?? string.Empty;
			}
		}

		public ManagedProjectReferenceModel? Container {
			get {
				if (DeclaringType?.Resolve () is ManagedTypeDefinition td)
					return td.Container;

				return DeclaringNamespace?.Container;
			}
		}

		public string ArityName => HasGenericParameters ? $"{Name}`{GenericParameters.Count}" : Name;
	}
}

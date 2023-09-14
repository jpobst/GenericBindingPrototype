using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class BoundFieldAsProperty : PropertyWriter
{
	public static BoundFieldAsProperty Create (FieldDefinition field, GeneratorSettings settings)
	{
		var p = new BoundFieldAsProperty {
			Name = field.GetManagedName (settings)
		};

		if (field.IsPublic)
			p.IsPublic = true;

		if (field.IsProtected)
			p.IsProtected = true;

		p.IsStatic = field.IsStatic && !field.IsConstant;

		p.PropertyType = new TypeReferenceWriter (FormatExtensions.FormatTypeReference (field.FieldType, settings));

		p.HasGet = true;
		p.HasSet = true;

		p.GetBody.Add ("throw new global::System.NotImplementedException ();");

		return p;
	}
}

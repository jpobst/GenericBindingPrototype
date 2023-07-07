using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class BoundField : FieldWriter
{
	public static BoundField Create (FieldDefinition field)
	{
		var f = new BoundField {
			Name = field.GetName ()
		};

		if (field.IsPublic)
			f.IsPublic = true;

		if (field.IsProtected)
			f.IsProtected = true;

		f.IsConst = field.IsConstant;
		f.IsStatic = field.IsStatic && !field.IsConstant;

		f.Type = new TypeReferenceWriter (FormatExtensions.FormatTypeReference (field.FieldType));

		if (field.IsConstant)
			f.Value = FormatExtensions.SerializeConstantValue (field.Value, field.FieldType.Name);

		return f;
	}
}

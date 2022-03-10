using Javil;
using Xamarin.SourceWriter;

namespace generator2;

class BoundFieldAsProperty : PropertyWriter
{
	public static BoundFieldAsProperty Create (FieldDefinition field)
	{
		var p = new BoundFieldAsProperty {
			Name = field.GetName ()
		};

		if (field.IsPublic)
			p.IsPublic = true;

		if (field.IsProtected)
			p.IsProtected = true;

		p.IsStatic = field.IsStatic && !field.IsConstant;

		p.PropertyType = new TypeReferenceWriter (FormatExtensions.FormatTypeReference (field.FieldType));

		p.HasGet = true;
		p.HasSet = true;

		p.GetBody.Add ("throw new NotImplementedException ();");

		return p;
	}
}

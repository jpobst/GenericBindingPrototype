using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.SourceWriter
{
	public class GenericConstraintModel
	{
		public string Name { get; set; }
		public string ConstraintType { get; set; }

		public GenericConstraintModel (string name, string constraintType)
		{
			Name = name;
			ConstraintType = constraintType;
		}
	}
}

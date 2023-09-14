using Javil;
using Xamarin.SourceWriter;

namespace Java.Interop.Tools.BindingsGenerator;

class DefaultModelBinder
{
	private readonly GeneratorSettings settings;

	public DefaultModelBinder (GeneratorSettings settings)
	{
		this.settings = settings;
	}

	public List<TypeWriter> CreateModel (ContainerDefinition container)
	{
		var model = new List<TypeWriter> ();

		foreach (var type in container.Types.Where (t => t.IsPublicApi ()))
			model.Add (CreateType (type));

		return model;
	}

	TypeWriter CreateType (TypeDefinition type)
	{
		if (type.IsInterface)
			return CreateInterface (type);

		return CreateClass (type);
	}

	ClassWriter CreateClass (TypeDefinition type)
	{
		var klass = BoundClass.Create (type, settings);

		foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
			klass.NestedTypes.Add (CreateType (nested));

		return klass;
	}

	InterfaceWriter CreateInterface (TypeDefinition type)
	{
		var iface = BoundInterface.Create (type, settings);

		foreach (var nested in type.NestedTypes.Where (t => t.IsPublic || t.IsProtected))
			iface.NestedTypes.Add (CreateType (nested));

		return iface;
	}
}

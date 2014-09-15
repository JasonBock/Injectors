using Mono.Cecil;

namespace Injectors.Core.Extensions
{
	internal static class TypeDefinitionExtensions
	{
		internal static void Inject(this TypeDefinition @this)
		{
			@this.RunInjectors();

			foreach(var method in @this.Methods)
			{
				method.Inject();
			}

			foreach(var field in @this.Fields)
			{
				field.Inject();
			}

			foreach(var property in @this.Properties)
			{
				property.Inject();
			}

			foreach(var @event in @this.Events)
			{
				@event.Inject();
			}
		}
	}
}

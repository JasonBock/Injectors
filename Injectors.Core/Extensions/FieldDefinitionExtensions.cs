using Mono.Cecil;

namespace Injectors.Core.Extensions
{
	internal static class FieldDefinitionExtensions
	{
		internal static void Inject(this FieldDefinition @this)
		{
			@this.RunInjectors();
		}
	}
}
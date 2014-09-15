using Mono.Cecil;

namespace Injectors.Core.Extensions
{
	internal static class PropertyDefinitionExtensions
	{
		internal static void Inject(this PropertyDefinition @this)
		{
			@this.RunInjectors();
		}
	}
}
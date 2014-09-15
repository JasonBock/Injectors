using Mono.Cecil;

namespace Injectors.Core.Extensions
{
	internal static class ParameterDefinitionExtensions
	{
		internal static void Inject(this ParameterDefinition @this)
		{
			@this.RunInjectors();
		}
	}
}

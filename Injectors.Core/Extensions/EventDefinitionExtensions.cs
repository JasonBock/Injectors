using Mono.Cecil;

namespace Injectors.Core.Extensions
{
	internal static class EventDefinitionExtensions
	{
		internal static void Inject(this EventDefinition @this)
		{
			@this.RunInjectors();
		}
	}
}
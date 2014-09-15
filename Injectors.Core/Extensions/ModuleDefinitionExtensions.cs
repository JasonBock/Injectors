using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Injectors.Core.Extensions
{
	internal static class ModuleDefinitionExtensions
	{
		internal static void Inject(this ModuleDefinition @this)
		{
			@this.RunInjectors();

			foreach(var type in @this.GetAllTypes())
			{
				type.Inject();
			}
		}
	}
}

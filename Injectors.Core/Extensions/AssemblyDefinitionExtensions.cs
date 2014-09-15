using Mono.Cecil;

namespace Injectors.Core.Extensions
{
	internal static class AssemblyDefinitionExtensions
	{
		internal static void Inject(this AssemblyDefinition @this)
		{
			@this.RunInjectors();

			foreach(var module in @this.Modules)
			{
				module.Inject();
			}
		}
	}
}

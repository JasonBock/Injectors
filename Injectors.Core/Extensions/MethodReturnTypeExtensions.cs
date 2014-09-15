using Mono.Cecil;

namespace Injectors.Core.Extensions
{
	internal static class MethodReturnTypeExtensions
	{
		internal static void Inject(this MethodReturnType @this)
		{
			@this.RunInjectors();
		}
	}
}

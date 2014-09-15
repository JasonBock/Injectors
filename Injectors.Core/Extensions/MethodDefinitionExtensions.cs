using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace Injectors.Core.Extensions
{
	internal static class MethodDefinitionExtensions
	{
		internal static void Inject(this MethodDefinition @this)
		{
			@this.RunInjectors();

			foreach(var parameter in @this.Parameters)
			{
				parameter.Inject();
			}
		}

		internal static SequencePoint FindSequencePoint(this MethodDefinition @this)
		{
			return (from instruction in @this.Body.Instructions
					  where instruction.SequencePoint != null
					  where instruction.SequencePoint.Document != null
					  select instruction.SequencePoint).FirstOrDefault();
		}
	}
}

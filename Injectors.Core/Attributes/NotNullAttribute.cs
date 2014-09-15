using System;
using Injectors.Core.Attributes.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Injectors.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	[Serializable]
	public sealed class NotNullAttribute : InjectorAttribute<ParameterDefinition>
	{
		protected override void OnInject(ParameterDefinition target)
		{
			if (!target.ParameterType.IsValueType)
			{
				var method = (target.Method as MethodDefinition);
				var argumentNullExceptionCtor = method.DeclaringType.Module.Assembly.MainModule.Import(
					typeof(ArgumentNullException).GetConstructor(new Type[] { typeof(string) }));

				var processor = method.Body.GetILProcessor();
				var first = processor.Body.Instructions[0];
				var ldArgInstruction = processor.Create(OpCodes.Ldarg, target);

				processor.InsertBefore(first, ldArgInstruction);
				processor.InsertBefore(first, processor.Create(OpCodes.Brtrue_S, first));
				processor.InsertBefore(first, processor.Create(OpCodes.Ldstr, target.Name));
				processor.InsertBefore(first, processor.Create(OpCodes.Newobj, argumentNullExceptionCtor));
				processor.InsertBefore(first, processor.Create(OpCodes.Throw));
			}
		}
	}
}

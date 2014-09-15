using Injectors.Core.Attributes.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Reflection;

namespace Injectors.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor,
		AllowMultiple = false, Inherited = true)]
	[Serializable]
	public sealed class TraceAttribute : InjectorAttribute<MethodDefinition>
	{
		private const string ConcatMethodName = "Concat";
		private const string ExceptionTraceMessage = " - exception was thrown";
		private const string FinishedTraceMessage = " finished";
		private const string GetCurrentMethodMethodName = "GetCurrentMethod";
		private const string StartedTraceMessage = " started";
		private const string ToStringMethodName = "ToString";
		private const string WriteLineMethodName = "WriteLine";

		private static void AddTrace(ILProcessor processor, Instruction beforeInstruction,
			VariableDefinition methodDescription, string message,
			MethodReference concat, MethodReference writeLine)
		{
			processor.InsertBefore(beforeInstruction,
				processor.Create(OpCodes.Ldloc, methodDescription));
			processor.InsertBefore(beforeInstruction,
				processor.Create(OpCodes.Ldstr, message));
			processor.InsertBefore(beforeInstruction,
				processor.Create(OpCodes.Call, concat));
			processor.InsertBefore(beforeInstruction,
				processor.Create(OpCodes.Call, writeLine));
		}

		protected override void OnInject(MethodDefinition target)
		{
			var assembly = target.DeclaringType.Module.Assembly;

			var methodDescription = new VariableDefinition(
				assembly.MainModule.Import(typeof(string)));
			target.Body.Variables.Add(methodDescription);
			target.Body.InitLocals = true;

			var writeLine = assembly.MainModule.Import(
				typeof(Console).GetMethod(TraceAttribute.WriteLineMethodName,
					new Type[] { typeof(string) }));
			var concat = assembly.MainModule.Import(
				typeof(string).GetMethod(TraceAttribute.ConcatMethodName,
					new Type[] { typeof(string), typeof(string) }));
			var getCurrentMethod = assembly.MainModule.Import(
				typeof(MethodBase).GetMethod(TraceAttribute.GetCurrentMethodMethodName, Type.EmptyTypes));
			var toString = assembly.MainModule.Import(
				typeof(MethodBase).GetMethod(TraceAttribute.ToStringMethodName, Type.EmptyTypes));

			var processor = target.Body.GetILProcessor();

			var firstInstruction = target.Body.Instructions[0];
			processor.InsertBefore(firstInstruction,
				processor.Create(OpCodes.Call, getCurrentMethod));
			processor.InsertBefore(firstInstruction,
				processor.Create(OpCodes.Callvirt, toString));
			processor.InsertBefore(firstInstruction,
				processor.Create(OpCodes.Stloc, methodDescription));

			TraceAttribute.AddTrace(processor, firstInstruction, methodDescription,
				TraceAttribute.StartedTraceMessage, concat, writeLine);

			var i = target.Body.Instructions.Count - 1;

			while(i >= 0)
			{
				var instruction = target.Body.Instructions[i];

				if(instruction.OpCode == OpCodes.Ret)
				{
					TraceAttribute.AddTrace(processor, instruction, methodDescription,
						TraceAttribute.FinishedTraceMessage, concat, writeLine);
				}
				else if(instruction.OpCode == OpCodes.Throw)
				{
					TraceAttribute.AddTrace(processor, instruction, methodDescription,
						TraceAttribute.ExceptionTraceMessage, concat, writeLine);
				}

				i--;
			}
		}
	}
}
using Injectors.Core.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Injectors.Core.Tests.Attributes
{
	[TestClass]
	public sealed class TraceAttributeTests
	{
		[TestMethod]
		public void Inject()
		{
			var typeName = Guid.NewGuid().ToString("N");
			var methodName = Guid.NewGuid().ToString("N");

			AssemblyManager.Manage((assembly) =>
			{
				var traceAttributeCtor = assembly.MainModule.Import(
					typeof(TraceAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(traceAttributeCtor);

				var type = AssemblyManager.AddType(assembly, typeName);
				var method = AssemblyManager.AddMethod(type, methodName);
				method.CustomAttributes.Add(attribute);

				var processor = method.Body.GetILProcessor();
				processor.Append(Instruction.Create(OpCodes.Ret));
			}, (assembly) =>
			{
				var injector = new TraceAttribute();

				var targetMethod = (from type in assembly.MainModule.GetAllTypes()
										  where type.Name == typeName
										  from method in type.Methods
										  where method.Name == methodName
										  select method).First();

				Assert.AreEqual(1, targetMethod.Body.Instructions.Count);
				injector.Inject(targetMethod);
				var instructions = targetMethod.Body.Instructions;
				Assert.AreEqual(12, instructions.Count);

				Assert.AreEqual(OpCodes.Call, instructions[0].OpCode);
				Assert.AreEqual(OpCodes.Callvirt, instructions[1].OpCode);
				Assert.AreEqual(OpCodes.Stloc, instructions[2].OpCode);
				Assert.AreEqual(OpCodes.Ldloc, instructions[3].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[4].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[5].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[6].OpCode);
				Assert.AreEqual(OpCodes.Ldloc, instructions[7].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[8].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[9].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[10].OpCode);
				Assert.AreEqual(OpCodes.Ret, instructions[11].OpCode);
			}, false);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
		[TestMethod]
		public void InjectWithThrowOpCode()
		{
			var typeName = Guid.NewGuid().ToString("N");
			var methodName = Guid.NewGuid().ToString("N");

			AssemblyManager.Manage((assembly) =>
			{
				var traceAttributeCtor = assembly.MainModule.Import(
					typeof(TraceAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(traceAttributeCtor);

				var type = AssemblyManager.AddType(assembly, typeName);
				var method = AssemblyManager.AddMethod(type, methodName,
					new List<ParameterDefinition> { new ParameterDefinition(assembly.MainModule.Import(typeof(bool))) }, null);
				method.CustomAttributes.Add(attribute);

				var processor = method.Body.GetILProcessor();

				var first = Instruction.Create(OpCodes.Ldarg_0);
				processor.Append(first);
				var argumentNullExceptionCtor = assembly.MainModule.Import(
					typeof(ArgumentNullException).GetConstructor(new Type[] { typeof(string) }));
				processor.Append(Instruction.Create(OpCodes.Ldstr, "error"));
				processor.Append(Instruction.Create(OpCodes.Newobj, argumentNullExceptionCtor));
				processor.Append(Instruction.Create(OpCodes.Throw));
				var last = Instruction.Create(OpCodes.Ret);
				processor.Append(last);
				processor.InsertAfter(first, Instruction.Create(OpCodes.Brfalse_S, last));
			}, (assembly) =>
			{
				var injector = new TraceAttribute();

				var targetMethod = (from type in assembly.MainModule.GetAllTypes()
										  where type.Name == typeName
										  from method in type.Methods
										  where method.Name == methodName
										  select method).First();

				Assert.AreEqual(6, targetMethod.Body.Instructions.Count);
				injector.Inject(targetMethod);
				Assert.AreEqual(21, targetMethod.Body.Instructions.Count);

				var instructions = targetMethod.Body.Instructions;

				Assert.AreEqual(OpCodes.Call, instructions[0].OpCode);
				Assert.AreEqual(OpCodes.Callvirt, instructions[1].OpCode);
				Assert.AreEqual(OpCodes.Stloc, instructions[2].OpCode);

				Assert.AreEqual(OpCodes.Ldloc, instructions[3].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[4].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[5].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[6].OpCode);

				Assert.AreEqual(OpCodes.Ldarg_0, instructions[7].OpCode);
				Assert.AreEqual(OpCodes.Brfalse_S, instructions[8].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[9].OpCode);
				Assert.AreEqual(OpCodes.Newobj, instructions[10].OpCode);

				Assert.AreEqual(OpCodes.Ldloc, instructions[11].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[12].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[13].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[14].OpCode);
				Assert.AreEqual(OpCodes.Throw, instructions[15].OpCode);

				Assert.AreEqual(OpCodes.Ldloc, instructions[16].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[17].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[18].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[19].OpCode);
				Assert.AreEqual(OpCodes.Ret, instructions[20].OpCode);
			}, false);
		}
	}
}

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
	public sealed class NotNullAttributeTests
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
		[TestMethod]
		public void InjectWhenParameterIsReferenceType()
		{
			var typeName = Guid.NewGuid().ToString("N");
			var methodName = Guid.NewGuid().ToString("N");

			AssemblyManager.Manage((assembly) =>
			{
				var notNullAttributeCtor = assembly.MainModule.Import(
					typeof(NotNullAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(notNullAttributeCtor);
				var type = AssemblyManager.AddType(assembly, typeName);

				var parameter = new ParameterDefinition(assembly.MainModule.Import(typeof(object)));
				parameter.CustomAttributes.Add(attribute);

				var method = AssemblyManager.AddMethod(type, methodName,
					new List<ParameterDefinition> { parameter },
					null);

				var processor = method.Body.GetILProcessor();
				processor.Append(Instruction.Create(OpCodes.Ret));
			}, (assembly) =>
			{
				var injector = new NotNullAttribute();

				var targetMethod = (from type in assembly.MainModule.GetAllTypes()
										  where type.Name == typeName
										  from method in type.GetMethods()
										  where method.Name == methodName
										  select method).First();

				Assert.AreEqual(1, targetMethod.Body.Instructions.Count);
				injector.Inject(targetMethod.Parameters[0]);
				var instructions = targetMethod.Body.Instructions;
				Assert.AreEqual(6, instructions.Count);
				Assert.AreEqual(OpCodes.Ldarg, instructions[0].OpCode);
				Assert.AreEqual(OpCodes.Brtrue_S, instructions[1].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[2].OpCode);
				Assert.AreEqual(OpCodes.Newobj, instructions[3].OpCode);
				Assert.AreEqual(OpCodes.Throw, instructions[4].OpCode);
				Assert.AreEqual(OpCodes.Ret, instructions[5].OpCode);
			}, false);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
		[TestMethod]
		public void InjectWhenParameterIsValueType()
		{
			var typeName = Guid.NewGuid().ToString("N");
			var methodName = Guid.NewGuid().ToString("N");

			AssemblyManager.Manage((assembly) =>
			{
				var notNullAttributeCtor = assembly.MainModule.Import(
					typeof(NotNullAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(notNullAttributeCtor);

				var parameter = new ParameterDefinition(assembly.MainModule.Import(typeof(int)));
				parameter.CustomAttributes.Add(attribute);

				var type = AssemblyManager.AddType(assembly, typeName);
				var method = AssemblyManager.AddMethod(type, methodName,
					new List<ParameterDefinition> { parameter },
					null);

				var processor = method.Body.GetILProcessor();
				processor.Append(Instruction.Create(OpCodes.Ret));
			}, (assembly) =>
			{
				var injector = new NotNullAttribute();

				var targetMethod = (from type in assembly.MainModule.GetAllTypes()
										  where type.Name == typeName
										  from method in type.GetMethods()
										  where method.Name == methodName
										  select method).First();

				Assert.AreEqual(1, targetMethod.Body.Instructions.Count);
				injector.Inject(targetMethod.Parameters[0]);
				Assert.AreEqual(1, targetMethod.Body.Instructions.Count);
			}, false);
		}
	}
}

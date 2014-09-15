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
	public sealed class ToStringAttributeTests
	{
		[TestMethod]
		public void Inject()
		{
			var typeName = Guid.NewGuid().ToString("N");

			AssemblyManager.Manage((assembly) =>
			{
				var toStringAttributeCtor = assembly.MainModule.Import(
					typeof(ToStringAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(toStringAttributeCtor);
				var type = AssemblyManager.AddType(assembly, typeName);
				type.CustomAttributes.Add(attribute);
			}, (assembly) =>
			{
				var injector = new ToStringAttribute();

				var targetType = (from type in assembly.MainModule.GetAllTypes()
										where type.Name == typeName
										select type).First();

				Assert.AreEqual(0, targetType.Methods.Count);
				injector.Inject(targetType);
				Assert.AreEqual(1, targetType.Methods.Count);

				var instructions = targetType.Methods[0].Body.Instructions;
				Assert.AreEqual(2, instructions.Count);
				Assert.AreEqual(OpCodes.Ldstr, instructions[0].OpCode);
				Assert.AreEqual(OpCodes.Ret, instructions[1].OpCode);
			}, false);
		}

		[TestMethod]
		public void InjectWhenToStringMethodExistsOnType()
		{
			var typeName = Guid.NewGuid().ToString("N");

			AssemblyManager.Manage((assembly) =>
			{
				var toStringAttributeCtor = assembly.MainModule.Import(
					typeof(ToStringAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(toStringAttributeCtor);
				var type = AssemblyManager.AddType(assembly, typeName);
				type.CustomAttributes.Add(attribute);

				var method = AssemblyManager.AddMethod(type, "ToString",
					new List<ParameterDefinition>(), assembly.MainModule.Import(typeof(string)));
				method.CallingConvention = MethodCallingConvention.ThisCall;
				method.IsVirtual = true;
				method.IsStatic = false;
				method.IsHideBySig = true;

				var processor = method.Body.GetILProcessor();
				processor.Append(Instruction.Create(OpCodes.Ldnull));
				processor.Append(Instruction.Create(OpCodes.Ret));
			}, (assembly) =>
			{
				var injector = new ToStringAttribute();

				var targetType = (from type in assembly.MainModule.GetAllTypes()
										where type.Name == typeName
										select type).First();

				Assert.AreEqual(1, targetType.Methods.Count);
				injector.Inject(targetType);
				Assert.AreEqual(1, targetType.Methods.Count);

				var instructions = targetType.Methods[0].Body.Instructions;
				Assert.AreEqual(2, instructions.Count);
				Assert.AreEqual(OpCodes.Ldnull, instructions[0].OpCode);
				Assert.AreEqual(OpCodes.Ret, instructions[1].OpCode);
			}, false);
		}

		[TestMethod]
		public void InjectWhenTypeHasMultipleProperties()
		{
			var typeName = Guid.NewGuid().ToString("N");
			var propertyOneName = Guid.NewGuid().ToString("N");
			var propertyTwoName = Guid.NewGuid().ToString("N");

			AssemblyManager.Manage((assembly) =>
			{
				var toStringAttributeCtor = assembly.MainModule.Import(
					typeof(ToStringAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(toStringAttributeCtor);
				var type = AssemblyManager.AddType(assembly, typeName);
				type.CustomAttributes.Add(attribute);

				var methodOne = AssemblyManager.AddMethod(type, "get_" + propertyOneName,
					new List<ParameterDefinition>(), assembly.MainModule.Import(typeof(string)));
				methodOne.CallingConvention = MethodCallingConvention.ThisCall;
				methodOne.IsStatic = false;
				methodOne.IsHideBySig = true;

				var processorOne = methodOne.Body.GetILProcessor();
				processorOne.Append(Instruction.Create(OpCodes.Ldnull));
				processorOne.Append(Instruction.Create(OpCodes.Ret));

				var propertyOne = new PropertyDefinition(propertyOneName, PropertyAttributes.None,
					assembly.MainModule.Import(typeof(string)));
				propertyOne.GetMethod = methodOne;
				type.Properties.Add(propertyOne);

				var methodTwo = AssemblyManager.AddMethod(type, "get_" + propertyTwoName,
					new List<ParameterDefinition>(), assembly.MainModule.Import(typeof(string)));
				methodOne.CallingConvention = MethodCallingConvention.ThisCall;
				methodOne.IsStatic = false;
				methodOne.IsHideBySig = true;

				var processorTwo = methodTwo.Body.GetILProcessor();
				processorTwo.Append(Instruction.Create(OpCodes.Ldnull));
				processorTwo.Append(Instruction.Create(OpCodes.Ret));

				var propertyTwo = new PropertyDefinition(propertyTwoName, PropertyAttributes.None,
					assembly.MainModule.Import(typeof(string)));
				propertyTwo.GetMethod = methodTwo;
				type.Properties.Add(propertyTwo);
			}, (assembly) =>
			{
				var injector = new ToStringAttribute();

				var targetType = (from type in assembly.MainModule.GetAllTypes()
										where type.Name == typeName
										select type).First();

				Assert.AreEqual(2, targetType.Methods.Count);
				injector.Inject(targetType);
				Assert.AreEqual(3, targetType.Methods.Count);

				var targetMethod = (from type in assembly.MainModule.GetAllTypes()
										  where type.Name == typeName
										  from method in type.GetMethods()
										  where method.Name == "ToString"
										  select method).First();

				var instructions = targetMethod.Body.Instructions;
				Assert.AreEqual(15, instructions.Count);
				Assert.AreEqual(OpCodes.Newobj, instructions[0].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[1].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[2].OpCode);
				Assert.AreEqual(OpCodes.Ldarg_0, instructions[3].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[4].OpCode);
				Assert.AreEqual(OpCodes.Callvirt, instructions[5].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[6].OpCode);
				Assert.AreEqual(OpCodes.Callvirt, instructions[7].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[8].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[9].OpCode);
				Assert.AreEqual(OpCodes.Ldarg_0, instructions[10].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[11].OpCode);
				Assert.AreEqual(OpCodes.Callvirt, instructions[12].OpCode);
				Assert.AreEqual(OpCodes.Callvirt, instructions[13].OpCode);
				Assert.AreEqual(OpCodes.Ret, instructions[14].OpCode);
			}, false);
		}

		[TestMethod]
		public void InjectWhenTypeHasReferenceTypeProperty()
		{
			var typeName = Guid.NewGuid().ToString("N");
			var propertyName = Guid.NewGuid().ToString("N");

			AssemblyManager.Manage((assembly) =>
			{
				var toStringAttributeCtor = assembly.MainModule.Import(
					typeof(ToStringAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(toStringAttributeCtor);
				var type = AssemblyManager.AddType(assembly, typeName);
				type.CustomAttributes.Add(attribute);

				var method = AssemblyManager.AddMethod(type, "get_" + propertyName,
					new List<ParameterDefinition>(), assembly.MainModule.Import(typeof(string)));
				method.CallingConvention = MethodCallingConvention.ThisCall;
				method.IsStatic = false;
				method.IsHideBySig = true;

				var processor = method.Body.GetILProcessor();
				processor.Append(Instruction.Create(OpCodes.Ldnull));
				processor.Append(Instruction.Create(OpCodes.Ret));

				var property = new PropertyDefinition(propertyName, PropertyAttributes.None,
					assembly.MainModule.Import(typeof(string)));
				property.GetMethod = method;
				type.Properties.Add(property);
			}, (assembly) =>
			{
				var injector = new ToStringAttribute();

				var targetType = (from type in assembly.MainModule.GetAllTypes()
										where type.Name == typeName
										select type).First();

				Assert.AreEqual(1, targetType.Methods.Count);
				injector.Inject(targetType);
				Assert.AreEqual(2, targetType.Methods.Count);

				var targetMethod = (from type in assembly.MainModule.GetAllTypes()
										  where type.Name == typeName
										  from method in type.GetMethods()
										  where method.Name == "ToString"
										  select method).First();
				var instructions = targetMethod.Body.Instructions;
				Assert.AreEqual(8, instructions.Count);
				Assert.AreEqual(OpCodes.Newobj, instructions[0].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[1].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[2].OpCode);
				Assert.AreEqual(OpCodes.Ldarg_0, instructions[3].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[4].OpCode);
				Assert.AreEqual(OpCodes.Callvirt, instructions[5].OpCode);
				Assert.AreEqual(OpCodes.Callvirt, instructions[6].OpCode);
				Assert.AreEqual(OpCodes.Ret, instructions[7].OpCode);
			}, false);
		}

		[TestMethod]
		public void InjectWhenTypeHasValueTypeProperty()
		{
			var typeName = Guid.NewGuid().ToString("N");
			var propertyName = Guid.NewGuid().ToString("N");

			AssemblyManager.Manage((assembly) =>
			{
				var toStringAttributeCtor = assembly.MainModule.Import(
					typeof(ToStringAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(toStringAttributeCtor);
				var type = AssemblyManager.AddType(assembly, typeName);
				type.CustomAttributes.Add(attribute);

				var method = AssemblyManager.AddMethod(type, "get_" + propertyName,
					new List<ParameterDefinition>(), assembly.MainModule.Import(typeof(int)));
				method.CallingConvention = MethodCallingConvention.ThisCall;
				method.IsStatic = false;
				method.IsHideBySig = true;

				var processor = method.Body.GetILProcessor();
				processor.Append(Instruction.Create(OpCodes.Ldc_I4_0));
				processor.Append(Instruction.Create(OpCodes.Ret));

				var property = new PropertyDefinition(propertyName, PropertyAttributes.None,
					assembly.MainModule.Import(typeof(int)));
				property.GetMethod = method;
				type.Properties.Add(property);
			}, (assembly) =>
			{
				var injector = new ToStringAttribute();

				var targetType = (from type in assembly.MainModule.GetAllTypes()
										where type.Name == typeName
										select type).First();

				Assert.AreEqual(1, targetType.Methods.Count);
				injector.Inject(targetType);
				Assert.AreEqual(2, targetType.Methods.Count);

				var targetMethod = (from type in assembly.MainModule.GetAllTypes()
										  where type.Name == typeName
										  from method in type.GetMethods()
										  where method.Name == "ToString"
										  select method).First();
				var instructions = targetMethod.Body.Instructions;
				Assert.AreEqual(9, instructions.Count);
				Assert.AreEqual(OpCodes.Newobj, instructions[0].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[1].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[2].OpCode);
				Assert.AreEqual(OpCodes.Ldarg_0, instructions[3].OpCode);
				Assert.AreEqual(OpCodes.Call, instructions[4].OpCode);
				Assert.AreEqual(OpCodes.Box, instructions[5].OpCode);
				Assert.AreEqual(OpCodes.Callvirt, instructions[6].OpCode);
				Assert.AreEqual(OpCodes.Callvirt, instructions[7].OpCode);
				Assert.AreEqual(OpCodes.Ret, instructions[8].OpCode);
			}, false);
		}
	}
}

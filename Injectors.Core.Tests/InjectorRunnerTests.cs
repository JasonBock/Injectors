using Injectors.Core.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Injectors.Core.Tests
{
	[TestClass]
	public sealed class InjectorRunnerTests
	{
		[TestMethod]
		public void Run()
		{
			AssemblyManager.Manage((file) =>
			{
				var fileChanged = false;
				using(var watcher = new FileSystemWatcher(Path.GetDirectoryName(file.FullName)))
				{
					var changer = new FileSystemEventHandler((o, e) =>
					{
						fileChanged = e.Name == file.Name;
					});

					watcher.Changed += changer;
					watcher.EnableRaisingEvents = true;

					try
					{
						InjectorRunner.Run(file);
						Assert.IsTrue(fileChanged);
					}
					finally
					{
						watcher.Changed -= changer;
					}
				}
			}, false);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
		[TestMethod]
		public void RunWhenMethodHasAttribute()
		{
			var typeName = Guid.NewGuid().ToString("N");
			var methodName = Guid.NewGuid().ToString("N");

			AssemblyManager.ManageFile((assembly) =>
			{
				var traceAttributeCtor = assembly.MainModule.Import(
					typeof(TraceAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(traceAttributeCtor);

				var type = AssemblyManager.AddType(assembly, typeName);
				var method = AssemblyManager.AddMethod(type, methodName);
				method.CustomAttributes.Add(attribute);

				var processor = method.Body.GetILProcessor();
				processor.Append(Instruction.Create(OpCodes.Ret));
			}, (file) =>
			{
				var beforeAssembly = AssemblyDefinition.ReadAssembly(file.FullName);
				var beforeTargetMethod = (from type in beforeAssembly.MainModule.GetAllTypes()
												  where type.Name == typeName
												  from method in type.GetMethods()
												  where method.Name == methodName
												  select method).First();

				Assert.AreEqual(1, beforeTargetMethod.Body.Instructions.Count);

				InjectorRunner.Run(file);

				var afterAssembly = AssemblyDefinition.ReadAssembly(file.FullName);
				var afterTargetMethod = (from type in afterAssembly.MainModule.GetAllTypes()
												 where type.Name == typeName
												 from method in type.GetMethods()
												 where method.Name == methodName
												 select method).First();

				var instructions = afterTargetMethod.Body.Instructions;
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

		[TestMethod]
		public void RunWhenParameterHasAttribute()
		{
			var typeName = Guid.NewGuid().ToString("N");
			var methodName = Guid.NewGuid().ToString("N");

			AssemblyManager.ManageFile((assembly) =>
			{
				var notNullAttributeCtor = assembly.MainModule.Import(
					typeof(NotNullAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(notNullAttributeCtor);

				var parameter = new ParameterDefinition(assembly.MainModule.Import(typeof(object)));
				parameter.CustomAttributes.Add(attribute);

				var type = AssemblyManager.AddType(assembly, typeName);
				var method = AssemblyManager.AddMethod(type, methodName,
					new List<ParameterDefinition> { parameter },
					null);

				var processor = method.Body.GetILProcessor();
				processor.Append(Instruction.Create(OpCodes.Ret));
			}, (file) =>
			{
				var beforeAssembly = AssemblyDefinition.ReadAssembly(file.FullName);
				var beforeTargetMethod = (from type in beforeAssembly.MainModule.GetAllTypes()
												  where type.Name == typeName
												  from method in type.GetMethods()
												  where method.Name == methodName
												  select method).First();

				Assert.AreEqual(1, beforeTargetMethod.Body.Instructions.Count);

				InjectorRunner.Run(file);

				var afterAssembly = AssemblyDefinition.ReadAssembly(file.FullName);
				var afterTargetMethod = (from type in afterAssembly.MainModule.GetAllTypes()
												 where type.Name == typeName
												 from method in type.GetMethods()
												 where method.Name == methodName
												 select method).First();

				var instructions = afterTargetMethod.Body.Instructions;
				Assert.AreEqual(6, instructions.Count);
				Assert.AreEqual(OpCodes.Ldarg, instructions[0].OpCode);
				Assert.AreEqual(OpCodes.Brtrue_S, instructions[1].OpCode);
				Assert.AreEqual(OpCodes.Ldstr, instructions[2].OpCode);
				Assert.AreEqual(OpCodes.Newobj, instructions[3].OpCode);
				Assert.AreEqual(OpCodes.Throw, instructions[4].OpCode);
				Assert.AreEqual(OpCodes.Ret, instructions[5].OpCode);
			}, false);
		}

		[TestMethod]
		public void RunWhenTypeHasAttribute()
		{
			var typeName = Guid.NewGuid().ToString("N");

			AssemblyManager.ManageFile((assembly) =>
			{
				var toStringAttributeCtor = assembly.MainModule.Import(
					typeof(ToStringAttribute).GetConstructor(Type.EmptyTypes));
				var attribute = new CustomAttribute(toStringAttributeCtor);

				var type = AssemblyManager.AddType(assembly, typeName);
				type.CustomAttributes.Add(attribute);
			}, (file) =>
			{
				var beforeAssembly = AssemblyDefinition.ReadAssembly(file.FullName);
				var beforeType = (from type in beforeAssembly.MainModule.GetAllTypes()
										where type.Name == typeName
										select type).First();

				Assert.AreEqual(0, beforeType.Methods.Count());

				InjectorRunner.Run(file);

				var afterAssembly = AssemblyDefinition.ReadAssembly(file.FullName);
				var afterType = (from type in afterAssembly.MainModule.GetAllTypes()
									  where type.Name == typeName
									  select type).First();

				var methods = afterType.Methods.ToList();
				Assert.AreEqual(1, methods.Count);

				var instructions = methods[0].Body.Instructions;
				Assert.AreEqual(2, instructions.Count);
				Assert.AreEqual(OpCodes.Ldstr, instructions[0].OpCode);
				Assert.AreEqual(OpCodes.Ret, instructions[1].OpCode);
			}, false);
		}

		[TestMethod, ExpectedException(typeof(FileNotFoundException))]
		public void RunWhenLocationDoesNotExist()
		{
			InjectorRunner.Run(new FileInfo(Guid.NewGuid().ToString("N") + ".dll"));
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void RunWhenLocationIsNull()
		{
			InjectorRunner.Run(null);
		}
	}
}

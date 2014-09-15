using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;

namespace Injectors.Core.Tests
{
	internal static class AssemblyManager
	{
		internal static MethodDefinition AddMethod(TypeDefinition type, string methodName)
		{
			return AssemblyManager.AddMethod(type, methodName,
				new List<ParameterDefinition>(), null);
		}

		internal static MethodDefinition AddMethod(TypeDefinition type, string methodName,
			List<ParameterDefinition> parameters, TypeReference returnType)
		{
			var method = new MethodDefinition(methodName,
				MethodAttributes.Public | MethodAttributes.HideBySig,
				returnType ?? type.Module.Assembly.MainModule.Import(typeof(void)));

			foreach(var parameter in parameters)
			{
				method.Parameters.Add(parameter);
			}

			type.Methods.Add(method);
			return method;
		}

		internal static TypeDefinition AddType(AssemblyDefinition assembly, string typeName)
		{
			var type = new TypeDefinition(string.Empty, typeName, TypeAttributes.Public);
			assembly.MainModule.Types.Add(type);
			return type;
		}

		private static string Create(Action<AssemblyDefinition> addTo, bool addDebugSymbols)
		{
			var assemblyName = Guid.NewGuid().ToString("N");
			var assemblyFileName = assemblyName + ".dll";

			var assembly = AssemblyDefinition.CreateAssembly(
				new AssemblyNameDefinition(assemblyName, new Version(1, 0, 0, 0)), assemblyFileName,
				ModuleKind.Dll);

			if(addTo != null)
			{
				addTo(assembly);
			}

			assembly.Write(assemblyFileName, new WriterParameters() { WriteSymbols = addDebugSymbols });
			return assemblyFileName;
		}

		internal static void Manage(Action<AssemblyDefinition> addTo,
			Action<AssemblyDefinition> modify, bool addDebugSymbols)
		{
			var assemblyFileName = AssemblyManager.Create(addTo, addDebugSymbols);

			try
			{
				var assembly = AssemblyDefinition.ReadAssembly(assemblyFileName);
				modify(assembly);
				assembly.Write(assemblyFileName);
			}
			finally
			{
				AssemblyManager.RemoveFile(assemblyFileName);
			}
		}

		internal static void Manage(Action<FileSystemInfo> modify, bool addDebugSymbols)
		{
			var assemblyFileName = AssemblyManager.Create(null, addDebugSymbols);

			try
			{
				modify(new FileInfo(assemblyFileName));
			}
			finally
			{
				AssemblyManager.RemoveFile(assemblyFileName);
			}
		}

		internal static void ManageFile(Action<AssemblyDefinition> addTo,
			Action<FileSystemInfo> modify, bool addDebugSymbols)
		{
			var assemblyFileName = AssemblyManager.Create(addTo, addDebugSymbols);

			try
			{
				modify(new FileInfo(assemblyFileName));
			}
			finally
			{
				AssemblyManager.RemoveFile(assemblyFileName);
			}
		}

		private static void RemoveFile(string assemblyFileName)
		{
			if(File.Exists(assemblyFileName))
			{
				File.Delete(assemblyFileName);
			}
		}
	}
}

using System.IO;
using Injectors.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Pdb;
using Spackle.Extensions;

namespace Injectors.Core
{
	public static class InjectorRunner
	{
		public static void Run(FileSystemInfo assemblyLocation)
		{
			assemblyLocation.CheckParameterForNull("assemblyLocation");

			var hasDebugSymbols = false;
			AssemblyDefinition assembly = null;

			try
			{
				assembly = AssemblyDefinition.ReadAssembly(assemblyLocation.FullName,
					new ReaderParameters { ReadSymbols = true, SymbolReaderProvider = new PdbReaderProvider() });
				hasDebugSymbols = true;
			}
			catch (FileNotFoundException)
			{
				assembly = AssemblyDefinition.ReadAssembly(assemblyLocation.FullName);
			}

			assembly.Inject();
			assembly.Write(assemblyLocation.FullName,
				new WriterParameters { WriteSymbols = hasDebugSymbols });
		}
	}
}

using Injectors.Core;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Diagnostics;
using System.IO;

namespace Injectors.Task
{
	public sealed class InjectorTask : AppDomainIsolatedTask
	{
		public override bool Execute()
		{
			Log.LogMessage("Injecting assembly {0}...", this.AssemblyLocation);
			var stopwatch = Stopwatch.StartNew();
			InjectorRunner.Run(new FileInfo(this.AssemblyLocation));
			stopwatch.Stop();
			Log.LogMessage("Assembly injection for {0} complete - total time: {1}.",
				this.AssemblyLocation, stopwatch.Elapsed.ToString());
			return true;
		}

		[Required]
		public string AssemblyLocation { get; set; }
	}
}

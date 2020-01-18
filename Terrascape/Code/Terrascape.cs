using LogixEngine;
using LogixEngine.Utility;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Terrascape
{
	/* ---------------------------------------------------------------------------- *
     * Terrascape.cs created by DrLogiq on 15-01-2020 13:28.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	internal sealed class Terrascape : Game
	{
		public static void Main(string[] args)
		{
			new Terrascape(args).Run();
		}
		
		public Terrascape(IEnumerable<string> args) : base("Terrascape", "1.0.0", 1280, 720, 30, 60, args)
		{
		}

		protected override void PreWarm()
		{
			Debug.LogDebug($"Profiler resolution: {Debug.GetProfilerResolution()} ticks/sec, High res: {(Debug.IsProfilerHighResolution() ? "Yes" : "No")}");
		}

		protected override void Load()
		{
			Debug.Profile("Loading assets", true, new Task(() =>
			{
				Debug.Profile("Loading textures", true, new Task(() =>
				{
					Thread.Sleep(50); // TEMPORARY
				}));

				Debug.Profile("Loading shaders", true, new Task(() =>
				{
					Thread.Sleep(10); // TEMPORARY
				}));
			
				Debug.Profile("Loading models", true, new Task(() =>
				{
					Thread.Sleep(1); // TEMPORARY
				}));
			
				Debug.Profile("Loading sounds", true, new Task(() =>
				{
					
				}));
			
				Debug.Profile("Loading music", true, new Task(() =>
				{
					
				}));
			}));
		}

		protected override void Cycle(double delta)
		{
		}

		protected override void Render(double delta)
		{
		}

		protected override void Shutdown(bool crashed)
		{
			Debug.LogDebug("Shutting down" + (crashed ? " because of a crash" : ""));
		}
	}
}
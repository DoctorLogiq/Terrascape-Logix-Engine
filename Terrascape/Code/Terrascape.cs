using LogixEngine;

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
			using Terrascape terrascape = new Terrascape();
			terrascape.Run(args);
		}
		
		public Terrascape() : base("Terrascape", 1280, 720, 30, 60)
		{
		}

		protected override void PreWarm()
		{
		}

		protected override void Load()
		{
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
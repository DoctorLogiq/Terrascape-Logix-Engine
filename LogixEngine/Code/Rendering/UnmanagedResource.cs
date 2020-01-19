using LogixEngine.Registry;
using LogixEngine.Utility;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LogixEngine.Rendering
{
	/* ---------------------------------------------------------------------------- *
     * UnmanagedResource.cs created by DrLogiq on 19-01-2020 20:01.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	public abstract class UnmanagedResource : IIdentifiable
	{
		internal static readonly List<UnmanagedResource> UnmanagedResources = new List<UnmanagedResource>();
		
		public abstract identifier Identifier { get; }
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		protected abstract int OpenGL_ID { get; }

		protected UnmanagedResource()
		{
			UnmanagedResources.Add(this);
		}

		internal void Cleanup()
		{
			Debug.LogDebug($"Deleting unmanaged resource '{Identifier}' with OpenGL ID {OpenGL_ID}");
			Delete();
		}

		protected abstract void Delete();
	}
}
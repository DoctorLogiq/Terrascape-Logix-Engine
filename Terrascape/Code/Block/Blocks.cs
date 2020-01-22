using LogixEngine.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Terrascape.Block
{
	/* ---------------------------------------------------------------------------- *
     * CLASS.cs created by DrLogiq on DATE.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	public enum Blocks : ushort
	{
		None = 0,
		Air = 1,
		Grass = 2,
		Dirt = 3
	}

	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public static class __BlockInformation
	{
		private static readonly Dictionary<ushort, BlockInfo> AllBlocksInformation = new Dictionary<ushort, BlockInfo>();

		static __BlockInformation()
		{
			AllBlocksInformation.Add((ushort)Blocks.None, new BlockInfo(is_rendered:false));
			AllBlocksInformation.Add((ushort)Blocks.Air, new BlockInfo(is_rendered:false));
			AllBlocksInformation.Add((ushort)Blocks.Grass, new BlockInfo(is_rendered:true));
			AllBlocksInformation.Add((ushort)Blocks.Dirt, new BlockInfo(is_rendered:true));

			// Verify that no blocks were missed
			if (Debug.DebugMode)
			{
				bool error = false;
				foreach (Blocks block in Enum.GetValues(typeof( Blocks )))
				{
					if (!AllBlocksInformation.ContainsKey((ushort) block))
					{
						Debug.LogCritical($"Missing block information for block '{block.ToString()}'");
						if (!error)
							error = true;
					}
				}

				if (error)
				{
					Debug.LogDebug("---");
					throw new DeveloperFailureException("Some blocks were missing their information.");
				}
			}
		}

		public static Dictionary<ushort, BlockInfo> GetAllBlockInformation()
		{
			return AllBlocksInformation;
		}

		public static BlockInfo GetInfo(this Blocks block)
		{
			return AllBlocksInformation[(ushort) block];
		}

		public static Array GetAllBlocks(this Blocks block)
		{
			return Enum.GetValues(typeof( Blocks ));
		}
	}
	
	public struct BlockInfo
	{
		public readonly bool IsRendered;

		public BlockInfo(bool is_rendered = true)
		{
			IsRendered = is_rendered;
		}
	}
}
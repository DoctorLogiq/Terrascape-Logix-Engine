using LogixEngine.Rendering.Texture;

namespace LogixEngine.Registry
{
	/* ---------------------------------------------------------------------------- *
     * TextureRegistry.cs created by DrLogiq on 18-01-2020 14:22.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	public sealed class TextureRegistry : Registry<Texture>
	{
		private static readonly TextureRegistry Instance = new TextureRegistry();
		
		private TextureRegistry()
		{
		}

		public new static Texture Register(Texture texture)
		{
			return Instance.RegisterItem(texture);
		}

		public static Texture Find(identifier id)
		{
			return Instance.FindItem(id);
		}
	}
}
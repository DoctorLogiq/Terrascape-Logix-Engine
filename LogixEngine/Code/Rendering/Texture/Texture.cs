using LogixEngine.Registry;

namespace LogixEngine.Rendering.Texture
{
	/* ---------------------------------------------------------------------------- *
     * Texture.cs created by DrLogiq on 18-01-2020 14:20.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	public class Texture : IIdentifiable
	{
		public identifier Identifier { get; }

		private Texture(identifier id)
		{
			Identifier = id;
		}

		public static Texture Load(identifier id, string filename)
		{
			return new Texture(id);
		}
	}
}
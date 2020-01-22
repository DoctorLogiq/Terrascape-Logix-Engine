using LogixEngine.Rendering.Shader;

namespace LogixEngine.Registry
{
	/* ---------------------------------------------------------------------------- *
     * ShaderRegistry.cs created by DrLogiq on 21-01-2020 16:10.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	public sealed class ShaderRegistry : Registry<Shader>
	{
		private static readonly ShaderRegistry Instance = new ShaderRegistry();
		
		private ShaderRegistry()
		{
		}
		
		public static void CheckIn()
		{
			// NOTE(LOGIQ): Leave empty
		}

		public new static Shader Register(Shader shader)
		{
			return Instance.RegisterItem(shader);
		}

		public static Shader Find(identifier id)
		{
			return Instance.FindItem(id);
		}
	}
}
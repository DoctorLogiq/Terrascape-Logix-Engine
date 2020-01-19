using LogixEngine.IO;
using LogixEngine.Registry;
using LogixEngine.Utility;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;

namespace LogixEngine.Rendering.Texture
{
	/* ---------------------------------------------------------------------------- *
     * Texture.cs created by DrLogiq on 18-01-2020 14:20.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	public class Texture : UnmanagedResource
	{
		public override    identifier Identifier { get; }
		protected override int        OpenGL_ID  { get; }
		public readonly    int        Width, Height;
		public readonly    List<byte> RawData;
		public readonly    bool       HasRawData;

		private Texture(identifier id, int open_gl_id, int width, int height)
		{
			Identifier = id;
			OpenGL_ID  = open_gl_id;
			Width      = width;
			Height     = height;
			RawData    = null;
			HasRawData = false;

			Debug.LogDebug($"Created texture '{id}' with a resolution of {width}{SpecialCharacters.Multiply}{Height} px");
		}

		private Texture(identifier id, int open_gl_id, int width, int height, List<byte> raw_data)
		{
			Identifier = id;
			OpenGL_ID  = open_gl_id;
			Width      = width;
			Height     = height;
			RawData    = raw_data;
			HasRawData = true;

			Debug.LogDebug($"Created texture '{id}' with a resolution of {width}{SpecialCharacters.Multiply}{Height} px");
		}

		protected override void Delete()
		{
			GL.DeleteTexture(OpenGL_ID);
		}

		public static Texture Load(identifier id,                                       string           filename, bool suppress_extension_warning = false, bool store_raw_data = false,
			TextureWrapMode                   wrap_u     = TextureWrapMode.ClampToEdge, TextureWrapMode  wrap_v     = TextureWrapMode.ClampToEdge,
			TextureMinFilter                  min_filter = TextureMinFilter.Linear,     TextureMagFilter mag_filter = TextureMagFilter.Linear)
		{
			FileManager.VerifyExtension(ref filename, suppress_extension_warning, "png", "jpg", "jpeg");
			Image<Rgba32> image = FileManager.LoadTexture(filename);
			if (image == null)
				throw new AssetLoadFailedException(filename);

			Rgba32[]   temp_pixels = image.GetPixelSpan().ToArray();
			List<byte> pixels      = new List<byte>();

			foreach (Rgba32 pixel in temp_pixels)
			{
				pixels.Add(pixel.R);
				pixels.Add(pixel.G);
				pixels.Add(pixel.B);
				pixels.Add(pixel.A);
			}

			int texture_id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, texture_id);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,     (int) wrap_u);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,     (int) wrap_v);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) min_filter);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) mag_filter);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());

			return store_raw_data
				       ? new Texture(id, texture_id, image.Width, image.Height, pixels)
				       : new Texture(id, texture_id, image.Width, image.Height);
		}
	}
}
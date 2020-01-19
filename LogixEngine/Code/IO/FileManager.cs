using LogixEngine.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;

namespace LogixEngine.IO
{
	/* ---------------------------------------------------------------------------- *
     * FileManager.cs created by DrLogiq on 19-01-2020 19:05.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	// TODO(LOGIQ): Documentation for all functions
	public sealed class FileManager
	{
		private static readonly string Root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "Terrascape" +
		                                         Path.DirectorySeparatorChar;

		private static readonly string AssetsRoot = Root + "assets" + Path.DirectorySeparatorChar;

		/// <summary>
		/// Builds an asset file path from the given filename. This will be the full file path going through /AppData/Terrascape/assets/.
		/// </summary>
		public static void GetAssetPath(ref string filename)
		{
			filename = filename.Replace(Path.DirectorySeparatorChar == '/' ? '\\' : '/', Path.DirectorySeparatorChar);
			filename = AssetsRoot + filename;
		}

		/// <summary>
		/// Verifies that the given [filename] has one of the [valid_extensions] given.
		/// <para>You must provide at least 1 extension. Extensions must not contain the period character.</para>
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="valid_extensions">A list of file extensions to accept.</param>
		public static void VerifyExtension(ref string filename, params string[] valid_extensions)
		{
			VerifyExtension(ref filename, false, valid_extensions);
		}
		
		public static void VerifyExtension(ref string filename, bool suppress_warning, params string[] valid_extensions)
		{
			if (valid_extensions.Length < 1)
				throw new AssetFileInvalidException($"Cannot verify file extension for filename \"{filename}\" because no valid extensions were given");
			
			foreach (string valid in valid_extensions)
				if (filename.EndsWith($".{valid}"))
					return;
			
			if (filename.Contains(".")) // TODO(LOGIQ): Verify that no files will ever need to use a period character in the full file path
				throw new AssetFileInvalidException($"The filename \"{filename}\" appears to contain an invalid file extension");

			if (!suppress_warning)
				Debug.LogWarning($"Filename \"{filename}\" does not end with any of the specified valid extensions; using default (\"{valid_extensions[0]}\")");
			
			filename += $".{valid_extensions[0]}";
		}

		public static List<string> ReadAssetToStringList(string filename)
		{
			GetAssetPath(ref filename);
			if (!File.Exists(filename))
				throw new AssetNotFoundException(filename);
			
			List<string> result = new List<string>();

			try
			{
				using (StreamReader reader = new StreamReader(filename))
					result.AddRange(reader.ReadToEnd().Split('\n'));
			}
			catch (Exception exception)
			{
				throw new AssetReadFailedException(filename, exception);
			}

			return result;
		}
		
		public static string ReadAssetToString(string filename)
		{
			GetAssetPath(ref filename);
			if (!File.Exists(filename))
				throw new AssetNotFoundException(filename);
			
			string result;

			try
			{
				using (StreamReader reader = new StreamReader(filename))
					result = reader.ReadToEnd();
			}
			catch (Exception exception)
			{
				throw new AssetReadFailedException(filename, exception);
			}

			return result;
		}

		public static Image<Rgba32> LoadTexture(string filename)
		{
			GetAssetPath(ref filename);
			if (!File.Exists(filename))
				throw new AssetNotFoundException(filename);

			Image<Rgba32> image;
			try
			{
				image = Image.Load(filename) as Image<Rgba32>;
			}
			catch (Exception exception)
			{
				throw new AssetReadFailedException(filename, exception);
			}

			return image;
		}
	}

	public class AssetNotFoundException : ApplicationException
	{
		public AssetNotFoundException(string filename)
			: base($"Could not load asset file '{filename}' because the file does not appear to exist")
		{
		}
	}
	
	public class AssetReadFailedException : ApplicationException
	{
		public AssetReadFailedException(string filename, Exception cause)
			: base($"Could not read asset file '{filename}'", cause)
		{
		}
	}
	
	public class AssetLoadFailedException : ApplicationException
	{
		public AssetLoadFailedException(string filename)
			: base($"Could not load asset file '{filename}'")
		{
		}
	}
	
	public class AssetFileInvalidException : ApplicationException
	{
		public AssetFileInvalidException(string message) : base(message)
		{
		}
	}
}
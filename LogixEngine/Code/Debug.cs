using Colorful;
using System;
using System.Collections.Generic;
using System.Drawing;
using Console = Colorful.Console;
using static LogixEngine.SpecialCharacters;

namespace LogixEngine
{
	/* ---------------------------------------------------------------------------- *
     * Debug.cs created by DrLogiq on 17-01-2020 10:32.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	public sealed class Debug
	{
		public static           bool         DebugMode { get; internal set; }
		private static readonly List<string> Log = new List<string>();
		private static          int          line;

		#region Utility

		private static string Timestamp
		{
			get
			{
				DateTime now = DateTime.Now;
				return $"{now.Hour:00}:{now.Minute:00}:{now.Second:00}::{now.Millisecond:000}";
			}
		}

		private static string TimestampPlaceholder => "                          ";

		private static readonly string TagInfo        = $"{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct} INFO {AngleQuotesR} ";
		private static readonly string TagDebug       = $"{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct} DEBUG {AngleQuotesR} ";
		private static readonly string TagWarning     = $"{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct} WARNING {AngleQuotesR} ";
		private static readonly string TagError       = $"{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct} ERROR {AngleQuotesR} ";
		private static readonly string TagCritical    = $"{Interpunct}{Interpunct}{Interpunct}{Interpunct}{Interpunct} CRITICAL {AngleQuotesR} ";
		private static readonly string TagEngine      = $"{Interpunct} LOGIX-ENGINE {AngleQuotesR} ";
		private const           string TagPlaceholder = "                 ";

		#endregion

		#region Styling / Syntax Highlighting

		private static readonly Color ColourTimestamp = Color.DimGray;
		private static readonly Color ColourTag       = Color.CornflowerBlue;
		private static readonly Color ColourInfo      = Color.Azure;
		private static readonly Color ColourDebug     = Color.DimGray;
		private static readonly Color ColourWarning   = Color.Goldenrod;
		private static readonly Color ColourError     = Color.IndianRed;
		private static readonly Color ColourCritical  = Color.Red;
		internal static readonly Color ColourEngine    = Color.LimeGreen;

		private static readonly Color SyntaxColourNumber     = Color.DodgerBlue;
		private static readonly Color SyntaxColourIdentifier = Color.Coral;
		private static readonly Color SyntaxColourString     = Color.Yellow;

		private const string RegexNumber     = @"[-+]?[0-9]\d*(\.|:\d+)?";
		private const string RegexIdentifier = @"'[A-z0-9_]+'";
		private const string RegexString     = "\".+\"";

		private static readonly StyleSheet SyntaxStyleSheetInfo     = new StyleSheet(ColourInfo);
		private static readonly StyleSheet SyntaxStyleSheetDebug    = new StyleSheet(ColourDebug);
		private static readonly StyleSheet SyntaxStyleSheetWarning  = new StyleSheet(ColourWarning);
		private static readonly StyleSheet SyntaxStyleSheetError    = new StyleSheet(ColourError);
		private static readonly StyleSheet SyntaxStyleSheetCritical = new StyleSheet(ColourCritical);

		static Debug()
		{
			SyntaxStyleSheetInfo.AddStyle(RegexNumber,     SyntaxColourNumber);
			SyntaxStyleSheetInfo.AddStyle(RegexIdentifier, SyntaxColourIdentifier);
			SyntaxStyleSheetInfo.AddStyle(RegexString,     SyntaxColourString);

			SyntaxStyleSheetDebug.AddStyle(RegexNumber,     SyntaxColourNumber);
			SyntaxStyleSheetDebug.AddStyle(RegexIdentifier, SyntaxColourIdentifier);
			SyntaxStyleSheetDebug.AddStyle(RegexString,     SyntaxColourString);

			SyntaxStyleSheetWarning.AddStyle(RegexNumber,     SyntaxColourNumber);
			SyntaxStyleSheetWarning.AddStyle(RegexIdentifier, SyntaxColourIdentifier);
			SyntaxStyleSheetWarning.AddStyle(RegexString,     SyntaxColourString);

			SyntaxStyleSheetError.AddStyle(RegexNumber,     SyntaxColourNumber);
			SyntaxStyleSheetError.AddStyle(RegexIdentifier, SyntaxColourIdentifier);
			SyntaxStyleSheetError.AddStyle(RegexString,     SyntaxColourString);

			SyntaxStyleSheetCritical.AddStyle(RegexNumber,     SyntaxColourNumber);
			SyntaxStyleSheetCritical.AddStyle(RegexIdentifier, SyntaxColourIdentifier);
			SyntaxStyleSheetCritical.AddStyle(RegexString,     SyntaxColourString);
		}

		#endregion

		#region Logging

		public static void LogInfo(string message)
		{
			if (DebugMode)
			{
				Console.Write($"{++line:0000} {Timestamp} ", ColourTimestamp);
				Console.Write(TagInfo,                       ColourTag);
				Console.WriteStyled($"{message}\n", SyntaxStyleSheetInfo);
			}

			Log.Add($"{line:0000} {Timestamp} {TagInfo} {message}");
		}

		public static void LogDebug(string message)
		{
			if (!DebugMode)
				return;

			Console.Write($"{++line:0000} {Timestamp} ", ColourTimestamp);
			Console.Write(TagDebug,                      ColourTag);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetDebug);

			Log.Add($"{line:0000} {Timestamp} {TagDebug} {message}");
		}

		public static void LogWarning(string message)
		{
			if (DebugMode)
			{
				Console.Write($"{++line:0000} {Timestamp} ", ColourTimestamp);
				Console.Write(TagWarning,                    ColourTag);
				Console.WriteStyled($"{message}\n", SyntaxStyleSheetWarning);
			}

			Log.Add($"{line:0000} {Timestamp} {TagWarning} {message}");
		}

		public static void LogError(string message)
		{
			if (DebugMode)
			{
				Console.Write($"{++line:0000} {Timestamp} ", ColourTimestamp);
				Console.Write(TagError,                      ColourTag);
				Console.WriteStyled($"{message}\n", SyntaxStyleSheetError);
			}

			Log.Add($"{line:0000} {Timestamp} {TagError} {message}");
		}

		public static void LogCritical(string message)
		{
			if (DebugMode)
			{
				Console.Write($"{++line:0000} {Timestamp} ", ColourTimestamp);
				Console.Write(TagCritical,                   ColourTag);
				Console.WriteStyled($"{message}\n", SyntaxStyleSheetCritical);
			}

			Log.Add($"{line:0000} {Timestamp} {TagCritical} {message}");
		}

		public static void LogInfoContinued(string message)
		{
			if (DebugMode)
			{
				Console.Write($"{++line:0000} {TimestampPlaceholder} {TagPlaceholder}", ColourTimestamp);
				Console.WriteStyled($"{message}\n", SyntaxStyleSheetInfo);
			}

			Log.Add($"{line:0000} {TimestampPlaceholder} {TagPlaceholder} {message}");
		}

		public static void LogDebugContinued(string message)
		{
			if (!DebugMode)
				return;

			Console.Write($"{++line:0000} {TimestampPlaceholder} {TagPlaceholder}", ColourTimestamp);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetDebug);

			Log.Add($"{line:0000} {TimestampPlaceholder} {TagPlaceholder} {message}");
		}

		public static void LogWarningContinued(string message)
		{
			if (DebugMode)
			{
				Console.Write($"{++line:0000} {TimestampPlaceholder} {TagPlaceholder}", ColourTimestamp);
				Console.WriteStyled($"{message}\n", SyntaxStyleSheetWarning);
			}

			Log.Add($"{line:0000} {TimestampPlaceholder} {TagPlaceholder} {message}");
		}

		public static void LogErrorContinued(string message)
		{
			if (DebugMode)
			{
				Console.Write($"{++line:0000} {TimestampPlaceholder} {TagPlaceholder}", ColourTimestamp);
				Console.WriteStyled($"{message}\n", SyntaxStyleSheetError);
			}

			Log.Add($"{line:0000} {TimestampPlaceholder} {TagPlaceholder} {message}");
		}

		public static void LogCriticalContinued(string message)
		{
			if (DebugMode)
			{
				Console.Write($"{++line:0000} {TimestampPlaceholder} {TagPlaceholder}", ColourTimestamp);
				Console.WriteStyled($"{message}\n", SyntaxStyleSheetCritical);
			}

			Log.Add($"{line:0000} {TimestampPlaceholder} {TagPlaceholder} {message}");
		}

		internal static void LogEngineMessage(string message)
		{
			if (DebugMode)
			{
				Console.Write($"{++line:0000} {Timestamp} ", ColourTimestamp);
				Console.Write(TagEngine,                     ColourTag);
				Console.Write($"{message}\n",                ColourEngine);
			}

			Log.Add($"{line:0000} {Timestamp} {TagEngine} {message}");
		}

		internal static void WriteRaw(string message)
		{
			System.Console.WriteLine($"  {AngleQuotesR} {message}");
			Log.Add($"{++line:0000} {Timestamp} {TagPlaceholder} {message}");
		}

		internal static List<string> GetLog()
		{
			return Log;
		}

		#endregion
	}
}
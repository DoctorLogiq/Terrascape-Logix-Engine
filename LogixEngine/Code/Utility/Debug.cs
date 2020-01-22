using Colorful;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using static LogixEngine.Utility.SpecialCharacters;
using Console = Colorful.Console;

namespace LogixEngine.Utility
{
	/* ---------------------------------------------------------------------------- *
     * Debug.cs created by DrLogiq on 17-01-2020 10:32.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	public static class Debug
	{
		public static           bool         DebugMode { get; internal set; }
		private static readonly List<string> Log = new List<string>();
		private static          int          line;
		private static          int          indentation;
		private static          string       indentation_string = "";

		#region Utility

		private static string Timestamp
		{
			get
			{
				DateTime now = DateTime.Now;
				return $"{now.Hour:00}:{now.Minute:00}:{now.Second:00}::{now.Millisecond:000}";
			}
		}

		public static void Indent()
		{
			indentation++;
			indentation_string += "    ";
		}

		public static void Unindent()
		{
			indentation--;
			if (indentation < 0)
			{
				indentation = 0;
			}

			indentation_string = "";
			for (int i = 0; i < indentation; ++i)
			{
				indentation_string += "    ";
			}
		}

		public static void ResetIndentation()
		{
			indentation        = 0;
			indentation_string = "";
		}

		private const string TimestampPlaceholder = "             ";

		public static void RunTests()
		{
			LogDebug("Test debug");
			LogDebugContinued("Test debug continued");
			LogDebugContinued("Test debug continued with a \"string\" and an 'identifier' and the number 001");
			Indent();
			LogDebug("Test debug indented");
			LogDebugContinued("Test debug indented & continued");
			Unindent();

			LogInfo("Test info");
			LogInfoContinued("Test info continued");
			LogInfoContinued("Test info continued with a \"string\" and an 'identifier' and the number 001");
			Indent();
			LogInfo("Test info indented");
			LogInfoContinued("Test info indented & continued");
			Unindent();

			LogWarning("Test warning");
			LogWarningContinued("Test warning continued");
			LogWarningContinued("Test warning continued with a \"string\" and an 'identifier' and the number 001");
			Indent();
			LogWarning("Test warning indented");
			LogWarningContinued("Test warning indented & continued");
			Unindent();

			LogError("Test error");
			LogErrorContinued("Test error continued");
			LogErrorContinued("Test error continued with a \"string\" and an 'identifier' and the number 001");
			Indent();
			LogError("Test error indented");
			LogErrorContinued("Test error indented & continued");
			Unindent();

			LogCritical("Test critical");
			LogCriticalContinued("Test critical continued");
			LogCriticalContinued("Test critical continued with a \"string\" and an 'identifier' and the number 001");
			Indent();
			LogCritical("Test critical indented");
			LogCriticalContinued("Test critical indented & continued");
			Unindent();

			ResetIndentation();
		}

		#endregion

		#region Styling / Syntax Highlighting
		
		private static readonly  Color ColourTimestamp = Color.DimGray;
		private static readonly  Color ColourTag       = Color.CornflowerBlue;
		private static readonly  Color ColourInfo      = Color.Azure;
		private static readonly  Color ColourDebug     = Color.DimGray;
		private static readonly  Color ColourWarning   = Color.FromArgb(243, 119, 53);
		private static readonly  Color ColourError     = Color.FromArgb(233, 75, 60);
		private static readonly  Color ColourCritical  = Color.FromArgb(209, 17, 65);
		internal static readonly Color ColourEngine    = Color.FromArgb(60, 233, 75);

		private static readonly Color SyntaxColourNumber     = Color.FromArgb(0, 174, 219);
		private static readonly Color SyntaxColourIdentifier = Color.FromArgb(255, 196, 37);
		private static readonly Color SyntaxColourString     = Color.FromArgb(255, 196, 37);
		private static readonly Color SyntaxColourBoolTrue   = Color.FromArgb(60, 233, 75);
		private static readonly Color SyntaxColourBoolFalse  = Color.FromArgb(233, 75, 60);
		private static readonly Color SyntaxColourType       = Color.FromArgb(209,17,65);

		private const  string RegexNumber     = @"[-+.]?\d+([\.:,]\d+)*";
		private const  string RegexIdentifier = @"'[A-z0-9_]+'";
		private const  string RegexString     = "\"(?:[^\"\\\\]|\\.)*\"";
		private const  string RegexBoolTrue   = @"\b(yes|Yes|YES|true|True|TRUE)\b";
		private const  string RegexBoolFalse  = @"\b(no|No|NO|false|False|FALSE)\b";
		private static string regex_type      = @"\b([A-z]*(Exception)|int|bool|float|double|string|char|Texture|Shader|Model)\b";

		private static readonly StyleSheet SyntaxStyleSheetInfo     = new StyleSheet(ColourInfo);
		private static readonly StyleSheet SyntaxStyleSheetDebug    = new StyleSheet(ColourDebug);
		private static readonly StyleSheet SyntaxStyleSheetWarning  = new StyleSheet(ColourWarning);
		private static readonly StyleSheet SyntaxStyleSheetError    = new StyleSheet(ColourError);
		private static readonly StyleSheet SyntaxStyleSheetCritical = new StyleSheet(ColourCritical);

		private static readonly List<string> RegisteredTypes = new List<string>();

		internal static void RegisterType(string type)
		{
			if (Game.IsRunning)
				throw new ApplicationException("Types should be registered in the game's constructor'");
			
			regex_type = regex_type.Replace(@")\b", "") + $@"|{type})\b";

			// NOTE(LOGIQ): The number here MUST equal the number (in chronilogical order) that this regex was added to the StyleSheets in the static constructor!
			SyntaxStyleSheetInfo.Styles[5].Target     = new TextPattern(regex_type);
			SyntaxStyleSheetDebug.Styles[5].Target    = new TextPattern(regex_type);
			SyntaxStyleSheetWarning.Styles[5].Target  = new TextPattern(regex_type);
			SyntaxStyleSheetError.Styles[5].Target    = new TextPattern(regex_type);
			SyntaxStyleSheetCritical.Styles[5].Target = new TextPattern(regex_type);

			RegisteredTypes.Add(type);
		}

		internal static void LogDebugRegisteredTypes()
		{
			int    i         = 0;
			string type_list = "";
			foreach (string type in RegisteredTypes)
			{
				if (i < 10)
				{
					type_list += (type_list.Length > 0 ? ", " : "") + type;
					i++;
				}
				else
				{
					LogDebug("Registered types: " + type_list);
					i         = 0;
					type_list = "";
				}
			}

			if (type_list.Length > 0)
				LogDebug("Registered types: " + type_list);

			RegisteredTypes.Clear();
		}

		static Debug()
		{
			SyntaxStyleSheetInfo.AddStyle(RegexNumber,     SyntaxColourNumber);
			SyntaxStyleSheetInfo.AddStyle(RegexIdentifier, SyntaxColourIdentifier);
			SyntaxStyleSheetInfo.AddStyle(RegexString,     SyntaxColourString);
			SyntaxStyleSheetInfo.AddStyle(RegexBoolTrue,   SyntaxColourBoolTrue);
			SyntaxStyleSheetInfo.AddStyle(RegexBoolFalse,  SyntaxColourBoolFalse);
			SyntaxStyleSheetInfo.AddStyle(regex_type,      SyntaxColourType);

			SyntaxStyleSheetDebug.AddStyle(RegexNumber,     SyntaxColourNumber);
			SyntaxStyleSheetDebug.AddStyle(RegexIdentifier, SyntaxColourIdentifier);
			SyntaxStyleSheetDebug.AddStyle(RegexString,     SyntaxColourString);
			SyntaxStyleSheetDebug.AddStyle(RegexBoolTrue,   SyntaxColourBoolTrue);
			SyntaxStyleSheetDebug.AddStyle(RegexBoolFalse,  SyntaxColourBoolFalse);
			SyntaxStyleSheetDebug.AddStyle(regex_type,      SyntaxColourType);

			SyntaxStyleSheetWarning.AddStyle(RegexNumber,     SyntaxColourNumber);
			SyntaxStyleSheetWarning.AddStyle(RegexIdentifier, SyntaxColourIdentifier);
			SyntaxStyleSheetWarning.AddStyle(RegexString,     SyntaxColourString);
			SyntaxStyleSheetWarning.AddStyle(RegexBoolTrue,   SyntaxColourBoolTrue);
			SyntaxStyleSheetWarning.AddStyle(RegexBoolFalse,  SyntaxColourBoolFalse);
			SyntaxStyleSheetWarning.AddStyle(regex_type,      SyntaxColourType);

			SyntaxStyleSheetError.AddStyle(RegexNumber,     SyntaxColourNumber);
			SyntaxStyleSheetError.AddStyle(RegexIdentifier, SyntaxColourIdentifier);
			SyntaxStyleSheetError.AddStyle(RegexString,     SyntaxColourString);
			SyntaxStyleSheetError.AddStyle(RegexBoolTrue,   SyntaxColourBoolTrue);
			SyntaxStyleSheetError.AddStyle(RegexBoolFalse,  SyntaxColourBoolFalse);
			SyntaxStyleSheetError.AddStyle(regex_type,      SyntaxColourType);

			SyntaxStyleSheetCritical.AddStyle(RegexNumber,     SyntaxColourNumber);
			SyntaxStyleSheetCritical.AddStyle(RegexIdentifier, SyntaxColourIdentifier);
			SyntaxStyleSheetCritical.AddStyle(RegexString,     SyntaxColourString);
			SyntaxStyleSheetCritical.AddStyle(RegexBoolTrue,   SyntaxColourBoolTrue);
			SyntaxStyleSheetCritical.AddStyle(RegexBoolFalse,  SyntaxColourBoolFalse);
			SyntaxStyleSheetCritical.AddStyle(regex_type,      SyntaxColourType);
		}

		#endregion

		#region Logging

		/// <summary>
		/// Logs the given [message] on the "Info" channel.
		/// <para>
		/// The message will be formatted to contain the line number, timestamp and channel,
		/// and may also contain syntax-highlighting.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		public static void LogInfo(string message)
		{
			if (!Game.IsRunning)
			{
				Game.HasCrashed = true;
				Game.ConstructorException = new ApplicationException("Cannot perform tasks before the game starts running");
			}
			
			Console.Write($"{++line:0000} {Timestamp} {AngleQuotesR} {indentation_string}", ColourTimestamp);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetInfo);

			Log.Add($"{line:0000} {Timestamp} {AngleQuotesR} {indentation_string}{message}");
		}

		/// <summary>
		/// Logs the given [message] on the "Debug" channel. Only works if the game is running
		/// with the [-debug] flag (is running in debug mode).
		/// <para>
		/// The message will be formatted to contain the line number, timestamp and channel,
		/// and may also contain syntax-highlighting.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		public static void LogDebug(string message)
		{
			if (!DebugMode)
				return;
			
			if (!Game.IsRunning)
			{
				Game.HasCrashed           = true;
				Game.ConstructorException = new ApplicationException("Cannot perform tasks before the game starts running");
			}

			Console.Write($"{++line:0000} {Timestamp} {AngleQuotesR} {indentation_string}", ColourTimestamp);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetDebug);

			Log.Add($"{line:0000} {Timestamp} {AngleQuotesR} {indentation_string}{message}");
		}
		
		/// <summary>
		/// Logs the given [message] on the "Debug" channel. Only works if the game is running
		/// with the [-debug] flag (is running in debug mode). This overload disables syntax
		/// highlighting in favour of using a non-default colour.
		/// <para>
		/// The message will be formatted to contain the line number, timestamp and channel,
		/// and may also contain syntax-highlighting.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		/// <param name="colour">The colour to write the bulk of the message in</param>
		public static void LogDebug(string message, Color colour)
		{
			if (!DebugMode)
				return;
			
			if (!Game.IsRunning)
			{
				Game.HasCrashed           = true;
				Game.ConstructorException = new ApplicationException("Cannot perform tasks before the game starts running");
			}

			Console.Write($"{++line:0000} {Timestamp} {AngleQuotesR} {indentation_string}", ColourTimestamp);
			Console.Write($"{message}\n", colour);

			Log.Add($"{line:0000} {Timestamp} {AngleQuotesR} {indentation_string}{message}");
		}

		/// <summary>
		/// Logs the given [message] on the "Warning" channel.
		/// <para>
		/// The message will be formatted to contain the line number, timestamp and channel,
		/// and may also contain syntax-highlighting.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		public static void LogWarning(string message)
		{
			if (!Game.IsRunning)
			{
				Game.HasCrashed           = true;
				Game.ConstructorException = new ApplicationException("Cannot perform tasks before the game starts running");
			}
			
			Console.Write($"{++line:0000} {Timestamp} {AngleQuotesR} {indentation_string}", ColourTimestamp);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetWarning);

			Log.Add($"{line:0000} {Timestamp} {AngleQuotesR} {indentation_string}{message}");
		}

		/// <summary>
		/// Logs the given [message] on the "Error" channel.
		/// <para>
		/// The message will be formatted to contain the line number, timestamp and channel,
		/// and may also contain syntax-highlighting.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		public static void LogError(string message)
		{
			if (!Game.IsRunning)
			{
				Game.HasCrashed           = true;
				Game.ConstructorException = new ApplicationException("Cannot perform tasks before the game starts running");
			}
			
			Console.Write($"{++line:0000} {Timestamp} {AngleQuotesR} {indentation_string}", ColourTimestamp);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetError);

			Log.Add($"{line:0000} {Timestamp} {AngleQuotesR} {indentation_string}{message}");
		}

		/// <summary>
		/// Logs the given [message] on the "Critical" channel.
		/// <para>
		/// The message will be formatted to contain the line number, timestamp and channel,
		/// and may also contain syntax-highlighting.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		public static void LogCritical(string message)
		{
			if (!Game.IsRunning)
			{
				Game.HasCrashed           = true;
				Game.ConstructorException = new ApplicationException("Cannot perform tasks before the game starts running");
			}
			
			Console.Write($"{++line:0000} {Timestamp} {AngleQuotesR} {indentation_string}", ColourTimestamp);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetCritical);

			Log.Add($"{line:0000} {Timestamp} {AngleQuotesR} {indentation_string}{message}");
		}

		/// <summary>
		/// Logs the given [message] on the "Info" channel. 'Continued' denotes that this is a continuation
		/// of a long [message] that is being split onto multiple lines. Use LogInfo(...) before using this.
		/// <para>
		/// The message will be formatted to contain the line number,
		/// and may also contain syntax-highlighting.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		public static void LogInfoContinued(string message)
		{
			if (!Game.IsRunning)
			{
				Game.HasCrashed           = true;
				Game.ConstructorException = new ApplicationException("Cannot perform tasks before the game starts running");
			}
			
			Console.Write($"{++line:0000} {TimestampPlaceholder} {AngleQuotesR} {indentation_string}", ColourTimestamp);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetInfo);

			Log.Add($"{line:0000} {TimestampPlaceholder} {AngleQuotesR} {indentation_string}{message}");
		}

		/// <summary>
		/// Logs the given [message] on the "Debug" channel. Only works if the game is running
		/// with the [-debug] flag (is running in debug mode). 'Continued' denotes that this
		/// is a continuation of a long [message] that is being split onto multiple lines.
		/// Use LogDebug(...) before using this.
		/// <para>
		/// The message will be formatted to contain the line number,
		/// and may also contain syntax-highlighting.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		public static void LogDebugContinued(string message)
		{
			if (!Game.IsRunning)
			{
				Game.HasCrashed           = true;
				Game.ConstructorException = new ApplicationException("Cannot perform tasks before the game starts running");
			}
			
			if (!DebugMode)
				return;

			Console.Write($"{++line:0000} {TimestampPlaceholder} {AngleQuotesR} {indentation_string}", ColourTimestamp);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetDebug);

			Log.Add($"{line:0000} {TimestampPlaceholder} {AngleQuotesR} {indentation_string}{message}");
		}

		/// <summary>
		/// Logs the given [message] on the "Warning" channel. 'Continued' denotes that this is a continuation
		/// of a long [message] that is being split onto multiple lines. Use LogWarning(...) before using this.
		/// <para>
		/// The message will be formatted to contain the line number,
		/// and may also contain syntax-highlighting.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		public static void LogWarningContinued(string message)
		{
			if (!Game.IsRunning)
			{
				Game.HasCrashed           = true;
				Game.ConstructorException = new ApplicationException("Cannot perform tasks before the game starts running");
			}
			
			Console.Write($"{++line:0000} {TimestampPlaceholder} {AngleQuotesR} {indentation_string}", ColourTimestamp);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetWarning);

			Log.Add($"{line:0000} {TimestampPlaceholder} {AngleQuotesR} {indentation_string}{message}");
		}

		/// <summary>
		/// Logs the given [message] on the "Error" channel. 'Continued' denotes that this is a continuation
		/// of a long [message] that is being split onto multiple lines. Use LogError(...) before using this.
		/// <para>
		/// The message will be formatted to contain the line number,
		/// and may also contain syntax-highlighting.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		public static void LogErrorContinued(string message)
		{
			if (!Game.IsRunning)
			{
				Game.HasCrashed           = true;
				Game.ConstructorException = new ApplicationException("Cannot perform tasks before the game starts running");
			}
			
			Console.Write($"{++line:0000} {TimestampPlaceholder} {AngleQuotesR} {indentation_string}", ColourTimestamp);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetError);

			Log.Add($"{line:0000} {TimestampPlaceholder} {AngleQuotesR} {indentation_string}{message}");
		}

		/// <summary>
		/// Logs the given [message] on the "Critical" channel. 'Continued' denotes that this is a continuation
		/// of a long [message] that is being split onto multiple lines. Use LogCritical(...) before using this.
		/// <para>
		/// The message will be formatted to contain the line number,
		/// and may also contain syntax-highlighting.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		public static void LogCriticalContinued(string message)
		{
			if (!Game.IsRunning)
			{
				Game.HasCrashed           = true;
				Game.ConstructorException = new ApplicationException("Cannot perform tasks before the game starts running");
			}

			Console.Write($"{++line:0000} {TimestampPlaceholder} {AngleQuotesR} {indentation_string}", ColourTimestamp);
			Console.WriteStyled($"{message}\n", SyntaxStyleSheetCritical);

			Log.Add($"{line:0000} {TimestampPlaceholder} {AngleQuotesR} {indentation_string}{message}");
		}

		/// <summary>
		/// Logs the given [message] on the "Logix-Engine" channel.
		/// <para>
		/// The message will be formatted to contain the line number, timestamp and channel.
		/// </para>
		/// </summary>
		/// <param name="message">The message to write</param>
		/// <param name="is_debug">Whether or not this message should show only in debug mode</param>
		internal static void LogEngineMessage(string message, bool is_debug)
		{
			if (!is_debug || DebugMode)
			{
				Console.Write($"{++line:0000} {Timestamp} {AngleQuotesR} {indentation_string}",      ColourTimestamp);
				Console.Write($"{message}\n", ColourEngine);
			}

			Log.Add($"{line:0000} {Timestamp} {AngleQuotesR} {indentation_string}{message}");
		}

		/// <summary>
		/// Writes a message directly to the console without using colouring.
		/// </summary>
		/// <param name="message">The message to write</param>
		internal static void WriteRaw(string message)
		{
			System.Console.WriteLine($"  {AngleQuotesR} {message}");
			Log.Add($"{++line:0000} {Timestamp} {AngleQuotesR} {message}");
		}

		/// <summary>
		/// Returns the full log of all messages that have been printed.
		/// </summary>
		/// <returns>the full log of all messages that have been printed</returns>
		internal static List<string> GetLog()
		{
			return Log;
		}

		#endregion

		#region Profiling & Tasks

		/// <summary>
		/// Logs the [task_name] (with an ellipsis appended), indents, performs the task, unindents, then
		/// prints that the task has completed (no verification).
		/// </summary>
		/// <param name="task_name">The name of the task. Should start with a capital letter</param>
		/// <param name="task">The task to perform</param>
		public static void LogDebugTask(string task_name, Task task)
		{
			if (DebugMode)
			{
				LogDebug(task_name + "...");
				Indent();

				task.RunSynchronously();

				Unindent();
				LogDebug($"{task_name} completed");
			}
			else
			{
				task.RunSynchronously();
			}
		}
		
		/// <summary>
		/// Profiles how long a task takes to complete, printing the result in the Debug channel once
		/// the task has been run synchronously. Can also act like LogDebugTask(...) if the corresponding
		/// parameter ([log_debug_task]) is set to [true].
		/// <para>
		/// NOTE: This does not account for the time it takes to print to the console, or create, start
		/// and stop the timer, so expect a little variance in the results. Accuracy is dictated by the
		/// system clock granularity (or resolution, if you will), as well as the granularity of .NET's
		/// timers[*¹], which can be expected to be around 12-15 milliseconds, according to multiple
		/// sources. This also imparts a minimum time readout of 12-15 milliseconds, so if the task took
		/// less time than that, you may not expect the results to be accurate.
		/// </para>
		/// Use IsProfilerHighResolution() and GetProfilerResolution() to check the granularity available
		/// to you when profiling.
		/// <para>
		/// *¹ this uses System.Diagnostics.Stopwatch, which is said to be the highest resolution
		/// timer in .NET.
		/// </para>
		/// </summary>
		/// <param name="task_name">The name of the task. Should start with a capital letter</param>
		/// <param name="log_debug_task">Whether or not to print the [task_name] before running the task
		/// (see LogDebugTask(...) for more info)</param>
		/// <param name="task">The task to perform</param>
		public static void Profile(string task_name, bool log_debug_task, Task task)
		{
			if (DebugMode)
			{
				if (log_debug_task)
				{
					LogDebug(task_name + "...");
					Indent();
				}

				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				task.RunSynchronously();

				// NOTE: If a task throws an exception it won't be caught, so we have to check that the task didn't throw one, and if it did, we need to re-throw it
				if (task.IsFaulted && task.Exception?.InnerException != null)
					throw task.Exception.InnerException; // TODO(LOGIQ): We need the stacktrace!

				stopwatch.Stop();
				if (log_debug_task)
				{
					Unindent();
					LogDebug($"...completed in {stopwatch.ElapsedMilliseconds} {(stopwatch.ElapsedMilliseconds == 1 ? "millisecond" : "milliseconds")}");
				}
				else
				{
					LogDebug($"...took {stopwatch.ElapsedMilliseconds} {(stopwatch.ElapsedMilliseconds == 1 ? "millisecond" : "milliseconds")}");
				}
			}
			else
			{
				task.RunSynchronously();
			}
		}

		/// <summary>
		/// Profiles how long a task takes to complete, printing the result in the Debug channel once
		/// the task has been run synchronously. Can also act like LogDebugTask(...) if the corresponding
		/// parameter ([log_debug_task]) is set to [true].
		/// <para>
		/// NOTE: This does not account for the time it takes to print to the console, or create, start
		/// and stop the timer, so expect a little variance in the results. Accuracy is dictated by the
		/// system clock granularity (or resolution, if you will), as well as the granularity of .NET's
		/// timers[*¹], which can be expected to be around 12-15 milliseconds, according to multiple
		/// sources. This also imparts a minimum time readout of 12-15 milliseconds, so if the task took
		/// less time than that, you may not expect the results to be accurate.
		/// </para>
		/// Use IsProfilerHighResolution() and GetProfilerResolution() to check the granularity available
		/// to you when profiling.
		/// <para>
		/// *¹ this uses System.Diagnostics.Stopwatch, which is said to be the highest resolution
		/// timer in .NET.
		/// </para>
		/// </summary>
		/// <param name="task_name">The name of the task. Should start with a capital letter</param>
		/// <param name="log_debug_task">Whether or not to print the [task_name] before running the task
		/// (see LogDebugTask(...) for more info)</param>
		/// <param name="task">The task to perform</param>
		public static object Profile<T>(string task_name, bool log_debug_task, Task<T> task) where T : class
		{
			if (DebugMode)
			{
				if (log_debug_task)
				{
					LogDebug(task_name + "...");
					Indent();
				}

				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				task.RunSynchronously();
				T result = task.Result;

				// NOTE: If a task throws an exception it won't be caught, so we have to check that the task didn't throw one, and if it did, we need to re-throw it
				if (task.IsFaulted && task.Exception?.InnerException != null)
					throw task.Exception.InnerException; // TODO(LOGIQ): We need the stacktrace!

				stopwatch.Stop();
				if (log_debug_task)
				{
					Unindent();
					LogDebug($"...completed in {stopwatch.ElapsedMilliseconds} {(stopwatch.ElapsedMilliseconds == 1 ? "millisecond" : "milliseconds")}");
				}
				else
				{
					LogDebug($"...took {stopwatch.ElapsedMilliseconds} {(stopwatch.ElapsedMilliseconds == 1 ? "millisecond" : "milliseconds")}");
				}

				return result;
			}
			else
			{
				task.Start();
				T result = task.Result; // TODO(LOGIQ): Test
				return result;
			}
		}

		/// <summary>
		/// Returns whether or not the profiler can use high resolution timers. This is dictated by
		/// the system running the game.
		/// </summary>
		/// <returns>whether or not the profiler can use high resolution timers</returns>
		public static bool IsProfilerHighResolution()
		{
			return Stopwatch.IsHighResolution;
		}

		/// <summary>
		/// Returns the profiler resolution in ticks per second (not related to game ticks!).
		/// </summary>
		/// <returns>the profiler resolution in ticks per second (not related to game ticks!)</returns>
		public static long GetProfilerResolution()
		{
			return Stopwatch.Frequency;
		}

		#endregion
	}
}
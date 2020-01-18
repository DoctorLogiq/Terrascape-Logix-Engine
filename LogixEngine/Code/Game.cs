#region Imports

// System
using LogixEngine._Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using Console = Colorful.Console;
// OpenTK
using OpenTK;
using OpenTK.Graphics;
// LogixEngine
using LogixEngine.Utility;

#endregion

namespace LogixEngine
{
	/* ----------------------------------------------------------------------------  *
     * Game.cs created by DrLogiq on 15-01-2020 13:01.
     * Copyright © DrLogiq. All rights reserved.
     * ----------------------------------------------------------------------------  */
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
	public abstract class Game : IDisposable
	{
		public const string LogixEngineVersion = "1.0.0";
		
		private readonly string     name;
		private readonly string     version; // TODO(LOGIQ): Make a version class to use here
		private readonly int        target_cps;
		private          int        target_fps;
		private readonly bool       update_title_in_render;
		private readonly bool       render_while_resizing = true;
		private readonly GameWindow window;
		private          string     title_format;
		public static    double     Width,       Height, HalfWidth, HalfHeight;
		private          bool       has_crashed, has_disposed;
		private          bool       cf_per_second_override;

		protected int TargetFPS
		{
			get => target_fps;
			set
			{
				if (value > 0 && value <= 999)
				{
					target_fps                   = value;
					window.TargetRenderFrequency = value;
				}
				else throw new ApplicationException($"Cannot set the target FPS to {value}!");
			}
		}

		#region Interop Services

		// ReSharper disable all

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr WindowHandle);

		[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
		static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

		const int SW_RESTORE = 9;
		const int SW_HIDE    = 0;
		const int SW_SHOW    = 1;

		private static void FocusConsole(IntPtr window)
		{
			string originalTitle = Console.Title;
			string uniqueTitle   = Guid.NewGuid().ToString();
			Console.Title = uniqueTitle;
			Thread.Sleep(50);
			IntPtr handle = FindWindowByCaption(IntPtr.Zero, uniqueTitle);

			Console.Title = originalTitle;

			ShowWindowAsync(new HandleRef(null, handle), SW_RESTORE);
			SetForegroundWindow(handle);
		}

		// ReSharper restore all

		#endregion

		#region Constructor And Run Function

		/// <summary>
		/// Constructs your game. This constructor should only be called once, EVER! You MUST pass in the program arguments
		/// from your Main() entry-point function, so that the game can process the arguments the user wishes to use. Failure
		/// to do so will result in the game not performing as the user intends, and that's on you!
		/// </summary>
		/// <param name="name">The game's display name. Should start with a capital letter</param>
		/// <param name="version">The game's version</param>
		/// <param name="startup_width">The initial width of the game window</param>
		/// <param name="startup_height">The initial height of the game window</param>
		/// <param name="target_cps">The target cycle (update) rate (in Hz) to run the game at, this cannot be changed later</param>
		/// <param name="target_fps">The target render rate (in Hz) to start the game at, this can be changed later</param>
		/// <param name="arguments">MANDATORY - pass the unmodified program arguments from your Main() entry-point function here!</param>
		/// <param name="window_mode">The window's startup mode</param>
		/// <param name="gl_major">The OpenGL major version</param>
		/// <param name="gl_minor">The OpenGL minor version</param>
		/// <param name="update_title_in_render">Whether to update the title in the render cycle. If set to [false], the
		/// title will be updated in the update cycle instead</param>
		// TODO(LOGIQ): Implement a singleton design pattern here because you know programmers don't follow rules!
		protected Game(string   name,
			string              version,
			int                 startup_width,
			int                 startup_height,
			int                 target_cps,
			int                 target_fps,
			IEnumerable<string> arguments,
			WindowMode          window_mode            = WindowMode.Default,
			int                 gl_major               = 4,
			int                 gl_minor               = 4,
			bool                update_title_in_render = true)
		{
			// Set variables
			this.name                   = name;
			this.version                = version;
			this.target_cps             = target_cps;
			this.target_fps             = target_fps;
			this.update_title_in_render = update_title_in_render;

			// Set the default title format using the game's name
			string title_separator = $"     {SpecialCharacters.Separator}     ";
			title_format = $"%name% %version%{title_separator}%cps%/%tcps% cycles/sec{title_separator}%fps%/%tfps% frames/sec{title_separator}%res% px";

			// Choose the window flags to use based on the WindowMode (resizable? fullscreen? borderless?)
			GameWindowFlags startup_mode =
				(window_mode == WindowMode.Default)
					? GameWindowFlags.Default
					: (window_mode == WindowMode.DefaultFixed)
						? GameWindowFlags.FixedWindow
						: GameWindowFlags.Fullscreen;

			// Create the window
			window = new GameWindow(startup_width, startup_height, GraphicsMode.Default, name, startup_mode, DisplayDevice.Default, gl_major, gl_minor, GraphicsContextFlags.ForwardCompatible);

			// Set the window event handlers
			window.Load += (sender, args) =>
			{
				PreWarm();
				Load();
			};

			window.UpdateFrame += (sender, args) => DoCycle(args.Time);
			window.RenderFrame += (sender, args) => DoRender(args.Time);

			window.Resize += (sender, args) =>
			{
				Width      = window.Width;
				Height     = window.Height;
				HalfWidth  = Width / 2D;
				HalfHeight = Height / 2D;

				cf_per_second_override = true;
				UpdateTitle();
				cf_per_second_override = false;
				
				if (render_while_resizing)
				{
					DoRender(0.0D);
				}
			};
			
			// Process program arguments
			foreach (string argument in arguments)
			{
				switch (argument.ToLower())
				{
					default:
						Debug.LogWarning($"Unrecognized argument '{argument}' - ignoring it!");
						break;
					case "-debug":
					{
						Debug.DebugMode = true;
					}
					break;
					case "-drwr": // NOTE(LOGIQ): Don't render while resizing
					{
						render_while_resizing = false;
					}
					break;
				}
			} // end of: foreach (string argument in arguments)...
		}

		protected void Run()
		{
			// Store the console's original title so that if the game was called from an existing console, the
			//  title can be reverted back to what it was after the game stops.
			string original_console_title = Console.Title;
			Console.Title = $"{name} Debugger";

			// Hide the console window until we know if the user wants debug mode.
			// TODO(LOGIQ): Not if the console was already open before the game was called
			if (!Debug.DebugMode)
				ShowWindow(GetConsoleWindow(), SW_HIDE);

			if (Debug.DebugMode)
			{
				// Ensure the console window meets minimum width requirements (so as to avoid printouts ending
				// up wrapping onto multiple lines, as much as possible) 
				if (Console.BufferWidth < 160)
				{
					Console.SetBufferSize(160, Console.BufferHeight);
				}

				// Print helpful headers so users know what they're looking at, and a separator to keep
				// things clean and tidy
				Console.WriteLine();
				Console.WriteLine("Line H  M  S   Ms  Channel        Message", Color.DimGray);
				string separator      = "────┼──┼──┼──┼┼───┼──────────────┼─────────";
				for (int i = separator.Length; i < Console.BufferWidth - 1; ++i)
				{
					separator += "─";
				}

				Console.WriteLine(separator, Color.DimGray);
			}

			// Attempt to run the game, and catch any exceptions that are thrown. We'll assume that all
			//  exceptions are bad (because, well, they are!), and if the game wants to allow any exceptions
			//  it will have to explicitly catch them.
			try
			{
				Debug.LogDebug("Starting game");
				Debug.LogEngineMessage("This game is using LogixEngine version " + LogixEngineVersion);
				window.Run(target_cps, target_fps);
				Debug.LogDebug("Stopping game");
			}
			catch (Exception exception)
			{
				// An exception has been caught, so treat the game as having 'crashed'.
				has_crashed = true;

				// Close the window and make sure it actually closes here and now.
				window.Close();
				window.ProcessEvents();

				// If in debug mode, print the exception using Debug so as to have syntax highlighting,
				//  line numbers and timestamps
				if (Debug.DebugMode)
				{
					// TODO(LOGIQ): Debug.ResetIndentation();
					Debug.LogCritical("An unhandled exception was caught which caused the game to crash (safely).");

					Exception exc = exception;
					while (exc != null)
					{
						// Print the exception type and message (if it has one)
						Debug.LogCriticalContinued(!string.IsNullOrEmpty(exc.Message)
							                           ? $"Caused by an {exc.GetType().Name} with the message: {exc.Message}"
							                           : $"Caused by an {exc.GetType().Name}");

						// Print the stacktrace, reformatting it as we go to make it shorter and easier to understand
						if (!string.IsNullOrEmpty(exc.StackTrace))
						{
							foreach (string trace in exc.StackTrace.Split('\n'))
							{
								string stacktrace = trace.Replace(" at ", "").Trim();
								if (stacktrace.Contains("\\"))
									stacktrace = stacktrace.Substring(stacktrace.LastIndexOf('\\') + 1);
								Debug.LogCriticalContinued($"  at: {stacktrace}");
							}
						}

						exc = exc.InnerException;
						// TODO(LOGIQ): Debug.Indent();
					}
				}
				// If not in debug mode, print the exception using the normal System.Console.WriteLine
				//  function, so as to not cause strange re-colouring of the console text
				else
				{
					Console.WriteLine();
					Debug.WriteRaw("An unhandled exception was caught which caused the game to crash (safely).");

					Exception exc = exception;
					while (exc != null)
					{
						// Print the exception type and message (if it has one)
						Debug.WriteRaw(!string.IsNullOrEmpty(exc.Message)
							               ? $"Caused by an {exc.GetType().Name} with the message: {exc.Message}"
							               : $"Caused by an {exc.GetType().Name}");

						// Print the stacktrace, reformatting it as we go to make it shorter and easier to understand
						if (!string.IsNullOrEmpty(exc.StackTrace))
						{
							foreach (string trace in exc.StackTrace.Split('\n'))
							{
								string stacktrace = trace.Replace(" at ", "").Trim();
								if (stacktrace.Contains("\\"))
									stacktrace = stacktrace.Substring(stacktrace.LastIndexOf('\\') + 1);
								Debug.WriteRaw($"  at: {stacktrace}");
							}
						}

						exc = exc.InnerException;
					}
				}
			}

			// Ensure that the shutdown function has been run (this may not happen if an exception was
			//  caught). If this has been called already, nothing will happen.
			Dispose();

			// TODO(LOGIQ): Save log

			// If in debug mode, hold the console just to make sure that the console doesn't immediately close
			//  in case some shutdown messages need to be checked.
			if (Debug.DebugMode)
			{
				Debug.LogEngineMessage("Press any key to exit.");
				Console.Write("\tPRESS ANY KEY...", Debug.ColourEngine);
				Console.ReadKey();

				string end_separator = "────────────────────────────────────────────";
				for (int i = end_separator.Length; i < Console.BufferWidth - 1; ++i)
				{
					end_separator += "─";
				}

				Console.WriteLine(end_separator, Color.DimGray);
			}
			// If not in debug mode and the game crashed, hold the console to make sure the user is aware of
			//  what has happened.
			else if (has_crashed)
			{
				System.Console.Write("\tPress any key to aknowledge...");
				System.Console.ReadKey();
			}

			// Now revert the console's title back to what it was (in case the user is going to continue using
			//  the console) and show & focus the console window.
			Console.Title = original_console_title;
			ShowWindow(GetConsoleWindow(), SW_SHOW);
			FocusConsole(GetConsoleWindow());
		}

		#endregion

		#region Window

		protected void SetTitleFormat(string format)
		{
			title_format = format;
			UpdateTitle();
		}

		[SuppressMessage("ReSharper", "ConvertIfStatementToConditionalTernaryExpression")]
		private void UpdateTitle()
		{
			// Add game name
			string title = title_format.Replace("%name%", name);

			// Add game version
			title = title.Replace("%version", version);

			// Add cycles per second
			title = title.Replace("%cps%",
				(cf_per_second_override || window.UpdateFrequency > 999)
					? SpecialCharacters.TreeHorizontal.Times(target_cps > 99 ? 4 : 3) // If resizing, don't show potentially incorrect values
					: (target_cps > 99)                                               // Display cycles/sec with correct formatting based on number of significant figures of target cycle rate
						? $"{window.UpdateFrequency:000.0}"
						: $"{window.UpdateFrequency:00.0}");
			title = title.Replace("%tcps%",
				(cf_per_second_override || window.UpdateFrequency > 999)
					? SpecialCharacters.TreeHorizontal.Times(target_cps > 99 ? 4 : 3) // If resizing, don't show potentially incorrect values
					: (target_cps > 99)                                               // Display target cycles/sec with correct formatting based on number of significant figures of target cycle rate
						? $"{window.TargetUpdateFrequency:000.0}"
						: $"{window.TargetUpdateFrequency:00.0}");

			// Add frames per second
			title = title.Replace("%fps%",
				(cf_per_second_override || window.RenderFrequency > 999)
					? SpecialCharacters.TreeHorizontal.Times(target_fps > 99 ? 4 : 3) // If resizing, don't show potentially incorrect values
					: (target_fps > 99)                                               // Display frames/sec with correct formatting based on number of significant figures of target cycle rate
						? $"{window.RenderFrequency:000.0}"
						: $"{window.RenderFrequency:00.0}");
			title = title.Replace("%tfps%",
				(cf_per_second_override || window.RenderFrequency > 999)
					? SpecialCharacters.TreeHorizontal.Times(target_cps > 99 ? 4 : 3) // If resizing, don't show potentially incorrect values
					: (target_cps > 99)                                               // Display target frames/sec with correct formatting based on number of significant figures of target cycle rate
						? $"{window.TargetRenderFrequency:000.0}"
						: $"{window.TargetRenderFrequency:00.0}");

			// Add resolution
			title = title.Replace("%res%", $"{Width}{SpecialCharacters.Multiply}{Height}");

			// Update title

			window.Title = title;
		}

		#endregion

		#region Input

		private void UpdateInput()
		{
			// TODO(LOGIQ): Implement
		}

		#endregion

		private void DoCycle(double delta)
		{
			UpdateInput();

			Cycle(delta);

			if (!update_title_in_render)
				UpdateTitle();
		}

		private void DoRender(double delta)
		{
			// TODO(LOGIQ): Clear

			Render(delta);

			if (update_title_in_render)
				UpdateTitle();

			window.SwapBuffers();
		}

		#region API

		/// <summary>
		/// Called before Load(); use this to load any assets needed to display a loading screen and prepare your loading scene
		/// </summary>
		protected abstract void PreWarm();

		/// <summary>
		/// Called after PreWarm(); use this to load the bulk of your game's resources
		/// </summary>
		protected abstract void Load();

		/// <summary>
		/// Called once per update cycle, after input is updated; use this to update your game's logic
		/// </summary>
		/// <param name="delta">The time that has passed since the last update cycle</param>
		protected abstract void Cycle(double delta);

		/// <summary>
		/// Called once per render cycle, after the scene is cleared; use this to render your scene
		/// </summary>
		/// <param name="delta">The time that has passed since the last render cycle</param>
		protected abstract void Render(double delta);

		/// <summary>
		/// Called when the game shuts down (either the window is closed or the game encounters an exception and 'crashes'); use
		/// this to clean up any resources your game has
		/// </summary>
		/// <param name="crashed"></param>
		protected abstract void Shutdown(bool crashed);

		#endregion

		#region Shutdown

		public void Dispose()
		{
			if (has_disposed)
				return;

			Shutdown(has_crashed);
			has_disposed = true;
		}

		#endregion
	}
}
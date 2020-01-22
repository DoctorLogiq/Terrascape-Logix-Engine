using LogixEngine._Extensions;
using LogixEngine.Rendering;
using LogixEngine.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Console = Colorful.Console;
using OpenTK;
using OpenTK.Graphics;
using System.Diagnostics.CodeAnalysis;

namespace LogixEngine
{
	/* ----------------------------------------------------------------------------  *
     * Game.cs created by DrLogiq on 15-01-2020 13:01.
     * Copyright © DrLogiq. All rights reserved.
     * ----------------------------------------------------------------------------  */
	[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global")]
	public abstract class Game : IDisposable
	{
		public const string LogixEngineVersion = "1.0.0";

		#region Fields / Properties / Events

		/// <summary>
		/// Determines if this application was started by double-clicking the .exe, or run from a command line
		/// </summary>
		// NOTE(LOGIQ): Thanks to StackOverflow user 'Fowl' for this: https://stackoverflow.com/a/18307640/11878570
		private static readonly bool StartedFromGui = !Console.IsOutputRedirected
		                                              && !Console.IsInputRedirected
		                                              && !Console.IsErrorRedirected
		                                              && Environment.UserInteractive
		                                              && Environment.CurrentDirectory == System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location)
		                                              && Console.CursorTop == 0 && Console.CursorLeft == 0
		                                              && Console.Title == Environment.GetCommandLineArgs()[0]
		                                              && Environment.GetCommandLineArgs()[0] == System.Reflection.Assembly.GetEntryAssembly()?.Location;

		/// <summary>The game's display name</summary>
		private readonly string name;

		/// <summary>The game's version number</summary>
		private readonly string version; // TODO(LOGIQ): Make a version class to use here

		/// <summary>The game's target cycles (update ticks) per second</summary>
		private readonly int target_cps;

		/// <summary>Whether or not to update the window title in the render cycle. If false, update the title in the update cycle instead</summary>
		private readonly bool update_title_in_render;

		/// <summary>Whether or not to render while the game window is being resized</summary>
		private readonly bool render_while_resizing = true;

		/// <summary>The game's window</summary>
		private readonly GameWindow window;

		/// <summary>The game window's title format string. Variables in this string will be replaced when the title is updated</summary>
		private static string title_format;

		/// <summary>The minimum width (in pixels) the game window can have</summary>
		private static int minimum_width;

		/// <summary>The minimum height (in pixels) the game window can have</summary>
		private static int minimum_height;

		/// <summary>The width of the game window, in pixels (excluding window decorations, i.e. safe to use for rendering)</summary>
		public static double Width;

		/// <summary>The height of the game window, in pixels (excluding window decorations, i.e. safe to use for rendering)</summary>
		public static double Height;

		/// <summary>Half the width of the game window, in pixels. Useful for screen centering during rendering</summary>
		public static double HalfWidth;

		/// <summary>Half the height of the game window, in pixels. Useful for screen centering during rendering</summary>
		public static double HalfHeight;

		/// <summary>Whether or not we are in the Run() function. Used to determine if certain functions are being used too early</summary>
		internal static bool IsRunning;

		/// <summary>Whether or not the game is considered to be in a 'crashed' state (true if an unhandled exception has been caught)</summary>
		internal static bool HasCrashed;

		/// <summary>Whether or not the game has had its Dispose() and Shutdown() functions called</summary>
		internal static bool HasDisposed;

		protected static bool CanCancelWindowClose = true;

		/// <summary>Whether or not the game has a console</summary>
		internal static bool HasConsole; // TODO(LOGIQ): Is this still needed?

		/// <summary>The exception (if any) thrown during the constructor phase (or any point before Run() is called). Can (and should) be null</summary>
		internal static ApplicationException ConstructorException;

		/// <summary>Whether or not to override the cycles and frames per second displays in the window title (used when the game window is being resized)</summary>
		private static bool cf_per_second_override;

		/// <summary>The game's target render cycles per second</summary>
		private int target_fps;

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

		public delegate void BlankEventHandler();

		public delegate bool WindowCloseRequestedHandler();

		public event BlankEventHandler PreWarmStarted;
		public event BlankEventHandler PreWarmCompleted;
		public event BlankEventHandler LoadStarted;
		public event BlankEventHandler LoadCompleted;
		public event BlankEventHandler EnableOpenGLCapabilities;
		/// <summary>
		/// Handles an event where the window is wanting to close. The subscriber (which should only ever be the game) can return true
		/// to cancel the window close event, however, this is not recommended. Only intended to be used if you want to warn players
		/// about potential loss of data as a result of not having saved.
		/// </summary>
		public event WindowCloseRequestedHandler WindowCloseRequested;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructs your game. This constructor should only be called once, EVER! You MUST pass in the program arguments
		/// from your Main() entry-point function, so that the game can process the arguments the user wishes to use. Failure
		/// to do so will result in the game not performing as the user intends, and that's on you!
		/// </summary>
		/// <param name="name">The game's display name. Should start with a capital letter</param>
		/// <param name="version">The game's version</param>
		/// <param name="startup_width">The initial width of the game window</param>
		/// <param name="startup_height">The initial height of the game window</param>
		/// <param name="minimum_width">The minimum width the game window can have</param>
		/// <param name="minimum_height">The minimum height the game window can have</param>
		/// <param name="target_cps">The target cycle (update) rate (in Hz) to run the game at, this cannot be changed later</param>
		/// <param name="target_fps">The target render rate (in Hz) to start the game at, this can be changed later</param>
		/// <param name="arguments">MANDATORY - pass the unmodified program arguments from your Main() entry-point function here!</param>
		/// <param name="window_mode">The window's startup mode</param>
		/// <param name="gl_major">The OpenGL major version</param>
		/// <param name="gl_minor">The OpenGL minor version</param>
		/// <param name="update_title_in_render">Whether to update the title in the render cycle. If set to [false], the
		/// title will be updated in the update cycle instead</param>
		// TODO(LOGIQ): Implement a singleton design pattern here because you know programmers don't follow rules!
		protected Game(string name,                             string              version, int startup_width, int startup_height, int minimum_width, int minimum_height, int target_cps,
			int               target_fps,                       IEnumerable<string> arguments,
			WindowMode        window_mode = WindowMode.Default, int                 gl_major = 4, int gl_minor = 4, bool update_title_in_render = true)
		{
			// If started from windows explorer (double-clicking the .exe, usually), hide the console
			if (StartedFromGui)
				ShowWindow(GetConsoleWindow(), SW_HIDE);

			// Set variables
			this.name                   = name;
			this.version                = version;
			this.target_cps             = target_cps;
			this.target_fps             = target_fps;
			this.update_title_in_render = update_title_in_render;
			Game.minimum_width          = minimum_width;
			Game.minimum_height         = minimum_height;

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
				Debug.LogEngineMessage("Enabling OpenGL capabilities", true);
				EnableOpenGLCapabilities?.Invoke();
				
				Debug.LogEngineMessage("Invoking game pre-warm", true);
				PreWarmStarted?.Invoke();
				PreWarm();
				PreWarmCompleted?.Invoke();

				Debug.LogEngineMessage("Invoking game load", true);
				LoadStarted?.Invoke();
				Load();
				Debug.LogEngineMessage("Game load finished", true);
				LoadCompleted?.Invoke();
			};
			window.UpdateFrame += (sender, args) => DoCycle(args.Time);
			window.RenderFrame += (sender, args) => DoRender(args.Time);
			window.Resize += (sender, args) =>
			{
				if (window.Width < Game.minimum_width)
					window.Width = Game.minimum_width;
				if (window.Height < Game.minimum_height)
					window.Height = Game.minimum_height;

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
			window.Closing += (sender, args) =>
			{
				if (CanCancelWindowClose)
				{
					Debug.LogDebug("Window close requested");
					bool? cancel = WindowCloseRequested?.Invoke();
					if (cancel.HasValue && cancel.Value)
					{
						Debug.LogDebug("Window close request denied by the game", Color.FromArgb(233, 75, 60));
						args.Cancel = true;
					}
					else if (cancel.HasValue)
					{
						Debug.LogDebug("Window close request accepted by the game", Color.FromArgb(60, 233, 75));
					}
					else
					{
						Debug.LogDebug("Window close request accepted (event unhandled)");
					}
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
						if (StartedFromGui)
							ShowWindow(GetConsoleWindow(), SW_SHOW);
						HasConsole      = true;
						Debug.DebugMode = true;
					}
						break;
					case "-console":
					{
						if (StartedFromGui)
							ShowWindow(GetConsoleWindow(), SW_SHOW);
						HasConsole = true;
					}
						break;
					case "-drwr": // NOTE: Don't render while resizing
					{
						render_while_resizing = false;
					}
						break;
				}
			}
		}

		#endregion

		#region Run Function

		protected void Run()
		{
			IsRunning = true;

			#region Console Stuff

			// Store the console's original title so it can be reverted back later
			string original_console_title = Console.Title;
			// Set the console's new title
			Console.Title = $"{name} Debugger";
			// Print a header to help users understand the layout of the debug messages
			Console.WriteLine();
			/* DISABLED:
			Console.WriteLine("Line H  M  S   Ms    Message", Color.DimGray);
			string separator =      "────┼──┼──┼──┼┼───┼─┼─────────";*/
			string separator = "";
			for (int i = separator.Length; i < Console.BufferWidth - 1; ++i)
				separator += "─";
			Console.WriteLine(separator, Color.DimGray);

			#endregion

			#region RUN GAME

			// Attempt to run the game and catch unhandled exceptions
			try
			{
				// Make sure that first we check if an exception was thrown already from any pre-run phase
				if (HasCrashed && ConstructorException != null)
					throw ConstructorException;
				// If no exceptions have already been thrown, we're good to start!
				Debug.LogDebug("Starting game");
				Debug.LogEngineMessage("This game is using LogixEngine version " + LogixEngineVersion, false);
				Debug.LogDebugRegisteredTypes();
				// RUN THE GAME, WOO!
				window.Run(target_cps, target_fps);
				// This line shouldn't be hit until the window closes
				Debug.LogDebug("Stopping game");
			}
			catch (Exception exception)
			{
				// An exception has been caught, so treat the game as having 'crashed'.
				HasCrashed = true;
				
				CanCancelWindowClose = false;
				window.Close();
				window.ProcessEvents();

				if (Debug.DebugMode)
				{
					int max_message_characters_per_line = Console.BufferWidth - 22;
					
					// TODO(LOGIQ): Debug.ResetIndentation();
					Debug.LogCritical("An unhandled exception was caught which caused the game to crash (safely).");

					Exception exc = exception;
					while (exc != null)
					{
						// Print the exception type and message (if it has one)
						Debug.LogCriticalContinued($"Caused by an {exc.GetType().Name}" + (!string.IsNullOrEmpty(exc.Message) ? " with the message:" : ""));
						if (!string.IsNullOrEmpty(exc.Message))
						{
							if (exc.Message.Length > max_message_characters_per_line)
							{
								IEnumerable<string> lines = StringUtils.SplitToLines(exc.Message, max_message_characters_per_line);
								foreach (string line in lines)
								{
									Debug.LogErrorContinued(line);
								}
							}
							else
								Debug.LogErrorContinued(exc.Message);
						}

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

			#endregion

			#region Clean Up

			// Make sure the game has been shut down and unmanaged resources disposed of
			Dispose();

			// TODO(LOGIQ): Save log

			#endregion

			#region Hold Console

			// If the console is open, hold it open until the user confirms
			// TODO(LOGIQ): Should there be a switch for this?
			if (!StartedFromGui || HasConsole)
			{
				Console.Write("\tPRESS ANY KEY TO RETURN...", Debug.ColourEngine);
				Console.ReadKey();

				string end_separator = "────────────────────────────────────────────";
				for (int i = end_separator.Length; i < Console.BufferWidth - 1; ++i)
				{
					end_separator += "─";
				}

				Console.WriteLine(end_separator, Color.DimGray);

				Console.Title = original_console_title;
				FocusConsole(GetConsoleWindow());
			}

			#endregion
		}

		#endregion

		protected void SetTitleFormat(string format)
		{
			title_format = format;
			UpdateTitle();
		}

		private void UpdateTitle()
		{
			// Add game name
			string title = title_format.Replace("%name%", name);

			// Add game version
			title = title.Replace("%version%", version);

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

		private void UpdateInput()
		{
			// TODO(LOGIQ): Implement
		}

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

		public void Dispose()
		{
			if (HasDisposed)
				return;

			Debug.LogDebugTask("Deleting unmanaged resources", new Task(() =>
			{
				foreach (UnmanagedResource unmanaged_resource in UnmanagedResource.UnmanagedResources)
					unmanaged_resource.Cleanup();
			}));

			Shutdown(HasCrashed);
			HasDisposed = true;
		}

		protected static void RegisterType(string type)
		{
			Debug.RegisterType(type);
		}

		protected static void RegisterTypes(params string[] types) // TODO(LOGIQ): Should this not run in !DebugMode?
		{
			foreach (string type in types)
				Debug.RegisterType(type);
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
	}
}
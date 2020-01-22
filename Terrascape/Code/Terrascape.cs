#region Imports
using LogixEngine;
using LogixEngine.Registry;
using LogixEngine.Rendering.Shader;
using LogixEngine.Rendering.Texture;
using OpenTK.Graphics.ES11;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terrascape.Block;
using Debug = LogixEngine.Utility.Debug;
#endregion

namespace Terrascape
{
	/* ---------------------------------------------------------------------------- *
     * Terrascape.cs created by DrLogiq on 15-01-2020 13:28.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	internal sealed class Terrascape : Game
	{
		#region Startup Parameters
		
		private const int StartupWidth  = 1280;
		private const int StartupHeight = 720;
		private const int MinWidth      = 640;
		private const int MinHeight     = 480;
		private const int StartupCPS    = 30;
		private const int StartupFPS    = 60;
		
		#endregion
		#region Startup
		
		public static void Main(string[] args)
		{
			new Terrascape(args).Run();
		}
		
		public Terrascape(IEnumerable<string> args) : base("Terrascape", "1.0.0", StartupWidth, StartupHeight, MinWidth, MinHeight, StartupCPS, StartupFPS, args)
		{
			// Register custom types for syntax highlighting in the debugger
			RegisterTypes("Block", "Item", "Player", "Chunk");

			// REgister event handlers
			EnableOpenGLCapabilities += OnEnableOpenGLCapabilities;
			LoadCompleted            += OnLoadComplete;
		}
		
		#endregion
		#region Event Handlers

		private void OnEnableOpenGLCapabilities()
		{
			// Textures
			GL.Enable(EnableCap.Texture2D);
			// Alpha blending
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			// 3D
			GL.Enable(EnableCap.DepthTest);
			// Backface culling
			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);
			GL.FrontFace(FrontFaceDirection.Cw);
		}

		private void OnLoadComplete()
		{
			Debug.LogInfo("Terrascape is running");
			SwitchToMainMenu();
		}
		
		#endregion
		#region Load
		
		protected override void PreWarm()
		{
			Debug.LogDebug($"Profiler resolution: {Debug.GetProfilerResolution():n0} ticks/sec, High res: {(Debug.IsProfilerHighResolution() ? "Yes" : "No")}");
			TextureRegistry.CheckIn();
			ShaderRegistry.CheckIn();
			__BlockInformation.GetAllBlockInformation();
		}

		protected override void Load()
		{
			Debug.Profile("Loading assets", true, new Task(() =>
			{
				Debug.Profile("Loading textures", true, new Task(() =>
				{
					// Load textures
					TextureRegistry.Register(Texture.Load("loading_texture",   "textures/gui/loading.png"));
					TextureRegistry.Register(Texture.Load("block_placeholder", "textures/blocks/placeholder.png"));
				}));

				Debug.Profile("Loading shaders", true, new Task(() =>
				{
					// Load shaders
					ShaderRegistry.Register(Shader.Load("interface_shader", "shaders/ui_shader.vert", "shaders/ui_shader.frag"));
				}));

				/*Debug.Profile("Loading models", true, new Task(() =>
				{
					
				}));*/

				/*Debug.Profile("Loading sounds", true, new Task(() =>
				{
					
				}));*/

				/*Debug.Profile("Loading music", true, new Task(() =>
				{
					
				}));*/
			}));
		}
		
		#endregion

		private void SwitchToMainMenu()
		{
			foreach (Blocks block in Blocks.Air.GetAllBlocks())
			{
				Debug.LogDebug($"Block '{block.ToString()}' IsRendered: " + block.GetInfo().IsRendered);
			}
		}

		protected override void Cycle(double delta)
		{
		}

		protected override void Render(double delta)
		{
		}

		protected override void Shutdown(bool crashed)
		{
			Debug.LogDebug("Shutting down" + (crashed ? " because of a crash" : ""));
		}
	}
}
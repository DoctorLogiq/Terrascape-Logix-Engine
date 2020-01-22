using LogixEngine.IO;
using LogixEngine.Registry;
using LogixEngine.Utility;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Threading.Tasks;

namespace LogixEngine.Rendering.Shader
{
	/* ---------------------------------------------------------------------------- *
     * Shader.cs created by DrLogiq on 21-01-2020 15:46.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	public class Shader : UnmanagedResource
	{
		public sealed override identifier Identifier { get; }

		/// <summary>
		/// The OpenGL shader program ID
		/// </summary>
		protected sealed override int ObjectID { get; }

		private Shader(identifier id, int open_gl_id)
		{
			Identifier = id;
			ObjectID   = open_gl_id;
		}

		protected sealed override void Delete()
		{
			GL.DeleteProgram(ObjectID);
		}

		public static Shader Load(identifier id, string vertex_filename, string fragment_filename, bool suppress_extension_warning = false)
		{
			return Debug.Profile($"Loading shader '{id}'", true, new Task<Shader>(() =>
			{
				// Validate file extensions
				FileManager.VerifyExtension(ref vertex_filename,   suppress_extension_warning, "glsl", "vs", "vert", "shader");
				FileManager.VerifyExtension(ref fragment_filename, suppress_extension_warning, "glsl", "fs", "frag", "shader");
				// Load the vertex shader source
				string vertex_source = FileManager.ReadAssetToString(vertex_filename);
				if (vertex_source == null)
					throw new AssetLoadFailedException(vertex_filename);
				// Load the fragment shader source
				string fragment_source = FileManager.ReadAssetToString(fragment_filename);
				if (fragment_source == null)
					throw new AssetLoadFailedException(fragment_filename);
				// Compile the vertex shader
				int vertex_shader = GL.CreateShader(ShaderType.VertexShader);
				Debug.LogDebug($"Created temporary vertex shader {vertex_shader}");
				GL.ShaderSource(vertex_shader, vertex_source);
				GL.CompileShader(vertex_shader);
				string vertex_info_log = GL.GetShaderInfoLog(vertex_shader);
				if (!string.IsNullOrEmpty(vertex_info_log))
				{
					GL.DeleteShader(vertex_shader);
					Debug.LogDebug($"Deleted temporary vertex shader {vertex_shader}");
					throw new ShaderCompilationFailedException(id, ShaderType.VertexShader, vertex_info_log);
				}
				// Compile the fragment shader
				int fragment_shader = GL.CreateShader(ShaderType.FragmentShader);
				Debug.LogDebug($"Created temporary fragment shader {fragment_shader}");
				GL.ShaderSource(fragment_shader, fragment_source);
				GL.CompileShader(fragment_shader);
				string fragment_info_log = GL.GetShaderInfoLog(fragment_shader);
				if (!string.IsNullOrEmpty(fragment_info_log))
				{
					GL.DeleteShader(vertex_shader);
					Debug.LogDebug($"Deleted temporary vertex shader {vertex_shader}");
					GL.DeleteShader(fragment_shader);
					Debug.LogDebug($"Deleted temporary fragment shader {fragment_shader}");
					throw new ShaderCompilationFailedException(id, ShaderType.FragmentShader, fragment_info_log);
				}
				// Create the shader program
				int program = GL.CreateProgram();
				Debug.LogDebug($"Created shader program {fragment_shader}");
				// Bind shaders
				GL.AttachShader(program, vertex_shader);
				GL.AttachShader(program, fragment_shader);
				string program_info_log = GL.GetProgramInfoLog(program);
				if (!string.IsNullOrEmpty(program_info_log))
				{
					GL.DeleteShader(vertex_shader);
					Debug.LogDebug($"Deleted temporary vertex shader {vertex_shader}");
					GL.DeleteShader(fragment_shader);
					Debug.LogDebug($"Deleted temporary fragment shader {fragment_shader}");
					GL.DeleteProgram(program);
					Debug.LogDebug($"Deleted shader program {fragment_shader}");
					throw new ApplicationException($"Failed to attach shaders to the shader program for '{id}': {program_info_log}");
				}
				Debug.LogDebug("Bound shaders to shader program");
				// Link the shader program
				GL.LinkProgram(program);
				program_info_log = GL.GetProgramInfoLog(program);
				if (!string.IsNullOrEmpty(program_info_log))
				{
					GL.DeleteShader(vertex_shader);
					Debug.LogDebug($"Deleted temporary vertex shader {vertex_shader}");
					GL.DeleteShader(fragment_shader);
					Debug.LogDebug($"Deleted temporary fragment shader {fragment_shader}");
					GL.DeleteProgram(program);
					Debug.LogDebug($"Deleted shader program {fragment_shader}");
					throw new ApplicationException($"Failed to link shaders to the shader program for '{id}': {program_info_log}");
				}
				Debug.LogDebug($"Linked shader program {program}");
				// Cleanup
				GL.DetachShader(program, vertex_shader);
				GL.DeleteShader(vertex_shader);
				Debug.LogDebug($"Deleted temporary vertex shader {vertex_shader}");
				GL.DetachShader(program, fragment_shader);
				GL.DeleteShader(fragment_shader);
				Debug.LogDebug($"Deleted temporary fragment shader {fragment_shader}");
				// Create and return the shader
				return new Shader(id, program);
			})) as Shader;
		}
	}

	public sealed class ShaderCompilationFailedException : ApplicationException
	{
		public ShaderCompilationFailedException(identifier id, ShaderType type, string info_log)
			: base($"Failed to compile {type.ToString().ToLower()} shader '{id}': {info_log}")
		{
		}
	}
}
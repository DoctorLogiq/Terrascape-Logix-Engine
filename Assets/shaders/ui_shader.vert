#version 440 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 uvs;

uniform mat4 transformation_matrix;
uniform mat4 projection_matrix;

out vec2 pass_uvs;

void main(void)
{
    gl_Position = projection_matrix * transformation_matrix * vec4(position, 1.0);
    pass_uvs = uvs;
}
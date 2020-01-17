#version 440 core

in vec2 pass_uvs;

out vec4 OutColour;

uniform sampler2D texture_sampler;

void main(void)
{
    OutColour = texture(texture_sampler, pass_uvs);
}
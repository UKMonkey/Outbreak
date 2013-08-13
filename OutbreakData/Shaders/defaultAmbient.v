#version 120

uniform mat4 worldViewProjMat;

void main()
{
    gl_Position = worldViewProjMat * gl_Vertex;
    gl_FrontColor = gl_Color;
    gl_TexCoord[0] = gl_MultiTexCoord0;
}

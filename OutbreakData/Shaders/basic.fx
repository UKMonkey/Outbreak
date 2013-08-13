
float4x4 worldViewProjMat : WORLDVIEWPROJECTION;

struct VertexShaderInput
{
    float4 Position : POSITION;
    float4 Colour : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float4 Colour : COLOR0;
};

VertexShaderOutput Vertex_Shader(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, worldViewProjMat);
    output.Colour = input.Colour;

    return output;
}

float4 Pixel_Shader(VertexShaderOutput input) : COLOR0
{
    return input.Colour;
}

technique Basic
{
    pass P0
    {
        VertexShader = compile vs_2_0 Vertex_Shader();
        PixelShader = compile ps_2_0 Pixel_Shader();
    }
}

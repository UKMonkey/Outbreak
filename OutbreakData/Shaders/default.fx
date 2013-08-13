// transformations provided by the app:
float4x4 worldMat: WORLD;
float4x4 worldViewProjMat : WORLDVIEWPROJECTION;
float4 lightPos;
float lightIntensity;
float4 lightColour;
float4 ambient;
bool alphaBlend;

texture tex0;
sampler2D sampleTex0 = sampler_state
{
    texture = (tex0);
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

// the format of our vertex data
struct VS_OUTPUT
{
    float4 Pos  : POSITION;
    float2 Tex  : TEXCOORD0;
	float3 Coord : TEXCOORD1;
    float3 Normal : TEXCOORD2;
	float4 Diff : COLOR0;

};

VS_OUTPUT VSHADER(
	float4 Pos : POSITION, float4 Tex0 : TEXCOORD0,
	float4 Diff : COLOR0, float3 normal : NORMAL0)
{
    VS_OUTPUT Out = (VS_OUTPUT)0;

    float4x4 WVP = worldViewProjMat;

    Out.Pos = mul(Pos, WVP);
    Out.Tex = Tex0;
	Out.Coord = mul(Pos, worldMat);
	Out.Normal = normalize(mul(normal, (float3x3)worldMat));
    return Out;
}

float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);
}

float4 PSHADER(
	float4 tex : TEXCOORD0, float4 diff : COLOR0,
	float4 pos : TEXCOORD1, float3 normal : TEXCOORD2) : COLOR0
{

    float lightFactor = DotProduct(lightPos, pos, normal);
    lightFactor = saturate(lightFactor);

    float dist = length(pos - lightPos);
    float intensity = 1 / dist;

    lightFactor *= intensity;

    float4 useTex = tex2D(sampleTex0, tex);
    return (useTex * lightFactor) + (lightFactor * lightColour) + ambient;
}

technique TVertexAndPixelShader
{
    pass
    {
        ZEnable = true;
        ZWriteEnable = true;
        AlphaBlendEnable = alphaBlend;
        //AlphaBlendEnable = true;
        SrcBlend = One;
        DestBlend = One;

        VertexShader = compile vs_2_0 VSHADER();
        PixelShader = compile ps_2_0 PSHADER();
    }
}


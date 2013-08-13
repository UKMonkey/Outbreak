// from http://www.gamedev.net/page/resources/_/technical/directx-and-xna/cubic-shadow-mapping-in-direct3d-r2457
// also read: http://www.gamedev.net/topic/603903-dx9-cube-shadow-maps/ FOR LHMATRIX CONSTRUCTION
// http://http.developer.nvidia.com/GPUGems/gpugems_ch12.html

float4x4 worldMat;
float4x4 worldViewProjMat;
textureCUBE cubeShadowMap;
texture2D tex0;
float4 lightDiffuse;
bool z_enable;
bool z_write;
bool alphaBlend;
float depthBias;
float lightIntensity;
float3 lightPosition;
float4 eyePosition; //the origin of the camera or eye
float zOffset;
float geomScale;
float cutoff;

samplerCUBE cubeShadowMapSampler = sampler_state
{
	Texture = <cubeShadowMap>;
    MipFilter = NONE;
    MinFilter = NONE;
    MagFilter = NONE;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D sampler0 = sampler_state
{
    Texture = <tex0>;
    MipFilter = NONE;
    MinFilter = NONE;
    MagFilter = NONE;
    AddressU = wrap;
    AddressV = wrap;
};

struct lightFuncOutput
{
    float4 diffuseResult;
    float4 specularResult;
};

float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);
}

lightFuncOutput LightPointSH(float3 inObjPos,
						     float3 inNormal,
						     float3 inCam2Vertex)
{
    lightFuncOutput output;
    output.diffuseResult = float4(0.0f, 0.0f, 0.0f, 0.0f);
    output.specularResult = float4(0.0f, 0.0f, 0.0f, 0.0f);

    float3 vLight = (lightPosition.xyz - inObjPos) * geomScale;

    //float fDepth = dot(vLight.xyz, vLight.xyz) * depthBias; // OPT
    float fDepth = length(vLight.xyz) * depthBias;

    float shadowMapDepth = texCUBE(cubeShadowMapSampler, float4(-(vLight.xyz), 0.0f)).x;

    //if (fDepth - shadowMapDepth <= cutoff)
    if ((fDepth - zOffset) < shadowMapDepth)
    {
        return output;
    }
    else
    {
        //the pixel is not in shadow, so compute diffuse and specular and return
        float lightFactor = DotProduct(lightPosition, inObjPos, inNormal);
        lightFactor = saturate(lightFactor);
        float dist = length(lightPosition - inObjPos);
        float intensity = (1 / dist) * lightIntensity;

        lightFactor *= intensity;
        output.diffuseResult = lightDiffuse * lightFactor;

        return output;
    }
}


struct VS_OUTPUT
{
	float4 position  :  POSITION;
    float4 texCoord0 :  TEXCOORD0;
    float3 normalW   :  TEXCOORD1;
    float3 cam2Vert  :  TEXCOORD2;
	float3 worldPos  :  TEXCOORD3;
};

struct VS_OUTPUT_DEPTH
{
    float4 oPositionLight : POSITION;
    float lightDepth       : TEXCOORD3;
};

VS_OUTPUT_DEPTH depthMap_VS( float4 inPosition : POSITION )
{
    VS_OUTPUT_DEPTH output;

    float4 positionW = mul( inPosition, worldMat );
    output.oPositionLight = mul( inPosition, worldViewProjMat );

    //output.lightDepth = lightPosition - positionW.xyz;
    output.lightDepth = 1-(output.oPositionLight.z/output.oPositionLight.w);

    return output;
}

VS_OUTPUT cubicShadowMapping_VS(float4 inPosition  : POSITION,
                                float3 inNormal    : NORMAL,
                                float4 texCoord0   : TEXCOORD0)

{
    VS_OUTPUT output;

	float4 positionW = mul(inPosition, worldMat) * geomScale;

    output.texCoord0 = texCoord0;
    output.cam2Vert = (eyePosition - positionW).xyz;
    output.position = mul(inPosition, worldViewProjMat);
    output.worldPos = positionW.xyz;
    output.normalW = mul(inNormal, worldMat).xyz;

    return output;
}

float4 ambient_VS(float4 inPosition : POSITION) : POSITION
{
	return mul(inPosition, worldViewProjMat);
}

float4 depthMap_PS( VS_OUTPUT_DEPTH In ) : COLOR0
{
    return float4(In.lightDepth.x, 0, 0, 1);
}


float4 cubicShadowMapping_PS(VS_OUTPUT In) : COLOR0
{
    lightFuncOutput lightResult;

    float3 normal = normalize(In.normalW);
    float3 cam2Vert = normalize(In.cam2Vert);

    lightResult = LightPointSH(In.worldPos, normal, cam2Vert);

    return lightResult.diffuseResult * tex2D(sampler0, In.texCoord0);
}

float4 ambient_PS(float4 posWVP : POSITION) : COLOR0
{
	return float4(0.1f, 0.1f, 0.1f, 0.1f);
}


technique depthMap
{
    pass P0
    {
        ZWriteEnable = true;
        ZEnable = false;
        AlphaBlendEnable = true;

        VertexShader = compile vs_2_0 depthMap_VS( );
        PixelShader  = compile ps_2_0 depthMap_PS( );
    }
}

technique cubicShadowMapping
{
    pass P0
    {
        ZEnable = true;
        ZWriteEnable = false;
        AlphaBlendEnable = true;
        SrcBlend = One;
        DestBlend = One;

        VertexShader = compile vs_2_0 cubicShadowMapping_VS( );
        PixelShader  = compile ps_2_0 cubicShadowMapping_PS( );
    }
}


technique ambient
{
    pass P0
    {
        ZEnable = true;
        AlphaBlendEnable = false;
        ZWriteEnable = true;
        SrcBlend = One;
        DestBlend = Zero;

        VertexShader = compile vs_2_0 ambient_VS( );
        PixelShader  = compile ps_2_0 ambient_PS( );
    }
}
//------------------------------------------------------------------------------
// File: BasicRender.fx
//
// Portions - Copyright (c) Microsoft Corporation. All rights reserved.
//------------------------------------------------------------------------------

static const float PI = 3.14159265f;
static const float TwoPI = PI * 2;

float3 lightPosition;					// Position of light
float lightIntensity;                   // Light falloff
float3 lightWorldPosition;					// Position of light in world
float4 lightDiffuse;				// Light's diffuse color
float4 ambientColour;
texture tex0;              // Color texture for mesh
texture cubeShadowMap;			// Shadow map texture for lighting

float4x4 worldMat;                  // World matrix for object
float4x4 worldViewProjMat;

float viewingAngle;  //direction the person is facing
float viewDistance;  //how far the person can see
float viewAngle;  //what range they can see the above distance
float minViewRange; //minimum distance they can see

//------------------------------------------------------------------------------
// Texture samplers
//------------------------------------------------------------------------------
sampler sampler0 =
sampler_state
{
    Texture = <tex0>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
};
sampler cubeShadowMapSampler =
sampler_state
{
	Texture = <cubeShadowMap>;
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

//------------------------------------------------------------------------------
// Vertex shader output structure
//------------------------------------------------------------------------------
struct VS_OUTPUT
{
    float4 Position   : POSITION0;   // vertex position
    float2 TextureUV  : TEXCOORD0;  // vertex texture coords
    float3 wNormal	  : TEXCOORD1;
    float4 vPos       : TEXCOORD2;
    float3 wPos       : TEXCOORD3;
    float4 Colour     : COLOR0;
};

struct PS_INPUT
{
    float2 TextureUV  : TEXCOORD0;  // vertex texture coords
    float3 wNormal	  : TEXCOORD1;
    float4 vPos       : TEXCOORD2;
    float3 wPos       : TEXCOORD3;
    float4 Colour     : COLOR0;
};

struct VS_SHADOW_OUTPUT
{
	float4 Position : POSITION;
	float3 WorldPosition : TEXCOORD0;
};

VS_SHADOW_OUTPUT RenderShadowMapVS(float4 vPos: POSITION)
{
	VS_SHADOW_OUTPUT Out;

    Out.Position = mul(vPos, worldViewProjMat);
    Out.WorldPosition = mul(vPos, worldMat);

	return Out;
}

float4 RenderShadowMapPS( VS_SHADOW_OUTPUT In ) : COLOR0
{
    float3 vec = lightPosition - In.WorldPosition;
    float distance = dot(vec, vec);
    return distance;
}

struct PS_OUTPUT
{
    float4 RGBColor : COLOR0;  // Pixel color
};

VS_OUTPUT RenderShadowsVS(
     float3 position : POSITION,
     float3 normal : NORMAL,
     float2 TexCoord0 : TEXCOORD0,
     float4 colour : COLOR0)
{
    VS_OUTPUT Output;

    //transform the input position to the output
    Output.Position = mul(float4(position, 1.0), worldViewProjMat);

    //transform the normal to world space
    Output.wNormal =  mul(normal, worldMat);

    //do not transform the position needed for the
    //shadow map determination
    Output.vPos = float4(position,1.0);

    Output.wPos = mul(float4(position, 1.0), worldMat).xyz;

    //pass the texture coordinate as-is
    Output.TextureUV = TexCoord0;

    Output.Colour = colour;

    //return the output structure
    return Output;
}

PS_OUTPUT RenderShadowsPS( PS_INPUT In )
{
    PS_OUTPUT Output;

    float4 vTotalLightDiffuse = float4(0,0,0,0);
    float3 lightVecW = normalize(lightPosition - In.wPos);

    float diffuseLightIntensity = dot(In.wNormal, lightVecW);
    vTotalLightDiffuse += lightDiffuse * max(0, diffuseLightIntensity);

    float4 vPos = mul(In.vPos, worldMat);

    float shadowdepth = texCUBE(cubeShadowMapSampler, -lightVecW.xyz).x;

    float3 vec = vPos.xyz - lightPosition;
    float pixelDepth = dot(vec, vec);

    if (shadowdepth < (pixelDepth * 0.919)) // decimal value is the bias.
	{
	    // we're in shadow, cut the light
        vTotalLightDiffuse = float4(0,0,0,1);
	}

    float lightFalloff = (1 / pixelDepth) * (lightIntensity);
    Output.RGBColor = tex2D(sampler0, In.TextureUV) * In.Colour * vTotalLightDiffuse * lightFalloff;

    return Output;
}

PS_OUTPUT DiffuseOnlyPS(VS_OUTPUT In)
{
	 PS_OUTPUT Output;
     //calculate per-pixel diffuse
     float3 directionToLight = normalize(lightPosition - In.vPos);
     float diffuseIntensity = saturate( dot(directionToLight, In.wNormal));
     float4 diffuse = lightDiffuse * diffuseIntensity;

     float4 color = diffuse;
     color.a = 1.0;

     float4 rgba = tex2D(sampler0, In.TextureUV) * color;

     Output.RGBColor = rgba;

     return Output;
}

PS_OUTPUT TextureOnlyPS(float2 TextureUV  : TEXCOORD0)
{
     PS_OUTPUT Output;
     Output.RGBColor = tex2D(sampler0, TextureUV);

     return Output;
}

struct AmbientVSOutput
{
    float4 Pos  : POSITION;
    float2 Tex  : TEXCOORD0;
    float4 Colour : COLOR0;
    float3 wPos : TEXCOORD1;
};

AmbientVSOutput ambient_VS(
    float4 inPosition : POSITION,
    float4 inColour : COLOR0,
    float2 tex : TEXCOORD0)
{
    AmbientVSOutput output;

    output.Pos = mul(inPosition, worldViewProjMat);
    output.Tex = tex;
    output.Colour = inColour;
    output.wPos = mul(inPosition, worldMat).xyz;

    return output;
}

float4 ambient_PS(AmbientVSOutput input) : COLOR0
{
    float4 rgba = ambientColour * (input.Colour * tex2D(sampler0, input.Tex));
	float inputLength = length(input.wPos);
	float len = inputLength / minViewRange;

	float minValModifier;
	if (len < 1)
		minValModifier = 1;
	else if (len > 10)
		minValModifier = 0.1;
	else
		minValModifier = clamp(1-log10(len), 0.1, 1);

	len = inputLength / viewDistance;
	float maxValModifier;
	if (len < 1)
		maxValModifier = 1;
	else if (len > 10)
		maxValModifier = 0.1;
	else
		maxValModifier = clamp(1-log10(len), 0.1, 1);

	float angleModifier;
	float angle = atan2(input.wPos.y, input.wPos.x);
	if (angle < 0)
		angle = angle + TwoPI;

	float angleDif = abs(viewingAngle - angle);

	if (angleDif < viewAngle || angleDif > TwoPI-viewAngle)
		angleModifier = 1;
	else
		angleModifier = 0;

	float fullModifier = angleModifier * maxValModifier;
	float modifier = max(fullModifier, minValModifier);

	rgba *= modifier;
	return rgba;
}

technique cubicShadowMapping
{
	pass P0
	{
        ZEnable = true;
        ZWriteEnable = true;
        AlphaBlendEnable = true;
        SrcBlend = One;
        DestBlend = One;

        VertexShader = compile vs_2_0 RenderShadowsVS();
        PixelShader  = compile ps_2_0 RenderShadowsPS();
	}
}
technique depthMap
{
	pass P0
	{
		CullMode = NONE;
        ZEnable = true;
		ZWriteEnable = true;
        AlphaBlendEnable = false;
        SrcBlend = One;
        DestBlend = One;

        VertexShader = compile vs_2_0 RenderShadowMapVS();
        PixelShader  = compile ps_2_0 RenderShadowMapPS();

	}
}

technique ambient
{
    pass P0
    {
        CullMode = CCW;
        ZEnable = true;
        AlphaBlendEnable = false;
        ZWriteEnable = true;
        SrcBlend = One;
        DestBlend = Zero;

        VertexShader = compile vs_2_0 ambient_VS( );
        PixelShader  = compile ps_2_0 ambient_PS( );
    }
}
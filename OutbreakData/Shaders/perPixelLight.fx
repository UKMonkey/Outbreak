float4x4 world : World;
float4x4 view : View;
float4x4 projection : Projection;
float3 cameraPosition : WorldViewProjection;

//light properties
const float3 lightPosition0;
const float3 lightPosition1;
float4 ambientLightColor;
float4 diffuseLightColor;
float4 specularLightColor;

//material properties
float specularPower;
float specularIntensity;

struct VertexShaderOutputPerPixelDiffuse
{
     float4 Position : POSITION;
     float3 WorldNormal : TEXCOORD0;
     float3 WorldPosition : TEXCOORD1;
};

struct PixelShaderInputPerPixelDiffuse
{
     float3 WorldNormal : TEXCOORD0;
     float3 WorldPosition : TEXCOORD1;
};

VertexShaderOutputPerPixelDiffuse PerPixelDiffuseVS(
     float3 position : POSITION,
     float3 normal : NORMAL )
{
     VertexShaderOutputPerPixelDiffuse output;

     //generate the world-view-projection matrix
     float4x4 wvp = mul(mul(world, view), projection);

     //transform the input position to the output
     output.Position = mul(float4(position, 1.0), wvp);

     output.WorldNormal =  mul(normal, world);
     float4 worldPosition =  mul(float4(position, 1.0), world);
     output.WorldPosition = worldPosition / worldPosition.w;

     //return the output structure
     return output;
}

float4 DiffuseOnlyPS(PixelShaderInputPerPixelDiffuse input) : COLOR
{
     //calculate per-pixel diffuse
     float3 directionToLight = normalize(lightPosition0 - input.WorldPosition);
     float diffuseIntensity = saturate( dot(directionToLight, input.WorldNormal));
     float4 diffuse = diffuseLightColor * diffuseIntensity;

     float4 color = diffuse + ambientLightColor;
     color.a = 1.0;

     return color;
}

technique PerPixelDiffuse
{

    pass P0
    {
        ZEnable = true;

        // ZWrite is performed by a basic shader.
        ZWriteEnable = false;
        AlphaBlendEnable = true;
        SrcBlend = One;
        DestBlend = One;

        VertexShader = compile vs_2_0 PerPixelDiffuseVS();
        PixelShader = compile ps_2_0 DiffuseOnlyPS();
    }

}

//float4x4 World;
//float4x4 View;
//float4x4 Projection;

float4x4 mWorldViewProj;
float time;
float3 pos;

sampler2D tex1;

// TODO: add effect parameters here.

//struct VertexShaderInput
//{
//    float4 Position : POSITION0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
//};

//struct VertexShaderOutput
//{
//    float4 Position : POSITION0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
//};

void VertexShaderFunction(in float4 inPos : POSITION0, in float2 inTexCoords : TEXCOORD0,
						  out float4 outPos : POSITION0, out float2 outTexCoords : TEXCOORD0)
{
    outPos = mul(inPos, mWorldViewProj);
	outTexCoords.x = inTexCoords.x*cos(time * 0.05) - inTexCoords.y*sin(time * 0.05);
	outTexCoords.y = inTexCoords.x*sin(time * 0.05) + inTexCoords.y*cos(time * 0.05);
}

float4 PixelShaderFunction(in float2 texCoords : TEXCOORD0) : COLOR0
{
	float4 output = tex2D(tex1, texCoords);
    return output;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

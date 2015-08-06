float4x4 WorldViewProj;
float3 position;

Texture tex;
sampler LaserTexture = sampler_state 
	{ 
		texture = <tex>; 
		magfilter = LINEAR; 
		minfilter = LINEAR; 
		mipfilter = LINEAR; 
		AddressU = mirror; 
		AddressV = mirror;
	};

float time;

struct VertexShaderOutput
{
    float4 Position : POSITION;
	float2 TexCoords : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(float4 Position : POSITION, float2 TexCoords : TEXCOORD0)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = mul(Position + position, WorldViewProj);
	output.TexCoords = TexCoords;
	output.TexCoords.x += 0.1*time;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 texColor = tex2D(LaserTexture, input.TexCoords);
	return texColor;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

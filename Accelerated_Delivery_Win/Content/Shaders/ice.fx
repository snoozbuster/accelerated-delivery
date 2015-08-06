float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;
float4x4 xReflectionView;

float3 xCamPos;
float3 xLightDirection;
float xWaveLength;
float3 xNormal;

bool xEnableReflections;

Texture xReflectionMap;
sampler ReflectionSampler = sampler_state { texture = <xReflectionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

//Texture xRefractionMap;
//sampler RefractionSampler = sampler_state { texture = <xReflectionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xIceBumpMap;
sampler IceBumpMapSampler = sampler_state { texture = <xIceBumpMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

struct VertexShaderOutput
{
    float4 Position                    :  POSITION;
    float4 ReflectionMapSamplingPos    : TEXCOORD1;
	float4 RefractionMapSamplingPos    : TEXCOORD4;
    float4 Position3D                  : TEXCOORD2;
	float2 BumpMapSamplingPos		   : TEXCOORD3;
};

VertexShaderOutput VertexShaderFunction(float4 inPos : POSITION, float2 inTex: TEXCOORD)
{
    VertexShaderOutput Output = (VertexShaderOutput)0;

    float4x4 preViewProjection = mul (xView, xProjection);
    float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    float4x4 preReflectionViewProjection = mul (xReflectionView, xProjection);
    float4x4 preWorldReflectionViewProjection = mul (xWorld, preReflectionViewProjection);

    Output.Position = mul(inPos, preWorldViewProjection);
    Output.ReflectionMapSamplingPos = mul(inPos, preWorldReflectionViewProjection);
	Output.Position3D = mul(inPos, xWorld);
	Output.BumpMapSamplingPos = inTex/xWaveLength;	

    return Output;
}

float4 PixelShaderFunction(VertexShaderOutput PSIn) : COLOR0
{
	float4 output = float4(0,0,0,0);
	
	float2 ProjectedTexCoords = (float2)0;
	ProjectedTexCoords.x = PSIn.ReflectionMapSamplingPos.x/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f;
	ProjectedTexCoords.y = -PSIn.ReflectionMapSamplingPos.y/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f; 

	float4 bumpColor = tex2D(IceBumpMapSampler, PSIn.BumpMapSamplingPos);
	float2 perturbation = (bumpColor.rg - 0.5f)*2.0f;
	float2 perturbatedTexCoords = (float2)0;
	perturbatedTexCoords = ProjectedTexCoords + perturbation;

	float4 reflectiveColor = (float4)0;
	if(xEnableReflections)
		reflectiveColor = tex2D(ReflectionSampler, perturbatedTexCoords);

	float3 eyeVector = normalize(xCamPos - PSIn.Position3D);
	float3 normalVector = (bumpColor.rbg-0.5f)*2.0f;
	float temp = normalVector.g;
	normalVector.g = normalVector.b;
	normalVector.b = temp;

	float fresnelTerm = dot(eyeVector, normalVector);

	//float4 combinedColor = lerp(reflectiveColor, refractiveColor, fresnelTerm + 0.1f);

	//float4 naturalColor = tex2D(TextureSampler, PSIn.BumpMapSamplingPos);
	float4 naturalColor = tex2D(TextureSampler, perturbatedTexCoords);

	//output = lerp(combinedColor, naturalColor, 0.5f);	
	//output = reflectiveColor;
	if(xEnableReflections)
		output = lerp(reflectiveColor, naturalColor, fresnelTerm);	
	else
		output = naturalColor;

	float3 reflectionVector = -reflect(xLightDirection, normalVector);
	float specular = dot(normalize(reflectionVector), normalize(eyeVector));
	specular = pow(specular, 256);        
	output.rgb += specular;

    return output;
}

technique Ice
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

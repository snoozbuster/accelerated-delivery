float4x4 xReflectionView;
float4x4 xView;
float4x4 xWorld;
float4x4 xProjection;

float xWaveLength;
float xWaveHeight;
float3 xCamPos;
float xTime;
float3 xWindDirection;
float xWindForce;
float3 xLightDirection;

bool xEnableReflections;

Texture xColorMap;
sampler ColorSampler = sampler_state { texture = <xColorMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xReflectionMap;
sampler ReflectionSampler = sampler_state { texture = <xReflectionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xRefractionMap;
sampler RefractionSampler = sampler_state { texture = <xRefractionMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture xWaterBumpMap;
sampler WaterBumpMapSampler = sampler_state { texture = <xWaterBumpMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

//------- Technique: Water --------
struct WVertexToPixel
{
    float4 Position                    :  POSITION;
    float4 ReflectionMapSamplingPos    : TEXCOORD1;
    float2 BumpMapSamplingPos          : TEXCOORD2;
    float4 RefractionMapSamplingPos    : TEXCOORD3;
    float4 Position3D                  : TEXCOORD4;
};

struct WPixelToFrame
{
    float4 Color : COLOR0;
};

WVertexToPixel WaterVS(float4 inPos : POSITION, float2 inTex: TEXCOORD)
{    
    WVertexToPixel Output = (WVertexToPixel)0;

    float4x4 preViewProjection = mul (xView, xProjection);
    float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    float4x4 preReflectionViewProjection = mul (xReflectionView, xProjection);
    float4x4 preWorldReflectionViewProjection = mul (xWorld, preReflectionViewProjection);

    Output.Position = mul(inPos, preWorldViewProjection);
    Output.ReflectionMapSamplingPos = mul(inPos, preWorldReflectionViewProjection);
	Output.RefractionMapSamplingPos = mul(inPos, preWorldViewProjection);
	Output.Position3D = mul(inPos, xWorld);

	float3 windDir = normalize(xWindDirection);    
	float3 perpDir = cross(xWindDirection, float3(0,0,1));
	float ydot = dot(inTex, xWindDirection.xy);
	float xdot = dot(inTex, perpDir.xy);
	float2 moveVector = float2(xdot, ydot);
	moveVector.y += xTime*xWindForce;    
	Output.BumpMapSamplingPos = moveVector/xWaveLength;	

    return Output;
}

WPixelToFrame WaterPS(WVertexToPixel PSIn)
{
    WPixelToFrame Output = (WPixelToFrame)0;        
    
	float2 ProjectedTexCoords;
    ProjectedTexCoords.x = PSIn.ReflectionMapSamplingPos.x/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f;
	ProjectedTexCoords.y = -PSIn.ReflectionMapSamplingPos.y/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f;    

	float4 bumpColor = tex2D(WaterBumpMapSampler, PSIn.BumpMapSamplingPos);
	float2 perturbation = xWaveHeight*(bumpColor.rg - 0.5f)*2.0f;
	float2 perturbatedTexCoords = ProjectedTexCoords + perturbation;

	float4 reflectiveColor = (float4)0;
	if(xEnableReflections)
		reflectiveColor = tex2D(ReflectionSampler, perturbatedTexCoords);
	
	float4 refractiveColor = (float4)0;
	if(xEnableReflections)
	{
		float2 ProjectedRefrTexCoords;
		ProjectedRefrTexCoords.x = PSIn.RefractionMapSamplingPos.x/PSIn.RefractionMapSamplingPos.w/2.0f + 0.5f;
		ProjectedRefrTexCoords.y = -PSIn.RefractionMapSamplingPos.y/PSIn.RefractionMapSamplingPos.w/2.0f + 0.5f;    
		float2 perturbatedRefrTexCoords = ProjectedRefrTexCoords + perturbation;    
		refractiveColor = tex2D(RefractionSampler, perturbatedRefrTexCoords);
		if(refractiveColor.g == 0 && refractiveColor.b == 0 && refractiveColor.r == 0)
		{
			refractiveColor = float4(0.957f, 0.643f, 0.376f, 1);
		}
	}

	float3 eyeVector = normalize(xCamPos - PSIn.Position3D);
	
	float3 normalVector = (bumpColor.rbg-0.5f)*2.0f;
	float temp = normalVector.g;
	normalVector.g = normalVector.b;
	normalVector.b = temp;
	float fresnelTerm = dot(eyeVector, normalVector);
	float4 dullColor = float4(0.1f, 0.2f, 0.55f, 1);

	if(xEnableReflections)
	{
		//Output.Color = lerp(refractiveColor, dullColor, 0.2f);
		float4 combinedColor = lerp(reflectiveColor, refractiveColor, fresnelTerm);
		Output.Color = lerp(combinedColor, dullColor, 0.2f);
	}
	else
	{
		float4 texColor = tex2D(ColorSampler, perturbatedTexCoords);
		Output.Color = lerp(dullColor, texColor, fresnelTerm);
	}
	
	float3 reflectionVector = -reflect(xLightDirection, normalVector);
	float specular = dot(normalize(reflectionVector), normalize(eyeVector));
	specular = pow(specular, 256);        
	Output.Color.rgb += specular;
	//Output.Color = tex2D(ReflectionSampler, ProjectedTexCoords);
    return Output;
}

technique Water
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 WaterVS();
        PixelShader = compile ps_2_0 WaterPS();
    }
}
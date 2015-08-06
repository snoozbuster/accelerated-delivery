float4x4 mWorldViewProj;
float time; 
float3 pos;

struct VS_OUTPUT 
{ 
	float4 Position : POSITION; // vertex position 
	float2 TexCoord : TEXCOORD0; // tex coords 
}; 

struct VS_INPUT
{
	float4 vPosition : POSITION; 
	float2 texCoord : TEXCOORD0;
};


VS_OUTPUT vertexMain( VS_INPUT input ) 
{ 
	VS_OUTPUT Output; 
	
	// transform position to clip space 
	Output.Position = mul(input.vPosition + float4(pos, 0), mWorldViewProj); 
	
	Output.TexCoord = input.texCoord; 
	
	return Output; 
} 

sampler2D tex0; 
sampler2D tex1; 

float4 pixelMain( VS_OUTPUT input ) : COLOR0
{ 
	float4 noise = tex2D( tex1, input.TexCoord ); // sample color map 

	float2 T1 = input.TexCoord + float2(1.5,-1.5)*time*0.02; 
	float2 T2 = input.TexCoord + float2(-0.5,2.0)*time*0.01; 
	T1.x += (noise.x)*2.0; 
	T1.y += (noise.y)*2.0; 
	T2.x += (noise.y)*0.2; 
	T2.y += (noise.z)*0.2; 
	
	float p = tex2D( tex1, T1*2.0).a; 
	
	float4 col = tex2D( tex0, T2*2.0 ); 
	float4 temp = col*(float4(p,p,p,p)*2.0)+(col*col-0.1); 
	if(temp.r > 1.0 ) { temp.bg += clamp(temp.r-2.0,0.0,100.0); } 
	if(temp.g > 1.0 ) { temp.rb += temp.g-1.0; } 
	if(temp.b > 1.0 ) { temp.rg += temp.b-1.0; } 

	temp.a = col.a;
	
	return temp; 
} 

technique Lava
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 vertexMain();
		PixelShader = compile ps_2_0 pixelMain();
	}
}
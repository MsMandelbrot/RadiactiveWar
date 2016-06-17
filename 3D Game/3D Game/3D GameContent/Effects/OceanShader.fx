// XNA 4.0 Shader Programming #4 - Normal Mapping

// Matrix
float4x4 World;
float4x4 View;
float4x4 Projection;

// Light related
//float4 AmbientColor;
//float AmbientIntensity;

//float3 LightDirection;
//float4 DiffuseColor;
//float DiffuseIntensity;

//float4 SpecularColor;
float3 EyePos;

float TotalTime=0;

float textureLerp;

texture2D ColorMap;
sampler2D ColorMapSampler = sampler_state
{
	Texture = <ColorMap>;
	MinFilter = linear;
	MagFilter = linear;
	MipFilter = linear;
};

texture2D NormalMap;
sampler2D NormalMapSampler = sampler_state
{
	Texture = <NormalMap>;
	MinFilter = linear;
	MagFilter = linear;
	MipFilter = linear;
};

textureCUBE cubeTex;
samplerCUBE CubeTextureSampler = sampler_state
{
    Texture = <cubeTex>;
    MinFilter = linear;
	MagFilter = linear;
	MipFilter = linear;
   
};
// The input for the VertexShader
struct VertexShaderInput
{
    float3 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
	float3 normal : NORMAL0;
	//float3 Binormal : BINORMAL0;
	//float3 Tangent : TANGENT0;
};

// The output from the vertex shader, used for later processing
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
	//float3 View : TEXCOORD1;
	float3 worldPos			   : TEXCOORD1;
	//float3x3 WorldToTangentSpace : TEXCOORD2;
};

// The VertexShader.
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(float4(input.Position,1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    output.texCoord = input.texCoord*100;
    output.worldPos = worldPosition.xyz;

    return output;
}

// The Pixel Shader
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 diffuseColor = float4(0,0,1,1);
    
    float4 normalTexture1 = tex2D(NormalMapSampler, input.texCoord*0.1+float2(TotalTime,TotalTime));
    float4 normalTexture2 = tex2D(ColorMapSampler, input.texCoord*0.1+float2(TotalTime,TotalTime));
    float4 normalTexture = (textureLerp*normalTexture1)+((1-textureLerp)*normalTexture2);
    float4 normalTexture3 = tex2D(NormalMapSampler, input.texCoord*2+float2(-TotalTime,-TotalTime*2));
    float4 normalTexture4 = tex2D(ColorMapSampler, input.texCoord*2+float2(-TotalTime,-TotalTime*2));
    float4 normalTextureDetail = (textureLerp*normalTexture3)+((1-textureLerp)*normalTexture4);
    
    float3 normal = (((0.5*normalTexture)+(0.5*normalTextureDetail))*2)-1;
    normal.xyz = normal.xzy;
    normal = normalize(normal);
    
    float3 cubeTexCoords = reflect(input.worldPos-EyePos,normal);
    
    float3 cubeTex = texCUBE(CubeTextureSampler,cubeTexCoords).rgb;
    
    return float4((cubeTex*0.8)+(diffuseColor*0.2),1);
}

// Our Techinique
technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
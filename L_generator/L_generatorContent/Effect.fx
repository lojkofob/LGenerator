Matrix ViewProj;
Matrix World;
Matrix Scale;
Texture tex0;
Texture tex1;
Texture tex2;
Texture tex3;
Texture tex4;

Texture ins_tex;
float2 t_vect;
bool t_v;
bool InsVis, show0,show1,show2,show3,show4;
float ins_size;

int method0;
int method1;
int method2;
int method3;
int method4;

float4 wire_color = float4(1,1,1,1);
sampler textureSampler0 = sampler_state  {   texture=<tex0>; };
sampler textureSampler1 = sampler_state  {   texture=<tex1>; };
sampler textureSampler2 = sampler_state  {   texture=<tex2>; };
sampler textureSampler3 = sampler_state  {   texture=<tex3>; };
sampler textureSampler4 = sampler_state  {   texture=<tex4>; };
sampler textureSamplerins = sampler_state {   texture=<ins_tex>; };

// вершинный шейдер
void VS( in float4 inPos  : POSITION,     in float2 inTex : TEXCOORD0,
        out float4 outPos : POSITION,   out float2 outTex : TEXCOORD0 )
{
    outPos = mul(inPos, mul(World, mul(Scale, ViewProj)));
    outTex = inTex;
}



void VS1( in float4 inPos  : POSITION,  out float4 outPos : POSITION)
{
 outPos = mul(inPos, mul(World, mul(Scale, ViewProj)));
}


void VS3( in float4 inPos  : POSITION,     in float2 inTex : TEXCOORD0,
          out float4 outPos : POSITION,   out float2 outTex : TEXCOORD0 )
{
    outPos = inPos;
    outTex = float2(0,0);
}


// пиксельный шейдер
void PS(in float2 inTex : TEXCOORD0,  out float4 outColor : COLOR0 )
{
if (show0) outColor = tex2D(textureSampler0, inTex);

if (show1) if (method1 == 0) outColor = tex2D(textureSampler1, inTex);
if (show2) if (method2 == 0) outColor = tex2D(textureSampler2, inTex);
if (show3) if (method3 == 0) outColor = tex2D(textureSampler3, inTex);
if (show4) if (method4 == 0) outColor = tex2D(textureSampler4, inTex);
			   
if (show1) if (method1 == 1) outColor += tex2D(textureSampler1, inTex);
if (show2) if (method2 == 1) outColor += tex2D(textureSampler2, inTex);
if (show3) if (method3 == 1) outColor += tex2D(textureSampler3, inTex);
if (show4) if (method4 == 1) outColor += tex2D(textureSampler4, inTex);

if (show1) if (method1 == 2) outColor *= tex2D(textureSampler1, inTex);
if (show2) if (method2 == 2) outColor *= tex2D(textureSampler2, inTex);
if (show3) if (method3 == 2) outColor *= tex2D(textureSampler3, inTex);
if (show4) if (method4 == 2) outColor *= tex2D(textureSampler4, inTex);

			
    float2 pos = (inTex - t_vect)*2048/200;
	float4 outColor2 = float4(0, 0, 0, 0.5);
	if (pos.x> 0.5 - ins_size/3 && pos.x < 0.5 + ins_size/3)
	if (pos.y> 0.5 - ins_size/3 && pos.y < 0.5 + ins_size/3)
	{
	 outColor2 *= tex2D(textureSamplerins,1.5*(pos - float2(0.5,0.5))/ins_size + float2(0.5,0.5));
	 outColor2.x = outColor2.w;
	 outColor2.w = 1;
	 outColor += outColor2;
	}
	// * (1+( (inTex.x - 0.5)*(inTex.x - 0.5) + (inTex.y - 0.5)*(inTex.y - 0.5)) )
}



void PS1( out float4 outColor : COLOR0 )
{
    outColor = wire_color;
}

void PS3( in float4 inColor : COLOR0 , out float4 outColor : COLOR0 )
{
	outColor = inColor;
	// * (1+( (inTex.x - 0.5)*(inTex.x - 0.5) + (inTex.y - 0.5)*(inTex.y - 0.5)) )
}

 
// первая техника (по-умолчанию)
technique Technique0
{
		//basic
        pass P0
        {
                vertexShader = compile vs_3_0 VS();
                pixelShader = compile ps_3_0 PS();
        }

		//wire
        pass P1
        {
                vertexShader = compile vs_3_0 VS1();
                pixelShader = compile ps_3_0 PS1();
        }

		//2D
        pass P3
        {
                vertexShader = compile vs_3_0 VS3();
                pixelShader = compile ps_3_0 PS3();
        }
}
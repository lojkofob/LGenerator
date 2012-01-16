texture tex;    

sampler ScreenS = sampler_state
{
    texture = <tex>;    
};

float4 PS(float2 texCoord: TEXCOORD0) : COLOR
{
    float4 color = float4(texCoord.x,texCoord.y,texCoord.y*texCoord.x,0)+float4(0,0,0,1);    
    return color;
}
technique
{
    pass P0
    {
        PixelShader = compile ps_2_0 PS();
    }
}
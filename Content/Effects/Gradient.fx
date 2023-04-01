sampler inputTexture;
float3 bottomColor;
float3 topColor;

float4 MainPS(float2 UV : TEXCOORD0) : COLOR0
{
    float4 gamePixelColor = tex2D(inputTexture, UV);	
    gamePixelColor.rgb = lerp(topColor, bottomColor, UV.y);
    return gamePixelColor;
}

technique Techninque1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 MainPS();
        /*AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;*/
    }
};
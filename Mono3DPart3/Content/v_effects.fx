float4x4 xViewProjection;

struct VertexToPixel
{
    float4 Position     : POSITION;
    float4 Color        : COLOR0;
};

struct PixelToFrame
{
    float4 Color        : COLOR0;
};

VertexToPixel SimplestVertexShader(float4 inPos : POSITION)
{
    VertexToPixel Output = (VertexToPixel)0;

    Output.Position = mul(inPos, xViewProjection);
    Output.Color.rga = 1.0f;

    return Output;
}

PixelToFrame OurFirstPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;

    Output.Color = PSIn.Color;

    return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 SimplestVertexShader();
        PixelShader = compile ps_2_0 OurFirstPixelShader();
    }
}
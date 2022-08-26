// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "GlowingShader/SideGlow"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _OutLineSpread("Outline Spread", Range(0,0.01)) = 0.007
        _InlineSpread("Inline Spread", Range(0,0.01)) = 0.007
        _Color("Tint", Color) = (1,1,1,1)
        _ColorX("OutlineColor", Color) = (1,1,1,1)
        _ColorInline("InLineColor", Color) = (1,1,1,1)
        _Alpha("Alpha", Range(0,1)) = 1.0

        _FirstColor("First Color", Color) = (1,0,0,1)
        _SecondColor("Second Color", Color) = (0,1,0,1)
    }
        SubShader
        {
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off
             Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _OutLineSpread;
float4 _Color;
float4 _ColorX;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}

float4 frag(v2f i) : COLOR
{

float4 mainColor = (tex2D(_MainTex, i.texcoord - float2(-_OutLineSpread,_OutLineSpread))
+ tex2D(_MainTex, i.texcoord - float2(_OutLineSpread,-_OutLineSpread))
+ tex2D(_MainTex, i.texcoord - float2(_OutLineSpread,_OutLineSpread))
+ tex2D(_MainTex, i.texcoord - float2(_OutLineSpread,_OutLineSpread)));

mainColor.rgb = _ColorX.rgb;

float4 addcolor = tex2D(_MainTex, i.texcoord) * i.color;

if (mainColor.a > 0.40) { mainColor = _ColorX; }
if (addcolor.a > 0.40) { mainColor = addcolor; mainColor.a = addcolor.a; }

return mainColor * i.color.a;
}
ENDCG
}

 Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _InlineSpread;
float4 _Color;
float4 _ColorInline;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}

float4 frag(v2f i) : COLOR
{

float4 mainColor = (tex2D(_MainTex, i.texcoord + float2(_InlineSpread,-_InlineSpread))
+ tex2D(_MainTex, i.texcoord + float2(-_InlineSpread,_InlineSpread))
+ tex2D(_MainTex, i.texcoord + float2(-_InlineSpread,_InlineSpread))
+ tex2D(_MainTex, i.texcoord + float2(-_InlineSpread,_InlineSpread)));

mainColor.rgb = _ColorInline.rgb;

float4 addcolor = tex2D(_MainTex, i.texcoord) * i.color;

if (mainColor.a > 0.40) { mainColor = _ColorInline; }
if (addcolor.a > 0.40) { mainColor = addcolor; mainColor.a = addcolor.a; }

return mainColor * i.color.a;
}
ENDCG
}








           /* Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                fixed4 _FirstColor;
                struct appdata
                {
                    float4 vertex:POSITION;
                };
                struct v2f
                {
                    float4 clipPos:SV_POSITION;
                };
                v2f vert(appdata v)
                {
                    v2f o;
                    v.vertex -= float4(0.5,0,0,0);
                    o.clipPos = UnityObjectToClipPos(v.vertex);
                    return o;
                }
                fixed4 frag(v2f i) : SV_Target
                {
                    return _FirstColor;
                }
                ENDCG
            }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                fixed4 _SecondColor;
                struct appdata
                {
                    float4 vertex:POSITION;
                };
                struct v2f
                {
                    float4 clipPos:SV_POSITION;
                };
                v2f vert(appdata v)
                {
                    v2f o;
                    v.vertex += float4(1.5,0,0,0);
                    o.clipPos = UnityObjectToClipPos(v.vertex);
                    return o;
                }
                fixed4 frag(v2f i) : SV_Target
                {
                    return _SecondColor;
                }
                ENDCG
            }*/

                   
        }
}
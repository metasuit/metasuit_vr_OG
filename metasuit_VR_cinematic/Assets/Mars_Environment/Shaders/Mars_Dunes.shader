// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Nature/Mars_Dunes" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
    _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_Mask("Mask (RGBA)", 2D) = "red" {}
	_Splat1("Sand_1_BC", 2D) = "black" {}
	_Splat0("Sand_2_BC", 2D) = "white" {}

	_Normal1("Sand_1_N", 2D) = "bump" {}
	_Normal0("Sand_2_N", 2D) = "bump" {}

    _MainTex ("MainTex", 2D) = "white" {}
    _BumpMap ("Normalmap", 2D) = "bump" {}

	_Blur("Blur", Range(0.01, 1)) = 0.02
	_HeightSplatAll("RG", 2D) = "black" {}
}

CGINCLUDE
half _Blur;
sampler2D _Control;
sampler2D _Mask;
sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
sampler2D _Normal0, _Normal1, _Normal2, _Normal3;
sampler2D _HeightSplatAll;
fixed4 _Color;
half _Shininess;
samplerCUBE _Cube;

struct Input {
    float2 uv_MainTex : TEXCOORD0;
    float2 uv_BumpMap;
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
	float2 uv_Splat3 : TEXCOORD4;
	float3 worldRefl;
	float3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {

	half4 ColorTex = tex2D(_MainTex, IN.uv_MainTex);
	half4 MaskTex = tex2D(_Mask, IN.uv_MainTex);

	half4 Detail0 = tex2D(_Splat0, IN.uv_Splat0) * ColorTex * _Color;
	half4 Detail1 = tex2D(_Splat1, IN.uv_Splat1) * ColorTex * _Color;

	half4 HeightSplatTex1 = tex2D(_HeightSplatAll, IN.uv_Splat0).r;
	half4 HeightSplatTex2 = tex2D(_HeightSplatAll, IN.uv_Splat1).g;

	float a0 = MaskTex.r;
	float a1 = MaskTex.g;

	float ma = (max(HeightSplatTex1.rgb + a0, HeightSplatTex2.rgb + a1)) - _Blur;

	float b0 = max(HeightSplatTex1.rgb + a0 - ma, 0);
	float b1 = max(HeightSplatTex2.rgb + a1 - ma, 0);

	float4 texture0 = Detail0;
	float4 texture1 = Detail1;
	fixed4 tex = (texture0 * b0 + texture1 * b1) / (b0 + b1);

	texture0 = tex2D(_Normal0, IN.uv_Splat0);
	texture1 = tex2D(_Normal1, IN.uv_Splat1);
	
	float4 mixnormal = (texture0 * b0 + texture1 * b1) / (b0 + b1);

	o.Normal = normalize(UnpackNormal(mixnormal)*1.5 + UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) * 3);

	o.Albedo = tex;
	o.Gloss = tex.a;
	o.Specular = _Shininess;
}
ENDCG


SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 400

    CGPROGRAM
    #pragma surface surf BlinnPhong nodynlightmap
	#pragma target 3.0
    ENDCG
}

FallBack "Legacy Shaders/Specular"
}

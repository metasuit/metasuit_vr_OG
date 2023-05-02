// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Nature/Terrain/StandardTerrainMars_Shader" {
    Properties {
        // set by terrain engine
	_Color ("Main Color", Color) = (1,1,1,1)
	_Color_Sand ("Sand", Color) = (1,1,1,1)
	_Color_Sand_brown ("Sand_brown", Color) = (1,1,1,1)
	_Color_Sand_white ("Sand_white", Color) = (1,1,1,1)
	_Color_Sand_black ("Sand_black", Color) = (1,1,1,1)
	_Color_Cliff ("Color_Cliff", Color) = (1,1,1,1)
	_Color_Stone_2 ("Stone_2", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Smoothness ("Smoothness", Range (0.01, 1)) = 0.078125
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
        
        [HideInInspector] _Control_ST ("Control (RGBA)", 2D) = "red" {}

        [HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
        [HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
        [HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
        [HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
        [HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
        [HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
        [HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
        [HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
        [HideInInspector] [Gamma] _Metallic0 ("Metallic 0", Range(0.0, 1.0)) = 0.0
        [HideInInspector] [Gamma] _Metallic1 ("Metallic 1", Range(0.0, 1.0)) = 0.0
        [HideInInspector] [Gamma] _Metallic2 ("Metallic 2", Range(0.0, 1.0)) = 0.0
        [HideInInspector] [Gamma] _Metallic3 ("Metallic 3", Range(0.0, 1.0)) = 0.0
        [HideInInspector] _Smoothness0 ("Smoothness 0", Range(0.0, 1.0)) = 1.0
        [HideInInspector] _Smoothness1 ("Smoothness 1", Range(0.0, 1.0)) = 1.0
        [HideInInspector] _Smoothness2 ("Smoothness 2", Range(0.0, 1.0)) = 1.0
        [HideInInspector] _Smoothness3 ("Smoothness 3", Range(0.0, 1.0)) = 1.0

        // used in fallback on old cards & base map
        [HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
        [HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
		
		_Blur ("Blur", Range (0.01, 1)) = 0.02
        _Mask1 ("Mask1 (RGBA)", 2D) = "red" {}
        
		_SandDunes ("Sand_Dunes", Range (-1, 5)) = 0.02
		_SandCliff ("Sand_Cliff", Range (-1, 5)) = 0.02
	
		_ColorTex ("ColorMap (RGB)", 2D) = "black" {}
		_SandDuneN ("SandDuneN (RGB)", 2D) = "black" {}
		_Tiling ("Tiling", Range (0.01, 200)) = 0.05
	
		_SandMoveTex ("SandMoveMap (RGB)", 2D) = "white" {}	
	
		_HeightSplatAll ("Grass(R) Cliff(G) Stones(B) Snow(a)", 2D) = "black" {}
		_Parallax ("Height", Range (0.005, 0.08)) = 0.02

    }

    SubShader {
        Tags {
            "Queue" = "Geometry-100"
            "RenderType" = "Opaque"
        }

        CGPROGRAM
        #pragma surface surf StandardSpecular vertex:SplatmapVert finalcolor:SplatmapFinalColor finalgbuffer:SplatmapFinalGBuffer fullforwardshadows noinstancing
        #pragma multi_compile_fog
        #pragma target 3.0
        // needs more than 8 texcoords
        #pragma exclude_renderers gles psp2
        //#include "UnityPBSLighting_Mars.cginc" 

        #pragma multi_compile __ _TERRAIN_NORMAL_MAP
        #if defined(SHADER_API_D3D9) && defined(SHADOWS_SCREEN) && defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && defined(DYNAMICLIGHTMAP_ON) && defined(SHADOWS_SHADOWMASK) && defined(_TERRAIN_NORMAL_MAP) && defined(UNITY_SPECCUBE_BLENDING)
            // TODO : On d3d9 17 samplers would be used when : defined(SHADOWS_SCREEN) && defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && defined(DYNAMICLIGHTMAP_ON) && defined(SHADOWS_SHADOWMASK) && defined(_TERRAIN_NORMAL_MAP) && defined(UNITY_SPECCUBE_BLENDING)
            // In that case it would be probably acceptable to undef UNITY_SPECCUBE_BLENDING however at the moment (10/2/2016) we can't undef UNITY_SPECCUBE_BLENDING or other platform defines. CGINCLUDE being added after "Lighting.cginc".
            // For now, remove _TERRAIN_NORMAL_MAP in this case.
            #undef _TERRAIN_NORMAL_MAP
            #define DONT_USE_TERRAIN_NORMAL_MAP // use it to initialize o.Normal to (0,0,1) because the surface shader analysis still see this shader writes to per-pixel normal.
        #endif

        #define TERRAIN_STANDARD_SHADER
        #define TERRAIN_SURFACE_OUTPUT SurfaceOutputStandardSpecular
        #include "TerrainSplatmapCommon_Mars.cginc" 


        half _Metallic0;
        half _Metallic1;
        half _Metallic2;
        half _Metallic3;

        half _Smoothness0;
        half _Smoothness1;
        half _Smoothness2;
        half _Smoothness3;


        void surf (Input IN, inout SurfaceOutputStandardSpecular o) {

		    o.Normal = float3(0, 0, 1);
		    float3 n = WorldNormalVector(IN, o.Normal);
	     	float3 projNormal = saturate(pow(n * 1.5, 10));
		    float2 invertY = float2(1, -1);
	
			half4 h = tex2D (_HeightSplatAll, IN.uv_Splat2).b;
			float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir);
			IN.uv_Splat2 += offset;

			half4 h2 = tex2D (_HeightSplatAll, IN.uv_Splat3).a;
			float2 offset2 = ParallaxOffset (h2, _Parallax, IN.viewDir);
			IN.uv_Splat3 += offset2;
			
			half4 ColorTex = tex2D (_ColorTex, IN.tc_Control);
			half4 Control_ST = tex2D (_Control, IN.tc_Control);
			
			half4 MaskTex = tex2D (_Mask1, IN.tc_Control);
			
			
			//-------------------------------------------------------
			float4 x = tex2D(_Splat1, frac(IN.worldPos.zy / _Tiling)) * ColorTex;
			float4 x_n = tex2D(_Normal1, frac(IN.worldPos.zy / _Tiling));
			float4 x_h = tex2D(_HeightSplatAll, frac(IN.worldPos.zy / _Tiling)).g;
			// TOP / BOTTOM
			float4 y = tex2D(_Splat1, frac(IN.worldPos.xy / _Tiling)) * ColorTex;
			float4 y_n = tex2D(_Normal1, frac(IN.worldPos.xy / _Tiling));
			float4 y_h = tex2D(_HeightSplatAll, frac(IN.worldPos.xy / _Tiling)).g;
			// SIDE Z    
			float4 z = tex2D(_Splat1, frac(IN.worldPos.xy / _Tiling)) * ColorTex;
			float4 z_n = tex2D(_Normal1, frac(IN.worldPos.xy / _Tiling));
			float4 z_h = tex2D (_HeightSplatAll, frac(IN.worldPos.xy / _Tiling)).g;
			//-------------------------------------------------------
			half4 Detail_Cliff_Wind = tex2D (_SandMoveTex, IN.tc_Control).g * tex2D (_SandMoveTex, IN.tc_Control).r;
			half4 Detail_Mount_Wind = tex2D (_SandMoveTex, IN.tc_Control).r;
			half4 Detail_Mount_Flat = tex2D (_SandMoveTex, IN.tc_Control).g - tex2D (_SandMoveTex, IN.tc_Control).r;
			
			half4 Detail0 = tex2D (_Splat0, IN.uv_Splat0);
			half4 Detail1 = tex2D (_Splat1, IN.uv_Splat1) * ColorTex;
			half4 Detail2 = tex2D (_Splat2, IN.uv_Splat2);
			half4 Detail3 = tex2D (_Splat3, IN.uv_Splat3) * ColorTex;
			
			//-------------------------------------------------------
			
			half4 HeightSplatTex1 = tex2D (_HeightSplatAll, IN.uv_Splat0).r;
			
			half4 HeightSplatTex3_1 = tex2D (_HeightSplatAll, IN.uv_Splat2 - offset).b;
			half4 HeightSplatTex3_2 = tex2D (_HeightSplatAll, IN.uv_Splat2*1.88).b;
			
			half4 HeightSplatTex4 = tex2D (_HeightSplatAll, IN.uv_Splat3 - offset2).a;
			
			//-------------------------------------------------------
			fixed4 cliffheight = 0; 
			o.Albedo = z_h.rgb;
			o.Albedo = lerp(o.Albedo, x_h.rgb, projNormal.x);
			o.Albedo = lerp(o.Albedo, y_h.rgb, projNormal.y);
					    
			cliffheight.rgb = o.Albedo;
			
			fixed4 cliffheight_2 = 0; 
			o.Albedo = tex2D(_ColorTex, frac(IN.worldPos.xy / _Tiling)).a;
			o.Albedo = lerp(o.Albedo, tex2D(_ColorTex, frac(IN.worldPos.zy / _Tiling)).a, projNormal.x);
			o.Albedo = lerp(o.Albedo, tex2D(_ColorTex, frac(IN.worldPos.xy / _Tiling)).a, projNormal.y);
					    
			cliffheight_2.rgb = o.Albedo;
			
			//float a0 = MaskTex.b * Detail_Mount_Wind + cliffheight * MaskTex.g * Detail_Cliff_Wind + MaskTex.r + MaskTex.a*3 - MaskTex.a * 10.5 * tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a - tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a /8 + tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a * Control_ST.b/5 + Control_ST.b/5 + MaskTex.b/5;
			float a0 = MaskTex.r + MaskTex.a*1.2 + Detail_Mount_Wind/8 - MaskTex.g*2 - MaskTex.a * 10.5 * tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a + MaskTex.a/2;

			
			float a0_wind = tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/6).a;
			float a1 = MaskTex.g * 5 - MaskTex.a;
			float a2 = MaskTex.b/1.5 + Control_ST.a /2 + Control_ST.b /2 - MaskTex.a * 5.5 - tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/6).a * MaskTex.a - MaskTex.g + tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/6).b/6 - tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a*1.2;
			
			float a_Sand =  MaskTex.b - MaskTex.a * HeightSplatTex1 * (1 - tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a) + tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a + MaskTex.a / 2;

		    float ma = (max(max(max(HeightSplatTex1.rgb + a0, cliffheight.rgb/5 + a1), HeightSplatTex3_1.rgb + a2), HeightSplatTex3_2.rgb + a2)) - _Blur;
		    
		    float Stones_1 = max(HeightSplatTex3_1.rgb + a2 - ma, 0) + max(HeightSplatTex3_1.rgb + a2/1.5 + MaskTex.a*2.5 + Control_ST.a*3 * HeightSplatTex3_1 - ma, 0)*10;
		    float Stones_2 = max(HeightSplatTex3_2.rgb + a2 - ma, 0) + max(HeightSplatTex3_2.rgb + a2/1.5 + MaskTex.a*2.5 + Control_ST.a*3 * HeightSplatTex3_2 - ma, 0)*10;
		    	
		    float Sand = max(((MaskTex.r + MaskTex.g/5 * cliffheight + MaskTex.b*cliffheight_2*3 + (0.5-Detail_Cliff_Wind) *cliffheight_2/1.1) - tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a *8)*Detail_Mount_Wind, 0)
		    + max((1 - HeightSplatTex3_1.rgb) * a2 * 1.8 - 10 * tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a/3.5 - ma, 0)/2
		    + max(0.02 - Detail_Mount_Wind*10 - MaskTex.g*50, 0) + Detail_Cliff_Wind * MaskTex.g * cliffheight * 2
		    + max((HeightSplatTex1.rgb + MaskTex.r * 4500 * cliffheight - cliffheight_2 * MaskTex.g * 10 - ma) * Detail_Mount_Wind - MaskTex.a * 2000, 0)/220
		    + max((HeightSplatTex1.rgb + MaskTex.r * 3 - cliffheight_2 * MaskTex.g * 10 - ma) * Detail_Mount_Wind, 0);

  			float Sand_white = max(HeightSplatTex3_1.rgb + a2/1.5 + MaskTex.a*2.5 + Control_ST.a*3 * HeightSplatTex3_1 - ma, 0) + 
  			+ max((1 - HeightSplatTex3_1.rgb) * a2 * Control_ST.a * 1.3 + Detail_Mount_Wind * MaskTex.r - ma, 0)
  			+ max(HeightSplatTex1.rgb * Detail_Mount_Flat * 31 - MaskTex.a * 130.5 * tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a - ma, 0)/10
  			+ HeightSplatTex1 * MaskTex.b * Detail_Mount_Wind / 1.5 + Sand * Detail_Mount_Wind /8;

  			
  			float Sand_black = max(MaskTex.b * MaskTex.b/1.5 - MaskTex.a - tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a/3, 0) + max(HeightSplatTex1.rgb + Detail_Mount_Wind * Sand - MaskTex.g*20 - ma, 0) + max(HeightSplatTex1.rgb * MaskTex.b * 4 - ma, 0)*40 + max((HeightSplatTex1.rgb + a0_wind * cliffheight * 5000 * cliffheight * cliffheight) * MaskTex.g - ma, 0) *5
		    + max(HeightSplatTex1.rgb - HeightSplatTex1.rgb * MaskTex.b * 2 * tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a - ma, 0) + Sand /3 + max(HeightSplatTex1.rgb * Detail_Mount_Flat * 30 - MaskTex.a * 50.5 * tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a - ma, 0)
		    + max((HeightSplatTex1.rgb + MaskTex.r * 8 * cliffheight + MaskTex.r * 2 - ma) * Detail_Mount_Wind - MaskTex.a * 2000, 0);

		    float b0n = max(HeightSplatTex1.rgb + tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/6).a * 4.3 - 0.2 - ma, 0)/5 + MaskTex.a/5 + max(HeightSplatTex1.rgb * Detail_Mount_Flat * 35 - MaskTex.a * 40.5 * tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/10).a - ma, 0)*3;
		    float b0_wind = MaskTex.r * tex2D(_SandMoveTex, IN.uv_Splat0 / 10 + _Time/6).a / 8;
		    float Cliff = 0.0001 + max(Detail_Mount_Wind/5 - MaskTex.a * 20, 0);

		    
		    float b3 = max(HeightSplatTex4.rgb - ma, 0);
		    
		    float Sand_brown = max(Detail_Mount_Wind - Cliff *20, 0) + MaskTex.a/2 + tex2D(_SandMoveTex, IN.uv_Splat0 / 10).a * 5 *(max((HeightSplatTex1.rgb + HeightSplatTex1.rgb * a_Sand * 2 - MaskTex.g*50) - ma, 0) * 2 + HeightSplatTex1.rgb * MaskTex.b * Detail_Mount_Wind * Detail_Mount_Wind/2);


			float4 Sand_tex = tex2D (_Splat0, IN.uv_Splat0) * _Color_Sand * ColorTex;
		    float4 Sand_black_tex = tex2D (_Splat0, IN.uv_Splat0) * _Color_Sand_black * (0.5 + 0.5 * ColorTex);
		    float4 Sand_white_tex = tex2D (_Splat0, IN.uv_Splat0) * _Color_Sand_white * (0.5 + 0.5 * ColorTex);
		    float4 Sand_brown_tex =  tex2D (_Splat0, IN.uv_Splat0*1.5) * ColorTex * _Color_Sand_brown;
		    
		    float4 texture2_1 = tex2D (_Splat2, IN.uv_Splat2) * ColorTex;
		    float4 texture2_2 = tex2D (_Splat2, IN.uv_Splat2*1.88) * ColorTex * _Color_Stone_2;
		    
		    float4 texture3 = tex2D (_Splat3, IN.uv_Splat3);
		    
			fixed4 texstone = 0;	
			o.Albedo = z.rgb;
			o.Albedo = lerp(o.Albedo, x.rgb, projNormal.x);
			o.Albedo = lerp(o.Albedo, y.rgb, projNormal.y);
			
			texstone.rgb = o.Albedo * _Color_Cliff;
			
		    fixed4 tex = (Sand_white_tex * Sand_white + Sand_black_tex * Sand_black + Sand_brown_tex * Sand_brown + Sand_tex * Sand + texstone * Cliff + texture2_1 * Stones_1 + texture2_2 * Stones_2) / (Sand_black + Sand_brown  + Sand + Cliff + Stones_1 + Stones_2);

			fixed4 texture0 = tex2D (_Normal0, IN.uv_Splat0);
			texture2_1 = tex2D (_Normal2, IN.uv_Splat2);
			texture2_2 = tex2D (_Normal2, IN.uv_Splat2*1.88);
			fixed4 texture_Dune = tex2D (_SandDuneN, IN.uv_Splat0);
			texture3 = tex2D (_Normal3, IN.uv_Splat3);
			float4 mixnormal = (texture_Dune * Sand_brown + texture3 * b0_wind + texture0 * b0n + texture0 * Sand_black + texture3 * Cliff + texture2_1 * Stones_1 + texture2_2 * Stones_2 + texture3 * b3) / (Sand_black + Sand_brown + b0_wind + b0n + Cliff + Stones_1 + Stones_2 + b3);
			
			o.Normal = UnpackNormal(z_n*1.2)*3;
			o.Normal = lerp(o.Normal, UnpackNormal(x_n*1.2)*3, projNormal.x);
			o.Normal = lerp(o.Normal, UnpackNormal(y_n*1.2)*3, projNormal.y);
			o.Normal = lerp(UnpackNormal(mixnormal*1.07 + Detail_Mount_Wind/29), o.Normal, MaskTex.g + Control_ST.g);
			//o.Normal = UnpackNormal(texture_Dune);
			
			o.Albedo = tex.rgb * _Color.rgb * tex2D(_SandMoveTex, IN.uv_Splat0 / 20 + _Time/10).b * 10;
			//o.Albedo = tex.rgb * _Color.rgb * tex2D(_SandMoveTex, IN.worldPos.xy/200).b;
			//o.Albedo = cliffheight;
			//o.Gloss = tex.a;
			o.Alpha = 1.1;
			o.Specular = tex2D (_Splat0, IN.uv_Splat0 + _Time/80).a * tex.a * _Shininess*3 - Sand_black/300 - MaskTex.a /1000;
			//o.Albedo = cliffheight;
            o.Smoothness = tex.a * _Smoothness * _SpecColor;
            //o.Metallic = _Metallic * tex;
           // o.specColor = tex.a;
            //o.Albedo = texstone;
        }
        ENDCG
    }

    Dependency "AddPassShader" = "Hidden/TerrainEngine/Splatmap/Standard-AddPass"
    Dependency "BaseMapShader" = "Hidden/TerrainEngine/Splatmap/Standard-Base-Mars"

    Fallback "Nature/Terrain/Standard"
}

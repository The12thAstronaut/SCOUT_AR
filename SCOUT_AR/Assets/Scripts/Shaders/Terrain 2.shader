Shader"Custom/Terrain2"
{
	Properties
	{
		_Col("Colour", 2D) = "white" {}
		//_HeightMap("Height Map", 2D) = "white" {}
		//_Col00("Color-0-0", 2D) = "white" {}
		//_Col01("Color-0-1", 2D) = "white" {}
		//_Col10("Color-1-0", 2D) = "white" {}
		//_Col11("Color-1-1", 2D) = "white" {}
		//_Col20("Color-2-0", 2D) = "white" {}
		//_Col21("Color-2-1", 2D) = "white" {}
		//_Col30("Color-3-0", 2D) = "white" {}
		//_Col31("Color-3-1", 2D) = "white" {}
		_HeightMap00("Height Map-0-0", 2D) = "white" {}
		_HeightMap01("Height Map-0-1", 2D) = "white" {}
		_HeightMap10("Height Map-1-0", 2D) = "white" {}
		_HeightMap11("Height Map-1-1", 2D) = "white" {}
		_HeightMap20("Height Map-2-0", 2D) = "white" {}
		_HeightMap21("Height Map-2-1", 2D) = "white" {}
		_HeightMap30("Height Map-3-0", 2D) = "white" {}
		_HeightMap31("Height Map-3-1", 2D) = "white" {}
		_Longitude("Longitude", Range(-180, 180)) = 0
		_Latitude("Latitude", Range(-90, 90)) = 0
		_TestRadiusKM("Test Radius KM", Float) = 100

		[Header(Lighting)]
		_Contrast ("Contrast", Float) = 1
		_BrightnessAdd("Brightness Add", Float) = 0
		_BrightnessMul("Brightness Mul", Float) = 1

		[Header(Test)]
		_TestParams("Test Params", Vector) = (0,0,0,0)

		// Advanced options.
        [Toggle(_STENCIL)] _EnableStencil("Enable Stencil Testing", Float) = 0.0
        _StencilReference("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparison("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOperation("Stencil Operation", Int) = 0
        _StencilWriteMask("Stencil Write Mask", Range(0, 255)) = 255
        _StencilReadMask("Stencil Read Mask", Range(0, 255)) = 255

	}
	SubShader
	{		
		Pass
		{
			Tags {"LightMode" = "ForwardBase" "Queue" = "Geometry"}
			
			Stencil
			{
				Ref[_StencilReference]
				Comp[_StencilComparison]
				Pass[_StencilOperation]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "AutoLight.cginc"

			#include "Assets/Scripts/Shader Common/GeoMath.hlsl"
			#include "Assets/Scripts/Shader Common/Triplanar.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 objPos : TEXCOORD0;
				float3 worldNormal : NORMAL;
				LIGHTING_COORDS(4,5)
			};

			//sampler2D _Col00, _Col01, _Col10, _Col11, _Col20, _Col21, _Col30, _Col31;
			sampler2D _Col;
			sampler2D _HeightMap00, _HeightMap01, _HeightMap10, _HeightMap11, _HeightMap20, _HeightMap21, _HeightMap30, _HeightMap31;
			float4 _Col_TexelSize;

			float _BrightnessAdd, _BrightnessMul, _Contrast;
			float4 _TestParams;
			float _Longitude, _Latitude, _TestRadiusKM;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.objPos = v.vertex;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				
				float3 pointOnUnitSphere = normalize(i.objPos);
				float2 texCoord = pointToUV(pointOnUnitSphere);
	
	
				float3 unlitTerrainCol = 0;
				float mipLevel = calculateGeoMipLevel(texCoord, _Col_TexelSize.zw);
				unlitTerrainCol = tex2Dlod(_Col, float4(texCoord, 0, mipLevel));
	
				float4 col = tex2D(_Col, texCoord);
				float3 detailNormal = 0;
				//tex2D(_HeightMap, texCoord).rgb;
	
				int xIndex = int(texCoord.x * 4);
				int yIndex = int(texCoord.y * 2);
	
				if (xIndex == 4)
				{
					xIndex = 3;
				}
				if (yIndex == 2)
				{
					yIndex = 1;
				}
	
				float x = texCoord.x * 4 - xIndex;
				if (yIndex == 0)
				{
					float y = texCoord.y * 2;
					if (xIndex == 0)
					{
						float mipLevel = calculateGeoMipLevel(float2(x, y), _Col_TexelSize.zw);
						//unlitTerrainCol = tex2Dlod(_Col21, float4(float2(x, y), 0, mipLevel));
						detailNormal = tex2D(_HeightMap21, float2(x, y));
						//col = tex2D(_Col21, float2(x, y));
					}
					else if (xIndex == 1)
					{
						float mipLevel = calculateGeoMipLevel(float2(x, y), _Col_TexelSize.zw);
						//unlitTerrainCol = tex2Dlod(_Col31, float4(float2(x, y), 0, mipLevel));
						detailNormal = tex2D(_HeightMap31, float2(x, y));
						//col = tex2D(_Col31, float2(x, y));
					}
					else if (xIndex == 2)
					{
						float mipLevel = calculateGeoMipLevel(float2(x, y), _Col_TexelSize.zw);
						//unlitTerrainCol = tex2Dlod(_Col01, float4(float2(x, y), 0, mipLevel));
						detailNormal = tex2D(_HeightMap01, float2(x, y));
						//col = tex2D(_Col01, float2(x, y));
					}
					else
					{
						float mipLevel = calculateGeoMipLevel(float2(x, y), _Col_TexelSize.zw);
						//unlitTerrainCol = tex2Dlod(_Col11, float4(float2(x, y), 0, mipLevel));
						detailNormal = tex2D(_HeightMap11, float2(x, y));
						//col = tex2D(_Col11, float2(x, y));
					}
				}
				else
				{
					float y = texCoord.y * 2 - yIndex;
					if (xIndex == 0)
					{
						float mipLevel = calculateGeoMipLevel(float2(x, y), _Col_TexelSize.zw);
						//unlitTerrainCol = tex2Dlod(_Col20, float4(float2(x, y), 0, mipLevel));
						detailNormal = tex2D(_HeightMap20, float2(x, y));
						//col = tex2D(_Col20, float2(x, y));
					}
					else if (xIndex == 1)
					{
						float mipLevel = calculateGeoMipLevel(float2(x, y), _Col_TexelSize.zw);
						//unlitTerrainCol = tex2Dlod(_Col30, float4(float2(x, y), 0, mipLevel));
						detailNormal = tex2D(_HeightMap30, float2(x, y));
						//col = tex2D(_Col30, float2(x, y));
					}
					else if (xIndex == 2)
					{
						float mipLevel = calculateGeoMipLevel(float2(x, y), _Col_TexelSize.zw);
						//unlitTerrainCol = tex2Dlod(_Col00, float4(float2(x, y), 0, mipLevel));
						detailNormal = tex2D(_HeightMap00, float2(x, y));
						//col = tex2D(_Col00, float2(x, y));
					}
					else
					{
						float mipLevel = calculateGeoMipLevel(float2(x, y), _Col_TexelSize.zw);
						//unlitTerrainCol = tex2Dlod(_Col10, float4(float2(x, y), 0, mipLevel));
						detailNormal = tex2D(_HeightMap10, float2(x, y));
						//col = tex2D(_Col10, float2(x, y));
					}
				}
	
				float3 meshWorldNormal = normalize(i.worldNormal);
				detailNormal = normalize(detailNormal * 2 - 1);
	
				float3 worldNormal = normalize(meshWorldNormal * 2 + detailNormal * 1.25);
	
				float fakeLighting = pow(dot(worldNormal, pointOnUnitSphere), 3);
	
				float3 dirToSun = _WorldSpaceLightPos0.xyz;
	
				float3 shading = saturate(saturate(dot(worldNormal, dirToSun) + _BrightnessAdd)) * _BrightnessMul;
	
				float3 terrainCol = unlitTerrainCol/* * shading*/;
				terrainCol = lerp(0.5, terrainCol, _Contrast);
			    //terrainCol *= lerp(fakeLighting, 1, 0.5);
	
	
				// Test circle
				float2 coord = float2(_Longitude, _Latitude) * (3.1415 / 180);
				float3 testPoint = longitudeLatitudeToPoint(coord);
				const float moonRadiusKM = 1740;
				float dstKM = distanceBetweenPointsOnUnitSphere(testPoint, pointOnUnitSphere) * moonRadiusKM;
				if (dstKM < 25)
				{
					return 1;
				}
				if (dstKM < _TestRadiusKM)
				{
					return lerp(col, float4(1, 0, 0, 0), 0.75);
				}
				
				return float4(terrainCol, 1);
				//return col;
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
}

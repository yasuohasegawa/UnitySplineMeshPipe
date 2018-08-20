Shader "Custom/SoundWave"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			float random(float2 st) {
				return frac(sin(dot(st.xy, float2(12.9898, 78.233)))* 43758.5453123);
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float volume = tex2D(_MainTex, i.uv.y).r * 0.5;
				float uvX = i.uv.x - 0.5;
				float rnd = random(i.uv.xy*_Time.z);
				float3 color = float3(rnd, rnd, rnd);

				return lerp(fixed4(color.r, color.g, color.b, 1),fixed4(i.uv.x+sin(_Time.w), i.uv.x, i.uv.y + cos(_Time.w), 1),-volume < uvX && uvX < volume);
			}
			ENDCG
		}
	}
}

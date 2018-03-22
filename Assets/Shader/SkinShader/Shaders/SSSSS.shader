Shader "Hidden/SSSSS"
{
	Properties
	{
		_MainTex ("Tex", 2D) = "white" {}
	}
	SubShader
	{
	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	float _Offset;

    inline float4 getWeightedColor(float2 uv, float2 offset){
    	float4 c = tex2D(_MainTex, uv) * 0.324;
    	float2 offsetM3 = offset * 3;
    	float2 offsetM2 = offset * 2;
    	c += tex2D(_MainTex, uv + offsetM3) * 0.0205;
    	c += tex2D(_MainTex, uv + offsetM2) * 0.0855;
    	c += tex2D(_MainTex, uv + offset) * 0.232;
    	c += tex2D(_MainTex, uv - offsetM3) * 0.0205;
    	c += tex2D(_MainTex, uv - offsetM2) * 0.0855;
    	c += tex2D(_MainTex, uv - offset) * 0.232;
    	return c;
    }
    			struct v2f_mg
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 offset : TEXCOORD1;
			};
    			inline float4 frag_blur (v2f_mg i) : SV_Target
			{
				return getWeightedColor(i.uv, i.offset);
			}

	ENDCG
	//0. vert 1. hori 2. blend 3. mask
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
		//Vertical 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_blur


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};



			inline v2f_mg vert (appdata v)
			{
				v2f_mg o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.offset = _MainTex_TexelSize.xy * float2(0,_Offset);
				return o;
			}
			ENDCG
		}

		Pass
		{
		//Horizontal 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_blur


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};


			inline v2f_mg vert (appdata v)
			{
				v2f_mg o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.offset = _MainTex_TexelSize.xy * float2(_Offset,0);
				return o;
			}
			ENDCG
		}

		Pass{
			//Blend
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _BlendTex;
			float4 _BlendWeight;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 oneMinusWeight : TEXCOORD1;
			};

			inline v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.oneMinusWeight = float4(1,1,1,1) - _BlendWeight;
				return o;
			}

			inline float4 frag (v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) * i.oneMinusWeight + tex2D(_BlendTex, i.uv) * _BlendWeight;
			}
			ENDCG
		}

		Pass{
		//Mask
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _OriginTex;
			uniform sampler2D _SSMaskTex;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			inline v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			inline float4 frag (v2f i) : SV_Target
			{
				return lerp(tex2D(_OriginTex, i.uv), tex2D(_MainTex, i.uv), tex2D(_SSMaskTex,i.uv).r);
			}
			ENDCG
		}

		Pass
		{
		//DownSample
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 offset : TEXCOORD1;
			};

			inline v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.offset = float4(_MainTex_TexelSize.x,0,0,_MainTex_TexelSize.y);
				return o;
			}

			inline float4 frag (v2f i) : SV_Target{
				float2 offset = _MainTex_TexelSize;
			  	float4 c = tex2D(_MainTex, i.uv + i.offset.xy);
			  	c += tex2D(_MainTex, i.uv + i.offset.zw);
			  	c += tex2D(_MainTex, i.uv - i.offset.xy);
			  	c += tex2D(_MainTex, i.uv - i.offset.zw);
			  	c.r = saturate(c.r);
    			return c;
			}
			ENDCG
		}

		Pass
		{
		//DownSample
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 offset : TEXCOORD1;
			};

			inline v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.offset = float4(_MainTex_TexelSize.x,0,0,_MainTex_TexelSize.y);
				return o;
			}

			inline float4 frag (v2f i) : SV_Target{
			  	float4 c = tex2D(_MainTex, i.uv + i.offset.xy);
			  	c += tex2D(_MainTex, i.uv + i.offset.zw);
			  	c += tex2D(_MainTex, i.uv - i.offset.xy);
			  	c += tex2D(_MainTex, i.uv - i.offset.zw);
    			return c / 4;
			}
			ENDCG
		}
	}
}

Shader "Hidden/DepthCull"
{
	SubShader
	{
		Tags { "RenderType"="Opaque"}
		ZTest always
		ZWrite off
		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _CameraDepthTextureWithoutSkin;
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{

				float4 pos : SV_POSITION;
				float4 screenPos : TEXCOORD1;

			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.screenPos = ComputeScreenPos(o.pos);

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float currentDepth = Linear01Depth(i.pos.z / i.pos.w);
				float depth = DecodeFloatRGBA(tex2Dproj(_CameraDepthTextureWithoutSkin, i.screenPos));
				clip(depth - currentDepth);
				return 1;
			}
			ENDCG
		}
	}
}

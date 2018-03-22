// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/DepthCullFront" {
SubShader {
    Tags { "RenderType"="Opaque" }
    Pass {
    Cull Front
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

struct v2f {
    float4 pos : SV_POSITION;
    float3 worldPos : TEXCOORD0;
};

v2f vert (appdata_base v) {
    v2f o;
    v.vertex.xyz += v.normal * 0.005;
    o.pos = UnityObjectToClipPos (v.vertex);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex) - _WorldSpaceCameraPos;
    return o;
}

float4 frag(v2f i) : COLOR {
   return EncodeFloatRGBA(length(i.worldPos) / 255);
}
ENDCG
    }
}
}
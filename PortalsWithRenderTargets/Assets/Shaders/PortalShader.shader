Shader "Custom/PortalShader" {
	Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
	SubShader {
		Pass {
			CGPROGRAM

			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			
			struct v2f {
				float4 pos : SV_POSITION;
				float4 pos_frag : TEXCOORD0;
			};
			
			v2f vert(appdata_base v) {
				v2f o;
				float4 clipSpacePosition = UnityObjectToClipPos(v.vertex);
				o.pos = clipSpacePosition;
				// Copy of clip space position for fragment shader
				o.pos_frag = clipSpacePosition;
				return o;
			}
			
			half4 frag(v2f i) : SV_Target {
				// Perspective divide (Translate to NDC - (-1, 1))
				float2 uv = i.pos_frag.xy / i.pos_frag.w;
				// Map -1, 1 range to 0, 1 tex coord range
				uv = (uv + float2(1.0, 1.0)) * 0.5;
#if UNITY_UV_STARTS_AT_TOP
				uv.y = 1 - uv.y;
#endif
				return tex2D(_MainTex, uv);
			}
			ENDCG
		}
	}
}
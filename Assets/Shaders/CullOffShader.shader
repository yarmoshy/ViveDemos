Shader "Custom/CullOffShader" {
	Properties{
		_MainTex("Color (RGB) Alpha (A)", 2D) = "white"
	}
		SubShader{

		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Cull Front

		CGPROGRAM

#pragma surface surf Lambert vertex:vert alpha
		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
		float4 color : COLOR;
	};


	void vert(inout appdata_full v)
	{
		v.normal.xyz = v.normal * -1;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 result = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = result.rgb * result.a;
		o.Alpha = result.a;
	}

	ENDCG

	}

		Fallback "Diffuse"
}

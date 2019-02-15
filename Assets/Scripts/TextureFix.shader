// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TextureFix"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
	}

		SubShader
	{
		Pass
	{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag

		uniform sampler2D _MainTex;
	uniform float4x4 _Rotation;

	struct vertexInput
	{
		float4 vertex : POSITION;
		float4 texcoord : TEXCOORD0;
	};

	struct vertexOutput
	{
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
	};


	vertexOutput vert(vertexInput v)
	{
		vertexOutput o;

		float4 newPosition = UnityObjectToClipPos(mul(_Rotation, v.vertex));

		o.pos = newPosition;
		o.uv = v.texcoord;

		return o;
	}

	float4 frag(vertexOutput i) : COLOR
	{
		return tex2D(_MainTex, i.uv);
	}

		ENDCG
	}
	}
		Fallback "Diffuse"
}
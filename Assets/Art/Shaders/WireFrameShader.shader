Shader "FracturedRealm/WireFrameShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WireframeColor("Wireframe Color", Color) = (1,1,1,1)
        _WireframeWidth("Wireframe Width", float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom

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

            struct g2f
            {
                float4 pos : SV_POSITION;
                float3 barycentric : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _WireframeColor;
            float _WireframeWidth;  

            [maxvertexcount(3)]
            void geom(triangle v2f IN[3], inout TriangleStream<g2f> triStream){
                g2f o;
                o.pos = IN[0].vertex;
                o.barycentric = float3(1, 0, 0);
                triStream.Append(o);
                o.pos = IN[1].vertex;
                o.barycentric = float3(0, 1, 0);
                triStream.Append(o);
                o.pos = IN[2].vertex;
                o.barycentric = float3(0, 0, 1);
                triStream.Append(o);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (g2f i) : SV_Target
            {
                // calculate the width
                float3 unitWidth = fwidth(i.barycentric);
                // calculate the distance to the closest edge
                float3 edge = smoothstep(float3(0,0,0), unitWidth * _WireframeWidth, i.barycentric);
                // calculate aplha
                float alpha = 1 - min(edge.x, min(edge.y, edge.z));
                // return the color
                return fixed4(_WireframeColor.r, _WireframeColor.g, _WireframeColor.b, alpha);
            }
            ENDCG
        }
    }
}

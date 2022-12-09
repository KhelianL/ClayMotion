Shader "SuperSystems/Wireframe-Transparent-Culled"
{
	Properties
	{
		_WireThickness ("Wire Thickness", RANGE(0, 800)) = 100
		_WireSmoothness ("Wire Smoothness", RANGE(0, 20)) = 3
		_WireColor ("Wire Color", Color) = (0.0, 1.0, 0.0, 1.0)
		_MaxTriSize ("Max Tri Size", RANGE(0, 200)) = 25

		_BaseColor ("Base Color", Color) = (0.0, 0.0, 0.0, 0.0)
		_Shininess ("Shininess", Float) = 10
        _SpecColor ("Specular Color", Color) = (1, 1, 1, 1)

		_posLindex("Lindex Position", Vector) = (0.0, 0.0, 0.0)
		_posLmiddle("Lmiddle Position", Vector) = (0.0, 0.0, 0.0)
		_posLpinky("Lpinky Position", Vector) = (0.0, 0.0, 0.0)
		_posLring("Lring Position", Vector) = (0.0, 0.0, 0.0)
		_posLthumb("Lthumb Position", Vector) = (0.0, 0.0, 0.0)
		_posRindex("Rindex Position", Vector) = (0.0, 0.0, 0.0)
		_posRmiddle("Rmiddle Position", Vector) = (0.0, 0.0, 0.0)
		_posRpinky("Rpinky Position", Vector) = (0.0, 0.0, 0.0)
		_posRring("Rring Position", Vector) = (0.0, 0.0, 0.0)
		_posRthumb("Rthumb Position", Vector) = (0.0, 0.0, 0.0)
	}

	SubShader
	{
		Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
			Cull Back

			// Wireframe shader based on the the following
			// http://developer.download.nvidia.com/SDK/10/direct3d/Source/SolidWireframe/Doc/SolidWireframe.pdf
			Tags { "LightMode" = "ForwardBase" } //For the first light

			CGPROGRAM
		
				#pragma vertex vert
				#pragma geometry geom
				#pragma fragment frag

				#include "UnityCG.cginc"
				
				uniform float _WireThickness = 100;
				uniform float _WireSmoothness = 3;
				uniform float4 _WireColor = float4(0.0, 1.0, 0.0, 1.0);
				uniform float _MaxTriSize = 25.0;

				uniform float4 _LightColor0; //From UnityCG
				uniform float4 _BaseColor = float4(0.0, 0.0, 0.0, 0.0);
                uniform float4 _SpecColor;
                uniform float _Shininess;

				uniform float3 _posLindex = float3(0.0, 0.0, 0.0);
				uniform float3 _posLmiddle = float3(0.0, 0.0, 0.0);
				uniform float3 _posLpinky = float3(0.0, 0.0, 0.0);
				uniform float3 _posLring = float3(0.0, 0.0, 0.0);
				uniform float3 _posLthumb = float3(0.0, 0.0, 0.0);

				uniform float3 _posRindex = float3(0.0, 0.0, 0.0);
				uniform float3 _posRmiddle = float3(0.0, 0.0, 0.0);
				uniform float3 _posRpinky = float3(0.0, 0.0, 0.0);
				uniform float3 _posRring = float3(0.0, 0.0, 0.0);
				uniform float3 _posRthumb = float3(0.0, 0.0, 0.0);

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2g
				{
					float4 projectionSpaceVertex : SV_POSITION;
					float4 worldSpacePosition : TEXCOORD1;
					float3 normal : NORMAL;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				struct g2f
				{
					float4 projectionSpaceVertex : SV_POSITION;
					float4 worldSpacePosition : TEXCOORD0;
                    float3 normal : NORMAL;

					float4 dist : TEXCOORD1;
					float4 area : TEXCOORD2;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				v2g vert (appdata v)
				{
					v2g o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					o.projectionSpaceVertex = UnityObjectToClipPos(v.vertex);
					o.worldSpacePosition = mul(unity_ObjectToWorld, v.vertex);

					o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz); //Calculate the normal
					return o;
				}

				[maxvertexcount(3)]
				void geom(triangle v2g i[3], inout TriangleStream<g2f> triangleStream)
				{
					float2 p0 = i[0].projectionSpaceVertex.xy / i[0].projectionSpaceVertex.w;
					float2 p1 = i[1].projectionSpaceVertex.xy / i[1].projectionSpaceVertex.w;
					float2 p2 = i[2].projectionSpaceVertex.xy / i[2].projectionSpaceVertex.w;

					float2 edge0 = p2 - p1;
					float2 edge1 = p2 - p0;
					float2 edge2 = p1 - p0;

					float4 worldEdge0 = i[0].worldSpacePosition - i[1].worldSpacePosition;
					float4 worldEdge1 = i[1].worldSpacePosition - i[2].worldSpacePosition;
					float4 worldEdge2 = i[0].worldSpacePosition - i[2].worldSpacePosition;

					// To find the distance to the opposite edge, we take the
					// formula for finding the area of a triangle Area = Base/2 * Height, 
					// and solve for the Height = (Area * 2)/Base.
					// We can get the area of a triangle by taking its cross product
					// divided by 2.  However we can avoid dividing our area/base by 2
					// since our cross product will already be double our area.
					float area = abs(edge1.x * edge2.y - edge1.y * edge2.x);
					float wireThickness = 800 - _WireThickness;

					g2f o;

					o.normal = i[0].normal;

					o.area = float4(0, 0, 0, 0);
					o.area.x = max(length(worldEdge0), max(length(worldEdge1), length(worldEdge2)));

					o.worldSpacePosition = i[0].worldSpacePosition;
					o.projectionSpaceVertex = i[0].projectionSpaceVertex;
					o.dist.xyz = float3( (area / length(edge0)), 0.0, 0.0) * o.projectionSpaceVertex.w * wireThickness;
					o.dist.w = 1.0 / o.projectionSpaceVertex.w;
					UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(i[0], o);
					triangleStream.Append(o);

					o.worldSpacePosition = i[1].worldSpacePosition;
					o.projectionSpaceVertex = i[1].projectionSpaceVertex;
					o.dist.xyz = float3(0.0, (area / length(edge1)), 0.0) * o.projectionSpaceVertex.w * wireThickness;
					o.dist.w = 1.0 / o.projectionSpaceVertex.w;

					o.normal = i[1].normal;

					UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(i[1], o);
					triangleStream.Append(o);

					o.worldSpacePosition = i[2].worldSpacePosition;
					o.projectionSpaceVertex = i[2].projectionSpaceVertex;
					o.dist.xyz = float3(0.0, 0.0, (area / length(edge2))) * o.projectionSpaceVertex.w * wireThickness;
					o.dist.w = 1.0 / o.projectionSpaceVertex.w;
					
					o.normal = i[2].normal;

					UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(i[2], o);
					triangleStream.Append(o);
				}

				fixed4 frag(g2f i) : SV_Target
				{
					float minDistanceToEdge = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.dist[3];

					float distToTarget0 = length(_posLindex - i.worldSpacePosition);
					float distToTarget1 = length(_posLmiddle - i.worldSpacePosition);
					float distToTarget2 = length(_posLpinky - i.worldSpacePosition);
					float distToTarget3 = length(_posLring - i.worldSpacePosition);
					float distToTarget4 = length(_posLthumb - i.worldSpacePosition);
					float distToTarget5 = length(_posRindex - i.worldSpacePosition);
					float distToTarget6 = length(_posRmiddle - i.worldSpacePosition);
					float distToTarget7 = length(_posRpinky - i.worldSpacePosition);
					float distToTarget8 = length(_posRring - i.worldSpacePosition);
					float distToTarget9 = length(_posRthumb - i.worldSpacePosition);

					float range = 0.05f;
					float minDist = min(distToTarget0,min(distToTarget1,min(distToTarget2,min(distToTarget3,min(distToTarget4,min(distToTarget5,min(distToTarget6,min(distToTarget7,min(distToTarget8,distToTarget9)))))))));
					
					//fixed4 blinPhongColor = fixed4(_BaseColor.rgb,0);
					fixed4 blinPhongColor;
					{
						float3 normalDirection = normalize(i.normal);
						float3 viewDirection = normalize(_WorldSpaceCameraPos - i.worldSpacePosition.xyz);

						float3 vert2LightSource = _WorldSpaceLightPos0.xyz - i.worldSpacePosition.xyz;
						float oneOverDistance = 1.0 / length(vert2LightSource);
						float attenuation = lerp(1.0, oneOverDistance, _WorldSpaceLightPos0.w); //Optimization for spot lights. This isn't needed if you're just getting started.
						float3 lightDirection = _WorldSpaceLightPos0.xyz - i.worldSpacePosition.xyz * _WorldSpaceLightPos0.w;

						float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _BaseColor.rgb; //Ambient component
						float3 diffuseReflection = attenuation * _LightColor0.rgb * _BaseColor.rgb * max(0.0, dot(normalDirection, lightDirection)); //Diffuse component
						float3 specularReflection;
						if (dot(i.normal, lightDirection) < 0.0) //Light on the wrong side - no specular
						{
							specularReflection = float3(0.0, 0.0, 0.0);
						}
						else
						{
							//Specular component
							specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
						}
						blinPhongColor = fixed4((ambientLighting + diffuseReflection)  + specularReflection, 1.0); //Texture is not applient on specularReflection

					}
					
					fixed4 finalColor;
					fixed4 finalWireFrameColor;

					float t = exp2(_WireSmoothness * -1.0 * minDistanceToEdge * minDistanceToEdge);
					finalWireFrameColor = lerp(_BaseColor, _WireColor, t);
					
					float blendRange = range * 0.5f;
					float blend = (minDist - range) / blendRange;
					blend = blend < 0 ? 0 : blend > 1 ? 1 : blend ;
					blend = 1-blend;

					finalColor = lerp(blinPhongColor,finalWireFrameColor,blend);

					return finalColor;
				}

			ENDCG
		}
	}
}

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:34937,y:32430,varname:node_3138,prsc:2|custl-7922-OUT;n:type:ShaderForge.SFN_Color,id:1563,x:33491,y:32596,ptovrint:False,ptlb:BottonColor,ptin:_BottonColor,varname:node_1563,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.7103448,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:5379,x:33491,y:32781,ptovrint:False,ptlb:TopColor,ptin:_TopColor,varname:_node_1563_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.8896552,c3:0,c4:1;n:type:ShaderForge.SFN_TexCoord,id:7508,x:33491,y:32932,varname:node_7508,prsc:2,uv:0;n:type:ShaderForge.SFN_Lerp,id:8152,x:33743,y:32737,varname:node_8152,prsc:2|A-1563-RGB,B-5379-RGB,T-7508-V;n:type:ShaderForge.SFN_Fresnel,id:9184,x:33743,y:33034,varname:node_9184,prsc:2|EXP-1306-OUT;n:type:ShaderForge.SFN_Slider,id:1306,x:33334,y:33095,ptovrint:False,ptlb:Rim Intensity,ptin:_RimIntensity,varname:node_1306,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:15,cur:10,max:0;n:type:ShaderForge.SFN_Color,id:6015,x:33743,y:32885,ptovrint:False,ptlb:Rim Color,ptin:_RimColor,varname:node_6015,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7132353,c2:0.1835532,c3:0.1835532,c4:1;n:type:ShaderForge.SFN_Multiply,id:8162,x:33937,y:32885,varname:node_8162,prsc:2|A-6015-RGB,B-9184-OUT;n:type:ShaderForge.SFN_Add,id:3968,x:34094,y:32752,varname:node_3968,prsc:2|A-8152-OUT,B-8162-OUT;n:type:ShaderForge.SFN_VertexColor,id:9795,x:34094,y:32885,varname:node_9795,prsc:2;n:type:ShaderForge.SFN_Power,id:6191,x:34284,y:32904,varname:node_6191,prsc:2|VAL-9795-R,EXP-1475-OUT;n:type:ShaderForge.SFN_Slider,id:1475,x:33937,y:33029,ptovrint:False,ptlb:Occlusion Intensity,ptin:_OcclusionIntensity,varname:node_1475,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:2699,x:34548,y:32575,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_2699,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:7922,x:34761,y:32671,varname:node_7922,prsc:2|A-2699-RGB,B-5366-OUT;n:type:ShaderForge.SFN_Multiply,id:5366,x:34548,y:32737,varname:node_5366,prsc:2|A-3968-OUT,B-6191-OUT;proporder:2699-5379-1563-1306-6015-1475;pass:END;sub:END;*/

Shader "Lerry/2.0" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _TopColor ("TopColor", Color) = (1,0.8896552,0,1)
        _BottonColor ("BottonColor", Color) = (0,0.7103448,1,1)
        _RimIntensity ("Rim Intensity", Range(15, 0)) = 10
        _RimColor ("Rim Color", Color) = (0.7132353,0.1835532,0.1835532,1)
        _OcclusionIntensity ("Occlusion Intensity", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform float4 _BottonColor;
            uniform float4 _TopColor;
            uniform float _RimIntensity;
            uniform float4 _RimColor;
            uniform float _OcclusionIntensity;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 finalColor = (_Diffuse_var.rgb*((lerp(_BottonColor.rgb,_TopColor.rgb,i.uv0.g)+(_RimColor.rgb*pow(1.0-max(0,dot(normalDirection, viewDirection)),_RimIntensity)))*pow(i.vertexColor.r,_OcclusionIntensity)));
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

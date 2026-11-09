Shader "Lean/Common/Object" {
	Properties {
		_MainTex ("Main Tex", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,1)
		_Color1 ("Color 1", Vector) = (1,0.5,0.5,1)
		_Color2 ("Color 2", Vector) = (0.5,0.5,1,1)
		_Rim ("Rim", Float) = 1
		_Shift ("Shift", Float) = 1
	}
	SubShader {
		Tags { "DisableBatching" = "true" "PreviewType" = "Sphere" "QUEUE" = "Geometry" }
		Pass {
			Tags { "DisableBatching" = "true" "PreviewType" = "Sphere" "QUEUE" = "Geometry" }
			GpuProgramID 12164
			Program "vp" {
				SubProgram "gles3 hw_tier00 " {
					"!!GLES3
					#ifdef VERTEX
					#version 300 es
					
					#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
					#if HLSLCC_ENABLE_UNIFORM_BUFFERS
					#define UNITY_UNIFORM
					#else
					#define UNITY_UNIFORM uniform
					#endif
					#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
					#if UNITY_SUPPORTS_UNIFORM_LOCATION
					#define UNITY_LOCATION(x) layout(location = x)
					#define UNITY_BINDING(x) layout(binding = x, std140)
					#else
					#define UNITY_LOCATION(x)
					#define UNITY_BINDING(x) layout(std140)
					#endif
					uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
					uniform 	vec4 hlslcc_mtx4x4unity_WorldToObject[4];
					uniform 	vec4 hlslcc_mtx4x4unity_MatrixInvV[4];
					uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
					uniform 	vec4 _Color;
					in highp vec4 in_POSITION0;
					in highp vec2 in_TEXCOORD0;
					in highp vec3 in_NORMAL0;
					in highp vec4 in_COLOR0;
					out highp vec2 vs_TEXCOORD0;
					out highp vec3 vs_TEXCOORD1;
					out highp vec4 vs_COLOR0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
					    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
					    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[1].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].yyy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[0].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].xxx + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[2].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].zzz + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[3].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].www + u_xlat0.xyz;
					    vs_TEXCOORD1.x = dot(u_xlat0.xyz, in_NORMAL0.xyz);
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[1].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].yyy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[0].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].xxx + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[2].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].zzz + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[3].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].www + u_xlat0.xyz;
					    vs_TEXCOORD1.y = dot(u_xlat0.xyz, in_NORMAL0.xyz);
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[1].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].yyy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[0].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].xxx + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[2].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].zzz + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[3].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].www + u_xlat0.xyz;
					    vs_TEXCOORD1.z = dot(u_xlat0.xyz, in_NORMAL0.xyz);
					    vs_COLOR0 = in_COLOR0 * _Color;
					    return;
					}
					
					#endif
					#ifdef FRAGMENT
					#version 300 es
					
					precision highp float;
					precision highp int;
					#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
					#if HLSLCC_ENABLE_UNIFORM_BUFFERS
					#define UNITY_UNIFORM
					#else
					#define UNITY_UNIFORM uniform
					#endif
					#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
					#if UNITY_SUPPORTS_UNIFORM_LOCATION
					#define UNITY_LOCATION(x) layout(location = x)
					#define UNITY_BINDING(x) layout(binding = x, std140)
					#else
					#define UNITY_LOCATION(x)
					#define UNITY_BINDING(x) layout(std140)
					#endif
					uniform 	vec4 _Color1;
					uniform 	vec4 _Color2;
					uniform 	float _Rim;
					uniform 	float _Shift;
					UNITY_LOCATION(0) uniform mediump sampler2D _MainTex;
					in highp vec2 vs_TEXCOORD0;
					in highp vec3 vs_TEXCOORD1;
					in highp vec4 vs_COLOR0;
					layout(location = 0) out highp vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					mediump vec4 u_xlat16_1;
					void main()
					{
					    u_xlat0.x = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.x = (-vs_TEXCOORD1.z) * u_xlat0.x + 1.0;
					#ifdef UNITY_ADRENO_ES3
					    u_xlat0.x = min(max(u_xlat0.x, 0.0), 1.0);
					#else
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					#endif
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * _Rim;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat0.x = (-u_xlat0.x) + _Shift;
					    u_xlat1 = (-_Color1) + _Color2;
					    u_xlat0 = u_xlat0.xxxx * u_xlat1 + _Color1;
					    u_xlat16_1 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat0 = u_xlat0 * u_xlat16_1;
					    SV_Target0 = u_xlat0 * vs_COLOR0;
					    return;
					}
					
					#endif"
				}
				SubProgram "gles3 hw_tier01 " {
					"!!GLES3
					#ifdef VERTEX
					#version 300 es
					
					#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
					#if HLSLCC_ENABLE_UNIFORM_BUFFERS
					#define UNITY_UNIFORM
					#else
					#define UNITY_UNIFORM uniform
					#endif
					#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
					#if UNITY_SUPPORTS_UNIFORM_LOCATION
					#define UNITY_LOCATION(x) layout(location = x)
					#define UNITY_BINDING(x) layout(binding = x, std140)
					#else
					#define UNITY_LOCATION(x)
					#define UNITY_BINDING(x) layout(std140)
					#endif
					uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
					uniform 	vec4 hlslcc_mtx4x4unity_WorldToObject[4];
					uniform 	vec4 hlslcc_mtx4x4unity_MatrixInvV[4];
					uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
					uniform 	vec4 _Color;
					in highp vec4 in_POSITION0;
					in highp vec2 in_TEXCOORD0;
					in highp vec3 in_NORMAL0;
					in highp vec4 in_COLOR0;
					out highp vec2 vs_TEXCOORD0;
					out highp vec3 vs_TEXCOORD1;
					out highp vec4 vs_COLOR0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
					    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
					    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[1].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].yyy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[0].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].xxx + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[2].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].zzz + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[3].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].www + u_xlat0.xyz;
					    vs_TEXCOORD1.x = dot(u_xlat0.xyz, in_NORMAL0.xyz);
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[1].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].yyy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[0].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].xxx + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[2].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].zzz + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[3].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].www + u_xlat0.xyz;
					    vs_TEXCOORD1.y = dot(u_xlat0.xyz, in_NORMAL0.xyz);
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[1].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].yyy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[0].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].xxx + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[2].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].zzz + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[3].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].www + u_xlat0.xyz;
					    vs_TEXCOORD1.z = dot(u_xlat0.xyz, in_NORMAL0.xyz);
					    vs_COLOR0 = in_COLOR0 * _Color;
					    return;
					}
					
					#endif
					#ifdef FRAGMENT
					#version 300 es
					
					precision highp float;
					precision highp int;
					#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
					#if HLSLCC_ENABLE_UNIFORM_BUFFERS
					#define UNITY_UNIFORM
					#else
					#define UNITY_UNIFORM uniform
					#endif
					#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
					#if UNITY_SUPPORTS_UNIFORM_LOCATION
					#define UNITY_LOCATION(x) layout(location = x)
					#define UNITY_BINDING(x) layout(binding = x, std140)
					#else
					#define UNITY_LOCATION(x)
					#define UNITY_BINDING(x) layout(std140)
					#endif
					uniform 	vec4 _Color1;
					uniform 	vec4 _Color2;
					uniform 	float _Rim;
					uniform 	float _Shift;
					UNITY_LOCATION(0) uniform mediump sampler2D _MainTex;
					in highp vec2 vs_TEXCOORD0;
					in highp vec3 vs_TEXCOORD1;
					in highp vec4 vs_COLOR0;
					layout(location = 0) out highp vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					mediump vec4 u_xlat16_1;
					void main()
					{
					    u_xlat0.x = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.x = (-vs_TEXCOORD1.z) * u_xlat0.x + 1.0;
					#ifdef UNITY_ADRENO_ES3
					    u_xlat0.x = min(max(u_xlat0.x, 0.0), 1.0);
					#else
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					#endif
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * _Rim;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat0.x = (-u_xlat0.x) + _Shift;
					    u_xlat1 = (-_Color1) + _Color2;
					    u_xlat0 = u_xlat0.xxxx * u_xlat1 + _Color1;
					    u_xlat16_1 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat0 = u_xlat0 * u_xlat16_1;
					    SV_Target0 = u_xlat0 * vs_COLOR0;
					    return;
					}
					
					#endif"
				}
				SubProgram "gles3 hw_tier02 " {
					"!!GLES3
					#ifdef VERTEX
					#version 300 es
					
					#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
					#if HLSLCC_ENABLE_UNIFORM_BUFFERS
					#define UNITY_UNIFORM
					#else
					#define UNITY_UNIFORM uniform
					#endif
					#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
					#if UNITY_SUPPORTS_UNIFORM_LOCATION
					#define UNITY_LOCATION(x) layout(location = x)
					#define UNITY_BINDING(x) layout(binding = x, std140)
					#else
					#define UNITY_LOCATION(x)
					#define UNITY_BINDING(x) layout(std140)
					#endif
					uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
					uniform 	vec4 hlslcc_mtx4x4unity_WorldToObject[4];
					uniform 	vec4 hlslcc_mtx4x4unity_MatrixInvV[4];
					uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
					uniform 	vec4 _Color;
					in highp vec4 in_POSITION0;
					in highp vec2 in_TEXCOORD0;
					in highp vec3 in_NORMAL0;
					in highp vec4 in_COLOR0;
					out highp vec2 vs_TEXCOORD0;
					out highp vec3 vs_TEXCOORD1;
					out highp vec4 vs_COLOR0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
					    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
					    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[1].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].yyy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[0].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].xxx + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[2].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].zzz + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[3].xyz * hlslcc_mtx4x4unity_MatrixInvV[0].www + u_xlat0.xyz;
					    vs_TEXCOORD1.x = dot(u_xlat0.xyz, in_NORMAL0.xyz);
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[1].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].yyy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[0].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].xxx + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[2].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].zzz + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[3].xyz * hlslcc_mtx4x4unity_MatrixInvV[1].www + u_xlat0.xyz;
					    vs_TEXCOORD1.y = dot(u_xlat0.xyz, in_NORMAL0.xyz);
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[1].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].yyy;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[0].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].xxx + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[2].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].zzz + u_xlat0.xyz;
					    u_xlat0.xyz = hlslcc_mtx4x4unity_WorldToObject[3].xyz * hlslcc_mtx4x4unity_MatrixInvV[2].www + u_xlat0.xyz;
					    vs_TEXCOORD1.z = dot(u_xlat0.xyz, in_NORMAL0.xyz);
					    vs_COLOR0 = in_COLOR0 * _Color;
					    return;
					}
					
					#endif
					#ifdef FRAGMENT
					#version 300 es
					
					precision highp float;
					precision highp int;
					#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
					#if HLSLCC_ENABLE_UNIFORM_BUFFERS
					#define UNITY_UNIFORM
					#else
					#define UNITY_UNIFORM uniform
					#endif
					#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
					#if UNITY_SUPPORTS_UNIFORM_LOCATION
					#define UNITY_LOCATION(x) layout(location = x)
					#define UNITY_BINDING(x) layout(binding = x, std140)
					#else
					#define UNITY_LOCATION(x)
					#define UNITY_BINDING(x) layout(std140)
					#endif
					uniform 	vec4 _Color1;
					uniform 	vec4 _Color2;
					uniform 	float _Rim;
					uniform 	float _Shift;
					UNITY_LOCATION(0) uniform mediump sampler2D _MainTex;
					in highp vec2 vs_TEXCOORD0;
					in highp vec3 vs_TEXCOORD1;
					in highp vec4 vs_COLOR0;
					layout(location = 0) out highp vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					mediump vec4 u_xlat16_1;
					void main()
					{
					    u_xlat0.x = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.x = (-vs_TEXCOORD1.z) * u_xlat0.x + 1.0;
					#ifdef UNITY_ADRENO_ES3
					    u_xlat0.x = min(max(u_xlat0.x, 0.0), 1.0);
					#else
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					#endif
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * _Rim;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat0.x = (-u_xlat0.x) + _Shift;
					    u_xlat1 = (-_Color1) + _Color2;
					    u_xlat0 = u_xlat0.xxxx * u_xlat1 + _Color1;
					    u_xlat16_1 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat0 = u_xlat0 * u_xlat16_1;
					    SV_Target0 = u_xlat0 * vs_COLOR0;
					    return;
					}
					
					#endif"
				}
				SubProgram "vulkan hw_tier00 " {
					"spirv
					
					; SPIR-V
					; Version: 1.0
					; Generator: Khronos Glslang Reference Front End; 6
					; Bound: 260
					; Schema: 0
					                                                      OpCapability Shader 
					                                               %1 = OpExtInstImport "GLSL.std.450" 
					                                                      OpMemoryModel Logical GLSL450 
					                                                      OpEntryPoint Vertex %4 "main" %11 %74 %86 %88 %137 %141 %248 %249 
					                                                      OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
					                                                      OpName vs_TEXCOORD1 "vs_TEXCOORD1" 
					                                                      OpDecorate %11 Location 11 
					                                                      OpDecorate %16 ArrayStride 16 
					                                                      OpDecorate %17 ArrayStride 17 
					                                                      OpDecorate %18 ArrayStride 18 
					                                                      OpDecorate %19 ArrayStride 19 
					                                                      OpMemberDecorate %20 0 Offset 20 
					                                                      OpMemberDecorate %20 1 Offset 20 
					                                                      OpMemberDecorate %20 2 Offset 20 
					                                                      OpMemberDecorate %20 3 Offset 20 
					                                                      OpMemberDecorate %20 4 Offset 20 
					                                                      OpDecorate %20 Block 
					                                                      OpDecorate %22 DescriptorSet 22 
					                                                      OpDecorate %22 Binding 22 
					                                                      OpMemberDecorate %72 0 BuiltIn 72 
					                                                      OpMemberDecorate %72 1 BuiltIn 72 
					                                                      OpMemberDecorate %72 2 BuiltIn 72 
					                                                      OpDecorate %72 Block 
					                                                      OpDecorate vs_TEXCOORD0 Location 86 
					                                                      OpDecorate %88 Location 88 
					                                                      OpDecorate vs_TEXCOORD1 Location 137 
					                                                      OpDecorate %141 Location 141 
					                                                      OpDecorate %248 Location 248 
					                                                      OpDecorate %249 Location 249 
					                                               %2 = OpTypeVoid 
					                                               %3 = OpTypeFunction %2 
					                                               %6 = OpTypeFloat 32 
					                                               %7 = OpTypeVector %6 4 
					                                               %8 = OpTypePointer Private %7 
					                                Private f32_4* %9 = OpVariable Private 
					                                              %10 = OpTypePointer Input %7 
					                                 Input f32_4* %11 = OpVariable Input 
					                                              %14 = OpTypeInt 32 0 
					                                          u32 %15 = OpConstant 4 
					                                              %16 = OpTypeArray %7 %15 
					                                              %17 = OpTypeArray %7 %15 
					                                              %18 = OpTypeArray %7 %15 
					                                              %19 = OpTypeArray %7 %15 
					                                              %20 = OpTypeStruct %16 %17 %18 %19 %7 
					                                              %21 = OpTypePointer Uniform %20 
					Uniform struct {f32_4[4]; f32_4[4]; f32_4[4]; f32_4[4]; f32_4;}* %22 = OpVariable Uniform 
					                                              %23 = OpTypeInt 32 1 
					                                          i32 %24 = OpConstant 0 
					                                          i32 %25 = OpConstant 1 
					                                              %26 = OpTypePointer Uniform %7 
					                                          i32 %37 = OpConstant 2 
					                                          i32 %46 = OpConstant 3 
					                               Private f32_4* %50 = OpVariable Private 
					                                          u32 %70 = OpConstant 1 
					                                              %71 = OpTypeArray %6 %70 
					                                              %72 = OpTypeStruct %7 %6 %71 
					                                              %73 = OpTypePointer Output %72 
					         Output struct {f32_4; f32; f32[1];}* %74 = OpVariable Output 
					                                              %82 = OpTypePointer Output %7 
					                                              %84 = OpTypeVector %6 2 
					                                              %85 = OpTypePointer Output %84 
					                       Output f32_2* vs_TEXCOORD0 = OpVariable Output 
					                                              %87 = OpTypePointer Input %84 
					                                 Input f32_2* %88 = OpVariable Input 
					                                              %90 = OpTypeVector %6 3 
					                                             %136 = OpTypePointer Output %90 
					                       Output f32_3* vs_TEXCOORD1 = OpVariable Output 
					                                             %140 = OpTypePointer Input %90 
					                                Input f32_3* %141 = OpVariable Input 
					                                         u32 %144 = OpConstant 0 
					                                             %145 = OpTypePointer Output %6 
					                                         u32 %246 = OpConstant 2 
					                               Output f32_4* %248 = OpVariable Output 
					                                Input f32_4* %249 = OpVariable Input 
					                                         i32 %251 = OpConstant 4 
					                                          void %4 = OpFunction None %3 
					                                               %5 = OpLabel 
					                                        f32_4 %12 = OpLoad %11 
					                                        f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
					                               Uniform f32_4* %27 = OpAccessChain %22 %24 %25 
					                                        f32_4 %28 = OpLoad %27 
					                                        f32_4 %29 = OpFMul %13 %28 
					                                                      OpStore %9 %29 
					                               Uniform f32_4* %30 = OpAccessChain %22 %24 %24 
					                                        f32_4 %31 = OpLoad %30 
					                                        f32_4 %32 = OpLoad %11 
					                                        f32_4 %33 = OpVectorShuffle %32 %32 0 0 0 0 
					                                        f32_4 %34 = OpFMul %31 %33 
					                                        f32_4 %35 = OpLoad %9 
					                                        f32_4 %36 = OpFAdd %34 %35 
					                                                      OpStore %9 %36 
					                               Uniform f32_4* %38 = OpAccessChain %22 %24 %37 
					                                        f32_4 %39 = OpLoad %38 
					                                        f32_4 %40 = OpLoad %11 
					                                        f32_4 %41 = OpVectorShuffle %40 %40 2 2 2 2 
					                                        f32_4 %42 = OpFMul %39 %41 
					                                        f32_4 %43 = OpLoad %9 
					                                        f32_4 %44 = OpFAdd %42 %43 
					                                                      OpStore %9 %44 
					                                        f32_4 %45 = OpLoad %9 
					                               Uniform f32_4* %47 = OpAccessChain %22 %24 %46 
					                                        f32_4 %48 = OpLoad %47 
					                                        f32_4 %49 = OpFAdd %45 %48 
					                                                      OpStore %9 %49 
					                                        f32_4 %51 = OpLoad %9 
					                                        f32_4 %52 = OpVectorShuffle %51 %51 1 1 1 1 
					                               Uniform f32_4* %53 = OpAccessChain %22 %46 %25 
					                                        f32_4 %54 = OpLoad %53 
					                                        f32_4 %55 = OpFMul %52 %54 
					                                                      OpStore %50 %55 
					                               Uniform f32_4* %56 = OpAccessChain %22 %46 %24 
					                                        f32_4 %57 = OpLoad %56 
					                                        f32_4 %58 = OpLoad %9 
					                                        f32_4 %59 = OpVectorShuffle %58 %58 0 0 0 0 
					                                        f32_4 %60 = OpFMul %57 %59 
					                                        f32_4 %61 = OpLoad %50 
					                                        f32_4 %62 = OpFAdd %60 %61 
					                                                      OpStore %50 %62 
					                               Uniform f32_4* %63 = OpAccessChain %22 %46 %37 
					                                        f32_4 %64 = OpLoad %63 
					                                        f32_4 %65 = OpLoad %9 
					                                        f32_4 %66 = OpVectorShuffle %65 %65 2 2 2 2 
					                                        f32_4 %67 = OpFMul %64 %66 
					                                        f32_4 %68 = OpLoad %50 
					                                        f32_4 %69 = OpFAdd %67 %68 
					                                                      OpStore %50 %69 
					                               Uniform f32_4* %75 = OpAccessChain %22 %46 %46 
					                                        f32_4 %76 = OpLoad %75 
					                                        f32_4 %77 = OpLoad %9 
					                                        f32_4 %78 = OpVectorShuffle %77 %77 3 3 3 3 
					                                        f32_4 %79 = OpFMul %76 %78 
					                                        f32_4 %80 = OpLoad %50 
					                                        f32_4 %81 = OpFAdd %79 %80 
					                                Output f32_4* %83 = OpAccessChain %74 %24 
					                                                      OpStore %83 %81 
					                                        f32_2 %89 = OpLoad %88 
					                                                      OpStore vs_TEXCOORD0 %89 
					                               Uniform f32_4* %91 = OpAccessChain %22 %25 %25 
					                                        f32_4 %92 = OpLoad %91 
					                                        f32_3 %93 = OpVectorShuffle %92 %92 0 1 2 
					                               Uniform f32_4* %94 = OpAccessChain %22 %37 %24 
					                                        f32_4 %95 = OpLoad %94 
					                                        f32_3 %96 = OpVectorShuffle %95 %95 1 1 1 
					                                        f32_3 %97 = OpFMul %93 %96 
					                                        f32_4 %98 = OpLoad %9 
					                                        f32_4 %99 = OpVectorShuffle %98 %97 4 5 6 3 
					                                                      OpStore %9 %99 
					                              Uniform f32_4* %100 = OpAccessChain %22 %25 %24 
					                                       f32_4 %101 = OpLoad %100 
					                                       f32_3 %102 = OpVectorShuffle %101 %101 0 1 2 
					                              Uniform f32_4* %103 = OpAccessChain %22 %37 %24 
					                                       f32_4 %104 = OpLoad %103 
					                                       f32_3 %105 = OpVectorShuffle %104 %104 0 0 0 
					                                       f32_3 %106 = OpFMul %102 %105 
					                                       f32_4 %107 = OpLoad %9 
					                                       f32_3 %108 = OpVectorShuffle %107 %107 0 1 2 
					                                       f32_3 %109 = OpFAdd %106 %108 
					                                       f32_4 %110 = OpLoad %9 
					                                       f32_4 %111 = OpVectorShuffle %110 %109 4 5 6 3 
					                                                      OpStore %9 %111 
					                              Uniform f32_4* %112 = OpAccessChain %22 %25 %37 
					                                       f32_4 %113 = OpLoad %112 
					                                       f32_3 %114 = OpVectorShuffle %113 %113 0 1 2 
					                              Uniform f32_4* %115 = OpAccessChain %22 %37 %24 
					                                       f32_4 %116 = OpLoad %115 
					                                       f32_3 %117 = OpVectorShuffle %116 %116 2 2 2 
					                                       f32_3 %118 = OpFMul %114 %117 
					                                       f32_4 %119 = OpLoad %9 
					                                       f32_3 %120 = OpVectorShuffle %119 %119 0 1 2 
					                                       f32_3 %121 = OpFAdd %118 %120 
					                                       f32_4 %122 = OpLoad %9 
					                                       f32_4 %123 = OpVectorShuffle %122 %121 4 5 6 3 
					                                                      OpStore %9 %123 
					                              Uniform f32_4* %124 = OpAccessChain %22 %25 %46 
					                                       f32_4 %125 = OpLoad %124 
					                                       f32_3 %126 = OpVectorShuffle %125 %125 0 1 2 
					                              Uniform f32_4* %127 = OpAccessChain %22 %37 %24 
					                                       f32_4 %128 = OpLoad %127 
					                                       f32_3 %129 = OpVectorShuffle %128 %128 3 3 3 
					                                       f32_3 %130 = OpFMul %126 %129 
					                                       f32_4 %131 = OpLoad %9 
					                                       f32_3 %132 = OpVectorShuffle %131 %131 0 1 2 
					                                       f32_3 %133 = OpFAdd %130 %132 
					                                       f32_4 %134 = OpLoad %9 
					                                       f32_4 %135 = OpVectorShuffle %134 %133 4 5 6 3 
					                                                      OpStore %9 %135 
					                                       f32_4 %138 = OpLoad %9 
					                                       f32_3 %139 = OpVectorShuffle %138 %138 0 1 2 
					                                       f32_3 %142 = OpLoad %141 
					                                         f32 %143 = OpDot %139 %142 
					                                 Output f32* %146 = OpAccessChain vs_TEXCOORD1 %144 
					                                                      OpStore %146 %143 
					                              Uniform f32_4* %147 = OpAccessChain %22 %25 %25 
					                                       f32_4 %148 = OpLoad %147 
					                                       f32_3 %149 = OpVectorShuffle %148 %148 0 1 2 
					                              Uniform f32_4* %150 = OpAccessChain %22 %37 %25 
					                                       f32_4 %151 = OpLoad %150 
					                                       f32_3 %152 = OpVectorShuffle %151 %151 1 1 1 
					                                       f32_3 %153 = OpFMul %149 %152 
					                                       f32_4 %154 = OpLoad %9 
					                                       f32_4 %155 = OpVectorShuffle %154 %153 4 5 6 3 
					                                                      OpStore %9 %155 
					                              Uniform f32_4* %156 = OpAccessChain %22 %25 %24 
					                                       f32_4 %157 = OpLoad %156 
					                                       f32_3 %158 = OpVectorShuffle %157 %157 0 1 2 
					                              Uniform f32_4* %159 = OpAccessChain %22 %37 %25 
					                                       f32_4 %160 = OpLoad %159 
					                                       f32_3 %161 = OpVectorShuffle %160 %160 0 0 0 
					                                       f32_3 %162 = OpFMul %158 %161 
					                                       f32_4 %163 = OpLoad %9 
					                                       f32_3 %164 = OpVectorShuffle %163 %163 0 1 2 
					                                       f32_3 %165 = OpFAdd %162 %164 
					                                       f32_4 %166 = OpLoad %9 
					                                       f32_4 %167 = OpVectorShuffle %166 %165 4 5 6 3 
					                                                      OpStore %9 %167 
					                              Uniform f32_4* %168 = OpAccessChain %22 %25 %37 
					                                       f32_4 %169 = OpLoad %168 
					                                       f32_3 %170 = OpVectorShuffle %169 %169 0 1 2 
					                              Uniform f32_4* %171 = OpAccessChain %22 %37 %25 
					                                       f32_4 %172 = OpLoad %171 
					                                       f32_3 %173 = OpVectorShuffle %172 %172 2 2 2 
					                                       f32_3 %174 = OpFMul %170 %173 
					                                       f32_4 %175 = OpLoad %9 
					                                       f32_3 %176 = OpVectorShuffle %175 %175 0 1 2 
					                                       f32_3 %177 = OpFAdd %174 %176 
					                                       f32_4 %178 = OpLoad %9 
					                                       f32_4 %179 = OpVectorShuffle %178 %177 4 5 6 3 
					                                                      OpStore %9 %179 
					                              Uniform f32_4* %180 = OpAccessChain %22 %25 %46 
					                                       f32_4 %181 = OpLoad %180 
					                                       f32_3 %182 = OpVectorShuffle %181 %181 0 1 2 
					                              Uniform f32_4* %183 = OpAccessChain %22 %37 %25 
					                                       f32_4 %184 = OpLoad %183 
					                                       f32_3 %185 = OpVectorShuffle %184 %184 3 3 3 
					                                       f32_3 %186 = OpFMul %182 %185 
					                                       f32_4 %187 = OpLoad %9 
					                                       f32_3 %188 = OpVectorShuffle %187 %187 0 1 2 
					                                       f32_3 %189 = OpFAdd %186 %188 
					                                       f32_4 %190 = OpLoad %9 
					                                       f32_4 %191 = OpVectorShuffle %190 %189 4 5 6 3 
					                                                      OpStore %9 %191 
					                                       f32_4 %192 = OpLoad %9 
					                                       f32_3 %193 = OpVectorShuffle %192 %192 0 1 2 
					                                       f32_3 %194 = OpLoad %141 
					                                         f32 %195 = OpDot %193 %194 
					                                 Output f32* %196 = OpAccessChain vs_TEXCOORD1 %70 
					                                                      OpStore %196 %195 
					                              Uniform f32_4* %197 = OpAccessChain %22 %25 %25 
					                                       f32_4 %198 = OpLoad %197 
					                                       f32_3 %199 = OpVectorShuffle %198 %198 0 1 2 
					                              Uniform f32_4* %200 = OpAccessChain %22 %37 %37 
					                                       f32_4 %201 = OpLoad %200 
					                                       f32_3 %202 = OpVectorShuffle %201 %201 1 1 1 
					                                       f32_3 %203 = OpFMul %199 %202 
					                                       f32_4 %204 = OpLoad %9 
					                                       f32_4 %205 = OpVectorShuffle %204 %203 4 5 6 3 
					                                                      OpStore %9 %205 
					                              Uniform f32_4* %206 = OpAccessChain %22 %25 %24 
					                                       f32_4 %207 = OpLoad %206 
					                                       f32_3 %208 = OpVectorShuffle %207 %207 0 1 2 
					                              Uniform f32_4* %209 = OpAccessChain %22 %37 %37 
					                                       f32_4 %210 = OpLoad %209 
					                                       f32_3 %211 = OpVectorShuffle %210 %210 0 0 0 
					                                       f32_3 %212 = OpFMul %208 %211 
					                                       f32_4 %213 = OpLoad %9 
					                                       f32_3 %214 = OpVectorShuffle %213 %213 0 1 2 
					                                       f32_3 %215 = OpFAdd %212 %214 
					                                       f32_4 %216 = OpLoad %9 
					                                       f32_4 %217 = OpVectorShuffle %216 %215 4 5 6 3 
					                                                      OpStore %9 %217 
					                              Uniform f32_4* %218 = OpAccessChain %22 %25 %37 
					                                       f32_4 %219 = OpLoad %218 
					                                       f32_3 %220 = OpVectorShuffle %219 %219 0 1 2 
					                              Uniform f32_4* %221 = OpAccessChain %22 %37 %37 
					                                       f32_4 %222 = OpLoad %221 
					                                       f32_3 %223 = OpVectorShuffle %222 %222 2 2 2 
					                                       f32_3 %224 = OpFMul %220 %223 
					                                       f32_4 %225 = OpLoad %9 
					                                       f32_3 %226 = OpVectorShuffle %225 %225 0 1 2 
					                                       f32_3 %227 = OpFAdd %224 %226 
					                                       f32_4 %228 = OpLoad %9 
					                                       f32_4 %229 = OpVectorShuffle %228 %227 4 5 6 3 
					                                                      OpStore %9 %229 
					                              Uniform f32_4* %230 = OpAccessChain %22 %25 %46 
					                                       f32_4 %231 = OpLoad %230 
					                                       f32_3 %232 = OpVectorShuffle %231 %231 0 1 2 
					                              Uniform f32_4* %233 = OpAccessChain %22 %37 %37 
					                                       f32_4 %234 = OpLoad %233 
					                                       f32_3 %235 = OpVectorShuffle %234 %234 3 3 3 
					                                       f32_3 %236 = OpFMul %232 %235 
					                                       f32_4 %237 = OpLoad %9 
					                                       f32_3 %238 = OpVectorShuffle %237 %237 0 1 2 
					                                       f32_3 %239 = OpFAdd %236 %238 
					                                       f32_4 %240 = OpLoad %9 
					                                       f32_4 %241 = OpVectorShuffle %240 %239 4 5 6 3 
					                                                      OpStore %9 %241 
					                                       f32_4 %242 = OpLoad %9 
					                                       f32_3 %243 = OpVectorShuffle %242 %242 0 1 2 
					                                       f32_3 %244 = OpLoad %141 
					                                         f32 %245 = OpDot %243 %244 
					                                 Output f32* %247 = OpAccessChain vs_TEXCOORD1 %246 
					                                                      OpStore %247 %245 
					                                       f32_4 %250 = OpLoad %249 
					                              Uniform f32_4* %252 = OpAccessChain %22 %251 
					                                       f32_4 %253 = OpLoad %252 
					                                       f32_4 %254 = OpFMul %250 %253 
					                                                      OpStore %248 %254 
					                                 Output f32* %255 = OpAccessChain %74 %24 %70 
					                                         f32 %256 = OpLoad %255 
					                                         f32 %257 = OpFNegate %256 
					                                 Output f32* %258 = OpAccessChain %74 %24 %70 
					                                                      OpStore %258 %257 
					                                                      OpReturn
					                                                      OpFunctionEnd
					; SPIR-V
					; Version: 1.0
					; Generator: Khronos Glslang Reference Front End; 6
					; Bound: 112
					; Schema: 0
					                                                  OpCapability Shader 
					                                           %1 = OpExtInstImport "GLSL.std.450" 
					                                                  OpMemoryModel Logical GLSL450 
					                                                  OpEntryPoint Fragment %4 "main" %12 %98 %105 %108 
					                                                  OpExecutionMode %4 OriginUpperLeft 
					                                                  OpName vs_TEXCOORD1 "vs_TEXCOORD1" 
					                                                  OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
					                                                  OpDecorate vs_TEXCOORD1 Location 12 
					                                                  OpMemberDecorate %46 0 Offset 46 
					                                                  OpMemberDecorate %46 1 Offset 46 
					                                                  OpMemberDecorate %46 2 Offset 46 
					                                                  OpMemberDecorate %46 3 Offset 46 
					                                                  OpDecorate %46 Block 
					                                                  OpDecorate %48 DescriptorSet 48 
					                                                  OpDecorate %48 Binding 48 
					                                                  OpDecorate %85 RelaxedPrecision 
					                                                  OpDecorate %88 RelaxedPrecision 
					                                                  OpDecorate %88 DescriptorSet 88 
					                                                  OpDecorate %88 Binding 88 
					                                                  OpDecorate %89 RelaxedPrecision 
					                                                  OpDecorate %92 RelaxedPrecision 
					                                                  OpDecorate %92 DescriptorSet 92 
					                                                  OpDecorate %92 Binding 92 
					                                                  OpDecorate %93 RelaxedPrecision 
					                                                  OpDecorate vs_TEXCOORD0 Location 98 
					                                                  OpDecorate %102 RelaxedPrecision 
					                                                  OpDecorate %105 Location 105 
					                                                  OpDecorate %108 Location 108 
					                                           %2 = OpTypeVoid 
					                                           %3 = OpTypeFunction %2 
					                                           %6 = OpTypeFloat 32 
					                                           %7 = OpTypeVector %6 4 
					                                           %8 = OpTypePointer Private %7 
					                            Private f32_4* %9 = OpVariable Private 
					                                          %10 = OpTypeVector %6 3 
					                                          %11 = OpTypePointer Input %10 
					                    Input f32_3* vs_TEXCOORD1 = OpVariable Input 
					                                          %16 = OpTypeInt 32 0 
					                                      u32 %17 = OpConstant 0 
					                                          %18 = OpTypePointer Private %6 
					                                      u32 %24 = OpConstant 2 
					                                          %25 = OpTypePointer Input %6 
					                                      f32 %32 = OpConstant 3.674022E-40 
					                                      f32 %37 = OpConstant 3.674022E-40 
					                                          %46 = OpTypeStruct %7 %7 %6 %6 
					                                          %47 = OpTypePointer Uniform %46 
					Uniform struct {f32_4; f32_4; f32; f32;}* %48 = OpVariable Uniform 
					                                          %49 = OpTypeInt 32 1 
					                                      i32 %50 = OpConstant 2 
					                                          %51 = OpTypePointer Uniform %6 
					                                      i32 %63 = OpConstant 3 
					                           Private f32_4* %68 = OpVariable Private 
					                                      i32 %69 = OpConstant 0 
					                                          %70 = OpTypePointer Uniform %7 
					                                      i32 %74 = OpConstant 1 
					                           Private f32_4* %85 = OpVariable Private 
					                                          %86 = OpTypeImage %6 Dim2D 0 0 0 1 Unknown 
					                                          %87 = OpTypePointer UniformConstant %86 
					     UniformConstant read_only Texture2D* %88 = OpVariable UniformConstant 
					                                          %90 = OpTypeSampler 
					                                          %91 = OpTypePointer UniformConstant %90 
					                 UniformConstant sampler* %92 = OpVariable UniformConstant 
					                                          %94 = OpTypeSampledImage %86 
					                                          %96 = OpTypeVector %6 2 
					                                          %97 = OpTypePointer Input %96 
					                    Input f32_2* vs_TEXCOORD0 = OpVariable Input 
					                                         %104 = OpTypePointer Output %7 
					                           Output f32_4* %105 = OpVariable Output 
					                                         %107 = OpTypePointer Input %7 
					                            Input f32_4* %108 = OpVariable Input 
					                                      void %4 = OpFunction None %3 
					                                           %5 = OpLabel 
					                                    f32_3 %13 = OpLoad vs_TEXCOORD1 
					                                    f32_3 %14 = OpLoad vs_TEXCOORD1 
					                                      f32 %15 = OpDot %13 %14 
					                             Private f32* %19 = OpAccessChain %9 %17 
					                                                  OpStore %19 %15 
					                             Private f32* %20 = OpAccessChain %9 %17 
					                                      f32 %21 = OpLoad %20 
					                                      f32 %22 = OpExtInst %1 32 %21 
					                             Private f32* %23 = OpAccessChain %9 %17 
					                                                  OpStore %23 %22 
					                               Input f32* %26 = OpAccessChain vs_TEXCOORD1 %24 
					                                      f32 %27 = OpLoad %26 
					                                      f32 %28 = OpFNegate %27 
					                             Private f32* %29 = OpAccessChain %9 %17 
					                                      f32 %30 = OpLoad %29 
					                                      f32 %31 = OpFMul %28 %30 
					                                      f32 %33 = OpFAdd %31 %32 
					                             Private f32* %34 = OpAccessChain %9 %17 
					                                                  OpStore %34 %33 
					                             Private f32* %35 = OpAccessChain %9 %17 
					                                      f32 %36 = OpLoad %35 
					                                      f32 %38 = OpExtInst %1 43 %36 %37 %32 
					                             Private f32* %39 = OpAccessChain %9 %17 
					                                                  OpStore %39 %38 
					                             Private f32* %40 = OpAccessChain %9 %17 
					                                      f32 %41 = OpLoad %40 
					                                      f32 %42 = OpExtInst %1 30 %41 
					                             Private f32* %43 = OpAccessChain %9 %17 
					                                                  OpStore %43 %42 
					                             Private f32* %44 = OpAccessChain %9 %17 
					                                      f32 %45 = OpLoad %44 
					                             Uniform f32* %52 = OpAccessChain %48 %50 
					                                      f32 %53 = OpLoad %52 
					                                      f32 %54 = OpFMul %45 %53 
					                             Private f32* %55 = OpAccessChain %9 %17 
					                                                  OpStore %55 %54 
					                             Private f32* %56 = OpAccessChain %9 %17 
					                                      f32 %57 = OpLoad %56 
					                                      f32 %58 = OpExtInst %1 29 %57 
					                             Private f32* %59 = OpAccessChain %9 %17 
					                                                  OpStore %59 %58 
					                             Private f32* %60 = OpAccessChain %9 %17 
					                                      f32 %61 = OpLoad %60 
					                                      f32 %62 = OpFNegate %61 
					                             Uniform f32* %64 = OpAccessChain %48 %63 
					                                      f32 %65 = OpLoad %64 
					                                      f32 %66 = OpFAdd %62 %65 
					                             Private f32* %67 = OpAccessChain %9 %17 
					                                                  OpStore %67 %66 
					                           Uniform f32_4* %71 = OpAccessChain %48 %69 
					                                    f32_4 %72 = OpLoad %71 
					                                    f32_4 %73 = OpFNegate %72 
					                           Uniform f32_4* %75 = OpAccessChain %48 %74 
					                                    f32_4 %76 = OpLoad %75 
					                                    f32_4 %77 = OpFAdd %73 %76 
					                                                  OpStore %68 %77 
					                                    f32_4 %78 = OpLoad %9 
					                                    f32_4 %79 = OpVectorShuffle %78 %78 0 0 0 0 
					                                    f32_4 %80 = OpLoad %68 
					                                    f32_4 %81 = OpFMul %79 %80 
					                           Uniform f32_4* %82 = OpAccessChain %48 %69 
					                                    f32_4 %83 = OpLoad %82 
					                                    f32_4 %84 = OpFAdd %81 %83 
					                                                  OpStore %9 %84 
					                      read_only Texture2D %89 = OpLoad %88 
					                                  sampler %93 = OpLoad %92 
					               read_only Texture2DSampled %95 = OpSampledImage %89 %93 
					                                    f32_2 %99 = OpLoad vs_TEXCOORD0 
					                                   f32_4 %100 = OpImageSampleImplicitLod %95 %99 
					                                                  OpStore %85 %100 
					                                   f32_4 %101 = OpLoad %9 
					                                   f32_4 %102 = OpLoad %85 
					                                   f32_4 %103 = OpFMul %101 %102 
					                                                  OpStore %9 %103 
					                                   f32_4 %106 = OpLoad %9 
					                                   f32_4 %109 = OpLoad %108 
					                                   f32_4 %110 = OpFMul %106 %109 
					                                                  OpStore %105 %110 
					                                                  OpReturn
					                                                  OpFunctionEnd"
				}
				SubProgram "vulkan hw_tier01 " {
					"spirv
					
					; SPIR-V
					; Version: 1.0
					; Generator: Khronos Glslang Reference Front End; 6
					; Bound: 260
					; Schema: 0
					                                                      OpCapability Shader 
					                                               %1 = OpExtInstImport "GLSL.std.450" 
					                                                      OpMemoryModel Logical GLSL450 
					                                                      OpEntryPoint Vertex %4 "main" %11 %74 %86 %88 %137 %141 %248 %249 
					                                                      OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
					                                                      OpName vs_TEXCOORD1 "vs_TEXCOORD1" 
					                                                      OpDecorate %11 Location 11 
					                                                      OpDecorate %16 ArrayStride 16 
					                                                      OpDecorate %17 ArrayStride 17 
					                                                      OpDecorate %18 ArrayStride 18 
					                                                      OpDecorate %19 ArrayStride 19 
					                                                      OpMemberDecorate %20 0 Offset 20 
					                                                      OpMemberDecorate %20 1 Offset 20 
					                                                      OpMemberDecorate %20 2 Offset 20 
					                                                      OpMemberDecorate %20 3 Offset 20 
					                                                      OpMemberDecorate %20 4 Offset 20 
					                                                      OpDecorate %20 Block 
					                                                      OpDecorate %22 DescriptorSet 22 
					                                                      OpDecorate %22 Binding 22 
					                                                      OpMemberDecorate %72 0 BuiltIn 72 
					                                                      OpMemberDecorate %72 1 BuiltIn 72 
					                                                      OpMemberDecorate %72 2 BuiltIn 72 
					                                                      OpDecorate %72 Block 
					                                                      OpDecorate vs_TEXCOORD0 Location 86 
					                                                      OpDecorate %88 Location 88 
					                                                      OpDecorate vs_TEXCOORD1 Location 137 
					                                                      OpDecorate %141 Location 141 
					                                                      OpDecorate %248 Location 248 
					                                                      OpDecorate %249 Location 249 
					                                               %2 = OpTypeVoid 
					                                               %3 = OpTypeFunction %2 
					                                               %6 = OpTypeFloat 32 
					                                               %7 = OpTypeVector %6 4 
					                                               %8 = OpTypePointer Private %7 
					                                Private f32_4* %9 = OpVariable Private 
					                                              %10 = OpTypePointer Input %7 
					                                 Input f32_4* %11 = OpVariable Input 
					                                              %14 = OpTypeInt 32 0 
					                                          u32 %15 = OpConstant 4 
					                                              %16 = OpTypeArray %7 %15 
					                                              %17 = OpTypeArray %7 %15 
					                                              %18 = OpTypeArray %7 %15 
					                                              %19 = OpTypeArray %7 %15 
					                                              %20 = OpTypeStruct %16 %17 %18 %19 %7 
					                                              %21 = OpTypePointer Uniform %20 
					Uniform struct {f32_4[4]; f32_4[4]; f32_4[4]; f32_4[4]; f32_4;}* %22 = OpVariable Uniform 
					                                              %23 = OpTypeInt 32 1 
					                                          i32 %24 = OpConstant 0 
					                                          i32 %25 = OpConstant 1 
					                                              %26 = OpTypePointer Uniform %7 
					                                          i32 %37 = OpConstant 2 
					                                          i32 %46 = OpConstant 3 
					                               Private f32_4* %50 = OpVariable Private 
					                                          u32 %70 = OpConstant 1 
					                                              %71 = OpTypeArray %6 %70 
					                                              %72 = OpTypeStruct %7 %6 %71 
					                                              %73 = OpTypePointer Output %72 
					         Output struct {f32_4; f32; f32[1];}* %74 = OpVariable Output 
					                                              %82 = OpTypePointer Output %7 
					                                              %84 = OpTypeVector %6 2 
					                                              %85 = OpTypePointer Output %84 
					                       Output f32_2* vs_TEXCOORD0 = OpVariable Output 
					                                              %87 = OpTypePointer Input %84 
					                                 Input f32_2* %88 = OpVariable Input 
					                                              %90 = OpTypeVector %6 3 
					                                             %136 = OpTypePointer Output %90 
					                       Output f32_3* vs_TEXCOORD1 = OpVariable Output 
					                                             %140 = OpTypePointer Input %90 
					                                Input f32_3* %141 = OpVariable Input 
					                                         u32 %144 = OpConstant 0 
					                                             %145 = OpTypePointer Output %6 
					                                         u32 %246 = OpConstant 2 
					                               Output f32_4* %248 = OpVariable Output 
					                                Input f32_4* %249 = OpVariable Input 
					                                         i32 %251 = OpConstant 4 
					                                          void %4 = OpFunction None %3 
					                                               %5 = OpLabel 
					                                        f32_4 %12 = OpLoad %11 
					                                        f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
					                               Uniform f32_4* %27 = OpAccessChain %22 %24 %25 
					                                        f32_4 %28 = OpLoad %27 
					                                        f32_4 %29 = OpFMul %13 %28 
					                                                      OpStore %9 %29 
					                               Uniform f32_4* %30 = OpAccessChain %22 %24 %24 
					                                        f32_4 %31 = OpLoad %30 
					                                        f32_4 %32 = OpLoad %11 
					                                        f32_4 %33 = OpVectorShuffle %32 %32 0 0 0 0 
					                                        f32_4 %34 = OpFMul %31 %33 
					                                        f32_4 %35 = OpLoad %9 
					                                        f32_4 %36 = OpFAdd %34 %35 
					                                                      OpStore %9 %36 
					                               Uniform f32_4* %38 = OpAccessChain %22 %24 %37 
					                                        f32_4 %39 = OpLoad %38 
					                                        f32_4 %40 = OpLoad %11 
					                                        f32_4 %41 = OpVectorShuffle %40 %40 2 2 2 2 
					                                        f32_4 %42 = OpFMul %39 %41 
					                                        f32_4 %43 = OpLoad %9 
					                                        f32_4 %44 = OpFAdd %42 %43 
					                                                      OpStore %9 %44 
					                                        f32_4 %45 = OpLoad %9 
					                               Uniform f32_4* %47 = OpAccessChain %22 %24 %46 
					                                        f32_4 %48 = OpLoad %47 
					                                        f32_4 %49 = OpFAdd %45 %48 
					                                                      OpStore %9 %49 
					                                        f32_4 %51 = OpLoad %9 
					                                        f32_4 %52 = OpVectorShuffle %51 %51 1 1 1 1 
					                               Uniform f32_4* %53 = OpAccessChain %22 %46 %25 
					                                        f32_4 %54 = OpLoad %53 
					                                        f32_4 %55 = OpFMul %52 %54 
					                                                      OpStore %50 %55 
					                               Uniform f32_4* %56 = OpAccessChain %22 %46 %24 
					                                        f32_4 %57 = OpLoad %56 
					                                        f32_4 %58 = OpLoad %9 
					                                        f32_4 %59 = OpVectorShuffle %58 %58 0 0 0 0 
					                                        f32_4 %60 = OpFMul %57 %59 
					                                        f32_4 %61 = OpLoad %50 
					                                        f32_4 %62 = OpFAdd %60 %61 
					                                                      OpStore %50 %62 
					                               Uniform f32_4* %63 = OpAccessChain %22 %46 %37 
					                                        f32_4 %64 = OpLoad %63 
					                                        f32_4 %65 = OpLoad %9 
					                                        f32_4 %66 = OpVectorShuffle %65 %65 2 2 2 2 
					                                        f32_4 %67 = OpFMul %64 %66 
					                                        f32_4 %68 = OpLoad %50 
					                                        f32_4 %69 = OpFAdd %67 %68 
					                                                      OpStore %50 %69 
					                               Uniform f32_4* %75 = OpAccessChain %22 %46 %46 
					                                        f32_4 %76 = OpLoad %75 
					                                        f32_4 %77 = OpLoad %9 
					                                        f32_4 %78 = OpVectorShuffle %77 %77 3 3 3 3 
					                                        f32_4 %79 = OpFMul %76 %78 
					                                        f32_4 %80 = OpLoad %50 
					                                        f32_4 %81 = OpFAdd %79 %80 
					                                Output f32_4* %83 = OpAccessChain %74 %24 
					                                                      OpStore %83 %81 
					                                        f32_2 %89 = OpLoad %88 
					                                                      OpStore vs_TEXCOORD0 %89 
					                               Uniform f32_4* %91 = OpAccessChain %22 %25 %25 
					                                        f32_4 %92 = OpLoad %91 
					                                        f32_3 %93 = OpVectorShuffle %92 %92 0 1 2 
					                               Uniform f32_4* %94 = OpAccessChain %22 %37 %24 
					                                        f32_4 %95 = OpLoad %94 
					                                        f32_3 %96 = OpVectorShuffle %95 %95 1 1 1 
					                                        f32_3 %97 = OpFMul %93 %96 
					                                        f32_4 %98 = OpLoad %9 
					                                        f32_4 %99 = OpVectorShuffle %98 %97 4 5 6 3 
					                                                      OpStore %9 %99 
					                              Uniform f32_4* %100 = OpAccessChain %22 %25 %24 
					                                       f32_4 %101 = OpLoad %100 
					                                       f32_3 %102 = OpVectorShuffle %101 %101 0 1 2 
					                              Uniform f32_4* %103 = OpAccessChain %22 %37 %24 
					                                       f32_4 %104 = OpLoad %103 
					                                       f32_3 %105 = OpVectorShuffle %104 %104 0 0 0 
					                                       f32_3 %106 = OpFMul %102 %105 
					                                       f32_4 %107 = OpLoad %9 
					                                       f32_3 %108 = OpVectorShuffle %107 %107 0 1 2 
					                                       f32_3 %109 = OpFAdd %106 %108 
					                                       f32_4 %110 = OpLoad %9 
					                                       f32_4 %111 = OpVectorShuffle %110 %109 4 5 6 3 
					                                                      OpStore %9 %111 
					                              Uniform f32_4* %112 = OpAccessChain %22 %25 %37 
					                                       f32_4 %113 = OpLoad %112 
					                                       f32_3 %114 = OpVectorShuffle %113 %113 0 1 2 
					                              Uniform f32_4* %115 = OpAccessChain %22 %37 %24 
					                                       f32_4 %116 = OpLoad %115 
					                                       f32_3 %117 = OpVectorShuffle %116 %116 2 2 2 
					                                       f32_3 %118 = OpFMul %114 %117 
					                                       f32_4 %119 = OpLoad %9 
					                                       f32_3 %120 = OpVectorShuffle %119 %119 0 1 2 
					                                       f32_3 %121 = OpFAdd %118 %120 
					                                       f32_4 %122 = OpLoad %9 
					                                       f32_4 %123 = OpVectorShuffle %122 %121 4 5 6 3 
					                                                      OpStore %9 %123 
					                              Uniform f32_4* %124 = OpAccessChain %22 %25 %46 
					                                       f32_4 %125 = OpLoad %124 
					                                       f32_3 %126 = OpVectorShuffle %125 %125 0 1 2 
					                              Uniform f32_4* %127 = OpAccessChain %22 %37 %24 
					                                       f32_4 %128 = OpLoad %127 
					                                       f32_3 %129 = OpVectorShuffle %128 %128 3 3 3 
					                                       f32_3 %130 = OpFMul %126 %129 
					                                       f32_4 %131 = OpLoad %9 
					                                       f32_3 %132 = OpVectorShuffle %131 %131 0 1 2 
					                                       f32_3 %133 = OpFAdd %130 %132 
					                                       f32_4 %134 = OpLoad %9 
					                                       f32_4 %135 = OpVectorShuffle %134 %133 4 5 6 3 
					                                                      OpStore %9 %135 
					                                       f32_4 %138 = OpLoad %9 
					                                       f32_3 %139 = OpVectorShuffle %138 %138 0 1 2 
					                                       f32_3 %142 = OpLoad %141 
					                                         f32 %143 = OpDot %139 %142 
					                                 Output f32* %146 = OpAccessChain vs_TEXCOORD1 %144 
					                                                      OpStore %146 %143 
					                              Uniform f32_4* %147 = OpAccessChain %22 %25 %25 
					                                       f32_4 %148 = OpLoad %147 
					                                       f32_3 %149 = OpVectorShuffle %148 %148 0 1 2 
					                              Uniform f32_4* %150 = OpAccessChain %22 %37 %25 
					                                       f32_4 %151 = OpLoad %150 
					                                       f32_3 %152 = OpVectorShuffle %151 %151 1 1 1 
					                                       f32_3 %153 = OpFMul %149 %152 
					                                       f32_4 %154 = OpLoad %9 
					                                       f32_4 %155 = OpVectorShuffle %154 %153 4 5 6 3 
					                                                      OpStore %9 %155 
					                              Uniform f32_4* %156 = OpAccessChain %22 %25 %24 
					                                       f32_4 %157 = OpLoad %156 
					                                       f32_3 %158 = OpVectorShuffle %157 %157 0 1 2 
					                              Uniform f32_4* %159 = OpAccessChain %22 %37 %25 
					                                       f32_4 %160 = OpLoad %159 
					                                       f32_3 %161 = OpVectorShuffle %160 %160 0 0 0 
					                                       f32_3 %162 = OpFMul %158 %161 
					                                       f32_4 %163 = OpLoad %9 
					                                       f32_3 %164 = OpVectorShuffle %163 %163 0 1 2 
					                                       f32_3 %165 = OpFAdd %162 %164 
					                                       f32_4 %166 = OpLoad %9 
					                                       f32_4 %167 = OpVectorShuffle %166 %165 4 5 6 3 
					                                                      OpStore %9 %167 
					                              Uniform f32_4* %168 = OpAccessChain %22 %25 %37 
					                                       f32_4 %169 = OpLoad %168 
					                                       f32_3 %170 = OpVectorShuffle %169 %169 0 1 2 
					                              Uniform f32_4* %171 = OpAccessChain %22 %37 %25 
					                                       f32_4 %172 = OpLoad %171 
					                                       f32_3 %173 = OpVectorShuffle %172 %172 2 2 2 
					                                       f32_3 %174 = OpFMul %170 %173 
					                                       f32_4 %175 = OpLoad %9 
					                                       f32_3 %176 = OpVectorShuffle %175 %175 0 1 2 
					                                       f32_3 %177 = OpFAdd %174 %176 
					                                       f32_4 %178 = OpLoad %9 
					                                       f32_4 %179 = OpVectorShuffle %178 %177 4 5 6 3 
					                                                      OpStore %9 %179 
					                              Uniform f32_4* %180 = OpAccessChain %22 %25 %46 
					                                       f32_4 %181 = OpLoad %180 
					                                       f32_3 %182 = OpVectorShuffle %181 %181 0 1 2 
					                              Uniform f32_4* %183 = OpAccessChain %22 %37 %25 
					                                       f32_4 %184 = OpLoad %183 
					                                       f32_3 %185 = OpVectorShuffle %184 %184 3 3 3 
					                                       f32_3 %186 = OpFMul %182 %185 
					                                       f32_4 %187 = OpLoad %9 
					                                       f32_3 %188 = OpVectorShuffle %187 %187 0 1 2 
					                                       f32_3 %189 = OpFAdd %186 %188 
					                                       f32_4 %190 = OpLoad %9 
					                                       f32_4 %191 = OpVectorShuffle %190 %189 4 5 6 3 
					                                                      OpStore %9 %191 
					                                       f32_4 %192 = OpLoad %9 
					                                       f32_3 %193 = OpVectorShuffle %192 %192 0 1 2 
					                                       f32_3 %194 = OpLoad %141 
					                                         f32 %195 = OpDot %193 %194 
					                                 Output f32* %196 = OpAccessChain vs_TEXCOORD1 %70 
					                                                      OpStore %196 %195 
					                              Uniform f32_4* %197 = OpAccessChain %22 %25 %25 
					                                       f32_4 %198 = OpLoad %197 
					                                       f32_3 %199 = OpVectorShuffle %198 %198 0 1 2 
					                              Uniform f32_4* %200 = OpAccessChain %22 %37 %37 
					                                       f32_4 %201 = OpLoad %200 
					                                       f32_3 %202 = OpVectorShuffle %201 %201 1 1 1 
					                                       f32_3 %203 = OpFMul %199 %202 
					                                       f32_4 %204 = OpLoad %9 
					                                       f32_4 %205 = OpVectorShuffle %204 %203 4 5 6 3 
					                                                      OpStore %9 %205 
					                              Uniform f32_4* %206 = OpAccessChain %22 %25 %24 
					                                       f32_4 %207 = OpLoad %206 
					                                       f32_3 %208 = OpVectorShuffle %207 %207 0 1 2 
					                              Uniform f32_4* %209 = OpAccessChain %22 %37 %37 
					                                       f32_4 %210 = OpLoad %209 
					                                       f32_3 %211 = OpVectorShuffle %210 %210 0 0 0 
					                                       f32_3 %212 = OpFMul %208 %211 
					                                       f32_4 %213 = OpLoad %9 
					                                       f32_3 %214 = OpVectorShuffle %213 %213 0 1 2 
					                                       f32_3 %215 = OpFAdd %212 %214 
					                                       f32_4 %216 = OpLoad %9 
					                                       f32_4 %217 = OpVectorShuffle %216 %215 4 5 6 3 
					                                                      OpStore %9 %217 
					                              Uniform f32_4* %218 = OpAccessChain %22 %25 %37 
					                                       f32_4 %219 = OpLoad %218 
					                                       f32_3 %220 = OpVectorShuffle %219 %219 0 1 2 
					                              Uniform f32_4* %221 = OpAccessChain %22 %37 %37 
					                                       f32_4 %222 = OpLoad %221 
					                                       f32_3 %223 = OpVectorShuffle %222 %222 2 2 2 
					                                       f32_3 %224 = OpFMul %220 %223 
					                                       f32_4 %225 = OpLoad %9 
					                                       f32_3 %226 = OpVectorShuffle %225 %225 0 1 2 
					                                       f32_3 %227 = OpFAdd %224 %226 
					                                       f32_4 %228 = OpLoad %9 
					                                       f32_4 %229 = OpVectorShuffle %228 %227 4 5 6 3 
					                                                      OpStore %9 %229 
					                              Uniform f32_4* %230 = OpAccessChain %22 %25 %46 
					                                       f32_4 %231 = OpLoad %230 
					                                       f32_3 %232 = OpVectorShuffle %231 %231 0 1 2 
					                              Uniform f32_4* %233 = OpAccessChain %22 %37 %37 
					                                       f32_4 %234 = OpLoad %233 
					                                       f32_3 %235 = OpVectorShuffle %234 %234 3 3 3 
					                                       f32_3 %236 = OpFMul %232 %235 
					                                       f32_4 %237 = OpLoad %9 
					                                       f32_3 %238 = OpVectorShuffle %237 %237 0 1 2 
					                                       f32_3 %239 = OpFAdd %236 %238 
					                                       f32_4 %240 = OpLoad %9 
					                                       f32_4 %241 = OpVectorShuffle %240 %239 4 5 6 3 
					                                                      OpStore %9 %241 
					                                       f32_4 %242 = OpLoad %9 
					                                       f32_3 %243 = OpVectorShuffle %242 %242 0 1 2 
					                                       f32_3 %244 = OpLoad %141 
					                                         f32 %245 = OpDot %243 %244 
					                                 Output f32* %247 = OpAccessChain vs_TEXCOORD1 %246 
					                                                      OpStore %247 %245 
					                                       f32_4 %250 = OpLoad %249 
					                              Uniform f32_4* %252 = OpAccessChain %22 %251 
					                                       f32_4 %253 = OpLoad %252 
					                                       f32_4 %254 = OpFMul %250 %253 
					                                                      OpStore %248 %254 
					                                 Output f32* %255 = OpAccessChain %74 %24 %70 
					                                         f32 %256 = OpLoad %255 
					                                         f32 %257 = OpFNegate %256 
					                                 Output f32* %258 = OpAccessChain %74 %24 %70 
					                                                      OpStore %258 %257 
					                                                      OpReturn
					                                                      OpFunctionEnd
					; SPIR-V
					; Version: 1.0
					; Generator: Khronos Glslang Reference Front End; 6
					; Bound: 112
					; Schema: 0
					                                                  OpCapability Shader 
					                                           %1 = OpExtInstImport "GLSL.std.450" 
					                                                  OpMemoryModel Logical GLSL450 
					                                                  OpEntryPoint Fragment %4 "main" %12 %98 %105 %108 
					                                                  OpExecutionMode %4 OriginUpperLeft 
					                                                  OpName vs_TEXCOORD1 "vs_TEXCOORD1" 
					                                                  OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
					                                                  OpDecorate vs_TEXCOORD1 Location 12 
					                                                  OpMemberDecorate %46 0 Offset 46 
					                                                  OpMemberDecorate %46 1 Offset 46 
					                                                  OpMemberDecorate %46 2 Offset 46 
					                                                  OpMemberDecorate %46 3 Offset 46 
					                                                  OpDecorate %46 Block 
					                                                  OpDecorate %48 DescriptorSet 48 
					                                                  OpDecorate %48 Binding 48 
					                                                  OpDecorate %85 RelaxedPrecision 
					                                                  OpDecorate %88 RelaxedPrecision 
					                                                  OpDecorate %88 DescriptorSet 88 
					                                                  OpDecorate %88 Binding 88 
					                                                  OpDecorate %89 RelaxedPrecision 
					                                                  OpDecorate %92 RelaxedPrecision 
					                                                  OpDecorate %92 DescriptorSet 92 
					                                                  OpDecorate %92 Binding 92 
					                                                  OpDecorate %93 RelaxedPrecision 
					                                                  OpDecorate vs_TEXCOORD0 Location 98 
					                                                  OpDecorate %102 RelaxedPrecision 
					                                                  OpDecorate %105 Location 105 
					                                                  OpDecorate %108 Location 108 
					                                           %2 = OpTypeVoid 
					                                           %3 = OpTypeFunction %2 
					                                           %6 = OpTypeFloat 32 
					                                           %7 = OpTypeVector %6 4 
					                                           %8 = OpTypePointer Private %7 
					                            Private f32_4* %9 = OpVariable Private 
					                                          %10 = OpTypeVector %6 3 
					                                          %11 = OpTypePointer Input %10 
					                    Input f32_3* vs_TEXCOORD1 = OpVariable Input 
					                                          %16 = OpTypeInt 32 0 
					                                      u32 %17 = OpConstant 0 
					                                          %18 = OpTypePointer Private %6 
					                                      u32 %24 = OpConstant 2 
					                                          %25 = OpTypePointer Input %6 
					                                      f32 %32 = OpConstant 3.674022E-40 
					                                      f32 %37 = OpConstant 3.674022E-40 
					                                          %46 = OpTypeStruct %7 %7 %6 %6 
					                                          %47 = OpTypePointer Uniform %46 
					Uniform struct {f32_4; f32_4; f32; f32;}* %48 = OpVariable Uniform 
					                                          %49 = OpTypeInt 32 1 
					                                      i32 %50 = OpConstant 2 
					                                          %51 = OpTypePointer Uniform %6 
					                                      i32 %63 = OpConstant 3 
					                           Private f32_4* %68 = OpVariable Private 
					                                      i32 %69 = OpConstant 0 
					                                          %70 = OpTypePointer Uniform %7 
					                                      i32 %74 = OpConstant 1 
					                           Private f32_4* %85 = OpVariable Private 
					                                          %86 = OpTypeImage %6 Dim2D 0 0 0 1 Unknown 
					                                          %87 = OpTypePointer UniformConstant %86 
					     UniformConstant read_only Texture2D* %88 = OpVariable UniformConstant 
					                                          %90 = OpTypeSampler 
					                                          %91 = OpTypePointer UniformConstant %90 
					                 UniformConstant sampler* %92 = OpVariable UniformConstant 
					                                          %94 = OpTypeSampledImage %86 
					                                          %96 = OpTypeVector %6 2 
					                                          %97 = OpTypePointer Input %96 
					                    Input f32_2* vs_TEXCOORD0 = OpVariable Input 
					                                         %104 = OpTypePointer Output %7 
					                           Output f32_4* %105 = OpVariable Output 
					                                         %107 = OpTypePointer Input %7 
					                            Input f32_4* %108 = OpVariable Input 
					                                      void %4 = OpFunction None %3 
					                                           %5 = OpLabel 
					                                    f32_3 %13 = OpLoad vs_TEXCOORD1 
					                                    f32_3 %14 = OpLoad vs_TEXCOORD1 
					                                      f32 %15 = OpDot %13 %14 
					                             Private f32* %19 = OpAccessChain %9 %17 
					                                                  OpStore %19 %15 
					                             Private f32* %20 = OpAccessChain %9 %17 
					                                      f32 %21 = OpLoad %20 
					                                      f32 %22 = OpExtInst %1 32 %21 
					                             Private f32* %23 = OpAccessChain %9 %17 
					                                                  OpStore %23 %22 
					                               Input f32* %26 = OpAccessChain vs_TEXCOORD1 %24 
					                                      f32 %27 = OpLoad %26 
					                                      f32 %28 = OpFNegate %27 
					                             Private f32* %29 = OpAccessChain %9 %17 
					                                      f32 %30 = OpLoad %29 
					                                      f32 %31 = OpFMul %28 %30 
					                                      f32 %33 = OpFAdd %31 %32 
					                             Private f32* %34 = OpAccessChain %9 %17 
					                                                  OpStore %34 %33 
					                             Private f32* %35 = OpAccessChain %9 %17 
					                                      f32 %36 = OpLoad %35 
					                                      f32 %38 = OpExtInst %1 43 %36 %37 %32 
					                             Private f32* %39 = OpAccessChain %9 %17 
					                                                  OpStore %39 %38 
					                             Private f32* %40 = OpAccessChain %9 %17 
					                                      f32 %41 = OpLoad %40 
					                                      f32 %42 = OpExtInst %1 30 %41 
					                             Private f32* %43 = OpAccessChain %9 %17 
					                                                  OpStore %43 %42 
					                             Private f32* %44 = OpAccessChain %9 %17 
					                                      f32 %45 = OpLoad %44 
					                             Uniform f32* %52 = OpAccessChain %48 %50 
					                                      f32 %53 = OpLoad %52 
					                                      f32 %54 = OpFMul %45 %53 
					                             Private f32* %55 = OpAccessChain %9 %17 
					                                                  OpStore %55 %54 
					                             Private f32* %56 = OpAccessChain %9 %17 
					                                      f32 %57 = OpLoad %56 
					                                      f32 %58 = OpExtInst %1 29 %57 
					                             Private f32* %59 = OpAccessChain %9 %17 
					                                                  OpStore %59 %58 
					                             Private f32* %60 = OpAccessChain %9 %17 
					                                      f32 %61 = OpLoad %60 
					                                      f32 %62 = OpFNegate %61 
					                             Uniform f32* %64 = OpAccessChain %48 %63 
					                                      f32 %65 = OpLoad %64 
					                                      f32 %66 = OpFAdd %62 %65 
					                             Private f32* %67 = OpAccessChain %9 %17 
					                                                  OpStore %67 %66 
					                           Uniform f32_4* %71 = OpAccessChain %48 %69 
					                                    f32_4 %72 = OpLoad %71 
					                                    f32_4 %73 = OpFNegate %72 
					                           Uniform f32_4* %75 = OpAccessChain %48 %74 
					                                    f32_4 %76 = OpLoad %75 
					                                    f32_4 %77 = OpFAdd %73 %76 
					                                                  OpStore %68 %77 
					                                    f32_4 %78 = OpLoad %9 
					                                    f32_4 %79 = OpVectorShuffle %78 %78 0 0 0 0 
					                                    f32_4 %80 = OpLoad %68 
					                                    f32_4 %81 = OpFMul %79 %80 
					                           Uniform f32_4* %82 = OpAccessChain %48 %69 
					                                    f32_4 %83 = OpLoad %82 
					                                    f32_4 %84 = OpFAdd %81 %83 
					                                                  OpStore %9 %84 
					                      read_only Texture2D %89 = OpLoad %88 
					                                  sampler %93 = OpLoad %92 
					               read_only Texture2DSampled %95 = OpSampledImage %89 %93 
					                                    f32_2 %99 = OpLoad vs_TEXCOORD0 
					                                   f32_4 %100 = OpImageSampleImplicitLod %95 %99 
					                                                  OpStore %85 %100 
					                                   f32_4 %101 = OpLoad %9 
					                                   f32_4 %102 = OpLoad %85 
					                                   f32_4 %103 = OpFMul %101 %102 
					                                                  OpStore %9 %103 
					                                   f32_4 %106 = OpLoad %9 
					                                   f32_4 %109 = OpLoad %108 
					                                   f32_4 %110 = OpFMul %106 %109 
					                                                  OpStore %105 %110 
					                                                  OpReturn
					                                                  OpFunctionEnd"
				}
				SubProgram "vulkan hw_tier02 " {
					"spirv
					
					; SPIR-V
					; Version: 1.0
					; Generator: Khronos Glslang Reference Front End; 6
					; Bound: 260
					; Schema: 0
					                                                      OpCapability Shader 
					                                               %1 = OpExtInstImport "GLSL.std.450" 
					                                                      OpMemoryModel Logical GLSL450 
					                                                      OpEntryPoint Vertex %4 "main" %11 %74 %86 %88 %137 %141 %248 %249 
					                                                      OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
					                                                      OpName vs_TEXCOORD1 "vs_TEXCOORD1" 
					                                                      OpDecorate %11 Location 11 
					                                                      OpDecorate %16 ArrayStride 16 
					                                                      OpDecorate %17 ArrayStride 17 
					                                                      OpDecorate %18 ArrayStride 18 
					                                                      OpDecorate %19 ArrayStride 19 
					                                                      OpMemberDecorate %20 0 Offset 20 
					                                                      OpMemberDecorate %20 1 Offset 20 
					                                                      OpMemberDecorate %20 2 Offset 20 
					                                                      OpMemberDecorate %20 3 Offset 20 
					                                                      OpMemberDecorate %20 4 Offset 20 
					                                                      OpDecorate %20 Block 
					                                                      OpDecorate %22 DescriptorSet 22 
					                                                      OpDecorate %22 Binding 22 
					                                                      OpMemberDecorate %72 0 BuiltIn 72 
					                                                      OpMemberDecorate %72 1 BuiltIn 72 
					                                                      OpMemberDecorate %72 2 BuiltIn 72 
					                                                      OpDecorate %72 Block 
					                                                      OpDecorate vs_TEXCOORD0 Location 86 
					                                                      OpDecorate %88 Location 88 
					                                                      OpDecorate vs_TEXCOORD1 Location 137 
					                                                      OpDecorate %141 Location 141 
					                                                      OpDecorate %248 Location 248 
					                                                      OpDecorate %249 Location 249 
					                                               %2 = OpTypeVoid 
					                                               %3 = OpTypeFunction %2 
					                                               %6 = OpTypeFloat 32 
					                                               %7 = OpTypeVector %6 4 
					                                               %8 = OpTypePointer Private %7 
					                                Private f32_4* %9 = OpVariable Private 
					                                              %10 = OpTypePointer Input %7 
					                                 Input f32_4* %11 = OpVariable Input 
					                                              %14 = OpTypeInt 32 0 
					                                          u32 %15 = OpConstant 4 
					                                              %16 = OpTypeArray %7 %15 
					                                              %17 = OpTypeArray %7 %15 
					                                              %18 = OpTypeArray %7 %15 
					                                              %19 = OpTypeArray %7 %15 
					                                              %20 = OpTypeStruct %16 %17 %18 %19 %7 
					                                              %21 = OpTypePointer Uniform %20 
					Uniform struct {f32_4[4]; f32_4[4]; f32_4[4]; f32_4[4]; f32_4;}* %22 = OpVariable Uniform 
					                                              %23 = OpTypeInt 32 1 
					                                          i32 %24 = OpConstant 0 
					                                          i32 %25 = OpConstant 1 
					                                              %26 = OpTypePointer Uniform %7 
					                                          i32 %37 = OpConstant 2 
					                                          i32 %46 = OpConstant 3 
					                               Private f32_4* %50 = OpVariable Private 
					                                          u32 %70 = OpConstant 1 
					                                              %71 = OpTypeArray %6 %70 
					                                              %72 = OpTypeStruct %7 %6 %71 
					                                              %73 = OpTypePointer Output %72 
					         Output struct {f32_4; f32; f32[1];}* %74 = OpVariable Output 
					                                              %82 = OpTypePointer Output %7 
					                                              %84 = OpTypeVector %6 2 
					                                              %85 = OpTypePointer Output %84 
					                       Output f32_2* vs_TEXCOORD0 = OpVariable Output 
					                                              %87 = OpTypePointer Input %84 
					                                 Input f32_2* %88 = OpVariable Input 
					                                              %90 = OpTypeVector %6 3 
					                                             %136 = OpTypePointer Output %90 
					                       Output f32_3* vs_TEXCOORD1 = OpVariable Output 
					                                             %140 = OpTypePointer Input %90 
					                                Input f32_3* %141 = OpVariable Input 
					                                         u32 %144 = OpConstant 0 
					                                             %145 = OpTypePointer Output %6 
					                                         u32 %246 = OpConstant 2 
					                               Output f32_4* %248 = OpVariable Output 
					                                Input f32_4* %249 = OpVariable Input 
					                                         i32 %251 = OpConstant 4 
					                                          void %4 = OpFunction None %3 
					                                               %5 = OpLabel 
					                                        f32_4 %12 = OpLoad %11 
					                                        f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
					                               Uniform f32_4* %27 = OpAccessChain %22 %24 %25 
					                                        f32_4 %28 = OpLoad %27 
					                                        f32_4 %29 = OpFMul %13 %28 
					                                                      OpStore %9 %29 
					                               Uniform f32_4* %30 = OpAccessChain %22 %24 %24 
					                                        f32_4 %31 = OpLoad %30 
					                                        f32_4 %32 = OpLoad %11 
					                                        f32_4 %33 = OpVectorShuffle %32 %32 0 0 0 0 
					                                        f32_4 %34 = OpFMul %31 %33 
					                                        f32_4 %35 = OpLoad %9 
					                                        f32_4 %36 = OpFAdd %34 %35 
					                                                      OpStore %9 %36 
					                               Uniform f32_4* %38 = OpAccessChain %22 %24 %37 
					                                        f32_4 %39 = OpLoad %38 
					                                        f32_4 %40 = OpLoad %11 
					                                        f32_4 %41 = OpVectorShuffle %40 %40 2 2 2 2 
					                                        f32_4 %42 = OpFMul %39 %41 
					                                        f32_4 %43 = OpLoad %9 
					                                        f32_4 %44 = OpFAdd %42 %43 
					                                                      OpStore %9 %44 
					                                        f32_4 %45 = OpLoad %9 
					                               Uniform f32_4* %47 = OpAccessChain %22 %24 %46 
					                                        f32_4 %48 = OpLoad %47 
					                                        f32_4 %49 = OpFAdd %45 %48 
					                                                      OpStore %9 %49 
					                                        f32_4 %51 = OpLoad %9 
					                                        f32_4 %52 = OpVectorShuffle %51 %51 1 1 1 1 
					                               Uniform f32_4* %53 = OpAccessChain %22 %46 %25 
					                                        f32_4 %54 = OpLoad %53 
					                                        f32_4 %55 = OpFMul %52 %54 
					                                                      OpStore %50 %55 
					                               Uniform f32_4* %56 = OpAccessChain %22 %46 %24 
					                                        f32_4 %57 = OpLoad %56 
					                                        f32_4 %58 = OpLoad %9 
					                                        f32_4 %59 = OpVectorShuffle %58 %58 0 0 0 0 
					                                        f32_4 %60 = OpFMul %57 %59 
					                                        f32_4 %61 = OpLoad %50 
					                                        f32_4 %62 = OpFAdd %60 %61 
					                                                      OpStore %50 %62 
					                               Uniform f32_4* %63 = OpAccessChain %22 %46 %37 
					                                        f32_4 %64 = OpLoad %63 
					                                        f32_4 %65 = OpLoad %9 
					                                        f32_4 %66 = OpVectorShuffle %65 %65 2 2 2 2 
					                                        f32_4 %67 = OpFMul %64 %66 
					                                        f32_4 %68 = OpLoad %50 
					                                        f32_4 %69 = OpFAdd %67 %68 
					                                                      OpStore %50 %69 
					                               Uniform f32_4* %75 = OpAccessChain %22 %46 %46 
					                                        f32_4 %76 = OpLoad %75 
					                                        f32_4 %77 = OpLoad %9 
					                                        f32_4 %78 = OpVectorShuffle %77 %77 3 3 3 3 
					                                        f32_4 %79 = OpFMul %76 %78 
					                                        f32_4 %80 = OpLoad %50 
					                                        f32_4 %81 = OpFAdd %79 %80 
					                                Output f32_4* %83 = OpAccessChain %74 %24 
					                                                      OpStore %83 %81 
					                                        f32_2 %89 = OpLoad %88 
					                                                      OpStore vs_TEXCOORD0 %89 
					                               Uniform f32_4* %91 = OpAccessChain %22 %25 %25 
					                                        f32_4 %92 = OpLoad %91 
					                                        f32_3 %93 = OpVectorShuffle %92 %92 0 1 2 
					                               Uniform f32_4* %94 = OpAccessChain %22 %37 %24 
					                                        f32_4 %95 = OpLoad %94 
					                                        f32_3 %96 = OpVectorShuffle %95 %95 1 1 1 
					                                        f32_3 %97 = OpFMul %93 %96 
					                                        f32_4 %98 = OpLoad %9 
					                                        f32_4 %99 = OpVectorShuffle %98 %97 4 5 6 3 
					                                                      OpStore %9 %99 
					                              Uniform f32_4* %100 = OpAccessChain %22 %25 %24 
					                                       f32_4 %101 = OpLoad %100 
					                                       f32_3 %102 = OpVectorShuffle %101 %101 0 1 2 
					                              Uniform f32_4* %103 = OpAccessChain %22 %37 %24 
					                                       f32_4 %104 = OpLoad %103 
					                                       f32_3 %105 = OpVectorShuffle %104 %104 0 0 0 
					                                       f32_3 %106 = OpFMul %102 %105 
					                                       f32_4 %107 = OpLoad %9 
					                                       f32_3 %108 = OpVectorShuffle %107 %107 0 1 2 
					                                       f32_3 %109 = OpFAdd %106 %108 
					                                       f32_4 %110 = OpLoad %9 
					                                       f32_4 %111 = OpVectorShuffle %110 %109 4 5 6 3 
					                                                      OpStore %9 %111 
					                              Uniform f32_4* %112 = OpAccessChain %22 %25 %37 
					                                       f32_4 %113 = OpLoad %112 
					                                       f32_3 %114 = OpVectorShuffle %113 %113 0 1 2 
					                              Uniform f32_4* %115 = OpAccessChain %22 %37 %24 
					                                       f32_4 %116 = OpLoad %115 
					                                       f32_3 %117 = OpVectorShuffle %116 %116 2 2 2 
					                                       f32_3 %118 = OpFMul %114 %117 
					                                       f32_4 %119 = OpLoad %9 
					                                       f32_3 %120 = OpVectorShuffle %119 %119 0 1 2 
					                                       f32_3 %121 = OpFAdd %118 %120 
					                                       f32_4 %122 = OpLoad %9 
					                                       f32_4 %123 = OpVectorShuffle %122 %121 4 5 6 3 
					                                                      OpStore %9 %123 
					                              Uniform f32_4* %124 = OpAccessChain %22 %25 %46 
					                                       f32_4 %125 = OpLoad %124 
					                                       f32_3 %126 = OpVectorShuffle %125 %125 0 1 2 
					                              Uniform f32_4* %127 = OpAccessChain %22 %37 %24 
					                                       f32_4 %128 = OpLoad %127 
					                                       f32_3 %129 = OpVectorShuffle %128 %128 3 3 3 
					                                       f32_3 %130 = OpFMul %126 %129 
					                                       f32_4 %131 = OpLoad %9 
					                                       f32_3 %132 = OpVectorShuffle %131 %131 0 1 2 
					                                       f32_3 %133 = OpFAdd %130 %132 
					                                       f32_4 %134 = OpLoad %9 
					                                       f32_4 %135 = OpVectorShuffle %134 %133 4 5 6 3 
					                                                      OpStore %9 %135 
					                                       f32_4 %138 = OpLoad %9 
					                                       f32_3 %139 = OpVectorShuffle %138 %138 0 1 2 
					                                       f32_3 %142 = OpLoad %141 
					                                         f32 %143 = OpDot %139 %142 
					                                 Output f32* %146 = OpAccessChain vs_TEXCOORD1 %144 
					                                                      OpStore %146 %143 
					                              Uniform f32_4* %147 = OpAccessChain %22 %25 %25 
					                                       f32_4 %148 = OpLoad %147 
					                                       f32_3 %149 = OpVectorShuffle %148 %148 0 1 2 
					                              Uniform f32_4* %150 = OpAccessChain %22 %37 %25 
					                                       f32_4 %151 = OpLoad %150 
					                                       f32_3 %152 = OpVectorShuffle %151 %151 1 1 1 
					                                       f32_3 %153 = OpFMul %149 %152 
					                                       f32_4 %154 = OpLoad %9 
					                                       f32_4 %155 = OpVectorShuffle %154 %153 4 5 6 3 
					                                                      OpStore %9 %155 
					                              Uniform f32_4* %156 = OpAccessChain %22 %25 %24 
					                                       f32_4 %157 = OpLoad %156 
					                                       f32_3 %158 = OpVectorShuffle %157 %157 0 1 2 
					                              Uniform f32_4* %159 = OpAccessChain %22 %37 %25 
					                                       f32_4 %160 = OpLoad %159 
					                                       f32_3 %161 = OpVectorShuffle %160 %160 0 0 0 
					                                       f32_3 %162 = OpFMul %158 %161 
					                                       f32_4 %163 = OpLoad %9 
					                                       f32_3 %164 = OpVectorShuffle %163 %163 0 1 2 
					                                       f32_3 %165 = OpFAdd %162 %164 
					                                       f32_4 %166 = OpLoad %9 
					                                       f32_4 %167 = OpVectorShuffle %166 %165 4 5 6 3 
					                                                      OpStore %9 %167 
					                              Uniform f32_4* %168 = OpAccessChain %22 %25 %37 
					                                       f32_4 %169 = OpLoad %168 
					                                       f32_3 %170 = OpVectorShuffle %169 %169 0 1 2 
					                              Uniform f32_4* %171 = OpAccessChain %22 %37 %25 
					                                       f32_4 %172 = OpLoad %171 
					                                       f32_3 %173 = OpVectorShuffle %172 %172 2 2 2 
					                                       f32_3 %174 = OpFMul %170 %173 
					                                       f32_4 %175 = OpLoad %9 
					                                       f32_3 %176 = OpVectorShuffle %175 %175 0 1 2 
					                                       f32_3 %177 = OpFAdd %174 %176 
					                                       f32_4 %178 = OpLoad %9 
					                                       f32_4 %179 = OpVectorShuffle %178 %177 4 5 6 3 
					                                                      OpStore %9 %179 
					                              Uniform f32_4* %180 = OpAccessChain %22 %25 %46 
					                                       f32_4 %181 = OpLoad %180 
					                                       f32_3 %182 = OpVectorShuffle %181 %181 0 1 2 
					                              Uniform f32_4* %183 = OpAccessChain %22 %37 %25 
					                                       f32_4 %184 = OpLoad %183 
					                                       f32_3 %185 = OpVectorShuffle %184 %184 3 3 3 
					                                       f32_3 %186 = OpFMul %182 %185 
					                                       f32_4 %187 = OpLoad %9 
					                                       f32_3 %188 = OpVectorShuffle %187 %187 0 1 2 
					                                       f32_3 %189 = OpFAdd %186 %188 
					                                       f32_4 %190 = OpLoad %9 
					                                       f32_4 %191 = OpVectorShuffle %190 %189 4 5 6 3 
					                                                      OpStore %9 %191 
					                                       f32_4 %192 = OpLoad %9 
					                                       f32_3 %193 = OpVectorShuffle %192 %192 0 1 2 
					                                       f32_3 %194 = OpLoad %141 
					                                         f32 %195 = OpDot %193 %194 
					                                 Output f32* %196 = OpAccessChain vs_TEXCOORD1 %70 
					                                                      OpStore %196 %195 
					                              Uniform f32_4* %197 = OpAccessChain %22 %25 %25 
					                                       f32_4 %198 = OpLoad %197 
					                                       f32_3 %199 = OpVectorShuffle %198 %198 0 1 2 
					                              Uniform f32_4* %200 = OpAccessChain %22 %37 %37 
					                                       f32_4 %201 = OpLoad %200 
					                                       f32_3 %202 = OpVectorShuffle %201 %201 1 1 1 
					                                       f32_3 %203 = OpFMul %199 %202 
					                                       f32_4 %204 = OpLoad %9 
					                                       f32_4 %205 = OpVectorShuffle %204 %203 4 5 6 3 
					                                                      OpStore %9 %205 
					                              Uniform f32_4* %206 = OpAccessChain %22 %25 %24 
					                                       f32_4 %207 = OpLoad %206 
					                                       f32_3 %208 = OpVectorShuffle %207 %207 0 1 2 
					                              Uniform f32_4* %209 = OpAccessChain %22 %37 %37 
					                                       f32_4 %210 = OpLoad %209 
					                                       f32_3 %211 = OpVectorShuffle %210 %210 0 0 0 
					                                       f32_3 %212 = OpFMul %208 %211 
					                                       f32_4 %213 = OpLoad %9 
					                                       f32_3 %214 = OpVectorShuffle %213 %213 0 1 2 
					                                       f32_3 %215 = OpFAdd %212 %214 
					                                       f32_4 %216 = OpLoad %9 
					                                       f32_4 %217 = OpVectorShuffle %216 %215 4 5 6 3 
					                                                      OpStore %9 %217 
					                              Uniform f32_4* %218 = OpAccessChain %22 %25 %37 
					                                       f32_4 %219 = OpLoad %218 
					                                       f32_3 %220 = OpVectorShuffle %219 %219 0 1 2 
					                              Uniform f32_4* %221 = OpAccessChain %22 %37 %37 
					                                       f32_4 %222 = OpLoad %221 
					                                       f32_3 %223 = OpVectorShuffle %222 %222 2 2 2 
					                                       f32_3 %224 = OpFMul %220 %223 
					                                       f32_4 %225 = OpLoad %9 
					                                       f32_3 %226 = OpVectorShuffle %225 %225 0 1 2 
					                                       f32_3 %227 = OpFAdd %224 %226 
					                                       f32_4 %228 = OpLoad %9 
					                                       f32_4 %229 = OpVectorShuffle %228 %227 4 5 6 3 
					                                                      OpStore %9 %229 
					                              Uniform f32_4* %230 = OpAccessChain %22 %25 %46 
					                                       f32_4 %231 = OpLoad %230 
					                                       f32_3 %232 = OpVectorShuffle %231 %231 0 1 2 
					                              Uniform f32_4* %233 = OpAccessChain %22 %37 %37 
					                                       f32_4 %234 = OpLoad %233 
					                                       f32_3 %235 = OpVectorShuffle %234 %234 3 3 3 
					                                       f32_3 %236 = OpFMul %232 %235 
					                                       f32_4 %237 = OpLoad %9 
					                                       f32_3 %238 = OpVectorShuffle %237 %237 0 1 2 
					                                       f32_3 %239 = OpFAdd %236 %238 
					                                       f32_4 %240 = OpLoad %9 
					                                       f32_4 %241 = OpVectorShuffle %240 %239 4 5 6 3 
					                                                      OpStore %9 %241 
					                                       f32_4 %242 = OpLoad %9 
					                                       f32_3 %243 = OpVectorShuffle %242 %242 0 1 2 
					                                       f32_3 %244 = OpLoad %141 
					                                         f32 %245 = OpDot %243 %244 
					                                 Output f32* %247 = OpAccessChain vs_TEXCOORD1 %246 
					                                                      OpStore %247 %245 
					                                       f32_4 %250 = OpLoad %249 
					                              Uniform f32_4* %252 = OpAccessChain %22 %251 
					                                       f32_4 %253 = OpLoad %252 
					                                       f32_4 %254 = OpFMul %250 %253 
					                                                      OpStore %248 %254 
					                                 Output f32* %255 = OpAccessChain %74 %24 %70 
					                                         f32 %256 = OpLoad %255 
					                                         f32 %257 = OpFNegate %256 
					                                 Output f32* %258 = OpAccessChain %74 %24 %70 
					                                                      OpStore %258 %257 
					                                                      OpReturn
					                                                      OpFunctionEnd
					; SPIR-V
					; Version: 1.0
					; Generator: Khronos Glslang Reference Front End; 6
					; Bound: 112
					; Schema: 0
					                                                  OpCapability Shader 
					                                           %1 = OpExtInstImport "GLSL.std.450" 
					                                                  OpMemoryModel Logical GLSL450 
					                                                  OpEntryPoint Fragment %4 "main" %12 %98 %105 %108 
					                                                  OpExecutionMode %4 OriginUpperLeft 
					                                                  OpName vs_TEXCOORD1 "vs_TEXCOORD1" 
					                                                  OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
					                                                  OpDecorate vs_TEXCOORD1 Location 12 
					                                                  OpMemberDecorate %46 0 Offset 46 
					                                                  OpMemberDecorate %46 1 Offset 46 
					                                                  OpMemberDecorate %46 2 Offset 46 
					                                                  OpMemberDecorate %46 3 Offset 46 
					                                                  OpDecorate %46 Block 
					                                                  OpDecorate %48 DescriptorSet 48 
					                                                  OpDecorate %48 Binding 48 
					                                                  OpDecorate %85 RelaxedPrecision 
					                                                  OpDecorate %88 RelaxedPrecision 
					                                                  OpDecorate %88 DescriptorSet 88 
					                                                  OpDecorate %88 Binding 88 
					                                                  OpDecorate %89 RelaxedPrecision 
					                                                  OpDecorate %92 RelaxedPrecision 
					                                                  OpDecorate %92 DescriptorSet 92 
					                                                  OpDecorate %92 Binding 92 
					                                                  OpDecorate %93 RelaxedPrecision 
					                                                  OpDecorate vs_TEXCOORD0 Location 98 
					                                                  OpDecorate %102 RelaxedPrecision 
					                                                  OpDecorate %105 Location 105 
					                                                  OpDecorate %108 Location 108 
					                                           %2 = OpTypeVoid 
					                                           %3 = OpTypeFunction %2 
					                                           %6 = OpTypeFloat 32 
					                                           %7 = OpTypeVector %6 4 
					                                           %8 = OpTypePointer Private %7 
					                            Private f32_4* %9 = OpVariable Private 
					                                          %10 = OpTypeVector %6 3 
					                                          %11 = OpTypePointer Input %10 
					                    Input f32_3* vs_TEXCOORD1 = OpVariable Input 
					                                          %16 = OpTypeInt 32 0 
					                                      u32 %17 = OpConstant 0 
					                                          %18 = OpTypePointer Private %6 
					                                      u32 %24 = OpConstant 2 
					                                          %25 = OpTypePointer Input %6 
					                                      f32 %32 = OpConstant 3.674022E-40 
					                                      f32 %37 = OpConstant 3.674022E-40 
					                                          %46 = OpTypeStruct %7 %7 %6 %6 
					                                          %47 = OpTypePointer Uniform %46 
					Uniform struct {f32_4; f32_4; f32; f32;}* %48 = OpVariable Uniform 
					                                          %49 = OpTypeInt 32 1 
					                                      i32 %50 = OpConstant 2 
					                                          %51 = OpTypePointer Uniform %6 
					                                      i32 %63 = OpConstant 3 
					                           Private f32_4* %68 = OpVariable Private 
					                                      i32 %69 = OpConstant 0 
					                                          %70 = OpTypePointer Uniform %7 
					                                      i32 %74 = OpConstant 1 
					                           Private f32_4* %85 = OpVariable Private 
					                                          %86 = OpTypeImage %6 Dim2D 0 0 0 1 Unknown 
					                                          %87 = OpTypePointer UniformConstant %86 
					     UniformConstant read_only Texture2D* %88 = OpVariable UniformConstant 
					                                          %90 = OpTypeSampler 
					                                          %91 = OpTypePointer UniformConstant %90 
					                 UniformConstant sampler* %92 = OpVariable UniformConstant 
					                                          %94 = OpTypeSampledImage %86 
					                                          %96 = OpTypeVector %6 2 
					                                          %97 = OpTypePointer Input %96 
					                    Input f32_2* vs_TEXCOORD0 = OpVariable Input 
					                                         %104 = OpTypePointer Output %7 
					                           Output f32_4* %105 = OpVariable Output 
					                                         %107 = OpTypePointer Input %7 
					                            Input f32_4* %108 = OpVariable Input 
					                                      void %4 = OpFunction None %3 
					                                           %5 = OpLabel 
					                                    f32_3 %13 = OpLoad vs_TEXCOORD1 
					                                    f32_3 %14 = OpLoad vs_TEXCOORD1 
					                                      f32 %15 = OpDot %13 %14 
					                             Private f32* %19 = OpAccessChain %9 %17 
					                                                  OpStore %19 %15 
					                             Private f32* %20 = OpAccessChain %9 %17 
					                                      f32 %21 = OpLoad %20 
					                                      f32 %22 = OpExtInst %1 32 %21 
					                             Private f32* %23 = OpAccessChain %9 %17 
					                                                  OpStore %23 %22 
					                               Input f32* %26 = OpAccessChain vs_TEXCOORD1 %24 
					                                      f32 %27 = OpLoad %26 
					                                      f32 %28 = OpFNegate %27 
					                             Private f32* %29 = OpAccessChain %9 %17 
					                                      f32 %30 = OpLoad %29 
					                                      f32 %31 = OpFMul %28 %30 
					                                      f32 %33 = OpFAdd %31 %32 
					                             Private f32* %34 = OpAccessChain %9 %17 
					                                                  OpStore %34 %33 
					                             Private f32* %35 = OpAccessChain %9 %17 
					                                      f32 %36 = OpLoad %35 
					                                      f32 %38 = OpExtInst %1 43 %36 %37 %32 
					                             Private f32* %39 = OpAccessChain %9 %17 
					                                                  OpStore %39 %38 
					                             Private f32* %40 = OpAccessChain %9 %17 
					                                      f32 %41 = OpLoad %40 
					                                      f32 %42 = OpExtInst %1 30 %41 
					                             Private f32* %43 = OpAccessChain %9 %17 
					                                                  OpStore %43 %42 
					                             Private f32* %44 = OpAccessChain %9 %17 
					                                      f32 %45 = OpLoad %44 
					                             Uniform f32* %52 = OpAccessChain %48 %50 
					                                      f32 %53 = OpLoad %52 
					                                      f32 %54 = OpFMul %45 %53 
					                             Private f32* %55 = OpAccessChain %9 %17 
					                                                  OpStore %55 %54 
					                             Private f32* %56 = OpAccessChain %9 %17 
					                                      f32 %57 = OpLoad %56 
					                                      f32 %58 = OpExtInst %1 29 %57 
					                             Private f32* %59 = OpAccessChain %9 %17 
					                                                  OpStore %59 %58 
					                             Private f32* %60 = OpAccessChain %9 %17 
					                                      f32 %61 = OpLoad %60 
					                                      f32 %62 = OpFNegate %61 
					                             Uniform f32* %64 = OpAccessChain %48 %63 
					                                      f32 %65 = OpLoad %64 
					                                      f32 %66 = OpFAdd %62 %65 
					                             Private f32* %67 = OpAccessChain %9 %17 
					                                                  OpStore %67 %66 
					                           Uniform f32_4* %71 = OpAccessChain %48 %69 
					                                    f32_4 %72 = OpLoad %71 
					                                    f32_4 %73 = OpFNegate %72 
					                           Uniform f32_4* %75 = OpAccessChain %48 %74 
					                                    f32_4 %76 = OpLoad %75 
					                                    f32_4 %77 = OpFAdd %73 %76 
					                                                  OpStore %68 %77 
					                                    f32_4 %78 = OpLoad %9 
					                                    f32_4 %79 = OpVectorShuffle %78 %78 0 0 0 0 
					                                    f32_4 %80 = OpLoad %68 
					                                    f32_4 %81 = OpFMul %79 %80 
					                           Uniform f32_4* %82 = OpAccessChain %48 %69 
					                                    f32_4 %83 = OpLoad %82 
					                                    f32_4 %84 = OpFAdd %81 %83 
					                                                  OpStore %9 %84 
					                      read_only Texture2D %89 = OpLoad %88 
					                                  sampler %93 = OpLoad %92 
					               read_only Texture2DSampled %95 = OpSampledImage %89 %93 
					                                    f32_2 %99 = OpLoad vs_TEXCOORD0 
					                                   f32_4 %100 = OpImageSampleImplicitLod %95 %99 
					                                                  OpStore %85 %100 
					                                   f32_4 %101 = OpLoad %9 
					                                   f32_4 %102 = OpLoad %85 
					                                   f32_4 %103 = OpFMul %101 %102 
					                                                  OpStore %9 %103 
					                                   f32_4 %106 = OpLoad %9 
					                                   f32_4 %109 = OpLoad %108 
					                                   f32_4 %110 = OpFMul %106 %109 
					                                                  OpStore %105 %110 
					                                                  OpReturn
					                                                  OpFunctionEnd"
				}
			}
			Program "fp" {
				SubProgram "gles3 hw_tier00 " {
					"!!GLES3"
				}
				SubProgram "gles3 hw_tier01 " {
					"!!GLES3"
				}
				SubProgram "gles3 hw_tier02 " {
					"!!GLES3"
				}
				SubProgram "vulkan hw_tier00 " {
					"spirv"
				}
				SubProgram "vulkan hw_tier01 " {
					"spirv"
				}
				SubProgram "vulkan hw_tier02 " {
					"spirv"
				}
			}
		}
	}
}
Shader "Shader Graphs/PhysicalMaterial3DsMax" {
	Properties {
		_BASE_COLOR_WEIGHT ("BaseColorWeight", Range(0, 1)) = 1
		_BASE_COLOR ("BaseColor", Vector) = (1,1,1,1)
		[NoScaleOffset] _BASE_COLOR_MAP ("BaseColorMap", 2D) = "white" {}
		_REFLECTIONS_WEIGHT ("ReflectionsWeight", Float) = 1
		_REFLECTIONS_COLOR ("ReflectionsColor", Vector) = (1,1,1,1)
		[NoScaleOffset] _REFLECTIONS_COLOR_MAP ("ReflectionsColorMap", 2D) = "white" {}
		_REFLECTIONS_ROUGHNESS ("ReflectionsRoughness", Range(0, 1)) = 1
		[NoScaleOffset] _REFLECTIONS_ROUGHNESS_MAP ("ReflectionsRoughnessMap", 2D) = "white" {}
		_METALNESS ("Metalness", Range(0, 1)) = 0
		[NoScaleOffset] _METALNESS_MAP ("MetalnessMap", 2D) = "white" {}
		_REFLECTIONS_IOR ("ReflectionsIOR", Range(0, 50)) = 1.52
		[NoScaleOffset] _REFLECTIONS_IOR_MAP ("ReflectionsIORMap", 2D) = "white" {}
		_TRANSPARENCY ("Transparency", Range(0, 1)) = 0
		[NoScaleOffset] _TRANSPARENCY_MAP ("TransparencyMap", 2D) = "white" {}
		_EMISSION_WEIGHT ("EmissionWeight", Range(0, 1)) = 1
		_EMISSION_COLOR ("Emission", Vector) = (0,0,0,0)
		[NoScaleOffset] _EMISSION_COLOR_MAP ("EmissionMap", 2D) = "white" {}
		_BUMP_MAP_STRENGTH ("BumpMapStrength", Range(0, 10)) = 0.3
		[NoScaleOffset] _BUMP_MAP ("BumpMap", 2D) = "white" {}
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}
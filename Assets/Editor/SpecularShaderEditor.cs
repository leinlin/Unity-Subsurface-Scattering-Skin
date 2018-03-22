using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Collections;

public class SpecularShaderEditor : ShaderGUI
{
	bool init = false;
	bool lastAlbedo = true;
	bool lastBump = true;
	bool lastSpecular = true;
	bool lastOcclusion = true;
	bool lastPhong = true;
	bool lastVertex = true;
	bool lastDetailAlbedo = true;
	bool lastDetailNormal = true;
	bool bump;
	bool specular;
	bool vertex;
	bool phong;
	bool occlusion;
	bool albedo;
	bool detailAlbedo;
	bool detailNormal;
	Dictionary<string, MaterialProperty> propDict = new Dictionary<string, MaterialProperty> (17);
	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		Material targetMat = materialEditor.target as Material;
		bool USE_FILTER = Array.IndexOf(targetMat.shaderKeywords, "USE_FILTER") != -1;

		EditorGUI.BeginChangeCheck();
		USE_FILTER = EditorGUILayout.Toggle("Use Tess Filter", USE_FILTER);

		if (EditorGUI.EndChangeCheck())
		{
			// enable or disable the keyword based on checkbox
			if (USE_FILTER)
				targetMat.EnableKeyword("USE_FILTER");
			else
				targetMat.DisableKeyword("USE_FILTER");
		}
		// render the default gui
		base.OnGUI (materialEditor, properties);


		// see if redify is set, and show a checkbox
		//	bool CS_BOOL = Array.IndexOf(targetMat.shaderKeywords, "CS_BOOL") != -1;
		propDict.Clear();
		for (int i = 0; i < properties.Length; ++i) {
			propDict.Add (properties [i].name, properties [i]);
		}
		//	CS_BOOL = EditorGUILayout.Toggle("CS_BOOL", CS_BOOL);
		bump = propDict ["_BumpMap"].textureValue != null;
		specular = propDict ["_SpecularMap"].textureValue != null;
		vertex = propDict ["_HeightMap"].textureValue != null;
		phong = propDict ["_Phong"].floatValue != 0;
		occlusion = propDict ["_OcclusionMap"].textureValue != null;
		albedo = propDict ["_MainTex"].textureValue != null;
		detailAlbedo = propDict ["_DetailAlbedo"].textureValue != null;
		detailNormal = propDict ["_DetailBump"].textureValue != null;
		if (!init) {
			init = true;
			if (albedo)
				targetMat.EnableKeyword ("USE_ALBEDO");
			else
				targetMat.DisableKeyword ("USE_ALBEDO");
			if (bump)
				targetMat.EnableKeyword ("USE_NORMAL");
			else
				targetMat.DisableKeyword ("USE_NORMAL");
			if (specular)
				targetMat.EnableKeyword ("USE_SPECULAR");
			else
				targetMat.DisableKeyword ("USE_SPECULAR");
			if (vertex)
				targetMat.EnableKeyword ("USE_VERTEX");
			else
				targetMat.DisableKeyword ("USE_VERTEX");
			if (phong)
				targetMat.EnableKeyword ("USE_PHONG");
			else
				targetMat.DisableKeyword ("USE_PHONG");
			if (occlusion)
				targetMat.EnableKeyword ("USE_OCCLUSION");
			else
				targetMat.DisableKeyword ("USE_OCCLUSION");
			if (detailAlbedo)
				targetMat.EnableKeyword ("USE_DETAILALBEDO");
			else
				targetMat.DisableKeyword ("USE_DETAILALBEDO");
			if (detailNormal)
				targetMat.EnableKeyword ("USE_DETAILNORMAL");
			else
				targetMat.DisableKeyword ("USE_DETAILNORMAL");
		}
		if (lastBump != bump) {
			lastBump = bump;
			if (bump)
				targetMat.EnableKeyword ("USE_NORMAL");
			else
				targetMat.DisableKeyword ("USE_NORMAL");
		}

		if (lastAlbedo != albedo) {
			lastAlbedo = albedo;
			if (albedo)
				targetMat.EnableKeyword ("USE_ALBEDO");
			else
				targetMat.DisableKeyword ("USE_ALBEDO");
		}

		if (lastOcclusion != occlusion) {
			lastOcclusion = occlusion;
			if (occlusion)
				targetMat.EnableKeyword ("USE_OCCLUSION");
			else
				targetMat.DisableKeyword ("USE_OCCLUSION");
		}

		if (lastSpecular != specular) {
			lastSpecular = specular;
			if (specular)
				targetMat.EnableKeyword ("USE_SPECULAR");
			else
				targetMat.DisableKeyword ("USE_SPECULAR");
		}

		if (lastPhong != phong) {
			lastPhong = phong;
			if (phong)
				targetMat.EnableKeyword ("USE_PHONG");
			else
				targetMat.DisableKeyword ("USE_PHONG");
		}

		if (lastVertex != vertex) {
			if (vertex)
				targetMat.EnableKeyword ("USE_VERTEX");
			else
				targetMat.DisableKeyword ("USE_VERTEX");
			lastVertex = vertex;
		}

		if (lastDetailAlbedo != detailAlbedo) {
			lastDetailAlbedo = detailAlbedo;
			if (detailAlbedo)
				targetMat.EnableKeyword ("USE_DETAILALBEDO");
			else
				targetMat.DisableKeyword ("USE_DETAILALBEDO");
		}

		if (lastDetailNormal != detailNormal) {
			lastDetailNormal = detailNormal;
			if (detailNormal)
				targetMat.EnableKeyword ("USE_DETAILNORMAL");
			else
				targetMat.DisableKeyword ("USE_DETAILNORMAL");
		}
	}
}
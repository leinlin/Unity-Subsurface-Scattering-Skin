using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Collections;

public class WaterShader : ShaderGUI
{
	bool init = false;
	bool lastAlbedo = true;
	bool lastBump = true;
	bool lastPhong = true;
	bool lastVertex = true;

	bool bump;

	bool vertex;
	bool phong;

	bool albedo;

	Dictionary<string, MaterialProperty> propDict = new Dictionary<string, MaterialProperty> (17);
	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		Material targetMat = materialEditor.target as Material;
		// render the default gui
		base.OnGUI (materialEditor, properties);


		// see if redify is set, and show a checkbox
		//	bool CS_BOOL = Array.IndexOf(targetMat.shaderKeywords, "CS_BOOL") != -1;
		propDict.Clear();
		for (int i = 0; i < properties.Length; ++i) {
			propDict.Add (properties [i].name, properties [i]);
		}
		bump = propDict ["_BumpMap"].textureValue != null;
		vertex = propDict ["_HeightMap"].textureValue != null;
		phong = propDict ["_Phong"].floatValue != 0;
		albedo = propDict ["_MainTex"].textureValue != null;

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

			if (vertex)
				targetMat.EnableKeyword ("USE_VERTEX");
			else
				targetMat.DisableKeyword ("USE_VERTEX");
			if (phong)
				targetMat.EnableKeyword ("USE_PHONG");
			else
				targetMat.DisableKeyword ("USE_PHONG");

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

	}
}
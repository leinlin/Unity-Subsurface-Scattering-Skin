// Creates a simple wizard that lets you create a Light GameObject
// or if the user clicks in "Apply", it will set the color of the currently
// object selected to red

using UnityEditor;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MixTexture : ScriptableWizard
{
	public string path = "Arts";
	public string fileName = "mixedTexture";
	public Texture2D RGB;
	public Texture2D A;


	[MenuItem ("Tools/Mix Texture")]
	static void CreateWizard ()
	{
		ScriptableWizard.DisplayWizard<MixTexture> ("Mix Texture", "Quit", "Create");
	}

	void OnWizardOtherButton ()
	{/*
		if (RGB.texelSize == A.texelSize) {
			Texture2D example = new Texture2D (RGB.width, RGB.height, TextureFormat.RGBA32, true);
			var RGBColors = RGB.GetPixels ();
			var AColors = A.GetPixels ();
			var colors = new Color[RGBColors.Length];
	
			for (int i = 0; i < colors.Length; ++i) {
				colors [i] = new Color (RGBColors [i].r, RGBColors [i].g, RGBColors [i].b, AColors [i].a);
			}
			
			example.SetPixels (colors);
			example.Apply ();
			SaveFileToPng (example, fileName);
			Debug.Log ("Success!");
		} else {
			Debug.LogWarning ("Unable To mix the texture because the size is different");
		}*/
		Texture2D ramp = new Texture2D (128, 1, TextureFormat.RGBA32, false);
		Color[] colors = new Color[128];
		for (int i = 0; i < colors.Length; ++i) {
			Color c = Color.Lerp (Color.black, Color.white, i / 128f);
			float lerpV = Mathf.Abs (c.r - 0.5f) * 2;
			c = Color.Lerp (new Color (1, 0.46666666f, 0f), c, lerpV);
			colors [i] = c;
		}
		ramp.SetPixels (colors);
		ramp.Apply ();
		SaveFileToPng (ramp, fileName);
	}

	void SaveFileToPng (Texture2D picture, string fileName)
	{
		string path1 = Application.dataPath + "/" + path + "/" + fileName + ".png";
		var binary = new BinaryWriter (new FileStream (path1, FileMode.Create));
		var bytes = picture.EncodeToPNG ();
		binary.Write (bytes);
		binary.Dispose ();
	}
}

public class SetTessMesh : ScriptableWizard
{
	public Mesh mesh;
	[MenuItem ("Tools/Set TessMesh")]
	static void CreateWizard ()
	{
		ScriptableWizard.DisplayWizard<SetTessMesh> ("SetTessMesh", "Set");
	}

	void SetMesh(){
		Vector3[] allVertex = mesh.vertices;
		bool[] vertIsIndependent = new bool[allVertex.Length];
		for (int i = 0; i < vertIsIndependent.Length; ++i) {
			vertIsIndependent [i] = true;
		}
		int[] triangle = mesh.triangles;
		Vector3[] normals = mesh.normals;
		for (int i = 0; (i + 2) < triangle.Length; i += 3) {
			int x = triangle[i];
			int y = triangle[i + 1];
			int z = triangle[i + 2];
			bool independent = normals [x] == normals [y] && normals [x] == normals [z] && normals [y] == normals [z];
			vertIsIndependent [x] &= independent;
			vertIsIndependent [y] &= independent;
			vertIsIndependent [z] &= independent;
		}
		Color[] colors = new Color[allVertex.Length];
		for (int i = 0; i < colors.Length; ++i) {
			colors [i] = Color.black;
		}
		for (int i = 0; i < vertIsIndependent.Length; ++i) {
			colors [i].a = vertIsIndependent [i] ? 0 : 1;
		}
		mesh.colors = colors;
	}

	void OnWizardCreate ()
	{
		SetMesh ();
		Debug.Log ("Success");
	}
}

public class CleanMesh : ScriptableWizard
{
	public Mesh mesh;
	public bool normal = true;
	public bool uv1 = true;
	public bool uv2 = true;
	public bool uv3 = false;
	public bool uv4 = false;
	public bool color = false;
	[MenuItem ("Tools/Clean Mesh")]
	static void CreateWizard ()
	{
		ScriptableWizard.DisplayWizard<CleanMesh> ("CleanMesh", "Set");
	}

	void SetMesh(){
		if (!uv1)
			mesh.uv = new Vector2[0];
		if (!uv2)
			mesh.uv2 = new Vector2[0];
		if (!uv3)
			mesh.uv3 = new Vector2[0];
		if (!uv4)
			mesh.uv4 = new Vector2[0];
		if (!color)
			mesh.colors = new Color[0];
		if (!normal) {
			mesh.normals = new Vector3[0];
			mesh.tangents = new Vector4[0];
		}
	}

	void OnWizardCreate ()
	{
		SetMesh ();
		Debug.Log ("Success");
	}
}


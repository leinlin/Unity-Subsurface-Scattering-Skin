using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSSSSDepthCullCam : MonoBehaviour {
	public SSSSSCamera current;
	Camera cam;
	RenderTexture maskRT;
	[System.NonSerialized]
	public Material mat;
	int maskID;
	// Use this for initialization
	void Awake () {
		cam = GetComponent<Camera> ();
		cam.SetReplacementShader (SSSSSCamera.depthCullShader, "RenderType");
		maskRT = new RenderTexture (Screen.width, Screen.height, 0, RenderTextureFormat.RFloat,RenderTextureReadWrite.Linear);
		cam.targetTexture = maskRT;
		maskID = Shader.PropertyToID ("_SSMaskTex");
	}

	void OnDestroy(){
		Destroy (maskRT);
	}

	void OnPreRender(){
		if (current.isScreenChanged) {
			maskRT.Release ();
			maskRT.width = current.width;
			maskRT.height = current.height;
		}
		Shader.SetGlobalTexture (maskID, maskRT);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest){
		RenderTexture blur1 = RenderTexture.GetTemporary (src.descriptor);
		Graphics.Blit (src, blur1,mat,4);
		Graphics.Blit (blur1, dest, mat, 4);
		RenderTexture.ReleaseTemporary (blur1);
	}
}

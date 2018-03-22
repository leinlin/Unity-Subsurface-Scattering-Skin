using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Threading.Tasks;

public class SSSSSDepthCamera : MonoBehaviour {
	public SSSSSCamera current;
	Camera cam;
	RenderTexture depthRT;
	int depthID;

	void Awake(){
		cam = GetComponent<Camera> ();
		depthID = Shader.PropertyToID ("_CameraDepthTextureWithoutSkin");
		depthRT = new RenderTexture (Screen.width, Screen.height, 24, RenderTextureFormat.ARGBFloat);
		cam.targetTexture = depthRT;
	}

	void OnPreRender(){
		if (current.isScreenChanged) {
			depthRT.Release ();
			depthRT.width = current.width;
			depthRT.height = current.height;
		}
		Shader.SetGlobalTexture (depthID, depthRT);
	}

	void OnDestroy(){
		Destroy (depthRT);
	}
}

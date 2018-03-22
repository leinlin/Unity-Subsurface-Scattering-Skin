using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class SSSSSCamera : MonoBehaviour {
	#region MASK_SPACE
	public static Shader depthCullShader;
	public static Material depthCullFrontMaterial;
	public static CommandBuffer currentCommandBuffer;
	public static bool isCommandBufferReady;
	public bool useBlur = true;
	[System.NonSerialized]
	public bool isScreenChanged;
	[System.NonSerialized]
	public int width;
	[System.NonSerialized]
	public int height;
	CommandBuffer commandBuffer;
	Camera cam;
	Camera depthCam;
	Camera depthCullCam;
	Shader depthShader;
	Material mat;
	[Range(0.02f, 100)]
	public float blurRange = 20;

	[Range(0,10)]
	public float offset = 0.5f;
	public LayerMask skinLayer = 1<<31;
	RenderTexture cullFrontDepthTex;
	int tempID1;
	int tempID2;
	// Use this for initialization
	void Awake () {
		if (depthCullShader == null) {
			depthCullShader = Shader.Find ("Hidden/DepthCull");
		}
		if (depthCullFrontMaterial == null) {
			depthCullFrontMaterial = new Material(Shader.Find ("Hidden/DepthCullFront"));
		}
		width = Screen.width;
		height = Screen.height;
		cam = GetComponent<Camera> ();
		var camG = new GameObject ("Depth Camera", typeof(Camera));
		depthCam = camG.GetComponent<Camera> ();
		depthCam.CopyFrom (cam);
		camG.transform.SetParent (transform);
		camG.transform.localPosition = Vector3.zero;
		camG.transform.localRotation = Quaternion.identity;
		camG.transform.localScale = Vector3.one;
		camG.hideFlags = HideFlags.HideAndDontSave;
		depthCam.renderingPath = RenderingPath.Forward;
		depthCam.SetReplacementShader (Shader.Find ("Hidden/Depth"),"RenderType");
		depthCam.farClipPlane = blurRange;
		depthCam.clearFlags = CameraClearFlags.Color;
		depthCam.backgroundColor = Color.red;
		depthCam.cullingMask = (~skinLayer) & depthCam.cullingMask;
		depthCam.depthTextureMode = DepthTextureMode.None;
		depthCam.enabled = false;
		var depthCullObj = Instantiate (camG, camG.transform.position, camG.transform.rotation, camG.transform.parent) as GameObject;
		depthCullCam = depthCullObj.GetComponent<Camera> ();
		depthCullCam.backgroundColor = Color.black;
		depthCullCam.cullingMask = cam.cullingMask & skinLayer;
		depthCullCam.enabled = false;

		depthCullObj.hideFlags = HideFlags.HideAndDontSave;
		originMapID = Shader.PropertyToID ("_OriginTex");
		blendTexID = Shader.PropertyToID ("_BlendTex");
		mat = new Material (Shader.Find ("Hidden/SSSSS"));
		blendWeightID = Shader.PropertyToID ("_BlendWeight");
		offsetID = Shader.PropertyToID ("_Offset");
		mat.SetFloat (offsetID, offset);
		camG.AddComponent<SSSSSDepthCamera> ().current = this;
		var dcc = depthCullObj.AddComponent<SSSSSDepthCullCam> ();
		dcc.mat = mat;
		dcc.current = this;
		commandBuffer = new CommandBuffer ();
		cullFrontDepthTex = new RenderTexture (Screen.width, Screen.height, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		if (cam.actualRenderingPath == RenderingPath.DeferredShading)
			cam.AddCommandBuffer (CameraEvent.BeforeGBuffer, commandBuffer);
		else
			cam.AddCommandBuffer (CameraEvent.BeforeForwardOpaque, commandBuffer);
		tempID1 = Shader.PropertyToID ("_CullFrontDepthTex");
		tempID2 = Shader.PropertyToID ("_Temp2");
	}

	void OnPreCull(){
		isScreenChanged = (width != Screen.width) || (height != Screen.height);
		if (isScreenChanged) {
			width = Screen.width;
			height = Screen.height;
			cullFrontDepthTex.Release ();
			cullFrontDepthTex.width = width;
			cullFrontDepthTex.height = height;
		}
		if (useBlur) {
			depthCam.Render ();
			depthCullCam.Render ();
		}
		commandBuffer.SetRenderTarget (cullFrontDepthTex);
		commandBuffer.ClearRenderTarget (true, true, Color.red);


		commandBuffer.ReleaseTemporaryRT (tempID1);
		currentCommandBuffer = commandBuffer;
		isCommandBufferReady = true;
	}

	void OnPreRender(){
		var dest = cullFrontDepthTex.descriptor;
		commandBuffer.GetTemporaryRT (tempID1, dest);
		commandBuffer.GetTemporaryRT (tempID2, dest);
		commandBuffer.Blit (BuiltinRenderTextureType.CurrentActive, tempID1);
		commandBuffer.Blit (tempID1, tempID2, mat, 5);
		commandBuffer.Blit (tempID2, tempID1, mat, 5);

	}

	void OnPostRender(){
		isCommandBufferReady = false;
		commandBuffer.Clear ();
	}

	void OnDestroy(){
		if(depthCam)
			Destroy (depthCam.gameObject);
		if (depthCullCam)
			Destroy (depthCullCam.gameObject);
		if (cam.actualRenderingPath == RenderingPath.DeferredShading)
			cam.RemoveCommandBuffer (CameraEvent.BeforeGBuffer, commandBuffer);
		else
			cam.RemoveCommandBuffer (CameraEvent.BeforeForwardOpaque, commandBuffer);
		commandBuffer.Dispose ();
	}
		
	#endregion

	#region POST_PROCESS
	int originMapID;
	int blendTexID;
	int blendWeightID;
	int offsetID;

	void OnRenderImage(RenderTexture src, RenderTexture dest){
		if (useBlur) {
			#if UNITY_EDITOR
			mat.SetFloat (offsetID, offset);
			#endif
			RenderTexture origin = RenderTexture.GetTemporary (src.descriptor);
			RenderTexture copyOri = RenderTexture.GetTemporary (src.descriptor);
			Graphics.Blit (src, origin);
			mat.SetTexture (originMapID, src);
			RenderTexture blur1 = RenderTexture.GetTemporary (src.descriptor);
			RenderTexture blur2 = RenderTexture.GetTemporary (src.descriptor);

			mat.SetTexture (blendTexID, origin);
			mat.SetVector (blendWeightID, new Vector4 (0.33f, 0.45f, 0.36f));
			Graphics.Blit (origin, blur1, mat, 0);
			Graphics.Blit (blur1, blur2, mat, 1);
			Graphics.Blit (blur2, copyOri, mat, 2);

			mat.SetTexture (blendTexID, copyOri);
			mat.SetVector (blendWeightID, new Vector4 (0.34f, 0.19f));
			Graphics.Blit (copyOri, blur1, mat, 0);
			Graphics.Blit (blur1, blur2, mat, 1);
			Graphics.Blit (blur2, origin, mat, 2);

			mat.SetTexture (blendTexID, origin);
			mat.SetVector (blendWeightID, new Vector4 (0.46f, 0f, 0.04f));
			Graphics.Blit (origin, blur1, mat, 0);
			Graphics.Blit (blur1, blur2, mat, 1);
			Graphics.Blit (blur2, copyOri, mat, 2);

			Graphics.Blit (copyOri, dest, mat, 3);
			RenderTexture.ReleaseTemporary (blur1);
			RenderTexture.ReleaseTemporary (blur2);
			RenderTexture.ReleaseTemporary (origin);
			RenderTexture.ReleaseTemporary (copyOri);
		} else {
			Graphics.Blit (src, dest);
		}
	}

	#endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
public class SSSSSSkinRenderer : MonoBehaviour {
	Mesh mesh;
	void Awake(){
		mesh = GetComponent<MeshFilter> ().sharedMesh;
	}

	void OnWillRenderObject(){
		
		if (SSSSSCamera.isCommandBufferReady) {
			SSSSSCamera.currentCommandBuffer.DrawMesh (mesh, transform.localToWorldMatrix, SSSSSCamera.depthCullFrontMaterial);
		}
	}
}

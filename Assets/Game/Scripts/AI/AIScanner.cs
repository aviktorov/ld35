using UnityEngine;
using System.Collections;

public class AIScanner : MonoBehaviour {
	
	public Camera captureCamera = null;
	public float scale = 1.0f;
	
	private ShapeshiftDisplay display = null;
	private Texture2D cpuColorData = null;
	private RenderTexture gpuData = null;
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
		cpuColorData = new Texture2D(display.sizeX,display.sizeY,TextureFormat.RGB24,false);
		gpuData = new RenderTexture(display.sizeX,display.sizeY,24);
		
		captureCamera.targetTexture = gpuData;
	}
	
	private void Update() {
		
		// Copy RT to CPU
		RenderTexture oldRT = RenderTexture.active;
		RenderTexture.active = gpuData;
		
		cpuColorData.ReadPixels(new Rect(0,0,display.sizeX,display.sizeY),0,0,false);
		cpuColorData.Apply();
		
		RenderTexture.active = oldRT;
		
		// Fill shapeshift display
		Color[] pixels = cpuColorData.GetPixels(0,0,display.sizeX,display.sizeY);
		
		Vector3 offset = new Vector3(display.sizeX * 0.5f - 0.5f,0.0f,display.sizeY * 0.5f - 0.5f);
		offset -= captureCamera.transform.position;
		
		RaycastHit hit;
		float maxHeight = 20.0f;
		for(int x = 0; x < display.sizeX; x++) {
			for(int y = 0; y < display.sizeY; y++) {
				
				// Color
				Color color = pixels[y * display.sizeX + x];
				
				// Height
				Vector3 position = new Vector3(x,0.0f,y) - offset;
				
				// Boxcast like a hell, cause we can't read the depth directly ;)
				// FIXME: sync with color info
				float height = 0.0f;
				
				if(Physics.BoxCast(position,Vector3.one * 0.25f,-Vector3.up,out hit,Quaternion.identity,maxHeight)) {
					height = maxHeight - hit.distance;
				}
				
				height *= scale;
				
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
}

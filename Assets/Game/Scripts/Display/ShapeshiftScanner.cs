using UnityEngine;
using System.Collections;

public class ShapeshiftScanner : MonoBehaviour {
	
	public Camera captureCamera = null;
	
	private ShapeshiftDisplay display = null;
	private Texture2D cpuData = null;
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
		cpuData = new Texture2D(display.sizeX,display.sizeY,TextureFormat.RGB24,false);
	}
	
	private void Update() {
		
		// Copy RT to CPU
		RenderTexture oldRT = RenderTexture.active;
		RenderTexture.active = captureCamera.targetTexture;
		
		cpuData.ReadPixels(new Rect(0,0,display.sizeX,display.sizeY),0,0,false);
		cpuData.Apply();
		
		RenderTexture.active = oldRT;
		
		// Fill shapeshift display
		Color[] pixels = cpuData.GetPixels(0,0,display.sizeX,display.sizeY);
		
		Vector3 offset = new Vector3(display.sizeX * 0.5f + 0.5f,0.0f,display.sizeY * 0.5f + 0.5f);
		offset -= captureCamera.transform.position;
		
		RaycastHit hit;
		float maxHeight = 20.0f;
		for(int x = 0; x < display.sizeX; x++) {
			for(int y = 0; y < display.sizeY; y++) {
				
				// Color
				Color color = pixels[y * display.sizeX + x];
				
				// Height
				Vector3 position = new Vector3(x,maxHeight,y) - offset;
				
				// Raycast like a hell, cause we can't read the depth directly ;)
				float height = maxHeight;
				
				if(Physics.Raycast(position,-Vector3.up,out hit)) {
					height = maxHeight - hit.distance;
				}
				
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
}

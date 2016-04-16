using UnityEngine;
using System.Collections;

public class AIScanner : MonoBehaviour {
	
	public Camera captureCamera = null;
	public float scale = 1.0f;
	public bool clear = false;
	
	private ShapeshiftDisplay display = null;
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
	}
	
	private void Update() {
		Vector3 offset = new Vector3(display.sizeX * 0.5f - 0.5f,0.0f,display.sizeY * 0.5f - 0.5f);
		offset -= captureCamera.transform.position;
		
		RaycastHit hit;
		float maxHeight = 20.0f;
		
		for(int x = 0; x < display.sizeX; x++) {
			for(int y = 0; y < display.sizeY; y++) {
				
				// Color
				Color color = (clear) ? Color.black : display.GetPixelColor(x,y);
				
				// Height
				float height = (clear) ? 0.0f : display.GetPixelHeight(x,y);
				
				// Raycast like a hell, cause we can't copy to CPU fast enough ;)
				Vector3 position = new Vector3(x,0.0f,y) - offset;
				
				if(Physics.Raycast(position,-Vector3.up,out hit,maxHeight)) {
					height += maxHeight - hit.distance;
					color += hit.collider.GetComponent<Renderer>().material.color;
				}
				
				height *= scale;
				
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
}

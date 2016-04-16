using UnityEngine;
using System.Collections;

public class ShapeshiftTester : MonoBehaviour {
	
	public float scale = 1.0f;
	public Transform item = null;
	public Transform location = null;
	
	private ShapeshiftDisplay display = null;
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
	}
	
	private float HeightFunc(int x,int y,float t,float distance) {
		return scale * Mathf.Pow(distance,0.3f);
	}
	
	private Color ColorFunc(int x,int y,float t,float distance) {
		return ColorExt.FromHSV(Mathf.Repeat(t / 10.0f,10.0f) * distance,1.0f - distance,1.0f,1.0f);
	}
	
	private void Update() {
		if(item == null || location == null) return;
		
		float t = Time.timeSinceLevelLoad;
		
		Vector3 screenOffset = new Vector3(display.sizeX,0.0f,display.sizeY);
		Vector3 offset = item.position - (location.position - screenOffset * 0.5f);
		offset.y = 0.0f;
		
		float iSize = 1.0f / (Mathf.Max(display.sizeX,display.sizeY) * 5);
		for(int x = 0; x < display.sizeX; x++) {
			for(int y = 0; y < display.sizeY; y++) {
				
				float distance = Mathf.Clamp01((offset - new Vector3(x,0.0f,y)).magnitude * iSize);
				
				float height = display.GetPixelHeight(x,y) * HeightFunc(x,y,t,distance);
				Color color = display.GetPixelColor(x,y) * ColorFunc(x,y,t,distance);
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
}

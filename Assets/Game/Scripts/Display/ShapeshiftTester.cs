using UnityEngine;
using System.Collections;

public class ShapeshiftTester : MonoBehaviour {
	
	public float scale = 1.0f;
	private ShapeshiftDisplay display = null;
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
	}
	
	private float HeightFunc(int x,int y,float t) {
		return scale * Mathf.Sin((float) x * Mathf.PI / display.sizeX + t) * Mathf.Cos((float)y * Mathf.PI / display.sizeY + t);
	}
	
	private Color ColorFunc(int x,int y,float t) {
		return ColorExt.FromHSV(Mathf.Repeat(t / 10.0f,10.0f),(float)x / display.sizeX,(float)y / display.sizeY,1.0f);
	}
	
	private void Update() {
		float t = Time.timeSinceLevelLoad;
		
		for(int x = 0; x < display.sizeX; x++) {
			for(int y = 0; y < display.sizeY; y++) {
				float height = HeightFunc(x,y,t);
				Color color = ColorFunc(x,y,t);
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
}

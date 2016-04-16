using UnityEngine;
using System.Collections;

public class ShapeshiftTester : MonoBehaviour {
	
	private ShapeshiftDisplay display = null;
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
	}
	
	private float HeightFunc(int x,int y,float t) {
		return 5.0f * Mathf.Sin(x / 10.0f + t) * Mathf.Cos(y / 10.0f + t);
	}
	
	private Color ColorFunc(int x,int y,float t) {
		return ColorExt.FromHSV(Mathf.Repeat(t / 10.0f,1.0f),1.0f - Mathf.Abs(x - display.sizeX * 0.5f) / display.sizeX,1.0f - Mathf.Abs(y - display.sizeY * 0.5f) / display.sizeY,1.0f);
	}
	
	private void Update() {
		float t = Time.timeSinceLevelLoad;
		
		for(int x = 0; x < display.sizeX; x++) {
			for(int y = 0; y < display.sizeY; y++) {
				bool isBorder = false; //((x == 0) || (y == 0) || (x == display.sizeX - 1) || (y == display.sizeY - 1));
				float height = isBorder ? 10.0f : HeightFunc(x,y,t);
				Color color = isBorder ? Color.white : ColorFunc(x,y,t);
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
}

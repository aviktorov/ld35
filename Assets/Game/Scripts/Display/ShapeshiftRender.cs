using UnityEngine;
using System.Collections;

public class ShapeshiftRender : MonoBehaviour {
	
	public Texture2D texture = null;
	public bool inverted = false;
	public float scale = 1.0f;
	
	private ShapeshiftDisplay display = null;
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
	}
	
	private void Update() {
		for(int x = 0; x < display.sizeX; x++) {
			float u = (float)x / display.sizeX;
			for(int y = 0; y < display.sizeY; y++) {
				float v = (float)y / display.sizeY;
				
				Color color = texture.GetPixelBilinear(u,v);
				float height = Mathf.Max(color.r,Mathf.Max(color.g,color.b));
				if(inverted) height = 1.0f - height;
				
				height *= scale;
				height += display.GetPixelHeight(x,y);
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
}

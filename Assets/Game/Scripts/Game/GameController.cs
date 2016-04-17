using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoSingleton<GameController> {
	
	public float degradationInterval = 0.2f;
	
	private ShapeshiftDisplay display;
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
		StartCoroutine(Degradate());
	}
	
	private IEnumerator Degradate() {
		while(true) {
			int numIterations = 10;
			while(numIterations > 0) {
				int x = Random.Range(0,display.sizeX);
				int y = Random.Range(0,display.sizeY);
				
				if(display.GetPixelEnabled(x,y)) {
					display.SetPixelEnabled(x,y,false);
					break;
				}
				
				numIterations--;
			}
			
			yield return new WaitForSeconds(degradationInterval);
		}
	}
	
	public void Restart() {
		SceneManager.LoadScene("main");
	}
	
}

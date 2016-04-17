using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoSingleton<GameController> {
	
	public void Restart() {
		SceneManager.LoadScene("main");
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

internal enum GameState {
	Paused,
	InGame,
	Win
}

public class GameController : MonoSingleton<GameController> {
	
	private GameState state;
	
	public void Awake() {
		state = GameState.InGame;
	}
	
	public void Win() {
		state = GameState.Win;
	}
	
	private void OnGUI() {
		
		GUILayout.BeginVertical();
		GUILayout.Label("Controls:");
		GUILayout.Label("W / S / A / D: move");
		GUILayout.Label("Q / R: rotate the camera");
		GUILayout.Label("Y / H / G / J: move location");
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		if(state == GameState.Win) {
			GUILayout.Label("You win!");
			if(GUILayout.Button("Restart")) {
				SceneManager.LoadScene("main");
			}
		}
		GUILayout.EndVertical();
	}
}

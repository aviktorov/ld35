using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoSingleton<UIController> {
	
	private void OnGUI() {
		// TODO: get rid of this
		GUILayout.BeginVertical("box");
		GUILayout.Label("Controls:");
		GUILayout.Label("W / S / A / D: move");
		GUILayout.Label("Q / R: rotate the camera");
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.EndVertical();
	}
}

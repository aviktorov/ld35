using UnityEngine;
using System.Collections;

internal enum AIState {
	Text,
	
}

public class AIController : MonoBehaviour {
	
	private ShapeshiftDisplay display = null;
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
	}
	
	private void Update() {
		
	}
}

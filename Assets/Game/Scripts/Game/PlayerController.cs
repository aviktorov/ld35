using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public float movementSpeed = 3.0f;
	
	private Rigidbody cachedBody;
	
	private void Start() {
		cachedBody = GetComponent<Rigidbody>();
	}
	
	private void Update() {
		Vector3 cameraRight = Camera.main.transform.right;
		cameraRight.y = 0;
		
		Vector3 cameraForward = new Vector3(-cameraRight.z,0,cameraRight.x);
		
		cameraRight = cameraRight.normalized;
		cameraForward = cameraForward.normalized;
		
		Vector3 movement = Vector3.zero;
		movement += cameraRight * Input.GetAxis("Horizontal");
		movement += cameraForward * Input.GetAxis("Vertical");
		
		if(movement.sqrMagnitude > 1.0f) movement.Normalize();
		movement *= movementSpeed;
		
		movement.y = cachedBody.velocity.y;
		
		cachedBody.velocity = movement;
	}
}

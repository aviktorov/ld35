using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public float movementSpeed = 3.0f;
	public float jumpForce = 10.0f;
	public float cooldown = 0.3f;
	
	private Rigidbody cachedBody;
	private bool onAir;
	private float currentCooldown;
	
	private void Start() {
		cachedBody = GetComponent<Rigidbody>();
		onAir = false;
		currentCooldown = 0.0f;
	}
	
	private void Update() {
		currentCooldown -= Time.deltaTime;
		
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
		
		if(!onAir && Input.GetButton("Jump")) {
			onAir = true;
			currentCooldown = cooldown;
			cachedBody.AddForce(Vector3.up * jumpForce,ForceMode.VelocityChange);
			cachedBody.velocity = cachedBody.velocity.WithY(0.0f);
		}
	}
	
	private void StayOnGround(Collision collision) {
		if(currentCooldown > 0.0f) return;
		if(!onAir) return;
		
		foreach(ContactPoint c in collision.contacts) {
			if(c.normal.z < Mathf.Epsilon) continue;
			
			onAir = false;
			break;
		}
	}
	
	private void OnCollisionEnter(Collision collision) {
		StayOnGround(collision);
	}
	
	private void OnCollisionStay(Collision collision) {
		StayOnGround(collision);
	}
}

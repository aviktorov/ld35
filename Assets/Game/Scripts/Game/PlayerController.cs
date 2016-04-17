using UnityEngine;
using System.Collections;

public class PlayerController : MonoSingleton<PlayerController> {
	
	public AudioClip[] jumpSfx;
	
	public float movementSpeed = 3.0f;
	public float jumpForce = 10.0f;
	public float slopeThreshold = 0.5f;
	
	private Rigidbody cachedBody;
	private Transform cachedTransform;
	private Transform currentPlatform;
	private bool canJump;
	
	public bool OnGround() {
		return (currentPlatform != null);
	}
	
	private void Awake() {
		cachedBody = GetComponent<Rigidbody>();
		cachedTransform = GetComponent<Transform>();
	}
	
	private void Start() {
		canJump = false;
		currentPlatform = null;
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
		
		RaycastHit hit;
		
		if(Physics.Raycast(cachedTransform.position + Vector3.up * Mathf.Epsilon,Vector3.down,out hit)) {
			if(hit.distance > slopeThreshold) currentPlatform = null;
		}
		
		if(currentPlatform != null) {
			cachedTransform.position = cachedTransform.position.WithY(currentPlatform.position.y);
		}
		
		if(canJump && Input.GetButton("Jump")) {
			cachedBody.velocity = cachedBody.velocity.WithY(0.0f);
			cachedBody.AddForce(Vector3.up * jumpForce,ForceMode.VelocityChange);
			currentPlatform = null;
			canJump = false;
			AudioSource.PlayClipAtPoint(jumpSfx[Random.Range(0,jumpSfx.Length)],cachedTransform.position);
		}
	}
	
	private void OnCollisionEnter(Collision collision) {
		bool hasGround = false;
		
		foreach(ContactPoint c in collision.contacts) {
			if(c.normal.y < 0.99f) continue;
			hasGround = true;
			break;
		}
		
		if(hasGround) {
			currentPlatform = collision.transform;
			canJump = true;
		}
	}
}

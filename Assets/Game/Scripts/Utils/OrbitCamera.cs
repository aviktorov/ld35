using UnityEngine;
using System.Collections;

/*
 */
public class OrbitCamera : MonoBehaviour {
	
	/*
	 */
	public float radius = 1.0f;
	public float theta = 0.0f;
	public float phi = 45.0f;
	
	public float rotationAmount = 45.0f;
	public float smoothness = 10.0f;
	
	public Transform target = null;
	
	/*
	 */
	private Transform cachedTransform;
	private float targetPhi;
	
	/*
	 */
	private void Start() {
		cachedTransform = GetComponent<Transform>();
		targetPhi = phi;
	}
	
	/*
	 */
	private void LateUpdate() {
		if(target == null) return;
		
		phi = Mathf.Lerp(phi,targetPhi,Time.deltaTime * smoothness);
		
		Vector3 newPosition = target.position;
		
		newPosition += new Vector3(
			Mathf.Sin(theta * Mathf.Deg2Rad) * Mathf.Cos(phi * Mathf.Deg2Rad),
			Mathf.Cos(theta * Mathf.Deg2Rad),
			Mathf.Sin(theta * Mathf.Deg2Rad) * Mathf.Sin(phi * Mathf.Deg2Rad)
		) * radius;
		
		cachedTransform.position = Vector3.Lerp(cachedTransform.position,newPosition,smoothness * Time.deltaTime);
		cachedTransform.LookAt(target);
		
		// Navigation
		if(Input.GetKey(KeyCode.Q)) {
			targetPhi -= rotationAmount * Time.deltaTime;
		}
		
		if(Input.GetKey(KeyCode.E)) {
			targetPhi += rotationAmount * Time.deltaTime;
		}
	}
}

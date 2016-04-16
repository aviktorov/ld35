using UnityEngine;
using System.Collections;

/*
 */
public class PageController : MonoBehaviour {
	
	/*
	 */
	public float pageOffset = 40.0f;
	
	/*
	 */
	private Transform cachedTransform;
	
	/*
	 */
	private void Start() {
		cachedTransform = GetComponent<Transform>();
	}
	
	/*
	 */
	private void LateUpdate() {
		
		if(Input.GetKeyDown(KeyCode.Y)) {
			cachedTransform.position += Vector3.forward * pageOffset;
		}
		
		if(Input.GetKeyDown(KeyCode.H)) {
			cachedTransform.position -= Vector3.forward * pageOffset;
		}
		
		if(Input.GetKeyDown(KeyCode.G)) {
			cachedTransform.position += Vector3.left * pageOffset;
		}
		
		if(Input.GetKeyDown(KeyCode.J)) {
			cachedTransform.position -= Vector3.left * pageOffset;
		}
	}
}

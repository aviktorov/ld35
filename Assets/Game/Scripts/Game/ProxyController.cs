using UnityEngine;
using System.Collections;

public class ProxyController : MonoBehaviour {
	
	public Transform proxy = null;
	public Transform player = null;
	public Transform location = null;
	
	private ShapeshiftDisplay display = null;
	private Transform cachedTransform = null;
	
	private void Awake() {
		cachedTransform = GetComponent<Transform>();
	}
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
	}
	
	private void OnTriggerEnter(Collider collider) {
		if(collider.tag != "Player") return;
		proxy.gameObject.SetActive(true);
	}
	
	private void OnTriggerStay(Collider collider) {
		if(collider.tag != "Player") return;
		Vector3 offset = (player.position - cachedTransform.position);
		offset.x -= display.sizeX * 0.5f;
		offset.z -= display.sizeY * 0.5f;
		
		proxy.position = location.position + offset;
		RaycastHit hit;
		if(Physics.Raycast(proxy.position,Vector3.down,out hit)) {
			proxy.position -= Vector3.up * hit.distance;
		}
	}
	
	private void OnTriggerExit(Collider collider) {
		if(collider.tag != "Player") return;
		proxy.gameObject.SetActive(false);
	}
	
}

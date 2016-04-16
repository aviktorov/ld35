using UnityEngine;
using System.Collections;

public class ProxyController : MonoBehaviour {
	
	public Transform proxy = null;
	public Transform player = null;
	public Transform location = null;
	
	private Transform cachedTransform = null;
	
	private void Awake() {
		cachedTransform = GetComponent<Transform>();
	}
	
	private void OnTriggerEnter(Collider collider) {
		if(collider.tag != "Player") return;
		proxy.gameObject.SetActive(true);
	}
	
	private void OnTriggerStay(Collider collider) {
		if(collider.tag != "Player") return;
		proxy.position = location.position + (player.position - cachedTransform.position);
	}
	
	private void OnTriggerExit(Collider collider) {
		if(collider.tag != "Player") return;
		proxy.gameObject.SetActive(false);
	}
	
}

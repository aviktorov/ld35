using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/*
 */
public class ItemController : MonoBehaviour {
	
	public UnityEvent evt = null;
	
	private void OnTriggerEnter() {
		if(evt != null) evt.Invoke();
		Destroy(gameObject);
	}
}

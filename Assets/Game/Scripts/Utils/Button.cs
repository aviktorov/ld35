using UnityEngine;
using System.Collections;

/*
 */
public enum ButtonType {
	Normal,
	OneShot,
	Trigger
}

/*
 */
public class Button : MonoBehaviour {
	
	/*
	 */
	public ButtonType type = ButtonType.Normal;
	public GameObject target = null;
	public string enabledStateMessage = "";
	public string disabledStateMessage = "";
	public Material enabledStateMaterial = null;
	public Material disabledStateMaterial = null;
	public AudioClip onPressedSound = null;
	public AudioClip onUnpressedSound = null;
	public float delay = 0.0f;
	
	/*
	 */
	private Renderer cachedRenderer;
	private AudioSource cachedAudio;
	private bool isEnabled;
	
	/*
	 */
	public void Activate() {
		if(isEnabled && type == ButtonType.OneShot) return;
		
		switch(type) {
			case ButtonType.Trigger: isEnabled = !isEnabled; break;
			case ButtonType.OneShot:
			case ButtonType.Normal: isEnabled = true; break;
		}
		
		AudioClip clip = (isEnabled) ? onPressedSound : onUnpressedSound;
		if(clip != null) cachedAudio.PlayOneShot(clip);
		
		cachedRenderer.material = (isEnabled) ? enabledStateMaterial : disabledStateMaterial;
		if(target != null) {
			string message = (isEnabled) ? enabledStateMessage : disabledStateMessage;
			if(message != "") target.SendMessage(message);
		}
	}
	
	/*
	 */
	public IEnumerator Deactivate() {
		if(type != ButtonType.Normal) yield break;
		
		if(delay != 0.0f) yield return new WaitForSeconds(delay);
		
		isEnabled = false;
		cachedRenderer.material = disabledStateMaterial;
		
		if(onUnpressedSound != null) cachedAudio.PlayOneShot(onUnpressedSound);
		
		if(target != null) {
			if(disabledStateMessage != "") target.SendMessage(disabledStateMessage);
		}
	}
	
	/*
	 */
	private void Start() {
		cachedRenderer = gameObject.GetComponent<Renderer>();
		cachedRenderer.material = disabledStateMaterial;
		isEnabled = false;
		
		cachedAudio = gameObject.AddComponent<AudioSource>();
		cachedAudio.playOnAwake = false;
		cachedAudio.rolloffMode = AudioRolloffMode.Linear;
	}
	
	/*
	 */
	private void OnTriggerEnter(Collider collider) {
		Activate();
	}
	
	/*
	 */
	private void OnTriggerExit(Collider collider) {
		StartCoroutine(Deactivate());
	}
}

﻿using UnityEngine;
using System.Collections;

public class ShapeshiftDisplay : MonoSingleton<ShapeshiftDisplay> {
	
	[Header("Grid")]
	
	[Range(1,100)]
	public int sizeX = 20;
	
	[Range(1,100)]
	public int sizeY = 20;
	
	[Header("Appearance")]
	public float heightSmoothness = 5.0f;
	public float colorSmoothness = 5.0f;
	
	public GameObject prefab = null;
	
	internal struct Pixel {
		public GameObject bar;
		public Transform barTransform;
		public Renderer barRenderer;
		public Color color;
		public float height;
	}
	
	private Pixel[] display = null;
	private Transform cachedTransform = null;
	
	private int index(int x,int y) {
		return x * sizeY + y;
	}
	
	private void Awake() {
		display = new Pixel[sizeX * sizeY];
		cachedTransform = GetComponent<Transform>();
	}
	
	private void Start() {
		Vector3 offset = new Vector3(0.5f,0.0f,0.5f);
		
		for(int x = 0; x < sizeX; x++) {
			for(int y = 0; y < sizeY; y++) {
				int id = index(x,y);
				
				Vector3 position = new Vector3(x,0.0f,y) + offset;
				
				GameObject bar = GameObject.Instantiate(prefab,position,Quaternion.identity) as GameObject;
				display[id].bar = bar;
				display[id].barTransform = bar.GetComponent<Transform>();
				display[id].barTransform.parent = cachedTransform;
				display[id].barRenderer = bar.GetComponentInChildren<Renderer>();
				display[id].height = 0.0f;
				display[id].color = Color.white;
			}
		}
		
		Vector3 min = transform.position;
		Vector3 max = min + new Vector3(sizeX,20.0f,sizeY);
		min.y = -20.0f;
		
		BoxCollider collider = gameObject.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		collider.center = (min + max) * 0.5f;
		collider.size = (max - min);
	}
	
	private void OnDrawGizmos() {
		Vector3 min = transform.position;
		Vector3 max = min + new Vector3(sizeX,20.0f,sizeY);
		min.y = -20.0f;
		Gizmos.DrawWireCube((min + max) * 0.5f,(max - min));
	}
	
	private void Update() {
		
		// TODO: parallelize, ~130ms, very slow
		// GS / VS mesh displacement & coloring + collider movement?
		float dHeight = heightSmoothness * Time.deltaTime;
		float dColor = colorSmoothness * Time.deltaTime;
		
		for(int x = 0; x < sizeX; x++) {
			for(int y = 0; y < sizeY; y++) {
				int id = index(x,y);
				
				Vector3 currentPosition = display[id].barTransform.position;
				Vector3 targetPosition = currentPosition;
				targetPosition.y = display[id].height;
				
				Color currentColor = display[id].barRenderer.material.color;
				Color targetColor = display[id].color;
				
				display[id].barTransform.position = Vector3.Lerp(currentPosition,targetPosition,dHeight);
				display[id].barRenderer.material.color = Color.Lerp(currentColor,targetColor,dColor);
			}
		}
	}
	
	public void SetPixelRaw(int x,int y,float height,Color color) {
		int id = index(x,y);
		display[id].height = height;
		display[id].color = color;
	}
	
	public void SetPixelColor(int x,int y,Color color) {
		int id = index(x,y);
		display[id].color = color;
	}
	
	public Color GetPixelColor(int x,int y) {
		int id = index(x,y);
		return display[id].color;
	}
	
	public void SetPixelHeight(int x,int y,float height) {
		int id = index(x,y);
		display[id].height = height;
	}
	
	public float GetPixelHeight(int x,int y) {
		int id = index(x,y);
		return display[id].height;
	}
}

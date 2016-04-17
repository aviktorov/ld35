﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameState {
	Intro,			// AI explains to player some basic rules
	Idle,			// some pretty math func with nice colors
	Punishment,		// stay alive for N seconds
	Lava,			// don't touch the lava until it'll reach the bottom
	Joke,			// listen for stupid joke and have a choice
	Encouraging,	// for obeying the AI
	Suicide,		// AI asks player to commit suicide
	Bonus			// mario endless?
}

public class GameController : MonoSingleton<GameController> {
	
	public float idleTime = 10.0f;
	public float punishTime = 30.0f;
	public float punishInterval = 0.2f;
	public float punishScale = 1.0f;
	public Texture2D[] punishmentLevels;
	public Texture2D[] lavaLevels;
	public Texture2D[] bonusLevels;
	public Transform playerTransform;
	public Transform roomTransform;
	
	private ShapeshiftDisplay display;
	private GameState state;
	private float stateTime;
	private float stateSubTime;
	private int stateIndex;
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
		state = GameState.Intro;
		stateTime = 0.0f;
		stateIndex = -1;
	}
	
	private void Update() {
		stateTime -= Time.deltaTime;
		
		switch(state) {
			case GameState.Intro: ProcessIntro(); break;
			case GameState.Idle: ProcessIdle(); break;
			case GameState.Punishment: ProcessPunishment(); break;
		}
	}
	
	private void ProcessIntro() {
		Vector3 offset = new Vector3(display.sizeX,0.0f,display.sizeY) * 0.5f;
		Vector3 distance = playerTransform.position - offset - roomTransform.position;
		distance.y = 0.0f;
		
		if(distance.sqrMagnitude < 2.0f) {
			stateTime = idleTime;
			state = GameState.Idle;
			return;
		}
		
		DrawIntro();
	}
	
	private void ProcessIdle() {
		if(stateTime < 0.0f) {
			stateTime = punishTime;
			stateSubTime = punishInterval;
			state = GameState.Punishment;
			stateIndex = Random.Range(0,punishmentLevels.Length);
			
			return;
		}
		
		DrawIdle();
	}
	
	private void ProcessPunishment() {
		if(stateTime < 0.0f) {
			stateTime = idleTime;
			state = GameState.Idle;
			return;
		}
		
		DrawLevel(punishScale,stateTime < punishTime,punishmentLevels[stateIndex]);
		
		stateSubTime -= Time.deltaTime;
		if(stateSubTime < 0.0f) {
			stateSubTime = punishInterval;
			Punish();
		}
	}
	
	private void DrawIntro() {
		
		float t = Time.timeSinceLevelLoad;
		float r = Mathf.Repeat(t * 15.0f,Mathf.Max(display.sizeX,display.sizeY));
		
		for(int x = 0; x < display.sizeX; x++) {
			float dx = (float)x - display.sizeX * 0.5f;
			for(int y = 0; y < display.sizeY; y++) {
				bool isBorder = (x == 0) || (x == display.sizeX - 1) || (y == 0) || (y == display.sizeY - 1);
				float dy = (float)y - display.sizeY * 0.5f;
				
				float checkRSqr = (dx * dx + dy * dy);
				float diff = 1.0f - Mathf.Clamp01(Mathf.Abs(checkRSqr - r * r) / 25.0f);
				
				Color color = (isBorder) ? Color.black : Color.red * diff;
				float height = (isBorder) ? 20.0f : diff * 5.0f;
				
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
	
	private void DrawIdle() {
		
		for(int x = 0; x < display.sizeX; x++) {
			for(int y = 0; y < display.sizeY; y++) {
				
				Color color = Color.blue;
				float height = 0.0f;
				
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
	
	private void DrawLevel(float scale,bool inverted,Texture2D texture) {
		
		for(int x = 0; x < display.sizeX; x++) {
			float u = (float)x / display.sizeX;
			for(int y = 0; y < display.sizeY; y++) {
				float v = (float)y / display.sizeY;
				
				Color color = texture.GetPixelBilinear(u,v);
				float height = Mathf.Max(color.r,Mathf.Max(color.g,color.b));
				if(inverted) height = 1.0f - height;
				
				height *= scale;
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
	
	private void Punish() {
		int numIterations = 10;
		while(numIterations > 0) {
			int x = Random.Range(0,display.sizeX);
			int y = Random.Range(0,display.sizeY);
			
			if(display.GetPixelEnabled(x,y)) {
				display.SetPixelEnabled(x,y,false);
				break;
			}
			
			numIterations--;
		}
	}
	
	public void Restart() {
		SceneManager.LoadScene("main");
	}
	
}

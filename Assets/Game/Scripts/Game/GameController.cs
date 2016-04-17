using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameState {
	Intro,			// AI explains to player some basic rules
	Idle,			// some pretty math func with nice colors
	Punishment,		// stay alive for N seconds
	Lava,			// don't touch the lava until it'll reach the bottom
	Joke,			// listen for stupid joke and have a choice
	Encourage,		// for obeying the AI
	Anger,			// for disliking the jokes, failing lava test
	Suicide,		// AI asks player to commit suicide
	Bonus			// (not implemented) mario endless?
}

public class GameController : MonoSingleton<GameController> {
	
	public AudioSource cameraAudio;
	public AudioClip introSpeech;
	public AudioClip jokeIntroSpeech;
	public AudioClip[] jokeSpeeches;
	public AudioClip[] angerSpeeches;
	public AudioClip[] encourageSpeeches;
	public AudioClip[] lavaSpeeches;
	public float idleTime = 10.0f;
	public float punishTime = 30.0f;
	public float punishInterval = 0.2f;
	public float punishScale = 1.0f;
	public float lavaTime = 30.0f;
	public float lavaScale = 20.0f;
	public float initialAngerLevel = 1.0f;
	public float angerIncrement = 0.2f;
	public float angerDecrement = 0.2f;
	public float angerTime = 5.0f;
	public float encourageTime = 5.0f;
	public float jokeTime = 5.0f;
	public float sanityDropInterval = 5.0f;
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
	private float angerLevel;
	private float sanityDropTime;
	
	private void Start() {
		display = ShapeshiftDisplay.instance;
		state = GameState.Intro;
		stateTime = 0.0f;
		stateIndex = -1;
		angerLevel = initialAngerLevel;
		cameraAudio.PlayOneShot(introSpeech, 1.0f);
	}
	
	private void Update() {
		stateTime -= Time.deltaTime;
		
		switch(state) {
			case GameState.Intro: ProcessIntro(); break;
			case GameState.Idle: ProcessIdle(); break;
			case GameState.Punishment: ProcessPunishment(); break;
			case GameState.Lava: ProcessLava(); break;
			case GameState.Anger: ProcessAnger(); break;
			case GameState.Encourage: ProcessEncourage(); break;
			case GameState.Joke: ProcessJoke(); break;
			case GameState.Suicide: ProcessSuicide(); break;
		}
		
		ProcessSanity();
	}
	
	private void PreparePunishment() {
		stateTime = punishTime;
		stateSubTime = punishInterval;
		state = GameState.Punishment;
		stateIndex = Random.Range(0,punishmentLevels.Length);
	}
	
	private void PrepareLava() {
		stateTime = lavaTime;
		stateSubTime = 0.0f;
		state = GameState.Lava;
		stateIndex = Random.Range(0,lavaLevels.Length);
		cameraAudio.PlayOneShot(lavaSpeeches[Random.Range(0,lavaSpeeches.Length)]);
	}
	
	private void PrepareIdle() {
		stateTime = idleTime;
		state = GameState.Idle;
	}
	
	private void PrepareAnger() {
		stateTime = angerTime;
		state = GameState.Anger;
		angerLevel += angerIncrement;
		cameraAudio.PlayOneShot(angerSpeeches[Random.Range(0,angerSpeeches.Length)]);
	}
	
	private void PrepareEncourage() {
		stateTime = encourageTime;
		state = GameState.Encourage;
		angerLevel -= angerDecrement;
		cameraAudio.PlayOneShot(encourageSpeeches[Random.Range(0,encourageSpeeches.Length)]);
	}
	
	private void PrepareJoke() {
		stateTime = jokeTime;
		state = GameState.Joke;
		stateSubTime = 1.0f;
		cameraAudio.PlayOneShot(jokeIntroSpeech);
	}
	
	private void PrepareSuicide() {
		stateTime = 0.0f;
		state = GameState.Suicide;
	}
	
	private void ProcessIntro() {
		Vector3 offset = new Vector3(display.sizeX,0.0f,display.sizeY) * 0.5f;
		Vector3 distance = playerTransform.position - offset - roomTransform.position;
		distance.y = 0.0f;
		
		if(PlayerController.instance.OnGround() && distance.sqrMagnitude < 20.0f) {
			PrepareIdle();
			return;
		}
		
		Clear(0.0f,Color.blue * 0.3f);
		DrawWave(15.0f,display.sizeX * 0.5f,display.sizeY * 0.5f,4.0f,2.0f,Color.blue);
	}
	
	private void ProcessIdle() {
		if(stateTime < 0.0f) {
			
			int decision = Random.Range(0,1000);
			if(decision < 800) PrepareLava();
			else if(decision < 999) PrepareJoke();
			else PrepareSuicide();
			
			return;
		}
		
		DrawIdle();
	}
	
	private void ProcessAnger() {
		if(stateTime < 0.0f) {
			PreparePunishment();
			return;
		}
		
		DrawAnger();
	}
	
	private void ProcessEncourage() {
		if(stateTime < 0.0f) {
			PrepareIdle();
			return;
		}
		
		DrawEncourage();
	}
	
	private void ProcessJoke() {
		Clear(0.0f,Color.black);
		
		if(stateTime > 0.0f) return;
		
		if(stateSubTime > 0.0f) {
			cameraAudio.PlayOneShot(jokeSpeeches[Random.Range(0,jokeSpeeches.Length)]);
			stateSubTime = 0.0f;
		}
		
		Vector3 likeOffset = new Vector3(display.sizeX * 0.8f,0.0f,display.sizeY * 0.5f);
		Vector3 dislikeOffset = new Vector3(display.sizeX * 0.2f,0.0f,display.sizeY * 0.5f);
		
		Vector3 likeDistance = playerTransform.position - likeOffset - roomTransform.position;
		likeDistance.y = 0.0f;
		
		Vector3 dislineDistance = playerTransform.position - dislikeOffset - roomTransform.position;
		dislineDistance.y = 0.0f;
		
		if(PlayerController.instance.OnGround() && likeDistance.sqrMagnitude < 20.0f) {
			PrepareEncourage();
			return;
		}
		
		if(PlayerController.instance.OnGround() && dislineDistance.sqrMagnitude < 20.0f) {
			PrepareAnger();
			return;
		}
		
		DrawWave(15.0f,dislikeOffset.x,dislikeOffset.z,2.0f,2.0f,Color.red);
		DrawWave(15.0f,likeOffset.x,likeOffset.z,2.0f,2.0f,Color.green);
	}
	
	private void ProcessPunishment() {
		if(stateTime < 0.0f) {
			PrepareIdle();
			return;
		}
		
		DrawLevel(punishScale,stateTime < punishTime * 0.5f,punishmentLevels[stateIndex]);
		
		stateSubTime -= Time.deltaTime;
		if(stateSubTime < 0.0f) {
			stateSubTime = punishInterval;
			Punish();
		}
	}
	
	private void ProcessLava() {
		if(stateTime < 0.0f) {
			PrepareIdle();
			return;
		}
		
		float lavaLevel = lavaScale * (1.0f - Mathf.Clamp01(stateTime / lavaTime)) - lavaScale * 0.2f;
		if(PlayerController.instance.OnGround() && playerTransform.position.y < lavaLevel) {
			PrepareAnger();
			return;
		}
		
		DrawLevel(lavaScale,false,lavaLevels[stateIndex]);
		DrawLava(lavaLevel);
	}
	
	private void ProcessSuicide() {
		// There's no escape, you musi kill yourself ;)
		DrawSuicide();
	}
	
	private void ProcessSanity() {
		if(angerLevel > 5.0f) {
			if(PlayerController.instance.OnGround()) {
				Vector3 playerRelativePosition = playerTransform.position - roomTransform.position;
				int x = (int)Mathf.Floor(playerRelativePosition.x);
				int y = (int)Mathf.Floor(playerRelativePosition.z);
				
				sanityDropTime -= Time.deltaTime;
				
				Color c = Color.white * Mathf.Clamp01(sanityDropTime / sanityDropInterval);
				display.SetPixelColor(x,y,c);
				
				if(sanityDropTime < 0.0f) {
					display.SetPixelEnabled(x,y,false);
					sanityDropTime = sanityDropInterval;
				}
			}
			else {
				sanityDropTime = sanityDropInterval;
			}
		}
		
		DrawSanity();
	}
	
	private void Clear(float height,Color color) {
		for(int x = 0; x < display.sizeX; x++) {
			for(int y = 0; y < display.sizeY; y++) {
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
	
	private void DrawWave(float frequency,float centerX,float centerY,float width,float waveScale,Color waveColor) {
		float t = Time.timeSinceLevelLoad;
		float r = Mathf.Repeat(t * frequency,Mathf.Max(display.sizeX,display.sizeY));
		
		for(int x = 0; x < display.sizeX; x++) {
			float dx = (float)x - centerX;
			for(int y = 0; y < display.sizeY; y++) {
				float dy = (float)y - centerY;
				
				float checkRSqr = (dx * dx + dy * dy);
				float diff = 1.0f - Mathf.Clamp01(Mathf.Abs(checkRSqr - r * r) - (width * width));
				
				if(diff < Mathf.Epsilon) continue;
				
				Color pixelColor = display.GetPixelColor(x,y);
				Color color = Color.Lerp(pixelColor,waveColor,diff);
				float height = diff * waveScale;
				
				display.SetPixelRaw(x,y,Mathf.Max(display.GetPixelHeight(x,y),height),color);
			}
		}
	}
	
	private void DrawSanity() {
		float t = Time.timeSinceLevelLoad;
		
		float noiseScale = Mathf.Max(0.0f,angerLevel - initialAngerLevel) / 5.0f;
		
		for(int x = 0; x < display.sizeX; x++) {
			float dx = (float)x / display.sizeX;
			for(int y = 0; y < display.sizeY; y++) {
				float dy = (float)y / display.sizeY;
				
				Color color = display.GetPixelColor(x,y);
				float height = display.GetPixelHeight(x,y);
				
				height += angerLevel * Mathf.Cos(dx + t) * Mathf.Sin(dy + t);
				height += noiseScale * Random.Range(-1.0f,1.0f);
				
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
	
	private void DrawSuicide() {
		
		float t = Time.timeSinceLevelLoad;
		for(int x = 0; x < display.sizeX; x++) {
			float u = Mathf.Abs((float)x - display.sizeX * 0.5f);
			for(int y = 0; y < display.sizeY; y++) {
				float v = Mathf.Abs((float)y - display.sizeY * 0.5f);
				
				Color color = Color.black;
				float height = Mathf.Sin(u - t) * Mathf.Cos(v - t) - Mathf.Max(u,v) * 0.5f;
				
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
	
	private void DrawIdle() {
		
		float t = Time.timeSinceLevelLoad;
		float hue = Mathf.Repeat(t,1.0f);
		
		for(int x = 0; x < display.sizeX; x++) {
			float s = (float)x / display.sizeX;
			for(int y = 0; y < display.sizeY; y++) {
				float v = (float)y / display.sizeY;
				
				Color color = ColorExt.FromHSV(hue,s,v,1.0f);
				float height = Mathf.Sin(x + t) * Mathf.Cos(y + t);
				
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
	
	private void DrawAnger() {
		
		float t = Time.timeSinceLevelLoad;
		
		for(int x = 0; x < display.sizeX; x++) {
			for(int y = 0; y < display.sizeY; y++) {
				Color color = Color.Lerp(Color.red,Color.black,(t * 5.0f) % 1.0f);
				float height = Random.Range(-2.0f,2.0f);
				
				display.SetPixelRaw(x,y,height,color);
			}
		}
	}
	
	private void DrawEncourage() {
		
		float t = Time.timeSinceLevelLoad;
		
		for(int x = 0; x < display.sizeX; x++) {
			float dx = (float)x / display.sizeX;
			for(int y = 0; y < display.sizeY; y++) {
				float dy = (float)y / display.sizeY;
				Color color = Color.Lerp(Color.green,Color.black,(t * 5.0f) % 1.0f);
				float height = 3.0f * Mathf.Cos(dx * 12.0f + t * 2.0f) * Mathf.Sin(dy * 12.0f + t * 3.0f);
				
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
	
	private void DrawLava(float level) {
		for(int x = 0; x < display.sizeX; x++) {
			for(int y = 0; y < display.sizeY; y++) {
				float height = display.GetPixelHeight(x,y);
				float k = Mathf.Clamp01(height - level);
				
				Color color = display.GetPixelColor(x,y);
				
				display.SetPixelRaw(x,y,height,Color.Lerp(Color.red,color,k));
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

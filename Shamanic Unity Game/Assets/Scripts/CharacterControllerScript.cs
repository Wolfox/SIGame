using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using System.Collections.Generic;
using Leap;
using System;
using ShamanicInterface.Classifier;
using ShamanicInterface.State;

public class CharacterControllerScript : MonoBehaviour {

	private enum GameState {Game, Pause, EndGame};
	private GameState state;

	public GameObject PlayerMesh;
	public HUDScript hudScript;
	public LevelManager levelScript;
	public HandController controller;
	public AudioSource audio;

	private LevelManager.GameColors potionColor;
	private bool onExit = false;
	private bool onPotion = false;

	private float horzInput;
	private float vertInput;
	private float minTimeToMove = 0.5f;
	private float lastTimeMove = 0;


	private HMMClassifier classifier;

	void Awake() {
		Game.StartCulture();
		UpdateState(Game.GameState());
	}

	// Use this for initialization
	void Start() {
		state = GameState.Game;
		potionColor = LevelManager.GameColors.Neutral;
		controller.EnableCustomGestures(Gesture.GestureType.TYPESCREENTAP, true);
		controller.EnableCustomGestures(Gesture.GestureType.TYPEKEYTAP, true);
		controller.EnableCustomGestures(Gesture.GestureType.TYPESWIPE, true);
		UpdateButtons();
	}

	void UpdateState(Actions state) {
		classifier = Game.GetClassifier(state);
	}

	public void UpdateButtons() {
		hudScript.UpdateButtons();
	}

	void FixedUpdate() {
		if(state == GameState.Pause && !hudScript.CheckPause()) {
			Resume ();
		}
		
		MoveAndRotate();
	}

	// Update is called once per frame
	void Update () {
		ResetMoveRotateInpute();
		if(Game.culture != "") {
			CheckGestureActions();
		} else {
			CheckCustomGestures();
		}
		CheckKeyBoardActions();
	}

	void CheckCustomGestures() {
		GestureList gestures = controller.GetCustomGestures();

		for(int i = 0; i < gestures.Count; i++) {
			Gesture gesture = gestures[i];
			switch(gesture.Type) {
			case Gesture.GestureType.TYPESCREENTAP:
				MoveFoward();
				break;
			case Gesture.GestureType.TYPEKEYTAP:
				KeyTapGesture keyTapGesture = new KeyTapGesture(gesture);
				Vector3 vector = keyTapGesture.Position.ToUnityScaled();
				hudScript.PressButton (controller.transform.TransformPoint(vector));
				break;
			case Gesture.GestureType.TYPESWIPE:
				SwipeGesture swipeGesture = new SwipeGesture(gesture);
				Rotate90(swipeGesture.Direction.x);
				break;
			default:
				break;
			}
		}
	}

	/*void PressButton(Vector3 vector) {
		foreach(Button button in buttons) {
			Vector3[] corners = new Vector3 [4];
			RectTransform rectTrans = button.gameObject.GetComponent<RectTransform>();
			rectTrans.GetWorldCorners(corners);
			if(ContainInWorld(corners, vector)) {
				button.onClick.Invoke();
			}
		}
	}

	private bool ContainInWorld(Vector3[] corners, Vector3 point) {
		Vector3[] cornersInCamera = Array.ConvertAll(corners, element => Camera.main.WorldToViewportPoint(element));
		Vector3 pointInCamera = Camera.main.WorldToViewportPoint(point); 

		return Contain(cornersInCamera[0], cornersInCamera[2], pointInCamera);
	}
	
	private bool Contain(Vector2 downLeft, Vector2 topRight, Vector2 pos){
		return (pos.x >= downLeft.x && pos.x <= topRight.x &&
		        pos.y >= downLeft.y && pos.y <= topRight.y);
	}*/

	void ResetMoveRotateInpute() {
		vertInput = 0;
		horzInput = 0;
	}

	void CheckKeyBoardActions() {
		float keyH = Input.GetAxis("Horizontal");
		float keyV = Input.GetAxis("Vertical");
		
		if(keyH != 0) {horzInput = keyH;}
		if(keyV != 0) {vertInput = keyV;}

		if(Input.GetButtonDown ("Grab") && onExit) {
			Grab();
		}
		if(Input.GetButtonDown("Drink") && onPotion) {
			Drink();
		}
		if(Input.GetButtonDown("Pause")) {
			if(state == GameState.Game) { Pause(); }
				else {
					if(state == GameState.Pause) { Resume(); }
			}
		}
		if(Input.GetButtonDown("Mute")) {
			if(audio.mute) { Unmute(); }
			else { Mute(); }
		}
		if(Input.GetButtonDown("Quit")) {
			hudScript.Quit();
		}
	}

	void CheckGestureActions() {
		List<string> allActions = controller.GetGestures(classifier);
		List<string> actions = Game.UpdateActionBuffer(allActions);
		hudScript.AddActionsToGUI(allActions);

		switch(state) {
		case GameState.Game:
			CheckMove(allActions);
			CheckPauseDrinkGrab(actions);
			break;
		case GameState.Pause:
			CheckPauseActions(actions);
			break;
		case GameState.EndGame:
			CheckEndGameActions(actions);
			break;
		default:
			break;
		}
	}

	void CheckMove(List<string> actions) {
		for (int i = 0; i< actions.Count; i++) {
			switch(actions[i]) {
			case "FRONT":
				vertInput = 1;
				break;
			case "BACK":
				vertInput = -1;
				break;
			case "RIGHT":
				horzInput = 1;
				break;
			case "LEFT":
				horzInput = -1;
				break;
			default:
				break;
			}
		}
	}

	void CheckPauseDrinkGrab(List<string> actions) {
		for (int i = 0; i< actions.Count; i++) {
			switch(actions[i]) {
			case "PAUSE":
				Pause();
				break;
			case "DRINK":
				Drink();
				break;
			case "GRAB":
				Grab();
				break;
			default:
				break;
			}
		}
	}

	void CheckPauseActions(List<string> actions) {
		for (int i = 0; i< actions.Count; i++) {
			switch(actions[i]) {
			case "RESUME":
				Resume ();
				break;
			case "MUTE":
				Mute();
				break;
			case "UNMUTE":
				Unmute();
				break;
			default:
				break;
			}
		}
	}

	void CheckEndGameActions(List<string> actions) {
		for (int i = 0; i< actions.Count; i++) {
			switch(actions[i]) {
			case "RESUME":
				hudScript.BackToMainMenu();
				break;
			case "QUIT":
				hudScript.Quit();
				break;
			default:
				break;
			}
		}
	}

	void OnTriggerEnter (Collider other) {
		switch(other.tag) {
			case "Potion":
				onPotion = true;
				potionColor = ((Potion)other.GetComponent(typeof(Potion))).color;
				break;
			case "Finish":
				onExit = true;
				break;
			default:
				break;
		}
	}

	void OnTriggerExist (Collider other) {
		switch(other.tag) {
			case "Potion":
				onPotion = false;
				potionColor = LevelManager.GameColors.Neutral;
				break;
			case "Finish":
				onExit = false;
				break;
			default:
				break;
		}
	}

	void MoveAndRotate() {
		if(vertInput != 0) { Move (vertInput); }

		if(horzInput != 0) {  Rotate(horzInput); }
	}

	void Move (float value) {
		if(state != GameState.Game) {return;}

		Vector3 target = transform.position + transform.forward*value;
		target.y = transform.position.y;
		transform.position = Vector3.MoveTowards(transform.position, target, 0.1f);
	}

	void MoveFoward() {
		if(state != GameState.Game) {return;}
		if(lastTimeMove + minTimeToMove > Time.time) { return; }
		lastTimeMove = Time.time;

		transform.Translate(Vector3.forward*5f);

		LevelManager.LvlVal place = levelScript.CheckPosition(transform.position);
		switch(place) {
		case LevelManager.LvlVal.RPotion:
			if(Game.numberOfColors >= 3) {
				Drink(LevelManager.GameColors.Red);
			}
			break;
		case LevelManager.LvlVal.GPotion:
			if(Game.numberOfColors >= 2) {
				Drink(LevelManager.GameColors.Green);
			}
			break;
				case LevelManager.LvlVal.BPotion:
				if(Game.numberOfColors >= 1) {
					Drink(LevelManager.GameColors.Blue);
				}
			break;
		case LevelManager.LvlVal.RBlock:
			if(gameObject.layer != LayerMask.NameToLayer("Red") && Game.numberOfColors >= 3) {
				transform.Translate(Vector3.back*5f);
			}
			break;
		case LevelManager.LvlVal.GBlock:
			if(gameObject.layer != LayerMask.NameToLayer("Green") && Game.numberOfColors >= 2) {
				transform.Translate(Vector3.back*5f);
			}
			break;
		case LevelManager.LvlVal.BBlock:
			if(gameObject.layer != LayerMask.NameToLayer("Blue") && Game.numberOfColors >= 1) {
				transform.Translate(Vector3.back*5f);
			}
			break;
		case LevelManager.LvlVal.Finish:
			Exit();
			break;
		case LevelManager.LvlVal.Nothing:
			transform.Translate(Vector3.back*5f);
			break;
		default:
			break;
		}
	}

	void Rotate (float value) {
		if(state != GameState.Game) {return;}
		transform.RotateAround(transform.position, Vector3.up, value);
	}

	void Rotate90(float value) {
		if(Mathf.Abs (value) < 0.5f) { return; }
		if(lastTimeMove + minTimeToMove > Time.time) { return; }
		lastTimeMove = Time.time;

		Rotate(Mathf.Sign(value)*90);
	}

	void Grab() {
		if(!onExit) {return;}
		Exit();
	}

	void Exit() {
		state = GameState.EndGame;
		UpdateState(Game.EndGameState());
		Game.StartActionBuffer();
		hudScript.EndGame();
	}

	void Drink() {
		if(!onPotion) {return;}
		Drink (potionColor);
	}

	void Drink(LevelManager.GameColors color) {
		switch(color) {
		case LevelManager.GameColors.Red:
			gameObject.layer = LayerMask.NameToLayer("Red");
			hudScript.ChangePlayerColor("RED");
			break;
		case LevelManager.GameColors.Green:
			gameObject.layer = LayerMask.NameToLayer("Green");
			hudScript.ChangePlayerColor("GREEN");
			break;
		case LevelManager.GameColors.Blue:
			gameObject.layer = LayerMask.NameToLayer("Blue");
			hudScript.ChangePlayerColor("BLUE");
			break;
		case LevelManager.GameColors.Neutral:
			gameObject.layer = 0;
			hudScript.ChangePlayerColor("");
			break;
		}
	}

	public void Pause() {
		state = GameState.Pause;
		UpdateState(Game.PauseState());
		Game.StartActionBuffer();
		hudScript.Pause();
	}

	public void Resume() {
		state = GameState.Game;
		UpdateState(Game.GameState());
		hudScript.Resume ();
	}

	public void Mute() {
		audio.mute = true;
		hudScript.Mute();
	}

	public void Unmute() {
		audio.mute = false;
		hudScript.Unmute();
	}
}

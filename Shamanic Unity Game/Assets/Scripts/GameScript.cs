using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using System.IO;
using Leap;
using UnityEngine.UI;
using System;
using ShamanicInterface.Culture;
using ShamanicInterface.Utils;
using ShamanicInterface.State;
using ShamanicInterface.Classifier;

public static class Game {

	private static Dictionary<string,float> actionsBuffer = new Dictionary<string, float>();
	private static float bufferStartTime = 0;
	private static float minActionTime = 0.9f;
	private static float timeToStartRead = 3; 

	public static string culture = "";
	public static int bufferSize = 50;
	public static int numberOfColors = 3;

	public static Dictionary<string, HiddenMarkovModel<MultivariateNormalDistribution>> allModels =
		new Dictionary<string, HiddenMarkovModel<MultivariateNormalDistribution>> ();

	public static bool alreadyStarted = false;

	public static CulturalLayer culturalLayer = new CulturalLayer();

	public static List<HiddenMarkovModel<MultivariateNormalDistribution>> GetModels(Actions state) {
		return ShamanicInterface.Utils.Utils.GetModelsWithCulture(allModels, state.GetActions(),
		                                                     culturalLayer, culture);
	}

	public static void InitAllModels() {
		allModels.Add("OPEN_HAND",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/OPEN_HAND.bin"));
		allModels.Add("POINT_FRONT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/POINT_FRONT.bin"));
		allModels.Add("POINT_RIGHT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/POINT_RIGHT.bin"));
		allModels.Add("POINT_LEFT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/POINT_LEFT.bin"));
		allModels.Add("POINT_BACK",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/POINT_BACK.bin"));
		allModels.Add("OPEN_FRONT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/OPEN_FRONT.bin"));
		allModels.Add("OPEN_RIGHT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/OPEN_RIGHT.bin"));
		allModels.Add("OPEN_LEFT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/OPEN_LEFT.bin"));
		allModels.Add("HAND_HALT",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/HALT_HAND.bin"));
		allModels.Add("HAND_ROTATING",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/HAND_ROTATING.bin"));
		allModels.Add("INDEX_HUSH",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/INDEX_HUSH.bin"));
		allModels.Add("INDEX_ROTATING",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/INDEX_ROTATING.bin"));
		allModels.Add("MOUTH_MIMIC",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/MOUTH_MIMIC.bin"));
		allModels.Add("INDEX",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/NUM1.bin"));
		allModels.Add("INDEX_MIDDLE",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/NUM2.bin"));
		allModels.Add("INDEX_MIDDLE_RINGER",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/NUM3.bin"));
		allModels.Add("THE_RING",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/THE_RING.bin"));
		allModels.Add("THUMBS_DOWN",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/THUMBS_DOWN.bin"));
		allModels.Add("THUMBS_UP",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/THUMBS_UP.bin"));
		allModels.Add("WAVE",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/WAVE.bin"));
		allModels.Add("WAVE_NO_THANKS",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/WAVE_NO_THANKS.bin"));
		allModels.Add("BOTTLE_MIMIC",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/DRINK_PT.bin"));
		allModels.Add("HOLDING_GLASS",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/DRINK_NL.bin"));
		allModels.Add("GRAB",
		              HiddenMarkovModel<MultivariateNormalDistribution>.Load("GestureModels/GRAB.bin"));
	}

	public static void InitCulturalLayer() {
		culturalLayer.AddDefaultGesture("NOTHING", "OPEN_HAND");
		culturalLayer.AddDefaultGesture("SELECT", "POINTING");
		culturalLayer.AddDefaultGesture("CLICK", "INDEX_CLICK");
		culturalLayer.AddDefaultGesture("NUMBER_1","INDEX");
		culturalLayer.AddDefaultGesture("NUMBER_2","INDEX_MIDDLE");
		culturalLayer.AddDefaultGesture("NUMBER_3","INDEX_MIDDLE_RINGER");
		culturalLayer.AddDefaultGesture("YES","THUMBS_UP");
		culturalLayer.AddDefaultGesture("MUTE", "INDEX_HUSH");
		culturalLayer.AddDefaultGesture("UNMUTE", "MOUTH_MIMIC");
		culturalLayer.AddDefaultGesture("PAUSE", "HAND_HALT");
		culturalLayer.AddDefaultGesture("QUIT", "WAVE");
		culturalLayer.AddDefaultGesture("GRAB", "GRAB");
		culturalLayer.AddDefaultGesture("MUTE", "INDEX_HUSH");
		//culturalLayer.AddDefaultGesture("", "");

		//culturalLayer.AddCultureGesture("NUMBER_0","PT","FIST");
		culturalLayer.AddCultureGesture("NO", "PT", "THUMBS_DOWN");
		culturalLayer.AddCultureGesture("FRONT", "PT", "POINT_FRONT");
		culturalLayer.AddCultureGesture("RIGHT", "PT", "POINT_RIGHT");
		culturalLayer.AddCultureGesture("LEFT", "PT", "POINT_LEFT");
		culturalLayer.AddCultureGesture("BACK", "PT", "POINT_BACK");
		culturalLayer.AddCultureGesture("DRINK", "PT", "BOTTLE_MIMIC");
		culturalLayer.AddCultureGesture("RESUME", "PT", "INDEX_ROTATING");
		//culturalLayer.AddCultureGesture("", "PT", "");


		//culturalLayer.AddCultureGesture("NUMBER_0", "NL", "THE_RING");
		culturalLayer.AddCultureGesture("NO", "NL", "WAVE_NO_THANKS");
		culturalLayer.AddCultureGesture("FRONT", "NL", "OPEN_FRONT");
		culturalLayer.AddCultureGesture("RIGHT", "NL", "OPEN_RIGHT");
		culturalLayer.AddCultureGesture("LEFT", "NL", "OPEN_LEFT");
		culturalLayer.AddCultureGesture("DRINK", "NL", "HOLDING_GLASS");
		culturalLayer.AddCultureGesture("RESUME", "NL", "HAND_ROTATING");
		//culturalLayer.AddCultureGesture("", "NL", "");
	}

	public static void StartCulture() {
		if(!alreadyStarted) {
			InitAllModels();
			InitCulturalLayer();
			alreadyStarted = true;
		}
	}

	public static HMMClassifier GetClassifier(Actions state) {
		HMMClassifier classifier = new HMMClassifier(GetModels(state), state.GetActions());
		classifier.StartClassifier();
		return classifier;
	}

	public static Actions GameState() {
		Actions state = new Actions("Game State");
		state.AddAction("NOTHING");
		state.AddAction("FRONT");
		state.AddAction("RIGHT");
		state.AddAction("LEFT");
		state.AddAction("BACK");
		state.AddAction("DRINK");
		state.AddAction("GRAB");
		state.AddAction("PAUSE");
		/*state.AddAction("MUTE");
		state.AddAction("UNMUTE");*/
		return state;
	}

	public static Actions ChooseNumberState() {
		Actions state = new Actions("Choose Number State");
		state.AddAction("NOTHING");
		state.AddAction("NUMBER_1");
		state.AddAction("NUMBER_2");
		state.AddAction("NUMBER_3");
		return state;
	}

	public static Actions StartGameState() {
		Actions state = new Actions("Pause State");
		state.AddAction("NOTHING");
		state.AddAction("YES");
		state.AddAction("NO");
		return state;
	}

	public static Actions PauseState() {
		Actions state = new Actions("Pause State");
		state.AddAction("NOTHING");
		state.AddAction("RESUME");
		state.AddAction("MUTE");
		state.AddAction("UNMUTE");
		return state;
	}

	public static Actions EndGameState() {
		Actions state = new Actions("End Game State");
		state.AddAction("NOTHING");
		state.AddAction("QUIT");
		state.AddAction("RESUME");
		return state;
	}

	public static Actions NothingState() {
		Actions state = new Actions("Nothing State");
		state.AddAction("NOTHING");
		return state;
	}

	public static void StartActionBuffer() {
		bufferStartTime = Time.time;
		actionsBuffer.Clear();
	}

	public static List<string> UpdateActionBuffer(List<string> actions) {
		List<string> actionsToCheck = new List<string> (actionsBuffer.Keys);
		List<string> returnActions = new List<string>();

		if((bufferStartTime + timeToStartRead) > Time.time) {
			return returnActions;
		}

		foreach(string action in actions) {
			actionsToCheck.Remove(action);
			if(!actionsBuffer.ContainsKey(action)) {
				actionsBuffer[action] = Time.time;
			}

			if(actionsBuffer[action] + minActionTime < Time.time) {
				returnActions.Add (action);
			}
		}

		foreach (string actionToCheck in actionsToCheck) {
			actionsBuffer.Remove(actionToCheck);
		}

		return returnActions;
	}

	public static string GetStringAction(string action) {
		switch(action) {
		case "NOTHING":
			return "";
		default:
			return action;
		}
	}

	/*private static void TapButtons(GestureList gestures, Button[] buttons, Camera camera) {
		
		for(int i = 0; i < gestures.Count; i++) {
			Gesture gesture = gestures[i];
			Vector3 vect = new Vector3();
			if(gesture.Type == ScreenTapGesture.ClassType()) {
				//Debug.Log ("tap");
				ScreenTapGesture screentapGesture = new ScreenTapGesture(gesture);
				vect = screentapGesture.Position.ToUnityScaled();
			}
			if(gesture.Type == KeyTapGesture.ClassType()) {
				//Debug.Log ("key");
				KeyTapGesture screentapGesture = new KeyTapGesture(gesture);
				vect = screentapGesture.Position.ToUnityScaled();
			}
			
			vect *= 20;
			vect -= new Vector3(0,5,3);
			
			foreach(Button button in buttons) {
				Vector3[] corners = new Vector3 [4];
				RectTransform rectTrans = button.gameObject.GetComponent<RectTransform>();
				rectTrans.GetWorldCorners(corners);
				if(ContainInWorld(corners, vect)) {
					button.onClick.Invoke();
				}
			}
		}
	}

	private static bool ContainInWorld(Vector3[] corners, Vector3 point) {
		Vector3[] cornersInCamera = Array.ConvertAll(corners, element => Camera.main.WorldToViewportPoint(element));
		Vector3 pointInCamera = Camera.main.WorldToViewportPoint(point); 
		
		//testCube.position = point;
		return Contain(cornersInCamera[0], cornersInCamera[2], pointInCamera);
	}
	
	private static bool Contain(Vector2 downLeft, Vector2 topRight, Vector2 pos){
		return (pos.x >= downLeft.x && pos.x <= topRight.x &&
		        pos.y >= downLeft.y && pos.y <= topRight.y);
	}*/
}
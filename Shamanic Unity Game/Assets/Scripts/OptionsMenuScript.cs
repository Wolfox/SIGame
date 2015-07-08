using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Leap;
using System;
using System.Linq;
using ShamanicInterface.Classifier;

public class OptionsMenuScript : MonoBehaviour {

	private enum OptionsMenuState {Main, Culture, Colors};
	private OptionsMenuState state;

	public GameObject culturePanel;
	public GameObject numOfColorsPanel;
	public Transform pointer;

	public HandController controller;
	private Button[] buttons = {};
	private HMMClassifier classifier;

	public void Awake() {
		Game.StartCulture();
		classifier = Game.GetClassifier(Game.ChooseNumberState());
	}

	public void Start() {
		state = OptionsMenuState.Main;
		controller.EnableCustomGestures(Gesture.GestureType.TYPESCREENTAP, true);
		controller.EnableCustomGestures(Gesture.GestureType.TYPEKEYTAP, true);
		UpdateButtons();
	}

	public void UpdateButtons() {
		buttons = GetComponentsInChildren<Button>();
	}

	public void Update() {
		if(state == OptionsMenuState.Colors && Game.culture != "") {
			NumberGestures();
		}
		TapGestures();
	}

	private void TapGestures() {

		GestureList gestures = controller.GetCustomGestures();

		for(int i = 0; i < gestures.Count; i++) {
			Gesture gesture = gestures[i];
			Vector3 vect = new Vector3();
			if(gesture.Type == ScreenTapGesture.ClassType()) {
				//Debug.Log ("screen");
				ScreenTapGesture screentapGesture = new ScreenTapGesture(gesture);
				vect = screentapGesture.Position.ToUnityScaled();
			}
			if(gesture.Type == KeyTapGesture.ClassType()) {
				//Debug.Log ("key");
				KeyTapGesture screentapGesture = new KeyTapGesture(gesture);
				vect = screentapGesture.Position.ToUnityScaled();
			}
			vect = controller.transform.TransformPoint(vect);

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

	private void NumberGestures() {
		string action = "";
		List<string> allActions = controller.GetGestures(classifier);
		List<string> actions = Game.UpdateActionBuffer(allActions);

		if(actions.Count == 1) {
			action = actions[0];
		}

		switch(action) {
			case "NUMBER_1":
				ChangeNumOfColors(1);
				break;
			case "NUMBER_2":
				ChangeNumOfColors(2);
				break;
			case "NUMBER_3":
				ChangeNumOfColors(3);
				break;
			case "NOTHING":
			default:
				break;
		}
	}

	private bool ContainInWorld(Vector3[] corners, Vector3 point) {
		Vector3[] cornersInCamera = Array.ConvertAll(corners, element => Camera.main.WorldToViewportPoint(element));
		Vector3 pointInCamera = Camera.main.WorldToViewportPoint(point); 

		pointer.position = point;
		return Contain(cornersInCamera[0], cornersInCamera[2], pointInCamera);
	}

	private bool Contain(Vector2 downLeft, Vector2 topRight, Vector2 pos){
		return (pos.x >= downLeft.x && pos.x <= topRight.x &&
		        pos.y >= downLeft.y && pos.y <= topRight.y);
	}

	public void LoadMenu(int level) {
		Application.LoadLevel(level);
	}

	public void OpenOption(int option) {
		if(state == OptionsMenuState.Main) {
			switch(option) {
			case(0):
				LoadMenu(0);
				break;
			case(1):
				OpenNumOfColors();
				break;
			case(2):
				OpenCulture();
				break;
			default:
				Debug.LogError("Option " + option + " not recognized");
				break;
			}
		}
	}

	private void OpenCulture() {
		state = OptionsMenuState.Culture;
		culturePanel.SetActive(true);
		UpdateButtons();
	}

	private void OpenNumOfColors() {
		state = OptionsMenuState.Colors;
		numOfColorsPanel.SetActive(true);
		numOfColorsPanel.GetComponent<NumberPanelScript>().UpdateInterface();
		classifier = Game.GetClassifier(Game.ChooseNumberState());
		Game.StartActionBuffer();
		UpdateButtons();
	}

	public void ChangeCulture(string culture) {
		Game.culture = culture;
		state = OptionsMenuState.Main;
		culturePanel.SetActive(false);
		UpdateButtons();
	}

	public void ChangeNumOfColors(int number) {
		Game.numberOfColors = number;
		state = OptionsMenuState.Main;
		numOfColorsPanel.SetActive(false);
		UpdateButtons();
	}

}

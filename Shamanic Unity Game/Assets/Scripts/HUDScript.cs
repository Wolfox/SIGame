using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class HUDScript : MonoBehaviour {

	public GameObject pausePanel;
	public GameObject endGamePanel;
	public Text guiText;
	public Image guiPanel;

	private Button[] buttons = {};
	public GameObject pauseButton;
	public Transform pointer;

	public void PressButton(Vector3 vector) {
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

		pointer.position = point;

		return Contain(cornersInCamera[0], cornersInCamera[2], pointInCamera);
	}
	
	private bool Contain(Vector2 downLeft, Vector2 topRight, Vector2 pos){
		return (pos.x >= downLeft.x && pos.x <= topRight.x &&
		        pos.y >= downLeft.y && pos.y <= topRight.y);
	}

	public void UpdateButtons() {
		pauseButton.SetActive(Game.culture == "");
		buttons = GetComponentsInChildren<Button>();
	}

	public void Pause() {
		pausePanel.SetActive(true);
		pausePanel.GetComponent<PausePanelScript>().SetButtons(Game.culture == "");
		UpdateButtons();
	}

	public void Resume() {
		pausePanel.SetActive(false);
		UpdateButtons();
	}

	public void ChangePlayerColor(string color) {
		//ChangeText(color);
		switch(color) {
		case "RED":
			ChangeColor(Color.red);
			break;
		case "GREEN":
			ChangeColor(Color.green);
			break;
		case "BLUE":
			ChangeColor(Color.blue);
			break;
		default:
			ChangeColor(Color.clear);
			break;
		}
	}


	public void AddActionsToGUI(List<string> actions) {
		string result = "";
		for(int i = 0; i < actions.Count; i++) {
			string action = Game.GetStringAction(actions[i]); 
			if(result != "" && action != "") {
				result += " + " + action;
			} else {
				result += action;
			}
		}
		ChangeText(result);
	}

	public void ChangeText(string text) {
		guiText.text = text;
	}

	public void ChangeTextColor(Color color) {
		guiText.color = color;
	}

	public void ChangeColor(Color color) {
		color.a /= 4;
		guiPanel.color = color;
	}

	/*public void ChangeAplha(float alpha) {
		Color color = guiPanel.color;
		color.a = alpha;
		guiPanel.color = color;
	}*/

	public string GetText() {
		return guiText.text;
	}

	public bool CheckPause() {
		return pausePanel.activeSelf;
	}

	public void EndGame() {
		endGamePanel.SetActive(true);
		endGamePanel.GetComponent<EndPanelScript>().SetButtons(Game.culture == "");
		UpdateButtons();
	}

	public void Quit() {
		Application.Quit();
	}

	public void BackToMainMenu() {
		Application.LoadLevel(0);
	}

	public void Mute() {
		pausePanel.GetComponent<PausePanelScript>().Mute();
		UpdateButtons();
	}

	public void Unmute() {
		pausePanel.GetComponent<PausePanelScript>().Unmute();
		UpdateButtons();
	}

}

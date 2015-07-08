using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PausePanelScript : MonoBehaviour {

	public GameObject ResumeButton;
	public GameObject MuteButton;
	public GameObject UnmuteButton;

	private bool muted = false;

	public void SetButtons(bool val) {
		ResumeButton.SetActive(val);
		MuteButton.SetActive(val);
		UnmuteButton.SetActive(val);
		if(val) { ActiveSoundButtons(); }
	}

	public void Mute() {
		muted = true;
		ActiveSoundButtons();
	}

	public void Unmute() {
		muted = false;
		ActiveSoundButtons();
	}

	public void ActiveSoundButtons() {
		MuteButton.SetActive(!muted && Game.culture == "");
		UnmuteButton.SetActive(muted && Game.culture == "");
	}
}

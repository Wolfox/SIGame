using UnityEngine;
using System.Collections;

public class EndPanelScript : MonoBehaviour {
	
	public GameObject ResumeButton;
	public GameObject QuitButton;
	
	public void SetButtons(bool val) {
		ResumeButton.SetActive(val);
		QuitButton.SetActive(val);
	}
}

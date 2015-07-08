using UnityEngine;
using System.Collections;

public class NumberPanelScript : MonoBehaviour {

	public GameObject Number1Button;
	public GameObject Number2Button;
	public GameObject Number3Button;
	public GameObject Number1Image;
	public GameObject Number2Image;
	public GameObject Number3Image;

	public void UpdateInterface() {
		Number1Button.SetActive(false);
		Number2Button.SetActive(false);
		Number3Button.SetActive(false);
		Number1Image.SetActive(false);
		Number2Image.SetActive(false);
		Number3Image.SetActive(false);
		switch(Game.culture){
		case "PT":
		case "NL":
			Number1Image.SetActive(true);
			Number2Image.SetActive(true);
			Number3Image.SetActive(true);
			break;
		case "":
		default:
			Number1Button.SetActive(true);
			Number2Button.SetActive(true);
			Number3Button.SetActive(true);
			break;
		}
	}
}

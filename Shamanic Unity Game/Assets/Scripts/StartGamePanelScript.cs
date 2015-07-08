using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartGamePanelScript : MonoBehaviour {

	public GameObject YesButton;
	public GameObject YesImage;
	public GameObject NoButton;
	public GameObject NoImage;

	public Sprite PTYes;
	public Sprite PTNo;
	public Sprite NLYes;
	public Sprite NLNo;

	public void UpdateYesNoInterface() {
		YesButton.SetActive(false);
		NoButton.SetActive(false);
		YesImage.SetActive(false);
		NoImage.SetActive(false);
		switch(Game.culture) {
		case "PT":
			YesImage.GetComponent<Image>().sprite = PTYes;
			NoImage.GetComponent<Image>().sprite = PTNo;
			YesImage.SetActive(true);
			NoImage.SetActive(true);
			break;
		case "NL":
			YesImage.GetComponent<Image>().sprite = NLYes;
			NoImage.GetComponent<Image>().sprite = NLNo;
			YesImage.SetActive(true);
			NoImage.SetActive(true);
			break;
		case "":
		default:
			YesButton.SetActive(true);
			NoButton.SetActive(true);
			break;
		}
	}

}

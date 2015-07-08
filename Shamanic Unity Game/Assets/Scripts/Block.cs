using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	public enum Side {Up, Down, Left, Right};

	public GameObject Up, Down, Left, Right, Bottom, Ceiling, Collider;
	
	public Material RedMaterial;
	public Material GreenMaterial;
	public Material BlueMaterial;
	public Material NeutralMaterial;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeWall(Side side, bool val) {
		switch(side) {
			case Side.Up:
				Up.SetActive(val);
				break;
			case Side.Down:
				Down.SetActive(val);
				break;
			case Side.Left:
				Left.SetActive(val);
				break;
			case Side.Right:
				Right.SetActive(val);
				break;
		}
	}

	public void FillColor(LevelManager.GameColors colour) {
		switch(colour) {
			case LevelManager.GameColors.Red:
				Collider.SetActive(true);
				ChangeColor(RedMaterial, "Red");
				break;
			case LevelManager.GameColors.Green:
				Collider.SetActive(true);
				ChangeColor(GreenMaterial, "Green");
				break;
			case LevelManager.GameColors.Blue:
				Collider.SetActive(true);
				ChangeColor(BlueMaterial, "Blue");
				break;
			case LevelManager.GameColors.Neutral:
			default:
				Collider.SetActive(false);
				ChangeColor(NeutralMaterial, "Default");
				break;
		}
	}

	private void ChangeColor(Material color, string layer) {
		Collider.layer = LayerMask.NameToLayer(layer);
		Up.GetComponent<Renderer>().material = color;
		Down.GetComponent<Renderer>().material = color;
		Left.GetComponent<Renderer>().material = color;
		Right.GetComponent<Renderer>().material = color;
		Bottom.GetComponent<Renderer>().material = color;
		Ceiling.GetComponent<Renderer>().material = color;
	}

}

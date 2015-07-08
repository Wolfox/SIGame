using UnityEngine;
using System.Collections;

public class Potion : MonoBehaviour {

	public LevelManager.GameColors color;
	public GameObject liquid;

	public Material RedMaterial;
	public Material GreenMaterial;
	public Material BlueMaterial;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		GetComponent<Rigidbody>().angularVelocity = new Vector3(0, Random.value * 5f, 0f);
	}

	public void FillColor(LevelManager.GameColors colour) {
		switch(colour) {
		case LevelManager.GameColors.Red:
			liquid.GetComponent<Renderer>().material = RedMaterial;
			break;
		case LevelManager.GameColors.Green:
			liquid.GetComponent<Renderer>().material = GreenMaterial;
			break;
		case LevelManager.GameColors.Blue:
			liquid.GetComponent<Renderer>().material = BlueMaterial;
			break;
		default:
			throw new System.ArgumentNullException();
		}
	}
}

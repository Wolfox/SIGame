using UnityEngine;
using System.Collections;

public class EndCube : MonoBehaviour {

	public bool randomRotation;

	private Color initColor;
	private Color endColor;
	private float colorRate = 3.0f;
	private float nextColorTime = 0.0f;

	private Color[] colors = new Color[] {
		Color.white, Color.red, Color.green, Color.blue,
		Color.cyan, Color.magenta, Color.yellow, Color.black};

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().angularVelocity = new Vector3(0.0f,1.0f,0.0f);

		GetComponent<Renderer>().material.color = Color.red;
		initColor = Color.white;
		endColor = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		if(randomRotation) {
			GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.value * 0.5f,
			                                             Random.value * 1f,
			                                             Random.value * 0.5f);

			if(Time.time > nextColorTime) {
				nextColorTime += colorRate;
				initColor = endColor;
				endColor = getRandomColor();
			}

			float lerp = (Time.time - (nextColorTime - colorRate)) / colorRate;
			GetComponent<Renderer>().material.color = Color.Lerp(initColor, endColor, lerp);
		}
	}

	Color getRandomColor() {
		if(colors.Length < 1) { return Color.white; }
		if(colors.Length == 1) { return colors[0]; }

		Color nextColor;
		do {
			nextColor = colors[Random.Range(0,colors.Length)];
		} while(nextColor == endColor);
		return nextColor;
	}
}

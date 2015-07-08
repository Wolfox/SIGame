using UnityEngine;
using System.Collections;
using System.IO;
using Leap;

public class CubesLevelTestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey("escape")) {
			Application.LoadLevel(0);
		}
	}
}

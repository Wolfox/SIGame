using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	public enum CameraType {Static, Following};

	public CameraType cameraType;

	public Transform player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(cameraType == CameraType.Following && player != null) {
			Vector3 newPos = new Vector3(player.position.x,
			                             transform.position.y,
			                             player.position.z);
			transform.position = newPos;
		}
	}
}

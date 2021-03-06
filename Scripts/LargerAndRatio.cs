using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LargerAndRatio : MonoBehaviour {
	OVRManager manager;
	int percent;
	// Use this for initialization
	void Start () {
		percent = 50;
	}
	
	// Update is called once per frame
	void Update () {

		if (OVRInput.Get (OVRInput.RawButton.A) || Input.GetKeyDown (KeyCode.A)) {
			Debug.Log ("Graph one is larger");
		} else if (OVRInput.Get (OVRInput.RawButton.B) || Input.GetKeyDown (KeyCode.B)) {
			Debug.Log ("Graph two is larger");
		} else if (OVRInput.Get (OVRInput.RawButton.X) || Input.GetKeyDown (KeyCode.UpArrow)) {
			percent += 5;
			Debug.Log ("Percent: " + percent);
		} else if (OVRInput.Get (OVRInput.RawButton.Y) || Input.GetKeyDown (KeyCode.DownArrow)) {
			percent -= 5;
			Debug.Log ("Percent: " + percent);
		}
	}
}

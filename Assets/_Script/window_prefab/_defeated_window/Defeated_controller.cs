using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defeated_controller : MonoBehaviour {
	public SceneController sceneController;

	void Start(){
		sceneController = GameObject.Find ("SceneController").GetComponent<SceneController> ();
	}
}

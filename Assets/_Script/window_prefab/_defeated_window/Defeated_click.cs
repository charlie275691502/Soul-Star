using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defeated_click : MonoBehaviour {
	public Defeated_controller defeated_controller;

	void OnMouseDown(){
		defeated_controller.sceneController.next_scene ("NorthMainCity");
	}
}

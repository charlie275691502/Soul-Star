using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_load_window_DialogController : MonoBehaviour {
	[HideInInspector] public Start_load_window_controller start_load_window_controller;

	void Start () {
		start_load_window_controller = GameObject.Find ("start_load_window(Clone)").GetComponent<Start_load_window_controller> ();
		change_parent_has_window_prefab(true);
	}


	public void change_parent_has_window_prefab(bool boolean){
		start_load_window_controller.has_window_prefab = boolean;
	}
}

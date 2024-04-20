using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAVELOAD_window_DialogController : MonoBehaviour {
	[HideInInspector] public SAVELOAD_window_controller sAVELOAD_window_controller;

	void Start () {
		sAVELOAD_window_controller = GameObject.Find ("SAVELOAD_window(Clone)").GetComponent<SAVELOAD_window_controller> ();
		change_parent_has_window_prefab(true);
	}


	public void change_parent_has_window_prefab(bool boolean){
		sAVELOAD_window_controller.has_window_prefab = boolean;
	}
}

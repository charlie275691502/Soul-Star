using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_window_controller : MonoBehaviour {
	public MainMapController mainMapController;
	public Stage_imformation_manager stage_imformation_manager;

	void Start(){
		mainMapController = GameObject.Find ("MainMapController").GetComponent<MainMapController> ();
		change_parent_has_window_prefab (true);
		stage_imformation_manager.setup ();
	}

	public void change_parent_has_window_prefab(bool boolean){
		mainMapController.has_window_prefab = boolean;
	}
}

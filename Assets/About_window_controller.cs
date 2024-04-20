using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class About_window_controller : MonoBehaviour {
	[HideInInspector] public MainCityController mainCityController;
	[HideInInspector] public DataController dataController;

	void Start(){
		mainCityController = GameObject.Find ("MainCityController").GetComponent<MainCityController> ();
		dataController = GameObject.Find ("DataController").GetComponent<DataController> ();
	}

	public void change_parent_has_window_prefab(bool boolean){
		mainCityController.has_window_prefab = boolean;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAVELOAD_window_start_game : MonoBehaviour {
	public int order;
	public close close;
	public SAVELOAD_window_controller sAVELOAD_window_controller;
	private DataController dataController;

	void Start(){
		dataController = GameObject.Find("DataController").GetComponent<DataController>();
	}

	void OnMouseDown(){
		if(sAVELOAD_window_controller.has_window_prefab)return;
		close.close_window ();

		dataController.data = sAVELOAD_window_controller.datas [order];

		if (dataController.data.main_progress == 1) sAVELOAD_window_controller.mainCityController.sceneController.next_scene("Story1");
		else if((dataController.data.main_progress / 100) % 2 == 1)sAVELOAD_window_controller.mainCityController.sceneController.next_scene("NorthMainCity");
		else if((dataController.data.main_progress / 100) % 2 == 0)sAVELOAD_window_controller.mainCityController.sceneController.next_scene("SouthMainCity");
	}
}

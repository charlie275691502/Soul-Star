using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_load_window_start_game : MonoBehaviour {
	public int order;
	public close close;
	public Start_load_window_controller start_load_window_controller;
	private DataController dataController;

	void Start(){
		dataController = GameObject.Find("DataController").GetComponent<DataController>();
	}

	void OnMouseDown(){
		if(start_load_window_controller.has_window_prefab)return;
		close.close_window ();

		dataController.data = start_load_window_controller.datas [order];

		if (dataController.data.main_progress == 1) start_load_window_controller.startController.sceneController.next_scene("Story1");
		else if((dataController.data.main_progress / 100) % 2 == 1)start_load_window_controller.startController.sceneController.next_scene("NorthMainCity");
		else if((dataController.data.main_progress / 100) % 2 == 0)start_load_window_controller.startController.sceneController.next_scene("SouthMainCity");
	}
}

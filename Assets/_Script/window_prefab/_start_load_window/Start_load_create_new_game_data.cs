using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_load_create_new_game_data : MonoBehaviour {
	public Start_load_window_controller start_load_window_controller;
	public int order;

	void OnMouseDown(){
		if(start_load_window_controller.has_window_prefab)return;
		start_load_window_controller.new_data(order);
	}
}

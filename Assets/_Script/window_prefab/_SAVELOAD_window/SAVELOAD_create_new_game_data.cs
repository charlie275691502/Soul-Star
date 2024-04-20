using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAVELOAD_create_new_game_data : MonoBehaviour {
	public SAVELOAD_window_controller sAVELOAD_window_controller;
	public int order;

	void OnMouseDown(){
		if(sAVELOAD_window_controller.has_window_prefab)return;
		sAVELOAD_window_controller.new_data(order);
	}
}

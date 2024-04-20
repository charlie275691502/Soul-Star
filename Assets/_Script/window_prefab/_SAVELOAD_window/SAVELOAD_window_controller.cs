using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAVELOAD_window_controller : MonoBehaviour {
	[HideInInspector] public bool has_window_prefab;
	[HideInInspector] public MainCityController mainCityController;

	public Color green_color;
	public Color red_color;
	public Data_box_struct[] data_progress_struct;
	public Data[] datas;


	void Start () {
		for (int i = 0; i < 3; i++) {
			datas [i].order = i + 1;
			datas [i].load_data ();
		}

		has_window_prefab = false;
		mainCityController = GameObject.Find ("MainCityController").GetComponent<MainCityController>();
		change_parent_has_window_prefab(true);
		for (int i = 0; i < 3; i++) construct_data_box (i);
	}

	public void construct_data_box(int order){
		if (datas [order].main_progress == 0) {
			data_progress_struct [order].new_game_box(true, green_color, red_color);
			return;
		}

		data_progress_struct [order].new_game_box(false, green_color, red_color);
		if  (datas [order].main_progress == 1)data_progress_struct [order].main_progress_text.text = "新手教學";
		else data_progress_struct [order].main_progress_text.text = datas [order].main_progress / 100 + "-" + datas [order].main_progress % 100;
	}

	public void new_data(int order){
		datas [order].new_data ();
		construct_data_box (order);
	}

	public void delete_data(int order){
		datas [order].delete_data ();
		construct_data_box (order);
	}

	public void change_parent_has_window_prefab(bool boolean){
		mainCityController.has_window_prefab = boolean;
	}
}

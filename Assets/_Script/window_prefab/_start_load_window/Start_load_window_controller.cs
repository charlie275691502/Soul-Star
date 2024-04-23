using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class Data_box_struct{
	public Text main_progress_text;

	public SpriteRenderer box_sr;
	public GameObject load_game_canvas;
	public GameObject new_game_button;

	public void new_game_box(bool boolean, Color green_color, Color red_color){
		load_game_canvas.SetActive (!boolean);
		new_game_button.SetActive (boolean);
		box_sr.color = (boolean) ? green_color : red_color;
	}
}

public class Start_load_window_controller : MonoBehaviour {
	[HideInInspector] public bool has_window_prefab;
	[HideInInspector] public StartController startController;

	public Color green_color;
	public Color red_color;
	public Data[] datas;
	public Data_box_struct[] data_progress_struct;

	void Start () {
		for (int i = 0; i < 3; i++) {
			datas [i].order = i + 1;
			datas [i].load_data ();
		}
		if (GameObject.Find ("click_start") != null) GameObject.Find ("click_start").SetActive (false);
		if (GameObject.Find ("subtitle") != null) GameObject.Find ("subtitle").SetActive (false);
		has_window_prefab = false;
		startController = GameObject.Find ("StartController").GetComponent<StartController>();
		change_parent_has_window_prefab(true);
		for (int i = 0; i < 3; i++) construct_data_box (i);
	}

	public void construct_data_box(int order){
		if (datas [order].main_progress == 0) {
			data_progress_struct [order].new_game_box(true, green_color, red_color);
			return;
		}

		data_progress_struct [order].new_game_box(false, green_color, red_color);
		if  (datas [order].main_progress == 1)data_progress_struct [order].main_progress_text.text = "Tutorial";
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
		startController.has_window_prefab = boolean;
	}
}

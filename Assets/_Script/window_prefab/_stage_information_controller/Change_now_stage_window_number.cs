using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_now_stage_window_number : MonoBehaviour {
	public MainMapController mainMapController;
	public int chapter;
	public int number;
	public int front_stage1_chapter;
	public int front_stage1_number;
	public int front_stage2_chapter;
	public int front_stage2_number;

	void OnMouseDown(){
		mainMapController.now_stage_window_chapter = chapter;
		mainMapController.now_stage_window_number = number;
	}
}

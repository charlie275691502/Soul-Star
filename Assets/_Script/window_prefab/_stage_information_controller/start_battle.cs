using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class start_battle : MonoBehaviour {
	public Stage_window_controller stage_window_controller;

	void OnMouseDown(){
		stage_window_controller.mainMapController.dataController.stage_loading_data.allies_order = stage_window_controller.stage_imformation_manager.select_character_manager.list();

		StageInformation s = stage_window_controller.mainMapController.dataController.stageInformation;
		if (s.has_start_story)stage_window_controller.mainMapController.sceneController.next_scene(s.stage_chapter.ToString() + "_" + s.stage_number.ToString() + "_start");
		else stage_window_controller.mainMapController.sceneController.next_scene("general_battle");
	}
}

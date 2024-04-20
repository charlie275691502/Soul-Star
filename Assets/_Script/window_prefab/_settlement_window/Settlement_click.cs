using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settlement_click : MonoBehaviour {
	public Settlement_controller settlement_controller;

	void OnMouseDown(){
		StageInformation s = GameObject.Find ("GameController").GetComponent<GameController> ().dataController.stageInformation;
		if(s.has_end_story)settlement_controller.sceneController.next_scene (s.stage_chapter.ToString() + "_" + s.stage_number.ToString() + "_end");
		else settlement_controller.sceneController.next_scene ("MainMap1");
	}
}

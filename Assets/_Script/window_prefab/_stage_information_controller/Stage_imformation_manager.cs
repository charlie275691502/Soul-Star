using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage_imformation_manager : MonoBehaviour {
	public MainMapController mainMapController;
	public DataController dataController;

	public Select_character_manager select_character_manager;

	public int stage_chapter;
	public int stage_number;
	public Text stage_number_text;
	public Text stage_name_text;
	public Text suggest_element_text;
	public Text stage_introduction_text;
	public Text victory_text;
	public Text defeat_text;

	void Awake(){
		mainMapController = GameObject.Find ("MainMapController").GetComponent<MainMapController> ();
		dataController = GameObject.Find ("DataController").GetComponent<DataController> ();
	}

	public void setup(){
		foreach (StageInformation s in dataController.config.stageInformation) {
			if(s.stage_number == mainMapController.now_stage_window_number){
				dataController.stageInformation = s;
				stage_chapter = s.stage_chapter;
				stage_number = s.stage_number;
				stage_number_text.text = s.stage_chapter.ToString () + "-" + s.stage_number.ToString ();
				stage_name_text.text = s.stage_name;
				suggest_element_text.text = s.suggest_element;
				stage_introduction_text.text = s.stage_introduction;
				victory_text.text = s.victory_condition.ToString ();
				defeat_text.text = s.defeat_condition.ToString ();
			}
		}
	}
}

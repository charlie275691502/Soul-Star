using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select_character_manager : MonoBehaviour {
	public Stage_window_controller stage_window_controller;
	public Transform selected_folder;
	public Transform owned_folder;
	public GameObject select_character_manager_click_character;
	public GameObject Lock;
	public GameObject Fork;
	public GameObject[] characters;
	public bool[] selecteds;

	int selected_length;
	int owned_length;


	void Start () {
		characters = new GameObject[stage_window_controller.stage_imformation_manager.dataController.data.characterData.Length];
		selecteds = new bool[characters.Length];
		selected_length = 0;
		owned_length = 0;

		// 角色加入 selected owned
		for (int i = 0; i < characters.Length; i++) {
			if(stage_window_controller.stage_imformation_manager.dataController.data.characterData [i].character_location == Character_location.Unavailable)continue;

			characters [i] = Instantiate (select_character_manager_click_character, new Vector3 (), Quaternion.identity);
			characters [i].GetComponent<SpriteRenderer> ().sprite = stage_window_controller.stage_imformation_manager.dataController.data.characterData [i].avatar;
			characters [i].GetComponent<Select_character_manager_click_character> ().order = i;
			characters [i].GetComponent<Select_character_manager_click_character> ().Name = stage_window_controller.stage_imformation_manager.dataController.data.characterData [i].Name;
			if (is_limit (characters [i].GetComponent<Select_character_manager_click_character> ().Name)) {
				selecteds [i] = true;
				characters [i].transform.parent = selected_folder;
				characters [i].transform.localPosition = new Vector3 ((selected_length++) * 4, 0, 0);
				Instantiate (Lock, characters [i].transform.position + new Vector3 (0, 2.5f, 0), Quaternion.identity).transform.parent = selected_folder;
			} else {
				selecteds [i] = false;
				characters [i].transform.parent = owned_folder;
				characters [i].transform.localPosition = new Vector3 ((owned_length++) * 4, 0, 0);
			}
		}

		for (int i = stage_window_controller.stage_imformation_manager.dataController.stageInformation.team_limit_number; i < 3; i++) {
			GameObject gmo = Instantiate (Fork, new Vector3 (), Quaternion.identity);
			gmo.transform.parent = selected_folder;
			gmo.transform.localPosition =  new Vector3 (i * 4, 0, 0);
		}
	}

	public void clicked(int order){
		if (selecteds [order]) {
			if (!is_limit (characters [order].GetComponent<Select_character_manager_click_character> ().Name)) {
				Vector3 hole = characters [order].transform.localPosition;
				selecteds [order] = false;
				characters [order].transform.parent = owned_folder;
				characters [order].transform.localPosition = new Vector3 ((owned_length++) * 4, 0, 0);

				//往前推進
				foreach (Transform child in selected_folder) {
					if (child.localPosition.x > hole.x && child.name != "fork(Clone)") child.position -= new Vector3 (4, 0, 0);
				}
				selected_length--;
			}
		} else {

			//不能選超過人數上限
			if (selected_length < stage_window_controller.stage_imformation_manager.dataController.stageInformation.team_limit_number) {
				Vector3 hole = characters [order].transform.localPosition;
				selecteds [order] = true;
				characters [order].transform.parent = selected_folder;
				characters [order].transform.localPosition = new Vector3 ((selected_length++) * 4, 0, 0);

				//往前推進
				foreach (Transform child in owned_folder) {
					if (child.localPosition.x > hole.x) child.position -= new Vector3 (4, 0, 0);
				}
				owned_length--;
			}
		}
	}

	public int[] list(){
		int isSelectCount = 0;
		foreach(var selected in selecteds)
			if (selected)
				isSelectCount ++;
		int auto_fill_amount = stage_window_controller.stage_imformation_manager.dataController.stageInformation.team_limit_number - isSelectCount;
		int[] ret = new int[stage_window_controller.stage_imformation_manager.dataController.stageInformation.team_limit_number];
		int ink = 0;
		for(int i=0;i<characters.Length;i++){
			if (selecteds [i])
				ret [ink++] = characters [i].GetComponent<Select_character_manager_click_character> ().order;
			else if (auto_fill_amount > 0){
				auto_fill_amount -= 1;
				ret [ink++] = characters [i].GetComponent<Select_character_manager_click_character> ().order;
			}
		}
		return ret;
	}

	bool is_limit(string Name){
		int length = stage_window_controller.stage_imformation_manager.dataController.stageInformation.team_limit.Length;
		for (int i = 0; i < length; i++)
			if (stage_window_controller.stage_imformation_manager.dataController.stageInformation.team_limit [i] == Name)
				return true;
		
		return false;
	}
}

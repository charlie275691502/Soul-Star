using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMapController : MonoBehaviour {
	public int chapter_order;
	[HideInInspector] public int now_stage_window_chapter;
	[HideInInspector] public int now_stage_window_number;
	[HideInInspector] public bool has_window_prefab;
	public Sprite challenge_stage_sprite;
	public Sprite complete_stage_sprite;
	public Sprite lock_stage_sprite;
	public GameObject[] stage_point_gmo;

	public SceneController sceneController;
	[HideInInspector] public DataController dataController;

	void Start(){
		dataController = GameObject.Find ("DataController").GetComponent<DataController> ();
		for (int i = 0; i < dataController.data.chapter_mission_completeds[chapter_order-1].mission_completeds.Length; i++) {
			if (dataController.data.chapter_mission_completeds [chapter_order - 1].mission_completeds [i] == true) {
				stage_point_gmo [i].SetActive(true);
				stage_point_gmo [i].GetComponent<SpriteRenderer> ().sprite = complete_stage_sprite;
			}
			else {
				int front_stage1_chapter = stage_point_gmo [i].GetComponent<Change_now_stage_window_number> ().front_stage1_chapter;
				int front_stage1_number  = stage_point_gmo [i].GetComponent<Change_now_stage_window_number> ().front_stage1_number;
				int front_stage2_chapter = stage_point_gmo [i].GetComponent<Change_now_stage_window_number> ().front_stage2_chapter;
				int front_stage2_number  = stage_point_gmo [i].GetComponent<Change_now_stage_window_number> ().front_stage2_number;

				//完成前置
				if ((front_stage1_chapter == 0 || dataController.data.chapter_mission_completeds [front_stage1_chapter - 1].mission_completeds [front_stage1_number - 1]) &&
					(front_stage2_chapter == 0 || dataController.data.chapter_mission_completeds [front_stage2_chapter - 1].mission_completeds [front_stage2_number - 1])) {
					stage_point_gmo [i].SetActive(true);
					stage_point_gmo [i].GetComponent<SpriteRenderer> ().sprite = challenge_stage_sprite;
				} else {
					stage_point_gmo [i].SetActive(false);
					stage_point_gmo [i].GetComponent<SpriteRenderer> ().sprite = lock_stage_sprite;
				}
			}
		}
	}
}

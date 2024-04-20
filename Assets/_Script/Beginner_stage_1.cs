using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Tutorial{
	public GameObject gmo;
	public Vector2 position;
	public Vector2 scale;
	public string text;
	[HideInInspector] public bool next;
}

public class Beginner_stage_1 : MonoBehaviour {
	public GameController gameController;
	public Tutorial[] tutorials;
	[HideInInspector] public int now_tutorial_order;

	// Use this for initialization
	void Start () {
		start_counting = false;
		time = 0.0f;
		sec_time = 0.0f;
		third_time = 0.0f;
		forth_time = 0.0f;
		gameController = GetComponent<GameController> ();
		foreach(Tutorial t in tutorials)t.next = false;

		StartCoroutine (beginner_stage ());
	}

	int i;
	GameObject gmo;
	IEnumerator beginner_stage(){
		int length = tutorials.Length;

		for (i = 0; i < length; i++) {
			now_tutorial_order = i;
			gmo = Instantiate (tutorials [i].gmo, Vector3.zero, Quaternion.identity);
			gmo.transform.position = tutorials [i].position;
			gmo.transform.localScale = tutorials [i].scale;
			gmo.transform.Find ("word").Find ("desc").GetComponent<Text> ().text = tutorials [i].text;

			while (!tutorials[i].next) yield return null;
			Destroy (gmo);
		}
	}

	bool start_counting;
	float time;
	float sec_time;
	float third_time;
	float forth_time;

	void Update(){
		Vector3 pos = Input.mousePosition;
		pos = Camera.main.ScreenToWorldPoint(pos);

		if(Input.GetMouseButtonDown(0)){
			if((now_tutorial_order == 0 || now_tutorial_order == 9) && Mathf.Abs(pos.x - tutorials [now_tutorial_order].position.x) < 2.5f && Mathf.Abs(pos.y - tutorials [now_tutorial_order].position.y) < 2.5f){
				tutorials [now_tutorial_order].next = true;
			}
		} else if(Input.GetMouseButtonDown(1)){
			if(( now_tutorial_order == 3 || now_tutorial_order == 5 || now_tutorial_order == 7)&& Mathf.Abs(pos.x - tutorials [now_tutorial_order].position.x) < 2.5f && Mathf.Abs(pos.y - tutorials [now_tutorial_order].position.y) < 2.5f){
				tutorials [now_tutorial_order].next = true;
			}
		}

		if(now_tutorial_order == 1 && Mathf.Abs(pos.x - tutorials [now_tutorial_order].position.x) < 2.5f && Mathf.Abs(pos.y - tutorials [now_tutorial_order].position.y) < 2.5f){
			start_counting = true;
			if (time > 1.0f) {
				i++;
				now_tutorial_order++;
				gmo.transform.Find ("word").Find ("desc").GetComponent<Text> ().text = tutorials [i].text;
				GameObject tmp = Instantiate (tutorials [i].gmo, Vector3.zero, Quaternion.identity);
				tmp.transform.parent = gmo.transform;
			}
		}

		if (start_counting)
			time += Time.deltaTime;
		else
			time = 0.0f;
		start_counting = false;

		GameObject skill = gameController.the_skill_in_frame (gameController.allies_shines [0].transform.GetChild (0));
		if (now_tutorial_order == 2 && skill != null) {
			tutorials [now_tutorial_order].next = true;
		}

		skill = gameController.the_skill_in_frame (gameController.allies_shines [0].transform.GetChild (1));
		if (now_tutorial_order == 4 && skill != null) {
			tutorials [now_tutorial_order].next = true;
		}

		skill = gameController.the_skill_in_frame (gameController.allies_shines [0].transform.GetChild (2));
		if (now_tutorial_order == 6 && skill != null) {
			tutorials [now_tutorial_order].next = true;
		}

		if (now_tutorial_order == 8) {
			sec_time += Time.deltaTime;
			if (sec_time > 3.0f) {
				tutorials [now_tutorial_order].next = true;
			}
		}

		if (now_tutorial_order == 10) {
			third_time += Time.deltaTime;
			if (third_time > 1.0f && gameController.can_edit) {
				tutorials [now_tutorial_order].next = true;
			}
		}

		if (now_tutorial_order == 11) {
			forth_time += Time.deltaTime;
			if (forth_time > 3.0f) {
				tutorials [now_tutorial_order].next = true;
			}
		}
	}
}

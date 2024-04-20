using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Beginner_stage_2 : MonoBehaviour {
	public GameController gameController;
	public Tutorial[] tutorials;

	// Use this for initialization
	void Start () {
		gameController = GetComponent<GameController> ();

		StartCoroutine (beginner_stage ());
	}

	int now_tutorial_order;
	GameObject gmo;
	int i;
	float time;
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

	void Update(){
		Vector3 pos = Input.mousePosition;
		pos = Camera.main.ScreenToWorldPoint(pos);

		if(Input.GetMouseButtonDown(0)){
			if((now_tutorial_order == 0 || now_tutorial_order == 1 || now_tutorial_order == 2) && Mathf.Abs(pos.x - tutorials [now_tutorial_order].position.x) < 2.5f && Mathf.Abs(pos.y - tutorials [now_tutorial_order].position.y) < 2.5f){
				tutorials [now_tutorial_order].next = true;
				if (now_tutorial_order == 1) {
					i++;
					now_tutorial_order++;
					gmo.transform.Find ("word").Find ("desc").GetComponent<Text> ().text = tutorials [i].text;
				}
			}


		}

		GameObject skill = gameController.the_skill_in_frame (gameController.allies_shines [1].transform.GetChild (4));
		if (now_tutorial_order == 3 && skill != null) {
			tutorials [now_tutorial_order].next = true;
		}

		if (now_tutorial_order == 4) {
			time += Time.deltaTime;
			if (time > 3.0f) {
				tutorials [now_tutorial_order].next = true;
			}
		}
	}
}

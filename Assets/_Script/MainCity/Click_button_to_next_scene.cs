using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click_button_to_next_scene : MonoBehaviour {
	public SceneController sceneController;
	public string next_scene_name;
	public MonoBehaviour next_scene_confirm;

	void OnMouseDown(){
		if (next_scene_confirm != null) {
			if((bool)next_scene_confirm.GetType ().GetField ("has_window_prefab").GetValue (next_scene_confirm))return;
		}
		sceneController.next_scene (next_scene_name);
	}
}

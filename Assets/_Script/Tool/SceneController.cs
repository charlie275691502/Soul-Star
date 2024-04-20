using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour {
	public fading fading;
	public string next_scene_name;
	public MonoBehaviour next_scene_confirm;

	public void next_scene(string name){
		if (next_scene_confirm != null) {
			if((bool)next_scene_confirm.GetType ().GetField ("has_window_prefab").GetValue (next_scene_confirm))return;
		}
		StartCoroutine(fading.start_fading (name));
	}

	public void next_scene(){
		next_scene (next_scene_name);
	}
}

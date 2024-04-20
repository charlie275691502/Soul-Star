using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class close : MonoBehaviour {
	public MonoBehaviour monoBehaviour;

	void OnMouseDown(){
		close_window ();
	}

	public void close_window(){
		if (monoBehaviour != null && (bool)monoBehaviour.GetType ().GetField ("has_window_prefab").GetValue (monoBehaviour))return;
		Debug.Log ("close");
		transform.parent.gameObject.GetComponent<Animator> ().Play("out");
		transform.parent.gameObject.SendMessage ("change_parent_has_window_prefab", false);
	}
}

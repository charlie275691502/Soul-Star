using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_load_window_ok : MonoBehaviour {
	public int order;

	void OnMouseDown(){
		Debug.Log ("ok");
		transform.parent.gameObject.GetComponent<Start_load_window_DialogController> ().start_load_window_controller.delete_data(order);
	}
}

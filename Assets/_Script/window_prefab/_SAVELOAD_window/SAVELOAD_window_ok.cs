using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAVELOAD_window_ok : MonoBehaviour {
	public int order;

	void OnMouseDown(){
		Debug.Log ("ok");
		transform.parent.gameObject.GetComponent<SAVELOAD_window_DialogController> ().sAVELOAD_window_controller.delete_data(order);
	}
}

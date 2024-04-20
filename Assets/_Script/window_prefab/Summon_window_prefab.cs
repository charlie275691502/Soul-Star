using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon_window_prefab : MonoBehaviour {
	public MonoBehaviour monoBehaviour;
	public GameObject window_prefab;

	void OnMouseDown(){
		if((bool)monoBehaviour.GetType ().GetField ("has_window_prefab").GetValue (monoBehaviour))return;
		Instantiate (window_prefab, Vector3.zero, Quaternion.identity);
	}
}

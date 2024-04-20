using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_bag_character : MonoBehaviour {
	public int order;
	public Character_window_controller character_window_controller;

	void OnMouseDown(){
		GameObject.Find("character_window(Clone)").GetComponent<Character_window_controller>().switch_character (order);
	}
}

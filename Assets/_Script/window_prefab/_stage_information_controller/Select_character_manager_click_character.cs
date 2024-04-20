using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select_character_manager_click_character : MonoBehaviour {
	public Select_character_manager select_character_manager;
	public int order;
	public string Name;

	void Start(){
		select_character_manager = GameObject.Find ("select_character").GetComponent < Select_character_manager> ();
	}

	void OnMouseDown(){
		select_character_manager.clicked (order);
	}
}

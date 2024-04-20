using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class click_init_position : MonoBehaviour {
	public int order;
	private GameController gameController;

	void Start () {
		gameController = GameObject.Find ("GameController").GetComponent<GameController> ();
	}

	void OnMouseDown(){
		gameController.Set_init_position_order = order;
		gameController.Set_init_position_halt = false;
	}
}

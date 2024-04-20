using UnityEngine;
using System.Collections;

public class Click_start : MonoBehaviour {

	[HideInInspector] public GameController gameController;

	void Start(){
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
	}

	// Update is called once per frame
	void Update () {

	}

	void OnMouseDown(){
		if (!gameController.can_edit)return;
		StartCoroutine (gameController.Start_battle ());
	}
}

using UnityEngine;
using System.Collections;

public class click_speed : MonoBehaviour {

	public GameController gameController;
	public Sprite normal;
	public Sprite speed;

	void Start(){
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
	}

	// Update is called once per frame
	void Update () {

	}

	void OnMouseDown(){
		if (!gameController.can_edit)return;
		if (gameController.acts.is_speeding)GetComponent<SpriteRenderer> ().sprite = normal;
		else GetComponent<SpriteRenderer> ().sprite = speed;
		gameController.acts.is_speeding = !gameController.acts.is_speeding;
	}
}

using UnityEngine;
using System.Collections;

public class click_icon : MonoBehaviour {
	public int order;
	private GameController gameController;

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){
		if (!gameController.can_edit)return;
		if (gameController.acts.game_over)return;
		if (order >= gameController.allies_prefab.Length)return;
		gameController.details.change (gameController.allies[order]);
	}
}

using UnityEngine;
using System.Collections;

public class jump_damage : MonoBehaviour {
	public int damage;
	private Vector3 init_position;
	private GameController gameController;
	public GameObject left;
	public GameObject mid;
	public GameObject right;
	public float jump_height;
	public float jump_time;

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		init_position = transform.position;
		change_damage();
		StartCoroutine (jump ());
	}

	void change_damage(){

		if (damage >= 1000)
			return;
		left .SetActive (damage >= 100);
		mid.SetActive (damage >= 10);
		right.SetActive (damage >= 0 );
		if (damage < 0) {
			Debug.Log ("damage < 0");
			return;
		}
		left .GetComponent<SpriteRenderer>().sprite = gameController.acts.round_font[damage / 100];
		mid .GetComponent<SpriteRenderer>().sprite = gameController.acts.round_font[(damage % 100) / 10];
		right.GetComponent<SpriteRenderer>().sprite = gameController.acts.round_font[damage % 10];
	}

	IEnumerator jump(){
		for (float i = 0; i < jump_time; i += Time.deltaTime) {
			transform.position = init_position + new Vector3 (0.0f, jump_height * i / jump_time, 0.0f);
			yield return null;
		}
		Destroy (gameObject);
	}
}

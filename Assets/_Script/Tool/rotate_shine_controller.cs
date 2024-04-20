using UnityEngine;
using System.Collections;

public class rotate_shine_controller : MonoBehaviour {
	public bool can_rotate;
	public bool find_gameController;
	public float first_wait;
	public float fade_in_time;
	public Vector2 vec;
	public float rotate_speed;
	public float fade_in_out_time;
	public float fade_in_out_deep;
	public Sprite start_sprite;
	public Sprite fight_sprite;
	private Vector2 init_position;
	private GameController gameController;


	void Start(){
		if(find_gameController)gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		init_position = transform.position;
		StartCoroutine (yee ());
	}

	void Update () {
		if (!can_rotate)return;
		transform.rotation *= Quaternion.Euler (0, 0, rotate_speed);
	}

	IEnumerator yee(){
		yield return new WaitForSeconds (first_wait);
		for (float i = 0; i < fade_in_time; i += Time.deltaTime) {
			transform.position = init_position + new Vector2 (vec.x * i / fade_in_time, vec.y * i / fade_in_time);
			GetComponent<SpriteRenderer>().color = new Color (1.0f, 1.0f, 1.0f,i / fade_in_time);
			yield return null;
		}
		transform.position = init_position + new Vector2 (vec.x, vec.y);
		GetComponent<SpriteRenderer>().color = new Color (1.0f, 1.0f, 1.0f,1.0f);
		StartCoroutine (fade_in_out ());
	}

	IEnumerator fade_in_out(){
		if (fade_in_out_time > 0) {
			while (true) {
				for (float i = 0; i < fade_in_out_time; i += Time.deltaTime) {
					if (gameController != null && !gameController.can_edit) GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
					else GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, fade_in_out_deep + Mathf.Sin( i / fade_in_out_time * Mathf.PI / 2) * ( 1 - fade_in_out_deep));
					yield return null;
				}
				for (float i = fade_in_out_time; i >= 0 ; i -= Time.deltaTime) {
					if (gameController != null && !gameController.can_edit) GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
					else GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, fade_in_out_deep + Mathf.Sin( i / fade_in_out_time * Mathf.PI / 2) * ( 1 - fade_in_out_deep));
					yield return null;
				}
			}
		}
	}
}

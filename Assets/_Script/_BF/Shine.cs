using UnityEngine;
using System.Collections;

public class Shine : MonoBehaviour {
	public GameObject[] frames;
	public bool[] full;
	public Vector2 range;
	public float speed;
	private float time;
	public GameController gameController;

	void Start(){
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		full = new bool[frames.Length];
		for (int i = 0; i < frames.Length; i++)full [i] = false;
	}

	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		float size = (gameController.can_edit) ? range.x + (range.y - range.x) * Mathf.Abs( Mathf.Sin (time * speed)) : range.x;
		for (int i = 0; i < frames.Length; i++) if(frames[i] != null)frames [i].transform.localScale = new Vector2 (size, size);
	}
}

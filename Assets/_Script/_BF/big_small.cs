using UnityEngine;
using System.Collections;

public class big_small : MonoBehaviour {

	public Vector2 range;
	public float speed;
	private float time;

	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		float size = range.x + (range.y - range.x) * Mathf.Sin(time * speed);
		transform.localScale = new Vector3 (size, size, 0.0f);
	}
}

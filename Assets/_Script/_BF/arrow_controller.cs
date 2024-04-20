using UnityEngine;
using System.Collections;

public class arrow_controller : MonoBehaviour {
	public bool back;
	public float rotate_speed;
	public float W;
	public float H;
	public float th;
	public float shot_time;
	public float stay_time;
	private Vector3 init_pos;
	public Sprite change_sprite;

	// Use this for initialization
	void Start () {
		init_pos = transform.localPosition;
		StartCoroutine (back ? arrow_back () : arrow_throw ());
	}
	
	// Update is called once per frame
	void Update () {
	}

	IEnumerator arrow_throw(){
		for (float i = 0.0f; ; i+=Time.deltaTime) {
			if(i>shot_time)i = shot_time;
			float x = W * i / shot_time;
			float a = (H - Mathf.Tan (th / 180 * Mathf.PI) * W) / (3 * W * W);
			float b = (2 * H + Mathf.Tan (th / 180 * Mathf.PI) * W) / (3 * W);
			float y = a * x * x + b * x;
			float angle = Mathf.Atan (2 * a * x + b) * 180 / Mathf.PI;
			transform.localPosition = init_pos + new Vector3 (x, y, 0.0f);
			transform.localRotation = Quaternion.Euler(0, 0, angle); 
			if(i == shot_time)break;
			yield return null;	
		}

		for (float i = stay_time; i >= 0; i -= Time.deltaTime) {
			GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, i / stay_time);
			yield return null;	

		}
		Destroy (gameObject);
	}

	IEnumerator arrow_back(){
		for (float i = 0.0f; i < shot_time ; i += Time.deltaTime) {
			if(i>shot_time)i = shot_time;
			transform.localPosition = init_pos + new Vector3 (W * i / shot_time, H * i / shot_time, 0.0f);
			transform.rotation *= Quaternion.Euler (0, 0, rotate_speed * Time.deltaTime);
			if(i == shot_time)break;
			if(i > shot_time / 2)GetComponent<SpriteRenderer>().sprite = change_sprite;
			yield return null;
		}
		for (float i = stay_time; i >= 0; i -= Time.deltaTime) {
			GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, i / stay_time);
			yield return null;	

		}
		Destroy (gameObject);
	}
}

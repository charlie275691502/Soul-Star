using UnityEngine;
using System.Collections;

public class avi : MonoBehaviour {
	private SpriteRenderer avi_sr;
	public float avi_play_time;
	public Sprite[] sprites;

	public bool is_fadeout;
	public float avi_fade_out_time;

	public bool destroy_after_playing;

	// Use this for initialization
	void Start () {
		avi_sr = GetComponent<SpriteRenderer> ();
		StartCoroutine(play_avi ());
	}	

	IEnumerator play_avi(){
		for (float i = 0; i < avi_play_time; i += Time.deltaTime) {
			avi_sr.sprite = sprites [(int)((i / avi_play_time) * (float)sprites.Length)];
			yield return null;
		}
		avi_sr.sprite = sprites [sprites.Length - 1];
		if (is_fadeout) {
			for (float i = 0; i < avi_fade_out_time; i += Time.deltaTime) {
				avi_sr.color = new Color (avi_sr.color.r / 256.0f, avi_sr.color.g / 256.0f, avi_sr.color.b / 256.0f, 1 - i);
				yield return null;
			}
			avi_sr.color = new Color (avi_sr.color.r / 256.0f, avi_sr.color.g / 256.0f, avi_sr.color.b / 256.0f, 0.0f);
		}
		yield return new WaitForSeconds (avi_play_time + avi_fade_out_time);
		if(destroy_after_playing)Destroy (gameObject);
	}
}

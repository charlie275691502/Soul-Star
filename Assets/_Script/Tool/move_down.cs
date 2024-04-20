using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class move_down : MonoBehaviour {
	public Vector2 range;
	public float time;

	// Use this for initialization
	void Start () {
		StartCoroutine (move_d ());
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Alpha0)) SceneManager.LoadScene (0);
		if (Input.GetKeyDown (KeyCode.Alpha1)) SceneManager.LoadScene (1);
		if (Input.GetKeyDown (KeyCode.Alpha2)) SceneManager.LoadScene (2);
		if (Input.GetKeyDown (KeyCode.Alpha3)) SceneManager.LoadScene (3);
		if (Input.GetKeyDown (KeyCode.Alpha4)) SceneManager.LoadScene (4);
		if (Input.GetKeyDown (KeyCode.Alpha5)) SceneManager.LoadScene (5);
		if (Input.GetKeyDown (KeyCode.Alpha6)) SceneManager.LoadScene (6);
		if (Input.GetKeyDown (KeyCode.Alpha7)) SceneManager.LoadScene (7);
		if (Input.GetKeyDown (KeyCode.Alpha8)) SceneManager.LoadScene (8);
		if (Input.GetKeyDown (KeyCode.Alpha9)) SceneManager.LoadScene (9);
	}

	IEnumerator move_d(){
		yield return new  WaitForSeconds (2);
		for (float i = 0; i < time; i += Time.deltaTime) {
			transform.position = new Vector3 (0, range.x + (range.y - range.x) * i / time, 0);
			yield return null;
		}
	}
}

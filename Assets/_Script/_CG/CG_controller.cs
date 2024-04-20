using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CG_controller : MonoBehaviour {
	public SceneController sceneController;

	void Update () {
		if (Input.GetKeyDown (KeyCode.H)) SceneManager.LoadScene (1);
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
		if (Input.GetMouseButtonDown (0))sceneController.next_scene ();
	}

}

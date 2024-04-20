using UnityEngine;
using System.Collections;

public class ChangeOrthographicSize : MonoBehaviour {

	public float baseWidth = 1920;
	public float baseHeight = 1080;
	public float baseOrthographicSize = 18;
	[HideInInspector] public float finalOrthographicSize;

	void Awake(){
		float newOrthographicSize = (float)Screen.height / (float)Screen.width * baseWidth / baseHeight * baseOrthographicSize;
		finalOrthographicSize = Mathf.Max (newOrthographicSize, baseOrthographicSize);
		GetComponent<Camera>().orthographicSize = finalOrthographicSize;
//		Debug.Log(Mathf.Max(newOrthographicSize , baseOrthographicSize));
	}
}

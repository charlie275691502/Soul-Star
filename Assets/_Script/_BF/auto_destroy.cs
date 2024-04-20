using UnityEngine;
using System.Collections;

public class auto_destroy : MonoBehaviour {
	public float life_time;
	private float time;

	// Use this for initialization
	void Start () {
		time = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time >= life_time)Destroy (gameObject);
	}
}

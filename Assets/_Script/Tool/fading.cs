using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GUI_Gear{
	public Vector2 gear_positions;
	public Vector2 gear_scales;
	public float gear_rotate_speeds;
}

public class fading : MonoBehaviour {
	[HideInInspector] public bool fading_in;
	[HideInInspector] public bool fading_out;
	public bool will_fade_in;
	public bool will_fade_out;
	public bool fade_in_gear;
	public bool fade_out_gear;
	public Texture2D fade_in_down_texture;
	public Texture2D fade_in_up_texture;
	public Texture2D fade_out_down_texture;
	public Texture2D fade_out_up_texture;
	[HideInInspector] public bool next_scene;

	public Texture2D gear;
	public GUI_Gear[] gui_gear;
	public float scale;

	private Vector2 screen;
	public float fadeSpeed;

	public float max_alpha;
	[HideInInspector] public float alpha;

	public float gear_time;
	private float time = 0.0f;

	void Awake(){
		next_scene = false;
		fading_in = will_fade_in;
		alpha = (fading_in) ? 1.0f : 0.0f;
	}

	void OnGUI(){
		screen = new Vector2 (Screen.width, Screen.height);
		time += Time.deltaTime;
		GUI.depth = -1000;
		if(fading_in){
			if (time < gear_time) {
				GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, 1.0f);
				GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), fade_in_down_texture);

				GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, max_alpha);
				GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), fade_in_up_texture);

				if(fade_in_gear)draw_gear ();
				return;
			}
			alpha -= fadeSpeed * Time.deltaTime;
			alpha = Mathf.Clamp01 (alpha); 

			GUI.depth = -1000;

			GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
			GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), fade_in_down_texture);

			GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha * max_alpha);
			GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), fade_in_up_texture);

			GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha * max_alpha);
			if(fade_in_gear)draw_gear ();
			if (alpha == 0.0f) fading_in = false;
		} else if(fading_out){
			alpha += fadeSpeed * Time.deltaTime;
			alpha = Mathf.Clamp01 (alpha);

			GUI.depth = -1000;

			GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
			GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), fade_out_down_texture);

			GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha * max_alpha);
			GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), fade_out_up_texture);

			GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha * max_alpha);
			if(fade_out_gear)draw_gear ();
			if (alpha == 1.0f) next_scene = true;
		}
	}

	void draw_gear(){
		for (int i = 0; i < gui_gear.Length; i++) {
			Vector2 position = 0.5f * screen + gui_gear [i].gear_positions  * Screen.height * scale;
			GUIUtility.RotateAroundPivot(time / gear_time * 360 * gui_gear[i].gear_rotate_speeds, position);
			GUI.DrawTexture (new Rect(position - 0.5f * gui_gear [i].gear_scales * Screen.height * scale, gui_gear [i].gear_scales * Screen.height * scale), gear);
			GUIUtility.RotateAroundPivot(-time / gear_time * 360 * gui_gear[i].gear_rotate_speeds, position);
		}
	}
		
	public IEnumerator start_fading(string scene_name){
		fading_out = true;
		while (!next_scene)yield return null;
		SceneManager.LoadScene (scene_name);
	}
}
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class number_setting{
	public const int max_story_commands_amount = 200;
}

public enum Character_Command_Type{
	Add,
	Change_image,
	Talk,
	Leave,
	Move,
	Aside,
	Shake
}

[System.Serializable]
public class Story_Character{
	public string name;
	public GameObject character;
}

[System.Serializable]
public class Character_Command{
	public Character_Command_Type type;

	//Add
	public string name;
	public bool in_dark;
//	public Sprite sprite;
	public Vector3 pos;

	//Change_image
//	public string name;
	public Sprite sprite;
	public bool is_shaking;

	//Talk
//	public string name;
	public string text;

	//Leave
//	public string name;

	//Aside
//	public string name;
	public Vector2 dest;
	public float time;

	public void copy_value(Character_Command ano){
		type = ano.type;
		name = ano.name;
		pos = ano.pos;
		sprite = ano.sprite;
		is_shaking = ano.is_shaking;
		text = ano.text;
	}
}

public class StoryController : MonoBehaviour {
	[HideInInspector] public int size;
	[HideInInspector] public Character_Command[] character_temp_Commands = new Character_Command[number_setting.max_story_commands_amount];
	[HideInInspector] public Character_Command[] character_Commands;
	[HideInInspector] public Story_Character[] characters;
	[HideInInspector] public Dictionary<string, GameObject> character_dic = new Dictionary<string, GameObject>();
	public Camera camera;
	public Text name_text;
	public Text talk_text;
	public float talking_rgb;
	public float talking_alpha;
	public float adding_animation_time;
	public float talking_animation_speed;
	public float leaving_animation_time;
	public int   shaking_animation_amount;
	public float shaking_animation_time;
	public float shaking_animation_speed;
	public SceneController sceneController;
	public GameObject character_gmo;
	public TextAsset storyController_textasset;

	private bool _continue;	

	void Start(){
		character_Commands = new Character_Command[size];
		for (int i = 0; i < size; i++)character_Commands [i] = character_temp_Commands [i];

		if (storyController_textasset != null) {
			string[] commands = storyController_textasset.ToString ().Split ('#');
			character_Commands = new Character_Command[commands.Length];
			for(int i=0;i<commands.Length;i++) {
				Debug.Log (commands [i]);
				string[] detail = commands[i].Split ('\n');
				character_Commands[i] = new Character_Command();
				character_Commands [i].type = (Character_Command_Type)System.Enum.Parse (typeof(Character_Command_Type), detail [1]);
				string[] temp;
				switch (detail [1]) {
				case "Add":
					character_Commands [i].name = detail [2];
					character_Commands[i].in_dark = (detail [3] == "y");
					character_Commands [i].sprite = (Sprite)Resources.Load ("Image/Storys/" + detail [4], typeof(Sprite));
					temp = detail [5].Split (' ');
					character_Commands [i].pos = new Vector3 (float.Parse(temp[0]), float.Parse(temp[1]));
					break;
				case "Change_image":
					character_Commands[i].name = detail [2];
					character_Commands[i].sprite = (Sprite)Resources.Load ("Image/Storys/" + detail [3], typeof(Sprite));
					character_Commands[i].is_shaking = (detail [4] == "y");
					break;
				case "Talk":
					character_Commands[i].name = detail [2];
					character_Commands[i].text = detail [3];
					break;
				case "Leave":
					character_Commands[i].name = detail [2];
					break;
				case "Move":
					character_Commands[i].name = detail [2];
					temp = detail [3].Split (' ');
					character_Commands [i].dest = new Vector3 (float.Parse(temp[0]), float.Parse(temp[1]));
					character_Commands[i].time = float.Parse(detail [4]);
					break;
				case "Shake":
					break;
				case "Aside":
					character_Commands [i].text = detail [2];
					break;
				}
			}
		}

		Debug.Log ("done");
		StartCoroutine (Story_line());
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.H)) SceneManager.LoadScene (1);
		if (Input.GetKeyDown (KeyCode.R))SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
		if (Input.GetKeyDown (KeyCode.P))Time.timeScale = 1 - Time.timeScale;
		if (Input.GetKeyDown (KeyCode.C) || Input.GetKeyDown (KeyCode.Space) || Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButtonDown(0))_continue = true;
	}

	IEnumerator Story_line(){
		while (gameObject.GetComponent<fading> ().fading_in)yield return null;
		yield return new WaitForSeconds(1);
		for (int i = 0; i < character_Commands.Length; i++) {
			switch (character_Commands [i].type) {
			case Character_Command_Type.Add:
				
				GameObject gmo = (GameObject)Instantiate (character_gmo, character_Commands [i].pos, Quaternion.identity);
				gmo.GetComponent<SpriteRenderer> ().sprite = character_Commands [i].sprite;

				silence_all ();
				gmo.GetComponent<SpriteRenderer> ().sortingOrder = 1;

				if(character_Commands [i].in_dark){
					gmo.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, talking_alpha);
				} else {
					for (float j = 0; j < adding_animation_time; j += Time.deltaTime) {
						gmo.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, j / adding_animation_time);
						yield return null;
					}
					gmo.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				}

				character_dic.Add (character_Commands[i].name, gmo);
				break;
			case Character_Command_Type.Change_image:
				if (character_Commands [i].is_shaking)StartCoroutine(camera_shake ());
				character_dic [character_Commands [i].name].GetComponent<SpriteRenderer> ().sprite = character_Commands [i].sprite;
				
				break;
			case Character_Command_Type.Talk:
				silence_all ();

				if (character_dic.ContainsKey (character_Commands [i].name)) {
					character_dic [character_Commands [i].name].GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
					character_dic [character_Commands [i].name].GetComponent<SpriteRenderer> ().sortingOrder = 1;
				}

				name_text.text = character_Commands [i].name;
				float animation_time = character_Commands [i].text.Length / talking_animation_speed;
				for (float j = 0; j < animation_time; j += Time.deltaTime) {
					if(_continue)break;
					talk_text.text = character_Commands [i].text.Substring (0, Mathf.Min (character_Commands [i].text.Length - 1, (int)(j / animation_time * character_Commands [i].text.Length)));
					yield return null;
				}
				talk_text.text = character_Commands [i].text;
				_continue = false;
				while (!_continue)yield return null;
				_continue = false;
				break;
			case Character_Command_Type.Leave:
				gmo = character_dic [character_Commands [i].name];

				for (float j = adding_animation_time; j >= 0; j -= Time.deltaTime) {
					gmo.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, j / adding_animation_time);
					yield return null;
				}
				gmo.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
				character_dic.Remove (character_Commands [i].name);
				break;
			case Character_Command_Type.Move:
				silence_all ();

				character_dic [character_Commands [i].name].GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				character_dic [character_Commands [i].name].GetComponent<SpriteRenderer> ().sortingOrder = 1;

				gmo = character_dic [character_Commands [i].name];
				Vector3 _from = gmo.transform.position;
				for (float j = 0; j < character_Commands [i].time; j += Time.deltaTime) {
					gmo.transform.position = _from + ((Vector3)character_Commands [i].dest - _from) * j / character_Commands [i].time;
					yield return null;
				}
				gmo.transform.position = character_Commands [i].dest;

				break;
			case Character_Command_Type.Shake:
				StartCoroutine(camera_shake ());
				break;
			case Character_Command_Type.Aside:
				name_text.text = "";
				animation_time = character_Commands [i].text.Length / talking_animation_speed;
				for (float j = 0; j < animation_time; j += Time.deltaTime) {
					if(_continue)break;
					talk_text.text = character_Commands [i].text.Substring (0, Mathf.Min (character_Commands [i].text.Length - 1, (int)(j / animation_time * character_Commands [i].text.Length)));
					yield return null;
				}
				talk_text.text = character_Commands [i].text;
				_continue = false;
				while (!_continue)yield return null;
				_continue = false;
				break;
			}
		}
		yield return null;

		sceneController.next_scene ();
	}

	float calculate_moving_animation_time(Vector3 _from, Vector3 _to, float speed){
		float dis = Mathf.Pow (Mathf.Pow (_from.x - _to.x, 2.0f) + Mathf.Pow (_from.y - _to.y, 2.0f), 0.5f);
		return dis / speed;
	}

	void silence_all(){
		foreach (GameObject gameObject in character_dic.Values) {
			gameObject.GetComponent<SpriteRenderer> ().color = new Color (talking_rgb, talking_rgb, talking_rgb, talking_alpha);
			gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 0;
		}
	}

	IEnumerator camera_shake(){
//		for (int i = 0; i < 2; i++) {
//			for (float j = 0; j < shaking_animation_time; j += Time.deltaTime) {
//				camera.GetComponent<Camera> ().orthographicSize -= shaking_animation_speed * Time.deltaTime;
//				yield return null;
//			}
//			for (float j = 0; j < shaking_animation_time; j += Time.deltaTime) {
//				camera.GetComponent<Camera> ().orthographicSize += shaking_animation_speed * Time.deltaTime;
//				yield return null;
//			}
//			camera.GetComponent<Camera> ().orthographicSize = camera.GetComponent<ChangeOrthographicSize> ().finalOrthographicSize;
//		}

		for (int i = 0; i < shaking_animation_amount; i++) {
			for (float j = 0; j < shaking_animation_time; j += Time.deltaTime) {
				camera.transform.position = new Vector3(camera.transform.position.x + shaking_animation_speed * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
				yield return null;
			}
			for (float j = 0; j < shaking_animation_time * 2; j += Time.deltaTime) {
				camera.transform.position = new Vector3(camera.transform.position.x - shaking_animation_speed * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
				yield return null;
			}
			for (float j = 0; j < shaking_animation_time; j += Time.deltaTime) {
				camera.transform.position = new Vector3(camera.transform.position.x + shaking_animation_speed * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
				yield return null;
			}
			camera.transform.position = new Vector3(0.0f, camera.transform.position.y, camera.transform.position.z);
		}
	}
}
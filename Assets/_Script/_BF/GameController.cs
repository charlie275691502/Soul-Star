using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public enum Victory_condition{
	DefeatAllEnemy,
	SurviveFor10Turns
}

public enum Defeat_condition{
	AnyAllyDied
}

[System.Serializable]
public class Editor_setting{
	public bool enable_ai_debug_mode;
	public bool enable_ai;
	public bool enable_cheat_mode;
	public bool print_ai_decision;
}

public enum Camp{
	allies,
	enemys
}

[System.Serializable]
public class Unit{
	public GameObject click_character;
	public Vector2 init_position;
}

[System.Serializable]
public class Second_Unit{
	public Unit unit;
	public Vector2[] possible_init_positions;
}

[System.Serializable]
public class Skill_Box_details{
	public Transform skills_folder;
	public GameObject easy_mover;
	public Vector2 easy_mover_from;
	public Vector2 easy_mover_to;
	public float easy_mover_time;
	public GameObject background;
	public Sprite[] background_images;
	public GameObject[] switchs;
	public Sprite[] focus;
	public Sprite[] unfocus;
}

[System.Serializable]
public class Details{
	public Character now_character;
	public GameObject details_box;
	public SpriteRenderer character_job_icon;
	public Text character_name;
	public Text hp_text;
	public Text mp_text;
	public Text ep_text;
	public Text max_hp_text;
	public Text max_mp_text;
	public Text max_ep_text;
	public Image detail_hp_bar;
	public Image detail_mp_bar;
	public Image detail_ep_bar;
	public SpriteRenderer skillsbox_image;
	public SpriteRenderer head_icon;
	public GameController gameController;

	public void change(Character character){
		//information
		now_character = character;
		character_job_icon.sprite = character.job_icon;
		character_name.text = character.Name;
		hp_text.text = character.hp.ToString();
		mp_text.text = character.mp.ToString();
		ep_text.text = character.ep.ToString();
		max_hp_text.text = character.max_hp.ToString();
		max_mp_text.text = character.max_mp.ToString();
		max_ep_text.text = character.max_ep.ToString();
		detail_hp_bar.fillAmount = ((float)character.hp / (float)character.max_hp);
		detail_mp_bar.fillAmount = ((float)character.mp / (float)character.max_mp);
		detail_ep_bar.fillAmount = ((float)character.ep / (float)character.max_ep);
		head_icon.sprite = character.head;
		skillsbox_image.sprite = character.skillsbox_image;

		//skill_box
		for (int i = 0; i < gameController.allies.Length; i++) {
			if (gameController.allies [i] == null)continue;
			for (int j = 0; j < gameController.allies [i].skills.Length; j++) {
				if (gameController.allies [i].skills [j] == null)continue;
				gameController.allies [i].skills [j].SetActive (character.character_camp == gameController.allies [i].character_camp && character.order == gameController.allies [i].order);
				if (gameController.allies [i].skills [j].GetComponent<Click_skill> ().is_in_frame (gameController.allies [i].skills [j].GetComponent<Click_skill> ().transform.position))gameController.allies [i].skills [j].SetActive (true);
			}
		}
		for (int i = 0; i < gameController.enemys.Length; i++) {
			if (gameController.enemys [i] == null)continue;
			for (int j = 0; j < gameController.enemys [i].skills.Length; j++) {
				if (gameController.enemys [i].skills [j] == null)continue;
				gameController.enemys [i].skills [j].SetActive (character.character_camp == gameController.enemys [i].character_camp && character.order == gameController.enemys [i].order);
				if (gameController.enemys [i].skills [j].GetComponent<Click_skill> ().is_in_frame (gameController.enemys [i].skills [j].GetComponent<Click_skill> ().transform.position))gameController.enemys [i].skills [j].SetActive (true);
			}
		}

		//tactic
		for (int i = 0; i < gameController.allies.Length; i++)gameController.allies_big_frames [i].GetComponent<SpriteRenderer> ().sprite = gameController.acts.normal_big_frame;
		if (character.character_camp == Character_camp.enemys)return;
		gameController.allies_big_frames [character.order].GetComponent<SpriteRenderer> ().sprite = gameController.acts.focus_big_frame;
	}
}

[System.Serializable]
public class Acts{
	public float skill_right_click_cd;
	public float double_click_delay;
	public float mouse_hover_delay;
	public float move_delay;
	public float fade_in_delay;
	public float start_delay;
	public GameObject start_gmo;
	public float show_delay;
	public float die_delay;
	public GameObject die_light;
	[HideInInspector] public bool someone_die = false;
	[HideInInspector] public bool game_over = false;
	public GameObject black;
	public float black_time;
	public GameObject victory;
	public GameObject settlement_window;
	public GameObject defeat;
	public GameObject defeated_window;
	public float victory_delay;
	public GameObject attack_view;
	public float board_blocks_distance;
	public Vector2 board_blocks_center;
	public float skill_blocks_distance;
	public Vector2 skill_blocks_center;
	[HideInInspector] public int round = 0;
	public GameObject round_number_0;
	public GameObject round_number_1;
	public Sprite[] round_font;
	public Vector2 hp_rate;
	public GameObject jump_damage;
	public float back_time;
	public float bounce_time;
	public int prioiriy_low_up_bound;
	public int prioiriy_mid_up_bound;
	public Sprite normal_big_frame;
	public Sprite focus_big_frame;
	[HideInInspector] public bool is_speeding;
	public GameObject skill_box_gear1;
	public GameObject skill_box_gear2;
	public float exp_fill_up_time;

	public Vector2 Board_Blocks_Vector_to_Coordinate(Vector2 position){
		return new Vector2 (position.x - 3, position.y - 2) * board_blocks_distance + board_blocks_center;
	}

	public Vector2 Skill_Blocks_Vector_to_Coordinate(Vector2 position){
		return new Vector2 ((position.x * 2 - 3) / 2, -position.y + 1) * skill_blocks_distance + skill_blocks_center;
	}
}

public class GameController : MonoBehaviour {

	public Unit[] allies_prefab;
	public Unit[] enemys_prefab;
	public GameObject[] allies_head;
	public GameObject[] enemys_head;
	[HideInInspector] public bool has_second_enemy;
	public Second_Unit[] second_enemys_prefab;
	public Vector2[] obstacles;
	public Vector2[] init_positions;
	public GameObject init_positions_gmo;
	public GameObject obstacle_gmo;
	[HideInInspector] public Character[] allies;
	[HideInInspector] public Character[] enemys;
	[HideInInspector] public int skill_box_focus;
	public GameObject[] allies_big_frames;
	public GameObject[] allies_shines;
	public GameObject[] enemys_big_frames;
	public GameObject[] enemys_shines;
	public Transform character_folder;

	public Transform used_skill;

	public SceneController sceneController;
	[HideInInspector] public bool can_edit;

	public Skill_Box_details skill_Box_Details;
	public Acts acts;
	public Editor_setting editor_setting;

	public Details details;
	public DataController dataController;

	// Use this for initialization
	bool start_update;
	void Start () {
		start_update = false;
		// load character's data
		dataController = (GameObject.Find("DataController") == null) ? null : GameObject.Find("DataController").GetComponent<DataController>();
		if (dataController != null && GetComponent<Beginner_stage_1>() == null && GetComponent<Beginner_stage_2>() == null ) {
			Load_allies_prefab ();
			Load_enemys_prefab ();
		}
		can_edit = true;
		skill_box_focus = 0;
		init_character ();
		init_skill_box ();
		init_obstacle ();
		acts.skill_box_gear2.GetComponent<rotate_shine_controller> ().can_rotate = false;
		acts.skill_box_gear1.GetComponent<rotate_shine_controller> ().can_rotate = false;
		StartCoroutine (Set_init_position ());
		window_up = false;
		start_update = true;
	}

	bool window_up;
	void Update () {
		if (!start_update)
			return;
		if (acts.game_over)foreach (GameObject gmo in GameObject.FindGameObjectsWithTag("Tactic_square")) Destroy (gmo);
		if (Input.GetKeyDown (KeyCode.C) || Input.GetMouseButtonDown (0)) {
			if (acts.game_over && !window_up) {
				if (GetComponent<Beginner_stage_1>() != null || GetComponent<Beginner_stage_2>() != null) {
					sceneController.next_scene ();
				} else if (acts.victory.activeSelf || acts.defeat.activeSelf) {
					if(acts.victory.activeSelf)Instantiate(acts.settlement_window, new Vector3(), Quaternion.identity);
					else Instantiate(acts.defeated_window, new Vector3(), Quaternion.identity);
					window_up = true;
				}
			}
		}

		if (editor_setting.enable_cheat_mode && allies.Length > 0) {
			Vector3 move = new Vector3 ();
			if (Input.GetKeyDown (KeyCode.B)) {
				foreach (Buff buff in allies[0].buffs.buff) {
					Debug.Log (buff.buff_type + " " + buff.para1 + " " + buff.turn);
				}
			}
			if (Input.GetKeyDown (KeyCode.H)) SceneManager.LoadScene (1);
			if (Input.GetKeyDown (KeyCode.W)) move = new Vector3 (0, 1, 0);
			if (Input.GetKeyDown (KeyCode.A)) move = new Vector3 (-1, 0, 0);
			if (Input.GetKeyDown (KeyCode.S)) move = new Vector3 (0, -1, 0);
			if (Input.GetKeyDown (KeyCode.D)) move = new Vector3 (1, 0, 0);
			if (Input.GetKeyDown (KeyCode.UpArrow)) move = new Vector3 (0, 1, 1);
			if (Input.GetKeyDown (KeyCode.LeftArrow)) move = new Vector3 (-1, 0, 1);
			if (Input.GetKeyDown (KeyCode.DownArrow)) move = new Vector3 (0, -1, 1);
			if (Input.GetKeyDown (KeyCode.RightArrow)) move = new Vector3 (1, 0, 1);
			if (move.z == 0 && allies [0] != null) allies [0].loc += new Vector3 (move.x, move.y, 0);
			else if (move.z != 0 && enemys [0] != null)enemys [0].loc += new Vector3(move.x, move.y, 0);
			if (move.z == 0 && allies[0] != null) allies [0].gmo.transform.position += new Vector3(move.x, move.y, 0) * acts.board_blocks_distance;
			else if (move.z != 0 && enemys [0] != null)enemys [0].gmo.transform.position += new Vector3(move.x, move.y, 0) * acts.board_blocks_distance;

			if (Input.GetKeyDown (KeyCode.R)) SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
			if (Input.GetKeyDown (KeyCode.P)) Time.timeScale = 1 - Time.timeScale;
			if (Input.GetKeyDown (KeyCode.K)) for (int i = 0; i < enemys.Length; i++) StartCoroutine(enemys [i].add_hp (-2147483648));
		}

	}

	void Load_allies_prefab(){
		int length = dataController.stage_loading_data.allies_order.Length;
		allies_prefab = new Unit[length];
		for (int i = 0; i < length; i++) {
			CharacterData characterData = dataController.data.characterData [dataController.stage_loading_data.allies_order [i]];
			int order = characterData.id;
			allies_prefab [i] = new Unit ();

			string order_string = ((order > 99) ? "" : (order > 9) ? "0" : "00") + order.ToString ();
			allies_prefab [i].click_character = (GameObject)Resources.Load ("Prefab/Allies/Allies_" + order_string + "/Allies_" + order_string, typeof(GameObject));
			Character character = allies_prefab [i].click_character.GetComponent<Click_Character> ().character;
			character.index = order;
			character.lv = characterData.lv;
			character.max_hp = characterData.hp;
			character.max_mp = characterData.mp;
			character.max_ep = characterData.ep;
			character.psy_atk = characterData.psy_atk;
			character.psy_def = characterData.psy_def;
			character.mag_atk = characterData.mag_atk;
			character.mag_def = characterData.mag_def;
			character.TEC = characterData.TEC;
			character.INT = characterData.INT;
			character.AGI = characterData.AGI;

			//skill set
			int ink = 0;
			for (int j = 0; j < characterData.skill_set.Length; j++)
				ink += characterData.skill_set [j];
			character.skills_prefab = new GameObject[ink];

			ink = 0;
			for (int j = 0; j < characterData.skill_set.Length; j++)
				for (int k = 0; k < characterData.skill_set [j]; k++)
					character.skills_prefab [ink++] = character.init_skills [j];

			//equip set

			foreach (int equip_id in characterData.equip_set) {
				if (equip_id == 0) continue;
				string equip_id_order_string = ((equip_id > 99) ? "" : (equip_id > 9) ? "0" : "00") + equip_id.ToString ();
				Equip equip = ((GameObject)Resources.Load ("Prefab/Equips/Equips_" + equip_id_order_string, typeof(GameObject))).GetComponent<Click_equip>().equip;
				character.max_hp += equip.hp;
				character.max_mp += equip.mp;
				character.max_ep += equip.ep;
				character.psy_atk += equip.psy_atk;
				character.psy_def += equip.psy_def;
				character.mag_atk += equip.mag_atk;
				character.mag_def += equip.mag_def;
				character.TEC += equip.TEC;
				character.INT += equip.INT;
				character.AGI += equip.AGI;
			}
		}
	}

	void Load_enemys_prefab(){
		Vector3[] enemys_config = dataController.stageInformation.enemys_config;
		int length = enemys_config.Length;
		int init_positions_length = 0;
		int obstacles_length = 0;
		int enemys_prefab_length = 0;
		for (int i = 0; i < length; i++) {
			if (enemys_config [i].z == -2)
				init_positions_length++;
			else if (enemys_config [i].z == -1)
				obstacles_length++;
			else 
				enemys_prefab_length++;
		}

		init_positions = new Vector2[init_positions_length];
		obstacles = new Vector2[obstacles_length];
		enemys_prefab = new Unit[enemys_prefab_length];
		int init_positions_ink = 0;
		int obstacles_ink = 0;
		int enemys_prefab_ink = 0;

		for (int i = 0; i < length; i++) {
			if (enemys_config [i].z == -2)
				init_positions [init_positions_ink++] = new Vector2 (enemys_config [i].x, enemys_config [i].y);
			else if (enemys_config [i].z == -1)
				obstacles [obstacles_ink++] = new Vector2 (enemys_config [i].x, enemys_config [i].y);
			else {
				int order = (int)enemys_config [i].z;
				enemys_prefab [enemys_prefab_ink] = new Unit ();
				string order_string = ((order > 99) ? "" : (order > 9) ? "0" : "00") + order.ToString ();
				enemys_prefab [enemys_prefab_ink].click_character = (GameObject)Resources.Load ("Prefab/Enemys/Enemys_" + order_string + "/Enemys_" + order_string, typeof(GameObject));
				enemys_prefab [enemys_prefab_ink].click_character.GetComponent<Click_Character> ().character.index = (int)enemys_config [i].z;
				enemys_prefab [enemys_prefab_ink].init_position = new Vector2(enemys_config [i].x, enemys_config [i].y);

				enemys_prefab_ink++;
			}
		}
	}

	public int Set_init_position_order;
	public bool Set_init_position_halt;
	IEnumerator Set_init_position(){
		can_edit = false;

		//讓玩家選初始位置
		GameObject[] init_positions_can_selects = new GameObject[init_positions.Length];
		for (int i = 0; i < init_positions.Length; i++) {
			init_positions_can_selects [i] = Instantiate (init_positions_gmo, acts.Board_Blocks_Vector_to_Coordinate (init_positions [i]), Quaternion.identity);
			init_positions_can_selects [i].GetComponent<click_init_position> ().order = i;
		}

		for (int i = 0; i < allies.Length; i++) {
			details.change (allies [i]);
			Set_init_position_halt = true;
			while(Set_init_position_halt)yield return null;

			Destroy (init_positions_can_selects[Set_init_position_order]);
			allies [i].loc = init_positions[Set_init_position_order];
			allies [i].gmo.transform.position = acts.Board_Blocks_Vector_to_Coordinate (allies[i].loc);
		}

		foreach (GameObject g in init_positions_can_selects) Destroy (g);
		details.change (allies [0]);

		can_edit = true;
		next_round();
	}

	void init_character(){
		// 設置敵人

		allies = new Character[allies_prefab.Length];
		enemys = new Character[enemys_prefab.Length];

		has_second_enemy = (second_enemys_prefab.Length > 0);
		for (int i = 0; i < enemys.Length; i++) {
			GameObject unit;
			unit = (GameObject)Instantiate (enemys_prefab [i].click_character, new Vector3(), Quaternion.identity);
			enemys[i] = unit.GetComponent<Click_Character>().character;
			unit.transform.parent = character_folder;
			unit.transform.position = acts.Board_Blocks_Vector_to_Coordinate(enemys_prefab [i].init_position);
			unit.GetComponent<AI> ().enemy = enemys[i];
			enemys [i].gmo = unit;
			enemys [i].character_camp = Character_camp.enemys;
			enemys [i].hp = enemys [i].max_hp;
			enemys [i].mp = enemys [i].max_mp;
			enemys [i].ep = enemys [i].max_ep;
			enemys [i].loc = enemys_prefab [i].init_position;
			enemys [i].order = i;
			enemys_head [i].GetComponent<SpriteRenderer> ().sprite = unit.transform.Find("Images").Find("head").gameObject.GetComponent<SpriteRenderer> ().sprite;
			enemys_big_frames [i].SetActive (true);
			set_tactic_color (i, 0.0f);
		}

		for (int i = 0; i < allies.Length; i++) {
			GameObject unit;
			unit = (GameObject)Instantiate (allies_prefab [i].click_character, new Vector3(), Quaternion.identity);
			allies[i] = unit.GetComponent<Click_Character>().character;
			allies_prefab [i].init_position = new Vector2 (99, 99);
			unit.transform.parent = character_folder;
			unit.transform.position = acts.Board_Blocks_Vector_to_Coordinate(allies_prefab [i].init_position);
			allies [i].gmo = unit;
			allies [i].character_camp = Character_camp.allies;
			allies [i].hp = allies [i].max_hp;
			allies [i].mp = allies [i].max_mp;
			allies [i].ep = allies [i].max_ep;
			allies [i].loc = allies_prefab [i].init_position;
			allies [i].order = i;
			allies_head [i].GetComponent<SpriteRenderer> ().sprite = unit.transform.Find("Images").Find("head").gameObject.GetComponent<SpriteRenderer> ().sprite;
			allies_big_frames [i].SetActive (true);
		}
	}

	public void init_second_enemys_character(){
		enemys = new Character[second_enemys_prefab.Length];
		for (int i = 0; i < enemys.Length; i++) {
			GameObject unit;
			Vector2 init_position;
			int j = 0;
			while (j < second_enemys_prefab [i].possible_init_positions.Length && the_character_on_block (second_enemys_prefab [i].possible_init_positions [j]) != null)j++;
			if(j >= second_enemys_prefab [i].possible_init_positions.Length)Debug.Log("second_enemys_prefab [i].possible_init_positions.Length is out of array range");
			init_position = second_enemys_prefab [i].possible_init_positions [j];
			unit = (GameObject)Instantiate (second_enemys_prefab [i].unit.click_character, new Vector3(), Quaternion.identity);
			enemys[i] = unit.GetComponent<Click_Character>().character;
			unit.transform.parent = character_folder;
			unit.transform.position = acts.Board_Blocks_Vector_to_Coordinate(init_position);
			enemys [i].gmo = unit;
			enemys [i].loc = init_position;
			enemys [i].character_camp = Character_camp.enemys;
			enemys [i].hp = enemys [i].max_hp;
			enemys [i].mp = enemys [i].max_mp;
			enemys [i].ep = enemys [i].max_ep;
			enemys [i].order = i;
			enemys_head [i].GetComponent<SpriteRenderer> ().sprite = unit.transform.Find("Images").Find("head").gameObject.GetComponent<SpriteRenderer> ().sprite;
			enemys_big_frames [i].SetActive (true);
			set_tactic_color (i, 0.0f);
		}
	}

	void init_obstacle(){
		for (int i = 0; i < obstacles.Length; i++) Instantiate (obstacle_gmo, acts.Board_Blocks_Vector_to_Coordinate(obstacles [i]), Quaternion.identity).GetComponent<Click_Character>().character.character_camp = Character_camp.obstacles;
	}

	public IEnumerator Start_battle(){
		if (editor_setting.enable_ai_debug_mode) {
			GameObject[] gmos = GameObject.FindGameObjectsWithTag ("Skill");
			foreach(GameObject gmo in gmos)Destroy(gmo);
			skill_Box_Details.easy_mover.SetActive (false);
			for (int i = 0; i < 3; i++)allies_big_frames [i].SetActive (false);
			for (int i = 0; i < 3; i++)allies_head [i].SetActive (false);
			details.details_box.SetActive (false);
			Vector3[] pos = new Vector3[5];
			for (int i = 0; i < 5; i++)pos[i] = new Vector3 (15 + 3.5f * i, 15.5f, 0);
			for (int i = 0; i < 10; i++) {
				enemys [0].gmo.GetComponent<AI>()._AI (pos);
				for (int j = 0; j < 5; j++)pos[j] -= new Vector3 (0, 3.5f, 0);
			}
			while (true)
				yield return null;
		}

		can_edit = false;
		for (int i = 0; i < enemys.Length; i++)if (enemys [i].hp > 0) {
			enemys [i].gmo.GetComponent<AI>()._AI (null);
			set_tactic_color (i, 0.0f);
		}
		StartCoroutine (easy_mover_move(false, skill_Box_Details.easy_mover_from, skill_Box_Details.easy_mover_to));
		yield return new WaitForSeconds (acts.move_delay + acts.fade_in_delay + acts.start_delay);

//		int acts_count = allies_shines [0].transform.childCount;

		//write here;

		GameObject[] queue = get_queue();

		for (int i = 0; i < queue.Length; i++) {
			GameObject skill = the_skill_in_frame (queue [i].transform);
			if (skill == null) continue;
			queue [i].GetComponent<SpriteRenderer> ().sortingLayerName = "toppest-1";
			queue [i].SetActive (true);

			//use skill here;

			skill.GetComponent<Click_skill> ().use_skill ();
			float rate = (acts.is_speeding) ? 2 : 1;
			yield return new WaitForSeconds ((skill.GetComponent<Click_skill>().skill_detail.show_time + acts.show_delay) * rate);
			if (acts.someone_die) {
				yield return new WaitForSeconds (acts.die_delay);
				acts.someone_die = false;
				queue [i]?.gameObject.SetActive (false);
				if(acts.game_over)yield return new WaitForSeconds (99999);
			}
			Destroy (skill);
			queue [i].GetComponent<SpriteRenderer> ().sortingLayerName = "shines";
			queue [i].gameObject.SetActive (false);
		}

		init_skill_box ();
		StartCoroutine (easy_mover_move(true, skill_Box_Details.easy_mover_to, skill_Box_Details.easy_mover_from));
		for (int i = 0; i < allies_prefab.Length; i++) if(allies[i].hp > 0) allies [i].regain ();
		for (int i = 0; i < enemys_prefab.Length; i++) if(enemys[i].hp > 0) enemys [i].regain ();

		yield return new WaitForSeconds (acts.move_delay * 1.5f);
	}

	GameObject[] get_queue(){
		GameObject[] ret = new GameObject[30];
		int ink = 0;
		for (int i = 0; i < 5; i++) {
			Transform[] temp = new Transform[6];
			int[] temp_speed = new int[6];
			int[] temp_order = new int[6];
			int jnk = 0;
			for (int j = 0; j < allies.Length; j++) {
				temp [jnk] = allies_shines [j].transform.GetChild (i);
				GameObject skill = the_skill_in_frame (temp [jnk]);
				temp_speed[jnk] = (skill != null) ? skill.GetComponent<Click_skill>().speed : 0 ;
				temp_order [jnk] = jnk;
				jnk++;
			}
			for (int j = 0; j < enemys.Length; j++) {
				temp [jnk] = enemys_shines [j].transform.GetChild (i);
				GameObject skill = the_skill_in_frame (temp [jnk]);
				temp_speed[jnk] = (skill != null) ? skill.GetComponent<Click_skill>().speed : 0 ;
				temp_order [jnk] = jnk;
				jnk++;
			}

			//sort
			for (int j = 0; j < jnk; j++) {
				for (int h = j+1; h < jnk; h++) {
					if (temp_speed [j] < temp_speed [h] || (temp_speed [j] == temp_speed [h] && temp_order[j] < temp_order[h])) {
						Transform ano_temp = temp[j];
						temp [j] = temp [h];
						temp [h] = ano_temp;
						int ano_temp2 = temp_speed[j];
						temp_speed [j] = temp_speed [h];
						temp_speed [h] = ano_temp2;
						int ano_temp3 = temp_order[j];
						temp_order [j] = temp_order [h];
						temp_order [h] = ano_temp3;
					}
				}
			}

			for (int j = 0; j < jnk; j++) ret [ink++] = temp [j].gameObject;
		}

		GameObject[] ano = new GameObject[ink];
		for (int i = 0; i < ink; i++)ano [i] = ret [i];
		return ano;
	}

	IEnumerator easy_mover_move(bool boolean, Vector2 _from, Vector2 _to){
		acts.skill_box_gear1.GetComponent<rotate_shine_controller> ().rotate_speed = -acts.skill_box_gear1.GetComponent<rotate_shine_controller> ().rotate_speed;
		acts.skill_box_gear2.GetComponent<rotate_shine_controller> ().rotate_speed = -acts.skill_box_gear2.GetComponent<rotate_shine_controller> ().rotate_speed;
		acts.skill_box_gear1.GetComponent<rotate_shine_controller> ().can_rotate = true;
		acts.skill_box_gear2.GetComponent<rotate_shine_controller> ().can_rotate = true;
		if (boolean) {
			for (float i = 0; i < acts.fade_in_delay; i += Time.deltaTime) {
				for(int j=0;j<3;j++)set_tactic_color (j, 1 - i / acts.fade_in_delay);
				yield return null;
			}
			details.change (allies [order_of_first_stand_allies()]);
		}

		for (float i = 0; i < skill_Box_Details.easy_mover_time; i += Time.deltaTime) {
			skill_Box_Details.easy_mover.transform.localPosition = new Vector3 (_from.x + (_to.x - _from.x) * Mathf.Sin(Mathf.Sin(i / skill_Box_Details.easy_mover_time * Mathf.PI / 2)* Mathf.PI / 2) ,_to.y, 0);
			yield return null;
		}
		skill_Box_Details.easy_mover.transform.localPosition = new Vector3 (_to.x ,_to.y, 0);
		for (int k = 0; k < allies_prefab.Length; k++) {
			for (int j = 0; j < allies[k].skills.Length; j++) {
					allies[k].skills [j].GetComponent<Click_skill> ().home = allies[k].skills [j].transform.position;
				}

		}

		if (!boolean) {
			acts.skill_box_gear1.GetComponent<rotate_shine_controller> ().can_rotate = false;
			acts.skill_box_gear2.GetComponent<rotate_shine_controller> ().can_rotate = false;
			for (float i = 0; i < acts.fade_in_delay; i += Time.deltaTime) {
				for(int j=0;j<3;j++)set_tactic_color (j, i / acts.fade_in_delay);
				yield return null;
			}
		}

		if (boolean) {
			acts.skill_box_gear1.GetComponent<rotate_shine_controller> ().can_rotate = false;
			acts.skill_box_gear2.GetComponent<rotate_shine_controller> ().can_rotate = false;
			can_edit = true;
			next_round ();
		}
	}

	public GameObject the_skill_in_frame(Vector3 pos){
		GameObject[] skills = GameObject.FindGameObjectsWithTag ("Skill");
		foreach (GameObject skill in skills) {
			if (skill.transform.position == pos) return skill;
		}
		return null;
	}

	public GameObject the_skill_in_frame(Transform trans){
		GameObject[] skills = GameObject.FindGameObjectsWithTag ("Skill");
		foreach (GameObject skill in skills) {
			if (skill.transform.position == trans.position) return skill;
		}
		return null;
	}

	bool find_big_frames(float y){
		for (int i = 0; i < allies.Length; i++) {
			if(allies_big_frames[i] == null)continue;
			if(allies_big_frames[i].transform.position.y == y)return true;
		}

		for (int i = 0; i < enemys.Length; i++) {
			if(enemys_big_frames[i] == null)continue;
			if(enemys_big_frames[i].transform.position.y == y)return true;
		}
		return false;
	}

	public void init_skill_box(){
		foreach(Transform skill in skill_Box_Details.skills_folder)Destroy(skill.gameObject);
		int first = order_of_first_stand_allies();
		for (int k = 0; k < allies_prefab.Length; k++) {
			if (allies [k] == null) {
				Debug.Log ("null");
				continue;
			}
			allies[k].skills = new GameObject[allies[k].skills_prefab.Length];
			for (int j = 0; j < allies[k].skills_prefab.Length; j++) {
				int x = j % 4;
				int y = j / 4;
				Vector2 position = acts.Skill_Blocks_Vector_to_Coordinate (new Vector2(x, y));
				allies[k].skills [j] = (GameObject)Instantiate (allies[k].skills_prefab [j], position, Quaternion.identity);
				allies[k].skills [j].name = allies[k].skills [j].name.Substring(0, allies[k].skills [j].name.Length - 7);
				allies[k].skills [j].transform.parent = skill_Box_Details.skills_folder;
				allies[k].skills [j].transform.localPosition = position;
				allies[k].skills [j].GetComponent<Click_skill> ().home = allies[k].skills [j].transform.position;
				allies[k].skills [j].GetComponent<Click_skill> ().shines = allies_shines [k].transform;
				allies[k].skills [j].SetActive (first == k);
			}
		}

		for (int k = 0; k < enemys_prefab.Length; k++) {
			if (enemys [k] == null) {
				Debug.Log ("null");
				continue;
			}
			enemys[k].skills = new GameObject[enemys[k].skills_prefab.Length];
			for (int j = 0; j < enemys[k].skills.Length; j++) {
				int x = j % 4;
				int y = j / 4;
				Vector2 position = acts.Skill_Blocks_Vector_to_Coordinate (new Vector2(x, y));
				enemys[k].skills [j] = (GameObject)Instantiate (enemys[k].skills_prefab [j], position, Quaternion.identity);
				enemys[k].skills [j].name = enemys[k].skills [j].name.Substring(0, enemys[k].skills [j].name.Length - 7);
				enemys[k].skills [j].transform.parent = skill_Box_Details.skills_folder;
				enemys[k].skills [j].transform.localPosition = position;
				enemys[k].skills [j].GetComponent<Click_skill> ().home = enemys[k].skills [j].transform.position;
				enemys[k].skills [j].GetComponent<Click_skill> ().shines = enemys_shines [k].transform;
				enemys[k].skills [j].GetComponent<Click_skill> ().can_click = false;
				enemys[k].skills [j].SetActive (false);
			}
		}
	}

//	public void change_skill_box(int order){
//		skill_box_focus = order;
//		for (int k = 0; k < allies_prefab.Length; k++) {
//			if (allies [k].hp <= 0 ) continue;
//			for (int i = 0; i < allies_prefab[k].skill_boxs.Length; i++) {
//				for (int j = 0; j < allies_prefab[k].skill_boxs [i].skills.Length; j++) {
//					if (allies [k].skill_boxs [i].skills [j] == null)return;
//					allies[k].skill_boxs [i].skills [j].SetActive (allies_focus == k && i == order);
//					if (allies[k].skill_boxs [i].skills [j].GetComponent<Click_skill> ().is_in_frame (allies[k].skill_boxs [i].skills [j].GetComponent<Click_skill> ().transform.position))allies[k].skill_boxs [i].skills [j].SetActive (true);
//				}
//			}
//		}
//
//		for(int i=0;i< skill_Box_Details.switchs.Length;i++)skill_Box_Details.switchs[i].GetComponent<SpriteRenderer> ().sprite = (i==order) ? skill_Box_Details.focus[i] : skill_Box_Details.unfocus[i];
//		skill_Box_Details.background.GetComponent<SpriteRenderer> ().sprite = skill_Box_Details.background_images [order];
//	}


	public GameObject the_character_on_block(Vector2 loc){
		Debug.Log ("asdf");
		Vector3 pos = acts.Board_Blocks_Vector_to_Coordinate (loc);
		GameObject[] characters = GameObject.FindGameObjectsWithTag ("Character");
		foreach (GameObject cha in characters) {
			if (cha.transform.position == pos)return cha;
		}
		return null;
	}

	void next_round(){
		acts.round++;
		if (acts.round < 10)
			acts.round_number_0.GetComponent<SpriteRenderer>().sprite = acts.round_font [acts.round];
		else {
			acts.round_number_0.GetComponent<SpriteRenderer>().sprite = acts.round_font [acts.round / 10];
			acts.round_number_1.GetComponent<SpriteRenderer>().sprite = acts.round_font [acts.round % 10];
		}

		if (acts.round >= 11 && dataController.stageInformation.victory_condition == Victory_condition.SurviveFor10Turns) {
			StartCoroutine(victory_ani ());
		}
		if(acts.round == 1)trigger_all_passive_skill (Passive_skill_launch_time.遊戲開始);
		trigger_all_passive_skill (Passive_skill_launch_time.回合開始);
	}

	public void trigger_all_passive_skill(Passive_skill_launch_time passive_skill_launch_time){
		trigger_all_passive_skill (passive_skill_launch_time, null);
	}

	public void trigger_all_passive_skill(Passive_skill_launch_time passive_skill_launch_time, string Name){
		for (int i = 0; i < allies.Length; i++) {
			foreach (GameObject gmo in allies[i].skills_prefab) {
				Click_skill s = gmo.GetComponent<Click_skill> ();
				if (s.skill_detail.skill_type == Skill_Type.Passive && s.skill_detail.passive_skill_launch_time == passive_skill_launch_time &&
					(Name == null || Name == s.Name))
					use_passive_skill (s, allies[i]);
			}
		}

		for (int i = 0; i < enemys.Length; i++) {
			foreach (GameObject gmo in enemys[i].skills_prefab) {
				Click_skill s = gmo.GetComponent<Click_skill> ();
				if (s.skill_detail.skill_type == Skill_Type.Passive && s.skill_detail.passive_skill_launch_time == passive_skill_launch_time&&
					(Name == null || Name == s.Name))
					use_passive_skill (s, enemys[i]);
			}
		}
	}

	void set_tactic_color(int order, float alpha){
		if (order >= enemys_prefab.Length)return;

		Color color = new Color (1.0f, 1.0f, 1.0f, alpha);
		enemys_head [order].GetComponent<SpriteRenderer> ().color = color;
		enemys_big_frames [order].GetComponent<SpriteRenderer> ().color = color;
		foreach(Transform shine in enemys_shines[order].transform){
			GameObject skill = the_skill_in_frame (shine);
			if(skill != null){
				if (skill.GetComponent<SpriteRenderer> () != null)skill.GetComponent<SpriteRenderer> ().color = color;
				skill.transform.Find("rotated_Image").GetComponent<SpriteRenderer> ().color = color;
			}
		}
	}

	int order_of_first_stand_allies(){
		for (int i = 0; i < allies_prefab.Length; i++)if (allies [i].hp > 0)return i;
		return -1;
	}

	public float Compute_counter_property(Property attack, Property defend){
		if (attack == Property.無 || defend == Property.無) return 1.0f;
		if (attack == Property.水 || defend == Property.火) return 1.25f;
		if (attack == Property.火 || defend == Property.地) return 1.25f;
		if (attack == Property.地 || defend == Property.水) return 1.25f;
		return 0.9f;
	}

	public IEnumerator victory_ani(){
		acts.game_over = true;
		yield return new WaitForSeconds(acts.victory_delay);
		for (float i = 0; i < acts.black_time; i += Time.deltaTime) {
			acts.black.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, i / acts.black_time * 0.9f);
			yield return null;
		}
		acts.black.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 0.9f);

		acts.victory.SetActive (true);
	}

	public IEnumerator defeat_ani(){
		acts.game_over = true;
		yield return new WaitForSeconds(acts.victory_delay);
		for (float i = 0; i < acts.black_time; i += Time.deltaTime) {
			acts.black.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, i / acts.black_time * 0.9f);
			yield return null;
		}
		acts.black.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 0.9f);
		acts.defeat.SetActive (true);
	}




	//  passive skill  //

	public void use_passive_skill(Click_skill s, Character passiver){
		switch (s.skill_detail.passive_Type) {
		case Passive_Type.吞噬:
			StartCoroutine (passive_吞噬 (s, passiver));
			break;
		case Passive_Type.岩石裝甲:
			StartCoroutine (passive_岩石裝甲 (s, passiver));
			break;
		case Passive_Type.鏡像:
			StartCoroutine (passive_鏡像 (s, passiver));
			break;
		case Passive_Type.首次攻擊額外傷害:
			StartCoroutine (passive_首次攻擊額外傷害 (s, passiver));
			break;
		}
	}


	IEnumerator passive_首次攻擊額外傷害(Click_skill s, Character passiver){
		passiver.buffs.Add (new Buff(Buff_Type.提昇首次攻擊, s.skill_detail.dmg_rate, 1));
		Debug.Log ("fuck");	
		yield return null;
	}

	IEnumerator passive_鏡像(Click_skill s, Character passiver){
		Character cloner = allies [0];
		passiver.max_hp = cloner.max_hp;
		passiver.max_mp = cloner.max_mp;
		passiver.max_ep = cloner.max_ep;
		passiver.hp = cloner.hp;
		passiver.mp = cloner.mp;
		passiver.ep = cloner.ep;
		passiver.psy_atk = cloner.psy_atk;
		passiver.psy_def = cloner.psy_def;
		passiver.mag_atk = cloner.mag_atk;
		passiver.mag_def = cloner.mag_def;
		passiver.TEC = cloner.TEC;
		passiver.INT = cloner.INT;
		passiver.AGI = cloner.AGI;
		yield return null;
	}

	IEnumerator passive_岩石裝甲(Click_skill s, Character passiver){
		if (acts.round % 2 == 1)
			passiver.buffs.Add (new Buff (Buff_Type.魔法減傷, 1, 1));
		if (acts.round % 2 == 0)
			passiver.buffs.Add (new Buff (Buff_Type.機械減傷, 1, 1));
		yield return null;
	}

	IEnumerator passive_吞噬(Click_skill s, Character passiver){
		StartCoroutine(passiver.add_hp (s.skill_detail.refill_hp));
		yield return null;
	}
}

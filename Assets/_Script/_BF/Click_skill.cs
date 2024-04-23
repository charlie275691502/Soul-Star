using UnityEngine;
using System.Collections;

public enum Property{
	無,
	水,
	火,
	地
}

public enum Damage_Type{
	無,
	機械,
	魔法,
	混合,
	真實
}

public enum Passive_skill_launch_time{
	遊戲開始,
	回合開始,
	立即
}

public enum Skill_Type{
	Move,
	Machine,
	Magic,
	Mix,
	Item,
	Passive
}

public enum Move_Type{
	pure_ahead,
	pure_oblique,
	Refill,
	緩動
}

public enum Machine_Type{
	pure_damage,
	move_damage,
	move_success_damage,
	random_damage,
	refill,
	Dance,
	拍擊
}

public enum Magic_Type{
	pure_damage,
	effect_damage,
	knockback,
	危險感應,
	石療
}

public enum Mix_Type{
	pure_damage,
	影穿,
	buff_damage
}

public enum Item_Type{

}

public enum Passive_Type{
	吞噬,
	岩石裝甲,
	首次攻擊額外傷害,
	鏡像
}

[System.Serializable]
public class Skill_detail{
	[SerializeField]
	public float show_time;
	public Skill_Type skill_type;
	[HideInInspector] public int dir;

	//Move
	public Move_Type move_Type;

	//Machine
	public Machine_Type machine_Type;
	public bool only_slash_once;
	public float dmg_rate;
	public int random_amount;
	//	public float bounce_time;
	public int attacks_range_size;
	public Vector3[] attacks_range;
	public float attack_delay;
	public int refill_hp;
	public int refill_mp;
	public int refill_ep;

	//Magic
	public Magic_Type magic_Type;
	public Buff_Type buff_Type;
	public int para1;
	public int turn;
	public float hit_wall_deal_damage;
	public bool hit_wall_cancel_skill;

	//Mix
	public Mix_Type mix_Type;

	//Item
	public Item_Type item_Type;

	//Passive
	public Passive_Type passive_Type;
	public Passive_skill_launch_time passive_skill_launch_time;

}

public class Click_skill : MonoBehaviour {
	[HideInInspector] public Transform shines;
	public string Name;
	public Damage_Type damage_type;
	public Property damage_property;
	public int speed;
	public int mp;
	public int ep;
	public string skill_desc;
	public int ahead_distance;
	public bool effect_Type_對手;
	public bool effect_Type_隊友;
	public bool effect_Type_障礙物;
	public bool effect_Type_空氣;
	[HideInInspector] public bool can_click;
	public GameObject slash;
	public bool save;
	private float right_click_time; // acts
	[HideInInspector] public Skill_detail skill_detail;
	[HideInInspector] public int length;
	[HideInInspector] public bool pre_on_tactic;
	[HideInInspector] public Character pre_target;
	[HideInInspector] public GameObject detail_box;
	[HideInInspector] public Vector3 home;
	[HideInInspector] public int skill_box_order;
	private bool can_drag;
	private float double_click_time;
	private float mouse_hover_time;
	private bool mouse_hover_first_entrance = true;

	[HideInInspector] public GameController gameController;
	private Transform rotated_Image;


	void Awake(){
		if(skill_detail.skill_type != Skill_Type.Passive)can_click = true;
		shines = null;
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		pre_on_tactic = false;
		setup_skiller ();
	}

	void Start(){
		if(can_click)can_click = (transform.parent == null || transform.parent.tag != "Skill" );
	}

	// Update is called once per frame
	void Update () {
		right_click_time += Time.deltaTime;
		double_click_time += Time.deltaTime;

		//reset hover
		if(!mouse_hover_first_entrance)mouse_hover_time += Time.deltaTime;
	}

	void OnMouseDown(){
		if (!gameController.can_edit)return;
		if (!can_click)return;
		//stop hover
		show_detail_box(false);
		mouse_hover_first_entrance = true;
		mouse_hover_time = 0.0f;

		//double_click
		if (double_click_time < gameController.acts.double_click_delay) {
			if (!is_in_frame (transform.position)) {
				turn_on_frame_shine ();
				bool flag = false;
				transform.parent = gameController.used_skill;
				foreach (Transform child in shines) {
					if (child.gameObject.activeSelf) {
						transform.position = child.position;
						flag = true;
						break;
					}
				}
				if (flag) {
					change_skill_sortingLayer ("on_tactic");
					change_down_list (true);
				}
				turn_off_frame_shine ();
			} else {
				transform.position = home;
				transform.parent = gameController.skill_Box_Details.skills_folder;
				change_down_list (false);
				change_skill_sortingLayer ("on_canvas");
			}
			can_drag = false;
		} else can_drag = true;
		double_click_time = 0.0f;
	}

	void OnMouseDrag(){
		if (!gameController.can_edit)return;
		if (!can_click)return;
		if (!can_drag)return;

		//don't show the hover flag
		show_detail_box(false);
		mouse_hover_first_entrance = true;
		mouse_hover_time = 0.0f;

		Vector3 pos =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
		pos = new Vector3 (pos.x, pos.y, 0);

		turn_on_frame_shine ();

		foreach (Transform child in shines) {
			if (Mathf.Abs (child.position.x - pos.x) < 2.0f && Mathf.Abs (child.position.y - pos.y) < 2.0f /*&& gameController.the_skill_in_frame (child) == null*/ && child.gameObject.activeSelf) {
				transform.parent = gameController.used_skill;
				pos = child.position;
				break;
			}
		}


		change_down_list (true);
		change_skill_sortingLayer ("toppest");

		if (Mathf.Abs (home.x - pos.x) < 2.0f && Mathf.Abs (home.y - pos.y) < 2.0f)pos = home;
		transform.position = pos;
	}

	void OnMouseUp(){

		if (!can_click)return;
		if (!gameController.can_edit)return;

		if (!is_in_frame (transform.position)) {
			transform.position = home;
			transform.parent = gameController.skill_Box_Details.skills_folder;
			change_down_list (false);
			turn_off_frame_shine ();
			change_skill_sortingLayer ("on_canvas");
		} else change_skill_sortingLayer ("on_tactic");

		turn_off_frame_shine ();
	}

	public bool is_in_frame(Vector2 pos){
		if(shines == null)return true;
		foreach (Transform child in shines) {
			if (child.position == (Vector3)pos )return true;
		}
		return false;
	}

	void OnMouseOver(){
		if (!gameController.can_edit)return;
		if (Input.GetMouseButton(1)) {
			if(right_click_time >= gameController.acts.skill_right_click_cd){
				right_clicked = true;
				right_click_time = 0.0f;
				skill_detail.dir = (skill_detail.dir + 1 ) % 4;
				rotated_Image.rotation *= Quaternion.Euler(0, 0, -90);
			}
		}

		if (mouse_hover_time >= gameController.acts.mouse_hover_delay /* && transform.position == home */) show_detail_box(true);
		if (mouse_hover_first_entrance) mouse_hover_first_entrance = false;
	}

	void OnMouseExit(){
		mouse_hover_first_entrance = true;
		mouse_hover_time = 0.0f;
		show_detail_box(false);
	}

	bool right_clicked = false;

	void attack_view_on(){
		if (!right_clicked && GameObject.FindGameObjectWithTag ("Attack_view") != null) return;
		attack_view_off ();
		right_clicked = false;
		GameObject mover = gameController.details.now_character.gmo;
		if (skill_detail.skill_type == Skill_Type.Move) {

		} else {
			Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);
			foreach (Vector3 aimed_location in aimed_locations) {
				Vector3 loc = mover.GetComponent<Click_Character> ().character.loc + aimed_location;
				if (!in_attack_range (mover, loc))continue;
				Instantiate (gameController.acts.attack_view, mover.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			}
		}
	}

	void attack_view_off(){
		foreach (GameObject gmo in GameObject.FindGameObjectsWithTag("Attack_view")) Destroy (gmo);
	}

	void setup_skiller(){
		length = 1;
		GameObject node = gameObject;
		while ((node = find_down (node)) != null)length++;

		rotated_Image = transform.Find ("rotated_Image");
		foreach (Transform child in transform) if (child.tag == "Detail_box")detail_box = child.gameObject;
		if (detail_box == null)Debug.Log (gameObject.name + " lost it's detail_box");

		if (find_down(gameObject) != null)find_down(gameObject).SetActive (false);

		//		if (skiller_names.Length == 0) {
		//			int size = gameController.allies.Length;
		//			skiller = new Transform[size];
		//			for (int i = 0; i < size; i++)skiller [i] = gameController.allies_shines [i].transform;
		//			return;
		//		}

		//		int ink = 0;
		//		int[] temp = new int[3];
		//		for (int i = 0; i < gameController.allies.Length; i++) {
		//			for (int j = 0; j < skiller_names.Length; j++) {
		//				if (gameController.allies_prefab [i].click_character.GetComponent<Click_Character>().character.name == skiller_names [j])temp [ink++] = i;
		//			}
		//		}
		//
		//		skiller = new Transform[ink];
		//		for (int i = 0; i < ink; i++)skiller [i] = gameController.allies_shines[temp [i]].transform;

	}

	void show_detail_box(bool boolean){
		detail_box.SetActive (boolean);
		if (boolean) {
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			pos = new Vector3 (pos.x, pos.y, 0);
			detail_box.transform.position = pos;
			attack_view_on ();
		} else attack_view_off ();
	}

	void change_down_list(bool boolean){
		GameObject node = gameObject;
		while ((node = find_down(node)) != null) node.SetActive (boolean);
	}

	void turn_on_frame_shine(){
		if(shines == null)return ;
		int ans = 0;
		for (int i=0;i<5;i++) {
			Transform child = shines.GetChild (i);
			ans = (gameController.the_skill_in_frame(child) != null && !gameObject_in_child(child)) ? 0 : ans + 1;
			if((length <= ans /*|| i==2 */ )&& can_use_skill(i))child.gameObject.SetActive (true);
		}
	}

	bool can_use_skill(int order){
		Character target = find_target (shines.GetChild(0).position);
		if (target.mp < mp || target.ep < ep)return false;

		for (int i = 0; i < 5; i++) {
			if (i == order)continue;
			if (i < order && !check_skill_pair (gameController.the_skill_in_frame (shines.GetChild (i)), i, this.gameObject, order))return false;
			if (i > order && !check_skill_pair (this.gameObject, order, gameController.the_skill_in_frame (shines.GetChild (i)), i))return false;
		}

		return true;
	}

	bool check_skill_pair(GameObject A_gmo, int a, GameObject B_gmo, int b){
		if (A_gmo == null || B_gmo == null)return true;
		Click_skill A = A_gmo.GetComponent<Click_skill> ();
		Click_skill B = B_gmo.GetComponent<Click_skill> ();
		if(A.skill_detail.dmg_rate > 0 && B.GetComponent<Click_skill>().Name == "Dash")return false;
		return true;
	}

	void turn_off_frame_shine(){
		if(shines == null)return ;
		foreach (Transform child in shines)child.gameObject.SetActive (false);
	}

	bool gameObject_in_child(Transform child){
		GameObject node = gameObject;
		do {
			if(node.transform.position == child.position)return true;
			node = find_down(node);
		} while(node != null);
		return false;
	}

	public void change_skill_sortingLayer(string layer){
		string name1, name2;
		if (layer == "toppest") {
			name1 = "toppest-1";
			name2 = "toppest-2";
		} else {
			name1 = "skill_blocks_" + layer;
			name2 = "skill_blocks_pointer_" + layer;
		}
		if(GetComponent<SpriteRenderer> () != null)GetComponent<SpriteRenderer> ().sortingLayerName = name1;
		rotated_Image.gameObject.GetComponent<SpriteRenderer> ().sortingLayerName = name2;

		if (layer == "toppest")return;

		if (layer == "on_tactic" && !pre_on_tactic) {
			find_target (transform.position).add_mp (-mp);
			find_target (transform.position).add_ep (-ep);
			if(gameController.can_edit)gameController.details.change (find_target (transform.position));
			pre_target = find_target (transform.position);
		}
		if (layer == "on_canvas" && pre_on_tactic) {
			pre_target.add_mp (mp);
			pre_target.add_ep (ep);
			if(gameController.can_edit)gameController.details.change (pre_target);
		}
		pre_on_tactic = (layer == "on_tactic");
	}

	Vector3 decode_dir(){
		if (skill_detail.dir == 0)return new Vector3 (1, 0, 0);
		if (skill_detail.dir == 1)return new Vector3 (0, -1, 0);
		if (skill_detail.dir == 2)return new Vector3 (-1, 0, 0);
		if (skill_detail.dir == 3)return new Vector3 (0, 1, 0);
		return new Vector3();
	}

	Vector3 decode_dir_oblique(){
		if (skill_detail.dir == 0)return new Vector3 (1, 1, 0);
		if (skill_detail.dir == 1)return new Vector3 (1, -1, 0);
		if (skill_detail.dir == 2)return new Vector3 (-1, -1, 0);
		if (skill_detail.dir == 3)return new Vector3 (-1, 1, 0);
		return new Vector3();
	}

	Character find_target(Vector3 pos){
		for (int i = 0; i < gameController.allies.Length; i++) {
			if (gameController.allies_shines [i] == null)continue;
			if (gameController.allies_shines [i].transform.position.y == pos.y)return gameController.allies [i];
		}
		for (int i = 0; i < gameController.enemys.Length; i++) {
			if (gameController.enemys_shines [i] == null)continue;
			if (gameController.enemys_shines [i].transform.position.y == pos.y)return gameController.enemys [i];
		}
		return null;
	}

	public bool is_obstacle(Vector2 loc){
		if (loc.x > 6 || loc.x < 0 || loc.y > 4 || loc.y < 0)return true;
		return (the_character_on_block (loc) != null && the_character_on_block (loc).GetComponent<Click_Character> ().character.character_camp == Character_camp.obstacles);
	}

	public GameObject the_character_on_block(Vector2 loc){
		Vector3 pos = gameController.acts.Board_Blocks_Vector_to_Coordinate (loc);
		GameObject[] characters = GameObject.FindGameObjectsWithTag ("Character");
		foreach (GameObject cha in characters) {
			if (cha.transform.position == pos)return cha;
		}
		return null;
	}

	public void use_skill(){
		switch (skill_detail.skill_type){
		case Skill_Type.Move:
			foreach(string name in System.Enum.GetNames(typeof(Move_Type)))if(skill_detail.move_Type.ToString() == name)gameObject.SendMessage("move_" + name);
			break;
		case Skill_Type.Machine:
			foreach(string name in System.Enum.GetNames(typeof(Machine_Type)))if(skill_detail.machine_Type.ToString() == name)gameObject.SendMessage("machine_" + name);
			break;
		case Skill_Type.Magic:
			foreach(string name in System.Enum.GetNames(typeof(Magic_Type)))if(skill_detail.magic_Type.ToString() == name)gameObject.SendMessage("magic_" + name);
			break;
		case Skill_Type.Mix:
			foreach(string name in System.Enum.GetNames(typeof(Mix_Type)))if(skill_detail.mix_Type.ToString() == name)gameObject.SendMessage("mix_" + name);
			break;
		case Skill_Type.Item:
			foreach(string name in System.Enum.GetNames(typeof(Item_Type)))if(skill_detail.item_Type.ToString() == name)gameObject.SendMessage("item_" + name);
			break;
		}
	}

	Vector3 Refill_move_adjust(Character character, Vector3 vec){
		Vector3 after_pos = character.loc + vec;
		if (after_pos.x > 6 || after_pos.x < 0 || after_pos.y > 4 || after_pos.y < 0 || (the_character_on_block (after_pos) != null && the_character_on_block (after_pos).GetComponent<Click_Character>().character.character_camp != character.character_camp))return new Vector3();
		character.loc = after_pos;
		return vec * gameController.acts.board_blocks_distance;
	}

	Vector3 move_adjust(Character character, Vector3 vec){
		Vector3 after_pos = character.loc + vec;
		if (after_pos.x > 6 || after_pos.x < 0 || after_pos.y > 4 || after_pos.y < 0 || the_character_on_block (after_pos) != null)return new Vector3();
		character.loc = after_pos;
		return vec * gameController.acts.board_blocks_distance;
	}

	//	public bool in_attack_range(GameObject attacker, Vector2 first_loc, Vector2 second_loc){
	//		if (first_loc.x < 0 || first_loc.x > 6 || first_loc.y < 0 || first_loc.y > 4 || second_loc.x < 0 || second_loc.x > 6 || second_loc.y < 0 || second_loc.y > 4)return true;
	//		//only none_then_diff_camp
	//		if (the_character_on_block (first_loc ) != null && effect_Type == Effect_Type.none_then_diff_camp)return true;
	//		if (the_character_on_block (second_loc) != null && effect_Type == Effect_Type.none_then_diff_camp)return attacker.GetComponent<Click_Character> ().character.character_camp == the_character_on_block (second_loc).GetComponent<Click_Character> ().character.character_camp;
	//		return false;
	//	}

	public bool in_attack_range(GameObject attacker_gmo, Vector2 loc){
		//出界
		if (loc.x < 0 || loc.x > 6 || loc.y < 0 || loc.y > 4)return false;

		GameObject defender_gmo = the_character_on_block (loc);

		if(defender_gmo == null) return effect_Type_空氣;

		Character attacker = attacker_gmo.GetComponent<Click_Character> ().character;
		Character defender = defender_gmo.GetComponent<Click_Character> ().character;

		if(attacker.character_camp == defender.character_camp) return effect_Type_隊友;
		if(defender.character_camp == Character_camp.obstacles) return effect_Type_障礙物;

		return effect_Type_對手;
	}

	public Vector3[] turn_attack_range(int x){
		Vector3[] vecs = new Vector3[skill_detail.attacks_range.Length];
		for(int i=0;i<vecs.Length;i++)vecs[i] = skill_detail.attacks_range[i];
		for (int i = 0; i < x; i++) {
			for(int j=0;j<vecs.Length;j++){
				vecs[j] = new Vector3 (vecs[j].y, -vecs[j].x, vecs[j].z);
			}
		}
		return vecs;
	}

	public GameObject find_down(GameObject gmo){
		foreach (Transform child in gmo.transform)if (child.tag == "Skill")return child.gameObject;
		return null;
	}




	/////////////////////////// move ///////////////////////////


	IEnumerator move_pure_ahead(){
		GameObject mover = find_target (transform.position).gmo;
		for (int i = 0; i < ahead_distance; i++) {
			Vector3 _from = mover.transform.position;
			Vector3 _to = _from + move_adjust(mover.GetComponent<Click_Character>().character, decode_dir ());
			if (_from == _to) {
				for (float j = 0; j < skill_detail.show_time / ahead_distance; j += Time.deltaTime) {
					mover.transform.position = _from + decode_dir () * gameController.acts.bounce_time * Mathf.Sin(j / (skill_detail.show_time / ahead_distance) * Mathf.PI);
					yield return null;
				}
			} else {
				for (float j = 0; j < skill_detail.show_time / ahead_distance; j += Time.deltaTime) {
					mover.transform.position = _from + (_to - _from) * Mathf.Sin(0.5f * Mathf.PI * Mathf.Sin( 0.5f * Mathf.PI * (j / (skill_detail.show_time / ahead_distance))));
					yield return null;
				}
			}
			mover.transform.position = _to;
		}
	}

	IEnumerator move_pure_oblique(){
		GameObject mover = find_target (transform.position).gmo;
		for (int i = 0; i < ahead_distance; i++) {
			Vector3 _from = mover.transform.position;
			Vector3 _to = _from + move_adjust(mover.GetComponent<Click_Character>().character, decode_dir_oblique ());
			if (_from == _to) {
				for (float j = 0; j < skill_detail.show_time / ahead_distance; j += Time.deltaTime) {
					mover.transform.position = _from + decode_dir_oblique () * gameController.acts.bounce_time * Mathf.Sin(j / (skill_detail.show_time / ahead_distance) * Mathf.PI);
					yield return null;
				}
			} else {
				for (float j = 0; j < skill_detail.show_time / ahead_distance; j += Time.deltaTime) {
					mover.transform.position = _from + (_to - _from) * Mathf.Sin(0.5f * Mathf.PI * Mathf.Sin( 0.5f * Mathf.PI * (j / (skill_detail.show_time / ahead_distance))));
					yield return null;
				}
			}
			mover.transform.position = _to;
		}
	}

	IEnumerator move_Refill(){
		GameObject mover = find_target (transform.position).gmo;
		for (int i = 0; i < ahead_distance; i++) {
			Vector3 _from = mover.transform.position;
			if (the_character_on_block (mover.GetComponent<Click_Character>().character.loc + decode_dir ()) != null && the_character_on_block (mover.GetComponent<Click_Character>().character.loc + decode_dir ()).GetComponent<Click_Character> ().character.character_camp == mover.GetComponent<Click_Character> ().character.character_camp) {
				the_character_on_block (mover.GetComponent<Click_Character>().character.loc + decode_dir ()).GetComponent<Click_Character> ().character.loc = mover.GetComponent<Click_Character> ().character.loc;
				the_character_on_block (mover.GetComponent<Click_Character>().character.loc + decode_dir ()).transform.position = mover.transform.position;
			}

			Vector3 _next = _from + Refill_move_adjust(mover.GetComponent<Click_Character>().character, decode_dir ());
			if (_from == _next) {
				for (float j = 0; j < skill_detail.show_time / ahead_distance; j += Time.deltaTime) {
					mover.transform.position = _from + decode_dir () * gameController.acts.bounce_time * Mathf.Sin(j / (skill_detail.show_time / ahead_distance) * Mathf.PI);
					yield return null;
				}
			} else {
				for (float j = 0; j < skill_detail.show_time / ahead_distance; j += Time.deltaTime) {
					mover.transform.position = _from + (_next - _from) * Mathf.Sin(0.5f * Mathf.PI * Mathf.Sin( 0.5f * Mathf.PI * (j / (skill_detail.show_time / ahead_distance))));
					yield return null;
				}
			}
			mover.transform.position = _next;
		}
		yield return null;
	}

	IEnumerator move_緩動(){
		if(Random.Range(0.0f, 1.0f) < 0.5f)StartCoroutine(move_pure_ahead());
		yield return null;
	}


	/////////////////////////// machine ///////////////////////////


	IEnumerator machine_pure_damage(){
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);

		if (skill_detail.only_slash_once)Instantiate (slash, attacker.transform.position, Quaternion.identity * Quaternion.Euler(0, 0, skill_detail.dir * -90));

		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			float dmg_rate = Mathf.Max (skill_detail.dmg_rate, aimed_location.z);
			if(the_character_on_block (loc) != null)the_character_on_block (loc).GetComponent<Click_Character> ().character.deal_dmg (dmg_rate, damage_type, damage_property, attacker.GetComponent<Click_Character>().character);
			if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}
		yield return null;
	}

	IEnumerator machine_move_damage(){

		GameObject mover = find_target (transform.position).gmo;
		for (int i = 0; i < ahead_distance; i++) {
			Vector3 _from = mover.transform.position;
			Vector3 _to = _from + move_adjust(mover.GetComponent<Click_Character>().character, decode_dir());
			if (_from == _to) {
				for (float j = 0; j < skill_detail.show_time / 2 / ahead_distance; j += Time.deltaTime) {
					mover.transform.position = _from + decode_dir() * gameController.acts.bounce_time * Mathf.Sin(j / (skill_detail.show_time / 2 / ahead_distance) * Mathf.PI);
					yield return null;
				}
			} else {
				for (float j = 0; j < skill_detail.show_time / 2 / ahead_distance; j += Time.deltaTime) {
					mover.transform.position = _from + (_to - _from) * Mathf.Sin(0.5f * Mathf.PI * Mathf.Sin( 0.5f * Mathf.PI * (j / (skill_detail.show_time / 2 / ahead_distance))));
					yield return null;
				}
			}
			mover.transform.position = _to;
		}

		effect_Type_對手 = true;
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);
		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			float dmg_rate = Mathf.Max (skill_detail.dmg_rate, aimed_location.z);
			if(the_character_on_block (loc) != null)the_character_on_block (loc).GetComponent<Click_Character> ().character.deal_dmg ( dmg_rate, damage_type, damage_property, attacker.GetComponent<Click_Character>().character);
			if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}
	}

	IEnumerator machine_move_success_damage(){

		GameObject mover = find_target (transform.position).gmo;
		int move_distance = 0;
		for (int i = 0; i < ahead_distance; i++) {
			Vector3 _from = mover.transform.position;
			Vector3 _to = _from + move_adjust(mover.GetComponent<Click_Character>().character, decode_dir());
			if (_to != _from)move_distance++;
			if (_from == _to) {
				for (float j = 0; j < skill_detail.show_time / 2 / ahead_distance; j += Time.deltaTime) {
					mover.transform.position = _from + decode_dir() * gameController.acts.bounce_time * Mathf.Sin(j / (skill_detail.show_time / 2 / ahead_distance) * Mathf.PI);
					yield return null;
				}
			} else {
				for (float j = 0; j < skill_detail.show_time / 2 / ahead_distance; j += Time.deltaTime) {
					mover.transform.position = _from + (_to - _from) * Mathf.Sin(0.5f * Mathf.PI * Mathf.Sin( 0.5f * Mathf.PI * (j / (skill_detail.show_time / 2 / ahead_distance))));
					yield return null;
				}
			}
			mover.transform.position = _to;
		}

		effect_Type_對手 = true;
		if (move_distance > 0) {
			GameObject attacker = find_target (transform.position).gmo;
			Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);
			foreach (Vector3 aimed_location in aimed_locations) {
				Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
				if (!in_attack_range (attacker, loc))continue;
				float dmg_rate = Mathf.Max (skill_detail.dmg_rate, aimed_location.z);
				if(the_character_on_block (loc) != null)the_character_on_block (loc).GetComponent<Click_Character> ().character.deal_dmg ( dmg_rate, damage_type, damage_property, attacker.GetComponent<Click_Character>().character);
				if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
				if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
			}
		}
	}

	IEnumerator machine_random_damage(){
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);

		for (int i = 0; i < 100; i++) {
			int r = Random.Range (0, aimed_locations.Length);
			Vector3 temp = aimed_locations [0];
			aimed_locations [0] = aimed_locations [r];
			aimed_locations [r] = temp;
		}

		int ink = 0;
		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			float dmg_rate = Mathf.Max (skill_detail.dmg_rate, aimed_location.z);
			if (the_character_on_block (loc) != null) {
				Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
				the_character_on_block (loc).GetComponent<Click_Character> ().character.deal_dmg ( dmg_rate, damage_type, damage_property, attacker.GetComponent<Click_Character>().character);
				if (++ink >= skill_detail.random_amount)break;
			}
			if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}
		yield return null;
	}

	IEnumerator machine_refill(){

		GameObject buffer = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);
		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = buffer.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (buffer, loc))continue;
			if (the_character_on_block (loc) != null) {
				the_character_on_block (loc).GetComponent<Click_Character> ().character.add_mp (skill_detail.refill_mp);
				the_character_on_block (loc).GetComponent<Click_Character> ().character.add_ep (skill_detail.refill_ep);
			}
			Instantiate (slash, buffer.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}

	}

	IEnumerator machine_Dance(){
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);

		if (skill_detail.only_slash_once)Instantiate (slash, attacker.transform.position, Quaternion.identity * Quaternion.Euler(0, 0, skill_detail.dir * -90));

		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			if(the_character_on_block (loc) != null)the_character_on_block (loc).GetComponent<Click_Character> ().character.deal_Dance_dmg (attacker.GetComponent<Click_Character>().character);
			if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}
		yield return null;
	}

	IEnumerator machine_拍擊(){
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);

		float init_rotation = 0;
		if (skill_detail.only_slash_once)Instantiate (slash, attacker.transform.position, Quaternion.identity * Quaternion.Euler(0, 0, init_rotation + skill_detail.dir * -90));

		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			float dmg_rate = Mathf.Max (skill_detail.dmg_rate, aimed_location.z);
			if (the_character_on_block (loc) != null) {
				the_character_on_block (loc).GetComponent<Click_Character> ().character.deal_dmg (dmg_rate, damage_type, damage_property, attacker.GetComponent<Click_Character> ().character);
				if (aimed_location == new Vector3 (1, 0, 0)) {
					Character target = the_character_on_block (loc).GetComponent<Click_Character> ().character;
					GameObject target_shines = (target.character_camp == Character_camp.allies) ? gameController.allies_shines [target.order] : gameController.enemys_shines [target.order];
					foreach (Transform child in target_shines.transform) {
						if (gameController.the_skill_in_frame (child) != null) {
							Destroy (gameController.the_skill_in_frame (child));
							break;
						}
					}
				}
			}

			if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}
		yield return null;
	}


	/////////////////////////// magic ///////////////////////////


	IEnumerator magic_pure_damage(){
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);

		float init_rotation = 0;
		if (skill_detail.only_slash_once)Instantiate (slash, attacker.transform.position, Quaternion.identity * Quaternion.Euler(0, 0, skill_detail.dir * -90));

		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			float dmg_rate = Mathf.Max (skill_detail.dmg_rate, aimed_location.z);
			if(the_character_on_block (loc) != null) the_character_on_block (loc).GetComponent<Click_Character> ().character.deal_dmg ( dmg_rate, damage_type, damage_property, attacker.GetComponent<Click_Character>().character);
			if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}
		yield return null;
	}

	IEnumerator magic_effect_damage(){
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);

		if (skill_detail.only_slash_once)Instantiate (slash, attacker.transform.position, Quaternion.identity * Quaternion.Euler(0, 0, skill_detail.dir * -90));

		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			float dmg_rate = Mathf.Max (skill_detail.dmg_rate, aimed_location.z);
			if (the_character_on_block (loc) != null) {
				the_character_on_block (loc).GetComponent<Click_Character> ().character.deal_dmg (dmg_rate, damage_type, damage_property, attacker.GetComponent<Click_Character>().character);
				the_character_on_block (loc).GetComponent<Click_Character> ().character.buffs.Add (new Buff(skill_detail.buff_Type, skill_detail.para1, skill_detail.turn));
			}
			if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}
		yield return null;
	}

	IEnumerator magic_knockback(){
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);

		if (skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position, Quaternion.identity * Quaternion.Euler(0, 0, skill_detail.dir * -90));
		yield return new WaitForSeconds(0.1f);

		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			float dmg_rate = Mathf.Max (skill_detail.dmg_rate, aimed_location.z);
			if (the_character_on_block (loc) != null) {
				GameObject mover = the_character_on_block (loc);
				mover.GetComponent<Click_Character> ().character.deal_dmg (dmg_rate, damage_type, damage_property, attacker.GetComponent<Click_Character>().character);
				Vector3 _from = mover.transform.position;
				Vector3 _to = _from + move_adjust(mover.GetComponent<Click_Character>().character, decode_dir ());
				if(_from == _to){
					mover.GetComponent<Click_Character> ().character.deal_dmg ( skill_detail.hit_wall_deal_damage, damage_type, damage_property, attacker.GetComponent<Click_Character>().character);
					for (float j = 0; j < skill_detail.show_time; j += Time.deltaTime) {
						mover.transform.position = _from + decode_dir () * gameController.acts.bounce_time * Mathf.Sin(j / skill_detail.show_time * Mathf.PI);
						yield return null;
					}
					mover.transform.position = _to;
					//cancel_skill
					if (skill_detail.hit_wall_cancel_skill) {
						Character target = mover.GetComponent<Click_Character> ().character;
						GameObject target_shines = (target.character_camp == Character_camp.allies) ? gameController.allies_shines [target.order] : gameController.enemys_shines [target.order];
						foreach (Transform child in target_shines.transform) {
							if (gameController.the_skill_in_frame (child) != null) {
								Destroy (gameController.the_skill_in_frame (child));
								break;
							}
						}
					}
				} else {
					for (float j = 0; j < skill_detail.show_time; j += Time.deltaTime) {
						mover.transform.position = _from + (_to - _from) * Mathf.Sin(0.5f * Mathf.PI * Mathf.Sin( 0.5f * Mathf.PI * (j / skill_detail.show_time)));
						yield return null;
					}
					mover.transform.position = _to;
				}
			}
			if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}
	}

	IEnumerator magic_危險感應(){
		GameObject attacker = find_target (transform.position).gmo;
		Character target = attacker.GetComponent<Click_Character> ().character;
		target.add_mp (skill_detail.refill_mp);
		target.add_ep (skill_detail.refill_ep);
		Debug.Log (target.this_turn_damage);
		if (target.this_turn_damage > 0) {
			bool flag = false;
			Debug.Log ("skill");
			GameObject target_shines = (target.character_camp == Character_camp.allies) ? gameController.allies_shines [target.order] : gameController.enemys_shines [target.order];
			foreach (Transform child in target_shines.transform) {
				if (gameController.the_skill_in_frame (child) != null) {
					if (flag) {
						Destroy (gameController.the_skill_in_frame (child));
						break;
					} else flag = true;
				}
			}
		} else target.buffs.Add (new Buff(Buff_Type.感應, 0, 2));
		yield return null;
	}

	IEnumerator magic_石療(){
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);

		if (skill_detail.only_slash_once)Instantiate (slash, attacker.transform.position, Quaternion.identity * Quaternion.Euler(0, 0, skill_detail.dir * -90));

		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			if (the_character_on_block (loc) != null) {
				Destroy (the_character_on_block (loc));
				StartCoroutine(attacker.GetComponent<Click_Character> ().character.add_hp (skill_detail.refill_hp));
			}
			if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}
		yield return null;
	}

	/////////////////////////// mix ///////////////////////////


	IEnumerator mix_pure_damage(){
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);

		float init_rotation = (gameObject.name == "貫風矢") ? 45 : 0;
		if (skill_detail.only_slash_once)Instantiate (slash, attacker.transform.position, Quaternion.identity * Quaternion.Euler(0, 0, init_rotation + skill_detail.dir * -90));

		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			float dmg_rate = Mathf.Max (skill_detail.dmg_rate, aimed_location.z);
			if (the_character_on_block (loc) != null) {
				if (the_character_on_block (loc).GetComponent<Click_Character> ().character.buffs.has_buff (Buff_Type.標記))dmg_rate += 20;
				the_character_on_block (loc).GetComponent<Click_Character> ().character.deal_dmg ( dmg_rate, damage_type, damage_property, attacker.GetComponent<Click_Character>().character);
			}
			if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			if (skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}
		yield return null;
	}

	IEnumerator mix_buff_damage(){
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);

		if (skill_detail.only_slash_once)Instantiate (slash, attacker.transform.position, Quaternion.identity * Quaternion.Euler(0, 0, skill_detail.dir * -90));

		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			float dmg_rate = Mathf.Max (skill_detail.dmg_rate, aimed_location.z);
			if (the_character_on_block (loc) != null) {
				the_character_on_block (loc).GetComponent<Click_Character> ().character.deal_dmg (dmg_rate, damage_type, damage_property, attacker.GetComponent<Click_Character>().character);
				attacker.GetComponent<Click_Character> ().character.buffs.Add (new Buff(skill_detail.buff_Type, skill_detail.para1, skill_detail.turn));
			}
			if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance, Quaternion.identity);
			if(skill_detail.attack_delay > 0 )yield return new WaitForSeconds(skill_detail.attack_delay);
		}
		yield return null;
	}

	IEnumerator mix_影穿(){
		GameObject attacker = find_target (transform.position).gmo;
		Vector3[] aimed_locations = turn_attack_range(skill_detail.dir);

		Vector3 target = new Vector3 ();
		foreach (Vector3 aimed_location in aimed_locations) {
			Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
			if (!in_attack_range (attacker, loc))continue;
			target = aimed_location;
			break;
		}

		Debug.Log (target);

		Vector3 _to = new Vector3();
		Vector3 will_move_to = new Vector3 ();

		if (target == new Vector3 ()) {
			_to = attacker.transform.position;
			foreach (Vector3 aimed_location in aimed_locations) {
				Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + aimed_location;
				if (loc.x < 0 || loc.x > 6 || loc.y < 0 || loc.y > 4)continue;
				attacker.GetComponent<Click_Character> ().character.loc += aimed_location;
				_to = attacker.transform.position + aimed_location * gameController.acts.board_blocks_distance;
				break;
			}
		} else {
			int[] temp = new int[4]{ 0, 1, 3, 2 };
			for (int i = 0; i < 4; i++) {
				skill_detail.dir += temp[i];
				Vector3 loc = attacker.GetComponent<Click_Character> ().character.loc + target + decode_dir ();
				if (the_character_on_block (loc) == null) {
					_to = attacker.transform.position + (target + decode_dir ()) * gameController.acts.board_blocks_distance;
					will_move_to = target + decode_dir ();
					skill_detail.dir -= temp[i];
					break;
				}
				skill_detail.dir -= temp[i];
			}

			Vector3 Loc = attacker.GetComponent<Click_Character> ().character.loc + target;
			Character defender = the_character_on_block (Loc).GetComponent<Click_Character> ().character;
			if (defender.character_camp == Character_camp.obstacles) {
				//				int damage = (int)Mathf.Max (skill_detail.damage, defender.hp / 2);
				//				StartCoroutine(defender.add_hp ( -damage, skill_detail.clas, attacker));
				//				if (!skill_detail.only_slash_once) Instantiate (slash, attacker.transform.position + target * gameController.acts.board_blocks_distance, Quaternion.identity);
			}

			attacker.GetComponent<Click_Character> ().character.loc += will_move_to;
		}


		attacker.transform.position = new Vector3 (100, 100, 0);
		yield return new WaitForSeconds (0.35f);
		attacker.transform.position = _to;
		yield return null;
	}
}
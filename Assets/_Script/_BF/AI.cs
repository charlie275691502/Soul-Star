using UnityEngine;
using System.Collections;

[System.Serializable]
public class AI_Skill{
	public GameObject skill;
	public int init_amount;
	[HideInInspector] public int amount;
}

public class Priority_dir{
	public int[] dir;
}

[System.Serializable]
public class History_loc{
	public Vector3[] locs;
}

public class AI : MonoBehaviour {

	public string map_color;
	[HideInInspector] public GameController gameController;
	[HideInInspector] public Character enemy;
	[HideInInspector] public GameObject[] skills;
	[HideInInspector] public int[] dir;
	[HideInInspector] public Priority_dir[] priority_dir;

	[HideInInspector] public Vector3 now_loc;
	[HideInInspector] public bool[] can_use;
	public AI_Skill[] ai_skills;
	[HideInInspector] public Character nearest_allies;
	[HideInInspector] public Character furthest_allies;
	[HideInInspector] public Vector3 nearest_allies_vec;
	[HideInInspector] public Vector3 furthest_allies_vec;


	void Start(){
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
	}

	public virtual void specific_AI(){
		
	}

	public void _AI(Vector3[] pos){

		if (!gameController.editor_setting.enable_ai)return;
		skills = new GameObject[5];
		dir = new int[5];
		can_use= new bool[5]{true, true, true, true, true};
		priority_dir = new Priority_dir[5];
		for (int i = 0; i < ai_skills.Length; i++)ai_skills [i].amount = ai_skills [i].init_amount;
		Debug.Log(ai_skills[0].amount);
		set_up_nearest_allies ();
		set_up_furthest_allies ();

		specific_AI ();
//		enemy.gmo.GetComponent("AI_" + enemy.index.ToString()).SendMessage("specific_AI", enemy);

//		check dir here

//		for(int i=0;i<5;i++)dir [i] = priority_dir [i].dir [0];

		Vector3 now_loc = enemy.loc;
		for (int i = 0; i < 5; i++) {
			if (priority_dir[i] == null || skills[i] == null) continue;
//			if (skills [i].name == "Shadow Attack") {
//				dir [i] = priority_dir [i].dir [0];
//				//我不想管這段了ＱＱ
//				continue;
//			}

			dir [i] = priority_dir [i].dir [0]; // has bug when ahead_distance > 1
			for (int j = 0; j < 4; j++) {
				int direction = priority_dir [i].dir [j];
				Click_skill skill = skills [i].GetComponent<Click_skill> ();
				//move
				if (skill.ahead_distance > 0) {
					Vector3 des_loc = now_loc + skill.ahead_distance * decode_dir (direction);
					if (skill.Name == "Thrust") {
						Vector3 second_loc = des_loc + decode_dir (direction);
						if (!skill.is_obstacle(des_loc) /* && no_enemys(des_loc, i, enemy.order) */ && !skill.is_obstacle(second_loc) /* && no_enemys(second_loc, i, enemy.order) */ ) {
							now_loc = des_loc;
							dir [i] = direction;
							break;
						}
					} 
					else {
						if (!skill.is_obstacle(des_loc) /* && no_enemys(des_loc, i, enemy.order) */) {
							now_loc = des_loc;
							dir [i] = direction;
							break;
						}
					}
				}

				//not move
				else {
					//GameObject attacker = find_target (transform.position).gmo;
					Vector3[] aimed_locations = skill.turn_attack_range(direction);

					bool all_miss = true;
					foreach (Vector3 aimed_location in aimed_locations) {
						Vector3 loc = now_loc + aimed_location;
						if (skill.in_attack_range (enemy.gmo, loc))all_miss = false;
					}

					if (!all_miss) {
						dir [i] = direction;
						break;
					}
				}
			}
		}

		if (gameController.editor_setting.print_ai_decision) {
			Debug.Log (enemy.Name + "\'s ai decision");
			for (int i = 0; i < 5; i++) {
				string s = (skills[i] == null) ? "no skill" : skills [i].GetComponent<Click_skill> ().Name;
				s += (priority_dir [i] == null) ? " no priority_dir" : (" " + priority_dir[i].dir[0].ToString() + " " + priority_dir[i].dir[1].ToString() + " " + priority_dir[i].dir[2].ToString() + " " + priority_dir[i].dir[3].ToString() + " ");
				Debug.Log (s);
			}
		}

		Transform skiller = gameController.enemys_shines[enemy.order].transform;

		for (int i = 0; i < 5; i++) {
			if (skills [i] == null) continue;
			Transform shine_transform = skiller.GetChild (i);
			skills[i].transform.parent = gameController.used_skill;
			skills[i].transform.position = (pos == null) ? shine_transform.position : pos[i];
			GameObject node = skills [i];
			while ((node = find_down(node)) != null) node.SetActive (true);
		}


		for (int i = 0; i < 5; i++) {
			if (skills [i] == null)continue;
			Transform shine_transform = skiller.GetChild (i);
			GameObject skill = (pos == null) ? gameController.the_skill_in_frame (shine_transform) : gameController.the_skill_in_frame (pos[i]);

			if(pos == null)skill.GetComponent<Click_skill>().change_skill_sortingLayer ("on_tactic");
			skill.GetComponent<Click_skill> ().skill_detail.dir = dir [i];

			skill.transform.Find("rotated_Image").rotation *= Quaternion.Euler(0, 0, -90 * dir [i]); 

//			GameObject node = skill;
//			if ((node = find_down(node)) != null) {
//				node.transform.Find("rotated_Image").rotation *= Quaternion.Euler (0, 0, 90 * dir [i]);
//				for(int j=0;j<dir [i];j++)node.transform.localPosition = new Vector3(-node.transform.localPosition.y, node.transform.localPosition.x, 0); 
//			}
		}

	}



	//count the player distance

	//5 A 5 M 

	//AI_move = distance * 2 / 3

	// 1/2 forward 1/2 backward

	//while backward, random dir

	//while forward, 2/3 in y, 1/3 in x

	//rest of the steps, 2/3 attack, 1/3 move

	//while attack, (converse attack) dir

	//while move, totally random

	public void set_up_nearest_allies(){
		GameObject[] character_gmos = GameObject.FindGameObjectsWithTag ("Character");
		float distance = 99999;
		int dis_x = 0, dis_y = 0;
		foreach(GameObject character_gmo in character_gmos){
			Character character = character_gmo.GetComponent<Click_Character> ().character;
			if (character.character_camp == Character_camp.enemys || character.character_camp == Character_camp.obstacles)continue;
			if (distance > Mathf.Abs (character.loc.x - enemy.loc.x) + Mathf.Abs (character.loc.y - enemy.loc.y)) {
				dis_x = (int)(character.loc.x - enemy.loc.x);
				dis_y = (int)(character.loc.y - enemy.loc.y);
				distance = Mathf.Abs (character.loc.x - enemy.loc.x) + Mathf.Abs (character.loc.y - enemy.loc.y);
				nearest_allies_vec = new Vector3 (dis_x, dis_y, distance);
				nearest_allies = character;
			}
		}
	}

	public void set_up_furthest_allies(){
		GameObject[] character_gmos = GameObject.FindGameObjectsWithTag ("Character");
		float distance = 0;
		int dis_x = 0, dis_y = 0;
		foreach(GameObject character_gmo in character_gmos){
			Character character = character_gmo.GetComponent<Click_Character> ().character;
			if (character.character_camp == Character_camp.enemys || character.character_camp == Character_camp.obstacles)continue;
			if (distance < Mathf.Abs (character.loc.x - enemy.loc.x) + Mathf.Abs (character.loc.y - enemy.loc.y)) {
				dis_x = (int)(character.loc.x - enemy.loc.x);
				dis_y = (int)(character.loc.y - enemy.loc.y);
				distance = Mathf.Abs (character.loc.x - enemy.loc.x) + Mathf.Abs (character.loc.y - enemy.loc.y);
				furthest_allies_vec = new Vector3 (dis_x, dis_y, distance);
				furthest_allies = character;
			}
		}
	}


	public GameObject find_down(GameObject gmo){
		foreach (Transform child in gmo.transform)if (child.tag == "Skill")return child.gameObject;
		return null;
	}

	public bool random(float x){
		return Random.Range(0.0f, 1.0f) < x;
	}

	// general_functions start from this line ---------------------------------------------------------------------
	public Priority_dir random_dir(){ 
		return random_dir ( new int [4] {0,1,2,3});
	}

	public Priority_dir random_dir(int posible_dir, int second_posible_dir, int third_posible_dir){
		return random_dir ( new int [1] {posible_dir}, new int [1] {second_posible_dir}, new int [1] {third_posible_dir});
	}

	public Priority_dir random_dir(int posible_dir){ 
		return random_dir ( new int [1] {posible_dir});
	}

	public Priority_dir random_dir(int[] posible_dir){ 
		return random_dir (posible_dir, new int[0], new int [0]);
	}

	public Priority_dir random_dir(int posible_dir, int[] second_posible_dir){
		return random_dir( new int[1] {posible_dir}, second_posible_dir, new int [0]);
	}

	public Priority_dir random_dir(int[] posible_dir, int second_posible_dir){
		return random_dir( posible_dir, new int[1] {second_posible_dir}, new int [0]);
	}

	public Priority_dir random_dir(int posible_dir, int second_posible_dir){
		return random_dir( new int[1] {posible_dir}, new int[1] {second_posible_dir}, new int [0]);
	}

	//不能重複
	public Priority_dir random_dir(int[] posible_dir, int[] second_posible_dir, int[] third_posible_dir){
		bool[] used = new bool[4];

		int length = posible_dir.Length + second_posible_dir.Length + third_posible_dir.Length;
		int[] random_array = new int[4];
		for (int i = 0; i < 4; i++)random_array [i] = -1;
		for (int i = 0; i < posible_dir.Length;) {
			used [posible_dir [i]] = true;
			int random = Random.Range (0, posible_dir.Length);
			if(random_array[random] == -1)random_array[random] = posible_dir[i++];
		}
		for (int i = 0; i < second_posible_dir.Length;) {
			used [second_posible_dir [i]] = true;
			int random = Random.Range (posible_dir.Length, length);
			if(random_array[random] == -1)random_array[random] = second_posible_dir[i++];
		}
		for (int i = 0; i < third_posible_dir.Length;) {
			used [third_posible_dir [i]] = true;
			int random = Random.Range (second_posible_dir.Length, length);
			if(random_array[random] == -1)random_array[random] = third_posible_dir[i++];
		}
		for (int i = 0; i < 4;) {
			if (used [i]) {
				i++;
				continue;
			}
			int random = Random.Range (length, 4);
			if(random_array[random] == -1)random_array[random] = i++;
		}
		Priority_dir priority_dir = new Priority_dir ();
		priority_dir.dir = random_array;
		return priority_dir;
	}

	public Vector3 vec_update(Vector3 vec,int move_dis, int move_dir){
		vec += decode_dir (move_dir) * move_dis;
		return new Vector3 (vec.x, vec.y, Mathf.Abs(vec.x)+Mathf.Abs(vec.y));
	}

	//正比性隨機
	public Priority_dir direct_ration_random(float x, float y){
		return direct_ration_random (x, y, 0);
	}

	public Priority_dir direct_ration_random(float x, float y, float n){
		int a = (x > 0) ? 0 : 2;
		int b = (y > 0) ? 3 : 1;
		if (random (n)) {
			a = 2 - a;
			b = 4 - b;
		}
		x = Mathf.Abs (x) + 1;
		y = Mathf.Abs (y) + 1;
		//find the other 2 directions which isn't x neither y

		if (x == 1) return random (y / (y + 1)) ? random_dir(a) : random_dir (new int[2] { (b + 1) % 4, (b + 3) % 4});
		else if (y == 1) return random (x / (x + 1)) ? random_dir(b) : random_dir (new int[2] { (a + 1) % 4, (a + 3) % 4});
		return random_dir(random (x / (x + y)) ? a : b);
	}

	public Priority_dir reverse_ration_random(float x, float y){
		return reverse_ration_random(x, y, 0);
	}

	public Priority_dir reverse_ration_random(float x, float y, float n){
		int a = (x > 0) ? 0 : 2;
		int b = (y > 0) ? 3 : 1;
		if (random (n)) {
			a = 2 - a;
			b = 4 - b;
		}
		x = Mathf.Abs (x) + 1;
		y = Mathf.Abs (y) + 1;
		//find the other 2 directions which isn't x neither y

		if (x == 1) return random (1 / (y + 1)) ? random_dir(b) : random_dir (new int[2] { (b + 1) % 4, (b + 3) % 4});
		else if (y == 1) return random (1 / (x + 1)) ? random_dir(a) : random_dir (new int[2] { (a + 1) % 4, (a + 3) % 4});
		return random_dir(random (y / (x + y)) ? a : b);
	}

	public Priority_dir relative_random_dir(int dir, float A, float R, float B, float L){
		int a = dir;
		int r = (dir + 1) % 4;
		int b = (dir + 2) % 4;
		int l = (dir + 3) % 4;

		if (random (A / (A + R + B + L))) {
			if (R == 0 && B == 0 && L == 0)return random_dir (a);
			if (random (R / (R + B + L))) return (B == 0 && L == 0) ? random_dir(a, r) : random (B / (B + L)) ? random_dir (a, r, b) : random_dir (a, r, l);
			if (random (B / (R + B + L))) return (R == 0 && L == 0) ? random_dir(a, b) : random (R / (R + L)) ? random_dir (a, b, r) : random_dir (a, b, l);
			if (random (L / (R + B + L))) return (R == 0 && B == 0) ? random_dir(a, l) : random (R / (R + B)) ? random_dir (a, l, r) : random_dir (a, l, b);
		}
		if (random (R / (R + B + L))) {
			if (A == 0 && B == 0 && L == 0)return random_dir (r);
			if (random (A / (A + B + L))) return (B == 0 && L == 0) ? random_dir(r, a) : random (B / (B + L)) ? random_dir (r, a, b) : random_dir (r, a, l);
			if (random (B / (A + B + L))) return (A == 0 && L == 0) ? random_dir(r, b) : random (A / (A + L)) ? random_dir (r, b, a) : random_dir (r, b, l);
			if (random (L / (A + B + L))) return (A == 0 && B == 0) ? random_dir(r, l) : random (A / (A + B)) ? random_dir (r, l, a) : random_dir (r, l, b);
		}
		if (random (B / (B + L))) {
			if (A == 0 && R == 0 && L == 0)return random_dir (b);
			if (random (A / (A + R + L))) return (R == 0 && L == 0) ? random_dir(b, a) : random (R / (R + L)) ? random_dir (b, a, r) : random_dir (b, a, l);
			if (random (R / (A + R + L))) return (A == 0 && L == 0) ? random_dir(b, r) : random (A / (A + L)) ? random_dir (b, r, a) : random_dir (b, r, l);
			if (random (L / (A + R + L))) return (A == 0 && R == 0) ? random_dir(b, l) : random (A / (A + R)) ? random_dir (b, l, a) : random_dir (b, l, l);
		} 
		if (L == 0) return relative_random_dir(dir, A, R, B, L);
		if (random (L / L)) {
			if (A == 0 && R == 0 && B == 0)return random_dir (l);
			if (random (A / (A + R + B))) return (R == 0 && B == 0) ? random_dir(l, a) : random (R / (R + B)) ? random_dir (l, a, r) : random_dir (l, a, b);
			if (random (R / (A + R + B))) return (A == 0 && B == 0) ? random_dir(l, r) : random (A / (A + B)) ? random_dir (l, r, a) : random_dir (l, r, b);
			if (random (B / (A + R + B))) return (A == 0 && R == 0) ? random_dir(l, b) : random (A / (A + R)) ? random_dir (l, b, a) : random_dir (l, b, r);
		}

		return relative_random_dir(dir, A, R, B, L);
	}

	public int face_dir(Vector3 vec){
		return face_dir (new Vector2(vec.x, vec.y));
	}

	public int face_dir(Vector2 vec){
		if (Mathf.Abs (vec.x) > Mathf.Abs (vec.y) || (Mathf.Abs (vec.x) == Mathf.Abs (vec.y) && random (0.5f))) return (vec.x > 0) ? 0 : 2;
		else  return (vec.y > 0) ? 3 : 1;
	}

	public int face_oblique_dir(Vector3 vec){
		return face_oblique_dir (new Vector2(vec.x, vec.y));
	}

	public int face_oblique_dir(Vector2 vec){
		if (vec.x >= 0 && vec.y >= 0)return 0;
		if (vec.x >= 0 && vec.y <  0)return 1;
		if (vec.x <  0 && vec.y <  0)return 2;
		if (vec.x <  0 && vec.y >= 0)return 3;
		return -1;
	}

	public bool has_skill(string name){
		for(int i=0;i<ai_skills.Length;i++)if(ai_skills[i].skill.GetComponent<Click_skill>().Name == name && ai_skills[i].amount > 0)return true;
		return false;
	}

	public int amount_of_skill(string name){
		for(int i=0;i<ai_skills.Length;i++)if(ai_skills[i].skill.GetComponent<Click_skill>().Name == name)return ai_skills[i].amount;
		return 0;
	}

	public void set_skill(string name, int order){
		AI_Skill ai_skill = null;
		for(int i=0;i<ai_skills.Length;i++)if(ai_skills[i].skill.GetComponent<Click_skill>().Name == name)ai_skill = ai_skills[i];
		if (ai_skill == null)Debug.Log ("cant find " + name);
		ai_skill.amount--;

		dismantle_skill((GameObject)Instantiate (ai_skill.skill, new Vector3 (), Quaternion.identity), order);
	}

	public void set_random_skill(int order){
		set_random_skill ("", order);
	}

	public void set_random_skill(string except, int order){
		AI_Skill[] aI_skill = get_random_skills (1, except);
		set_skill (aI_skill [0].skill.GetComponent<Click_skill>().Name, order);
	}

	public void set_random_skill(string[] excepts, int order){
		AI_Skill[] aI_skill = get_random_skills (1, excepts);
		set_skill (aI_skill [0].skill.GetComponent<Click_skill>().Name, order);
	}

	public void dismantle_skill(GameObject node, int order){
		if (node == null)return;
		dismantle_skill (find_down (node), order - 1);
		can_use [order] = false;
		skills [order] = (GameObject)Instantiate(node, new Vector3(), Quaternion.identity);
		int ink = 0;
		while(skills [order].name[ink] != '(')ink++;
		skills [order].name = skills [order].name.Substring (0, ink);
		Destroy (node);
		return;
	}

	public AI_Skill[] get_random_skills(int x){
		return get_random_skills (x, "");
	}

	public AI_Skill[] get_random_skills(int x, string except){
		return get_random_skills (x, new string[1] {except});
	}

	public AI_Skill[] get_random_skills(int x, string[] excepts){
		AI_Skill[] ret = new AI_Skill[x];
		int ink = 0;

		//count left skill amount
		int n = 0;
		int[] skill_amount = new int[ai_skills.Length];
//		string s = "";
		for (int i = 0; i < ai_skills.Length; i++) {
			n += ai_skills [i].amount;
			skill_amount [i] = ai_skills [i].amount;
//			s += ai_skills [i].amount.ToString () + " ";
		}

		for (int i = 0; i < x; i++) {
			int random = Random.Range (0, n);
//			Debug.Log (s +random.ToString());
			for (int j = 0; j < ai_skills.Length; j++) {
				random -= skill_amount [j];
				if (random < 0) {
					bool breakable = false;
					foreach (string except in excepts) if (ai_skills [j].skill.GetComponent<Click_skill>().Name == except) breakable = true;
					if (breakable) {
						i--;
						break;
					}

					ret [ink++] = ai_skills[j];
					skill_amount [j]--;
					n--;
					break;
				}
			}
		}
		return ret;
	}

	public int get_random_position(int min, int max){
		return get_random_position (min, max, 1);
	}

	//from 0 to 4 [min, max]
	public int get_random_position(int min, int max, int length){
		int[] posibles = new int[5];
		int ink = 0;
		int ans = 0;
		for (int i = 0; i < 5; i++) {
			ans = (can_use [i]) ? ans + 1 : 0;
			if (ans >= length && min <= i && i <= max)posibles [ink++] = i;
		}
		return posibles[Random.Range (0, ink)];
	}

	public void your_mom_so_fat(){
		// empty_statement
	}

	public char find_map_color(){
		return (map_color == "") ? 'A' : map_color[(int)enemy.loc.y * 7 + (int)enemy.loc.x];
	}

	public bool is_allies_have_buff(Buff_Type buff_type){
		return  !((gameController.allies.Length < 1 || !gameController.allies [0].buffs.has_buff (buff_type)) &&
			(gameController.allies.Length < 2 || !gameController.allies [1].buffs.has_buff (buff_type)) &&
			(gameController.allies.Length < 3 || !gameController.allies [2].buffs.has_buff (buff_type)));
	}

	Vector3 decode_dir(int dir){
		if (dir % 4 == 0)return new Vector3 (1, 0, 0);
		if (dir % 4 == 1)return new Vector3 (0, -1, 0);
		if (dir % 4 == 2)return new Vector3 (-1, 0, 0);
		if (dir % 4 == 3)return new Vector3 (0, 1, 0);
		return new Vector3();
	}
}
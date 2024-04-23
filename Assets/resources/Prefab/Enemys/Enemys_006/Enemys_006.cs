using UnityEngine;
using System.Collections;

public class Enemys_006 : MonoBehaviour {
	private AI aI;
	[HideInInspector] public Vector3 now_loc;
	[HideInInspector] public Vector3 vec;
	[HideInInspector] public Character enemy;
	[HideInInspector] public bool[] can_use;
	public AI_Skill[] ai_skills;
	private Character nearest_character;

	// Use this for initialization
	void Start () {
		aI = GameObject.FindGameObjectWithTag ("AI").GetComponent<AI> ();
	}

	void specific_AI(Character enemy_local){
		Click_skill click_skill = new Click_skill ();
		enemy = enemy_local;
		can_use = new bool[5]{true, true, true, true, true};
		for (int i = 0; i < ai_skills.Length; i++)ai_skills [i].amount = ai_skills [i].init_amount;
		now_loc = enemy.loc;
		aim_the_nearest_character();

		//沒有人受到標記&&自己非感應狀態時
		if (!is_allies_have_buff (Buff_Type.標記) && !enemy.buffs.has_buff (Buff_Type.感應)) {

			//If(EP>=20) 或10<=EP<20有30%機率
			if (enemy.ep >= 20 || (10 <= enemy.ep && enemy.ep < 20) && random (0.3f)) {
				//在第1~4(若與最接近敵人距離>=4則在第3~4)個行動隨機一格擺上Shadow Attack(機率加權為1:3:3:3)
				int random_position;
				if (vec.z >= 4)
					random_position = get_random_position (3, 4);
				else {
					if (random (0.1f))
						random_position = get_random_position (1, 1);
					else
						random_position = get_random_position (2, 4);
				}
				set_skill ("Shadow Attack", random_position);

				//方向向最接近敵人(不能是貼身的)距離較遠的方向，若兩個座標差相同則隨機選一個方向
				aim_the_nearest_character (true); //不貼身
				aI.priority_dir [random_position] = random_dir (face_dir ());

				//若第一次決定的Shadow Attack不適合發動(第三格為障礙物 || 敵人貼身)，且AI不是站在藍色格上
				if ((click_skill.is_obstacle(enemy.loc + decode_dir(aI.priority_dir [random_position].dir[0]) * 3) || vec.z == 1) && find_map_color() != 'B') {
					//清場
					destroy_all_skill();

					//在第1~2格隨機擇一放上Forward
					random_position = get_random_position (1, 2);
					set_skill ("Forward", random_position);

					//方向(3:2:4:1/5:5:8:1/不討論/3:3:3:1)
					if(find_map_color() == 'R')aI.priority_dir[random_position] = relative_random_dir(0, 3, 2, 4, 1);
					if(find_map_color() == 'Y')aI.priority_dir[random_position] = relative_random_dir(0, 5, 5, 8, 1);
					if(find_map_color() == 'G')aI.priority_dir[random_position] = relative_random_dir(0, 3, 3, 3, 1);

					//然後(更新與最接近對手距離)，在第三或四個行動隨機擇一放上Shadow Attack
					vec += decode_dir(aI.priority_dir[random_position].dir[0]);
					random_position = get_random_position (3, 4);
					set_skill ("Forward", random_position);

					//方向向最接近敵人(不是貼身的)距離較遠的方向(第三格不能剛好撞牆)(若新的x.diff==y.diff要換方向試，不一樣就不用了)
					aI.priority_dir [random_position] = random_dir (face_dir ());
				}

			}

			//若版面上已經有Shadow Attack了，且還沒有Forward
			if (!has_skill("Shadow Attack") && has_skill("Forward")) {
				//則隨機在Shadow Attack後的某一格擺上Forward
				int random_position = get_random_position (find_order("Shadow Attack") + 2, 5);
				set_skill ("Forward", random_position);

				//方向(1:1:1:1/3:2:7:1/3:8:2:1/5:2:5:3)
				if(find_map_color() == 'R')aI.priority_dir[random_position] = relative_random_dir(0, 1, 1, 1, 1);
				if(find_map_color() == 'Y')aI.priority_dir[random_position] = relative_random_dir(0, 3, 2, 7, 2);
				if(find_map_color() == 'B')aI.priority_dir[random_position] = relative_random_dir(0, 3, 8, 2, 1);
				if(find_map_color() == 'G')aI.priority_dir[random_position] = relative_random_dir(0, 5, 2, 5, 3);
			}

			//若版面上有Shadow Attack
			if (!has_skill("Shadow Attack")) {
				//在Shadow Attack後面有40%機率使用感應(隨機選擇一有效格，沒有效格就算了)
				int random_position;
				if (random (0.4f)) {
					random_position = get_random_position (find_order("Shadow Attack") + 2, 5);
					if (random_position != -1) {
						set_skill ("Danger Sensor", random_position);
						aI.priority_dir [random_position] = random_dir (0);
					}
				}

				//隨機一格放上氣刃
				random_position = get_random_position (0, 5);
				set_skill ("Shock wave", random_position);

				//方向向最接近敵人的方向，50%機率選擇反向(若敵人在直線上則障礙物較少的方向，不能有兩格被障礙物擋住)
				aI.priority_dir [random_position] = random_dir (face_dir());
				if(random(0.5f))for(int i=0;i<4;i++)aI.priority_dir [random_position].dir[0] = (aI.priority_dir [random_position].dir[0]+2)%4;

				//剩下放Up Thrust
				for (int i = 0; i < 5; i++) {
					if (!can_use [i])continue;
					set_skill ("Up Thrust", i);
					//Up Thrust方向由座標差作調整，距離較遠的係數+3，較短的+1(如x.diff=3, y.diff=2，則方向為x:y=3+3:2+1=6:3)，50%打反方向 //this is broken
					aI.priority_dir [random_position] = random_dir (face_dir());
				}

			}

			//若版面上沒有Shadow Attack
			if (has_skill("Shadow Attack")) {
				//先清版。
				destroy_all_skill();

				//若EP>21
				if (enemy.ep > 21) {

					//在第3~4格隨機擇一擺上氣刃(若距離<3，則是第2~4格)

					//方向向最接近敵人的方向(若敵人在直線上則障礙物較少的方向，不能有兩格被障礙物擋住)

					//在氣刃後一格放上影子飛劍

					//方向為與最接近敵人之兩軸距離成反比(兩方向加權各+1)

					//剩下的就隨機放上Up Thrust和Forward

					//Forward方向(2:1:2:1/2:3:7:1/3:5:3:1/1:1:1:0)

					//Up Thrust方向與兩軸成正比，距離較遠的係數+3，較短的+1
				}

				//若(EP<10) 或10<=EP<20有70%機率
				if (enemy.ep < 10 || (10 <= enemy.ep && enemy.ep < 20 && random(0.7f))) {

					//與最接近敵人(x.diff+y.diff)/4機率(距離>=4機率就是100%了): 在第2~4格隨機一格放上感應，加權(7:2:1)

					//在感應前隨機一格放上Forward

					//方向背向最接近敵人，與兩軸距離成反比，數字較大的加權+5，另一邊+2，若相同則1:1

					//隨機一格放上氣刃

					//方向向最接近敵人的方向(若敵人在直線上則障礙物較少的方向，不能有兩格被障礙物擋住)

					//剩下的格子放上Up Thrust

					//Up Thrust方向與兩軸成正比，距離較遠的係數+3，較短的+1
				}

				else {

					//隨機一格填上Forward

					//方向(2:1:2:1/2:3:7:1/3:5:3:1/1:1:1:0)

					//隨機一格放上氣刃

					//方向向最接近敵人的方向(若敵人在直線上則障礙物較少的方向，不能有兩格被障礙物擋住)

					//隨機兩格放上Up Thrust

					//Up Thrust方向與兩軸成正比，距離較遠的係數+3，較短的+1
				}
				//end of 沒有人受到標記&&自己非感應狀態時

				//有敵人受到標記&&自己非感應狀態
				if (true) {

					//若(EP>=20) && 最接近敵人HP<40
					if (enemy.ep >= 20 && nearest_character.hp < 40) {

						//在第1~4(若與最接近敵人距離>=4則在第3~4)個行動隨機一格擺上Shadow Attack(機率加權為1:3:3:3)

						//方向向最接近敵人(不能是貼身的)距離較遠的方向，若兩個座標差相同則隨機選一個方向(第三格不能是障礙物)
					}

					//若第一次決定的Shadow Attack不適合發動(第三格為障礙物 || 敵人貼身)，且AI不是站在藍色格上
					if (true) {

						////在第1~2格隨機擇一放上Forward

						//方向(3:2:4:1/5:5:8:1/不討論/3:3:3:1)

						//然後(更新與最接近對手距離)，在第三或四個行動隨機擇一放上Shadow Attack

						//方向向最接近敵人(不是貼身的)距離較遠的方向(第三格不能剛好撞牆)(若新的x.diff==y.diff要換方向試，不一樣就不用了)
					}

					//若版面上有Shadow Attack
					if (!has_skill("Shadow Attack")) {

						//在Shadow Attack後一格接一影子飛劍(方向記得轉)

						//隨機選一格氣刃

						//方向向最接近敵人的方向(若敵人在直線上則障礙物較少的方向，不能有兩格被障礙物擋住)

						//隨機選一格Up Thrust

						//Up Thrust方向與兩軸成正比，距離較遠的係數+3，較短的+1

						//最後一格放Forward

						//方向(2:1:2:1/2:3:7:1/3:5:3:1/1:1:1:0)					
					}

					//若仍然沒有Shadow Attack
					if (has_skill("Shadow Attack")) {
						//清版
						destroy_all_skill();

						//第一格背向最接近敵人Forward

						//方向與座標差反比，較大數字加權+4，較小方向+2

						//隨機一格感應(若HP<=15，20%機率換成影子飛劍)

						//若為影子飛劍，方向向最接近敵人距離較遠之方向

						//隨機一格氣刃

						//方向向最接近敵人的方向(若敵人在直線上則障礙物較少的方向，不能有兩格被障礙物擋住)

						//隨機選一格Up Thrust

						//Up Thrust方向與兩軸成正比，距離較遠的係數+3，較短的+1
					}
					//end of 有敵人受到標記&&自己非感應狀態

					//自己為感應狀態時
					if (enemy.buffs.has_buff(Buff_Type.感應)) {

						//EP<10任何狀況， 或對方未被標記，有70%機率
						if (true) {

							//第一格使用潛行

							//方向為與敵人拉開距離，比較兩方向Forward距離較遠的優先(相同時距離近的座標優先)

							//更新與最接近敵人距離

							//第二格使用感應

							//隨機(1:1:1)在接下來三格放上Forward、氣刃、Up Thrust

							//氣刃方向為向最接近敵人的方向(若敵人在直線上則障礙物較少的方向，不能有兩格被障礙物擋住)

							//Forward方向為(2:1:2:1/2:3:7:1/3:5:3:1/1:1:1:0)

							//Up Thrust方向為與兩軸成正比，距離較遠的係數+3，較短的+1

						}

						//EP>20，有人被標記 有30%機率
						if (true) {
							//鎖定距離差為被標記的單位

							//若有座標距離差>=2，以潛行接近被標記對手(兩座標均>2時選擇較遠的)

							//若沒有的話使用Forward

							//方向向對手Forward，兩方向隨機

							//更新座標差

							//使用Shadow Attack

							//方向向距離較遠的方向(如果第三格是障礙物就換方向，再不行的話將這個技能改成氣刃)

							//氣刃方向為向最接近敵人的方向(若敵人在直線上則障礙物較少的方向，不能有兩格被障礙物擋住)

							//第三格使用影子飛劍，方向向距離較遠方向，如果有使用Shadow Attack要反向

							//最後兩格用Up Thrust

							//Up Thrust方向為與兩軸成正比，距離較遠的係數+3，較短的+1，如果有使用Shadow Attack要反向
						}

						//剩下的自己感應狀態&&對方有人被標記的其他狀況
						if (true) {
							//瞄準被標記的對手

							//第一格使用潛行

							//方向向距離差>=2的方向(若相同或均為1則隨機方向)，貼身則選擇垂直隨機方向，不必優先選最遠距離

							//更新座標差											

							//在第二格Forward

							//方向向最接近對手，與兩軸距離成正比，加權各+1

							//隨機在接下來的格子選一個用影子飛劍

							//方向向被標記的敵人，座標差較大的方向優先

							//最後兩格使用Up Thrust，方向徹底隨機(不要打牆)

						}

						//任何其他狀況
						else {
							//座標差鎖定最接近敵人

							//第一格使用潛行

							//方向向距離差>=2的方向(若相同或均為1則隨機方向)

							//更新座標差

							//隨機擇一格使用氣刃

							//方向為向最接近敵人的方向(若敵人在直線上則障礙物較少的方向，不能有兩格被障礙物擋住)

							//隨機選一格使用Forward

							//方向為(2:1:2:1/2:3:7:1/3:5:3:1/1:1:1:0)

							//剩下兩格使用Up Thrust

							//方向為與兩軸成正比，距離較遠的係數+3，較短的+1
						}
					}
				}
			}
		}
	}

	Priority_dir random_dir(int posible_dir, int second_posible_dir, int third_posible_dir){
		return random_dir ( new int [1] {posible_dir}, new int [1] {second_posible_dir}, new int [1] {third_posible_dir});
	}

	Priority_dir random_dir(int posible_dir){ 
		return random_dir ( new int [1] {posible_dir});
	}

	Priority_dir random_dir(int[] posible_dir){ 
		return random_dir (posible_dir, new int[0], new int [0]);
	}

	Priority_dir random_dir(int posible_dir, int[] second_posible_dir){
		return random_dir( new int[1] {posible_dir}, second_posible_dir, new int [0]);
	}

	Priority_dir random_dir(int[] posible_dir, int second_posible_dir){
		return random_dir( posible_dir, new int[1] {second_posible_dir}, new int [0]);
	}

	Priority_dir random_dir(int posible_dir, int second_posible_dir){
		return random_dir( new int[1] {posible_dir}, new int[1] {second_posible_dir}, new int [0]);
	}

	//不能重複
	Priority_dir random_dir(int[] posible_dir, int[] second_posible_dir, int[] third_posible_dir){
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

	//找最接近敵人距離
	public void aim_the_nearest_character(){
		aim_the_nearest_character (false);
	}

	public void aim_the_nearest_character(bool except_neighbor){
		GameObject[] character_gmos = GameObject.FindGameObjectsWithTag ("Character");
		bool first = true;
		float distance = 99999;
		int dis_x = 0, dis_y = 0;
		foreach(GameObject character_gmo in character_gmos){
			Character character = character_gmo.GetComponent<Click_Character> ().character;
//			if (!character.character_camp || character.name == "obstacle")continue;
			if (first) {
				first = false;
				dis_x = (int)Mathf.Abs (character.loc.x - enemy.loc.x);
				dis_y = (int)Mathf.Abs (character.loc.y - enemy.loc.y);
				distance = dis_x + dis_y;
				nearest_character = character;
				continue;
			}
			if (distance > Mathf.Abs (character.loc.x - enemy.loc.x) + Mathf.Abs (character.loc.y - enemy.loc.y)) {
				if(except_neighbor && Mathf.Abs (character.loc.x - enemy.loc.x) + Mathf.Abs (character.loc.y - enemy.loc.y) == 1)continue;
				distance = Mathf.Abs (character.loc.x - enemy.loc.x) + Mathf.Abs (character.loc.y - enemy.loc.y);
				dis_x = (int)(character.loc.x - enemy.loc.x);
				dis_y = (int)(character.loc.y - enemy.loc.y);
				nearest_character = character;
			}
		}

		vec = new Vector3(dis_x, dis_y,distance);
	}

	//正比性隨機
	Priority_dir direct_ration_random(float x, float y){
		return direct_ration_random (x, y, 0);
	}

	Priority_dir direct_ration_random(float x, float y, float n){
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

	Priority_dir reverse_ration_random(float x, float y){
		return reverse_ration_random(x, y, 0);
	}

	Priority_dir reverse_ration_random(float x, float y, float n){
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


	bool random(float x){
		return Random.Range(0, 10000000) < x * 10000000;
	}

	Priority_dir relative_random_dir(int dir, float A, float R, float B, float L){
		int a = dir;
		int r = (dir + 1) % 4;
		int b = (dir + 2) % 4;
		int l = (dir + 3) % 4;

		//		return (random (A / (A + R + B + L))) ? (random (R / (R + B + L))) ? random (B / (B + L)) ? random_dir (a, r, b) : random_dir (a, r, l) : (random (B / (R + B + L))) ? random (R / (R + L)) ? random_dir (a, b, r) : random_dir (a, b, l) : random (R / (R + B)) ? random_dir (a, l, r) : random_dir (a, l, b) : (random (R / (A + R + B + L))) ? (random (A / (A + B + L))) ? random (B / (B + L)) ? random_dir (r, a, b) : random_dir (r, a, l) : (random (B / (A + B + L))) ? random (A / (A + L)) ? random_dir (r, b, a) : random_dir (r, b, l) : random (A / (A + B)) ? random_dir (r, l, a) : random_dir (r, l, b) : (random (B / (A + R + B + L))) ? (random (A / (A + R + L))) ? random (R / (R + L)) ? random_dir (b, a, r) : random_dir (b, a, l) : (random (R / (A + R + L))) ? random (A / (A + L)) ? random_dir (b, r, a) : random_dir (b, r, l) : random (A / (A + R)) ? random_dir (b, l, a) : random_dir (b, l, l) :  (random (A / (A + R + B))) ? random (R / (R + B)) ? random_dir (l, a, r) : random_dir (l, a, b) : (random (R / (A + R + B))) ? random (A / (A + B)) ? random_dir (l, a, a) : random_dir (l, a, b) : random (A / (A + R)) ? random_dir (l, a, a) : random_dir (l, a, r);
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
		if (random (L / L)) {
			if (A == 0 && R == 0 && B == 0)return random_dir (l);
			if (random (A / (A + R + B))) return (R == 0 && B == 0) ? random_dir(l, a) : random (R / (R + B)) ? random_dir (l, a, r) : random_dir (l, a, b);
			if (random (R / (A + R + B))) return (A == 0 && B == 0) ? random_dir(l, r) : random (A / (A + B)) ? random_dir (l, r, a) : random_dir (l, r, b);
			if (random (B / (A + R + B))) return (A == 0 && R == 0) ? random_dir(l, b) : random (A / (A + R)) ? random_dir (l, b, a) : random_dir (l, b, r);
		}

		return null;
	}

	int face_dir(){
		if (Mathf.Abs (vec.x) > Mathf.Abs (vec.y) || (Mathf.Abs (vec.x) == Mathf.Abs (vec.y) && random (0.5f))) return (vec.x > 0) ? 0 : 2;
		else  return (vec.y > 0) ? 3 : 1;
	}

	int face_oblique_dir(){
		if (vec.x >= 0 && vec.y >= 0)return 0;
		if (vec.x >= 0 && vec.y <  0)return 1;
		if (vec.x <  0 && vec.y <  0)return 2;
		if (vec.x <  0 && vec.y >= 0)return 3;
		return -1;
	}

	bool has_skill(string name){
		for(int i=0;i<ai_skills.Length;i++)if(ai_skills[i].skill.name == name && ai_skills[i].amount > 0)return true;
		return false;
	}

	int find_order(string name){
		for(int i=0;i<5;i++)if(aI.skills[i].name == name)return i;
		return -1;
	}

	void set_skill(string name, int order){
		AI_Skill ai_skill = null;
		for(int i=0;i<ai_skills.Length;i++)if(ai_skills[i].skill.name == name)ai_skill = ai_skills[i];
		if (ai_skill == null)Debug.Log ("cant find");
		ai_skill.amount--;

		dismantle_skill((GameObject)Instantiate (ai_skill.skill, new Vector3 (), Quaternion.identity), order);
	}

	AI_Skill[] get_random_skills(int x){
		return get_random_skills (x, null);
	}

	AI_Skill[] get_random_skills(int x, string except){
		AI_Skill[] ret = new AI_Skill[x];
		int ink = 0;

		//count left skill amount
		int n = 0;
		for (int i = 0; i < ai_skills.Length; i++)n += ai_skills [i].amount;

		for (int i = 0; i < x; i++) {
			int random = Random.Range (0, n);
			for (int j = 0; j < ai_skills.Length; j++) {
				random -= ai_skills [j].amount;
				if (random < 0) {
					if (except != null) {
						bool breakable = false;
						for (int k = 0; k < except.Length; k++) {
							if (ai_skills [j].skill.name == except) breakable = true;
						}
						if (breakable) {
							i--;
							break;
						}
					}

					ret [ink++] = ai_skills[j];
					ai_skills [j].amount--;
					n--;
					break;
				}
			}
		}
		return ret;
	}

	Vector3 decode_dir(int x){
		if (x == 0) return new Vector3 (1, 0, 0);
		if (x == 1) return new Vector3 (0, -1, 0);
		if (x == 2) return new Vector3 (-1, 0, 0);
		if (x == 3) return new Vector3 (0, 1, 0);
		return new Vector3 ();
	}

	int get_random_position(int min, int max){
		return get_random_position (min, max, 1);
	}

	//from 1 to 5
	int get_random_position(int min, int max, int length){
		int[] posibles = new int[5];
		int ink = 0;
		int ans = 0;
		for (int i = 0; i < 5; i++) {
			ans = (can_use [i]) ? ans + 1 : 0;
			if (ans >= length && min-1 <= i && i <= max-1)posibles [ink++] = i;
		}
		if (posibles.Length == 0) return -1;
		return posibles[Random.Range (0, ink)];
	}

	void dismantle_skill(GameObject node, int order){
		if (node == null)return;
		dismantle_skill (aI.find_down (node), order - 1);
		can_use [order] = false;
		aI.skills [order] = (GameObject)Instantiate(node, new Vector3(), Quaternion.identity);
		int ink = 0;
		while(aI.skills [order].name[ink] != '(')ink++;
		aI.skills [order].name = aI.skills [order].name.Substring (0, ink);
		Debug.Log (aI.skills [order].name);
		Destroy (node);
		return;
	}

	void destroy_all_skill(){
		for (int i = 0; i < 5; i++) {
			aI.skills [i] = null;
			can_use [i] = true;
		}
		GameObject[] delete_skills = GameObject.FindGameObjectsWithTag ("Skill");
		foreach (GameObject skill in delete_skills) {
			if (skill.transform.position == new Vector3 (0, 0, 0)) {
				for(int i=0;i<ai_skills.Length;i++)if(ai_skills[i].skill.name == skill.name)ai_skills[i].amount++;
				Destroy (skill);
			}
		}
	}

	void default_skill(){
		for (int i = 0; i < 5; i++) {
			set_skill ("Forward", i);
			aI.priority_dir [i] = random_dir (0);
		}
	}

	public char find_map_color(){
		return (aI.map_color == "") ? 'A' : aI.map_color[(int)enemy.loc.y * 7 + (int)enemy.loc.x];
	}

	public bool is_allies_have_buff(Buff_Type buff_type){
		return  !((aI.gameController.allies.Length < 1 || !aI.gameController.allies [0].buffs.has_buff (buff_type)) &&
			(aI.gameController.allies.Length < 2 || !aI.gameController.allies [1].buffs.has_buff (buff_type)) &&
			(aI.gameController.allies.Length < 3 || !aI.gameController.allies [2].buffs.has_buff (buff_type)));
	}
}

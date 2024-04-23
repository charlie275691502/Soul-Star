using UnityEngine;
using System.Collections;

public class Enemys_002 : AI {

	override public void specific_AI(){
		if (gameController.acts.round == 1) {
			
			return;
		}
		if (nearest_allies_vec.z >= 5) {
			//將前兩格設為Forward，方向與(X.diff+1: y.diff+1)成正比
			for (int i = 0; i < 2; i++) {
				set_skill ("Forward", i);
				priority_dir [i] = direct_ration_random (nearest_allies_vec.x, nearest_allies_vec.y);
			}

			//將剩下的四個隨機選三個放上去，方向與(X.diff+1: y.diff+1)成反比
			for (int i = 2; i < 5; i++) {
				set_random_skill (i);
				priority_dir[i] = reverse_ration_random (nearest_allies_vec.x, nearest_allies_vec.y);
			}
		} else if (3 <= nearest_allies_vec.z && nearest_allies_vec.z <= 4) {
			//第一格以3:1機率選擇Forward/Spur
			if (random (0.75f)) {
				set_skill ("Forward", 0);
				//若為Forward則方向與(X.diff+1:y.diff+1)成正比
				priority_dir [0] = direct_ration_random (nearest_allies_vec.x, nearest_allies_vec.y);
			} else {
				set_skill ("Spur", 0);
				//若為Spur則選擇diff>=2的方向，若均為2則1:1
				if(Mathf.Abs(nearest_allies_vec.x) >= 2 && Mathf.Abs(nearest_allies_vec.y) >= 2)priority_dir [0] = random_dir(new int[2]{(nearest_allies_vec.x > 0) ? 0 : 2, (nearest_allies_vec.y > 0) ? 3 : 1});
				else if(Mathf.Abs(nearest_allies_vec.x) >= 2)priority_dir [0] = random_dir((nearest_allies_vec.x > 0) ? 0 : 2);
				else if(Mathf.Abs(nearest_allies_vec.y) >= 2)priority_dir [0] = random_dir((nearest_allies_vec.y > 0) ? 3 : 1);
			}

			//在第2~5隨機選一格為Attack，方向與(X.diff+1: y.diff+1)成反比
			int random_position = Random.Range(1, 5);
			set_skill ("Attack", random_position);
			priority_dir[random_position] = reverse_ration_random (nearest_allies_vec.x, nearest_allies_vec.y);

			//在剩餘的個子中隨機選一格，以(7:3)設為Forward/Attack
			int random_position_2 = random_position;
			while (random_position_2 == random_position)random_position_2 = Random.Range(2, 5);
			if (random (0.7f)) {
				//若為Forward，方向原理同上，但以(2:1)機率進行進攻/撤退
				set_skill ("Forward", random_position_2);
				priority_dir [random_position_2] = direct_ration_random (nearest_allies_vec.x, nearest_allies_vec.y, 0.3333f);
			} else {
				//若為攻擊則與上同。
				set_skill ("Attack", random_position_2);
				priority_dir [random_position_2] = reverse_ration_random (nearest_allies_vec.x, nearest_allies_vec.y);
			}

			//將剩餘的隨機擺放，方向原理同上(Spur無撤退舉動)
			for (int i = 1; i < 5; i++) {
				if (i == random_position || i == random_position_2)continue;
				set_random_skill(i);
				if (skills [i].GetComponent<Click_skill>().Name == "Forward") {
					priority_dir [i] = direct_ration_random (nearest_allies_vec.x, nearest_allies_vec.y, 0.3333f);
				} else if (skills [i].GetComponent<Click_skill>().Name == "Attack") {
					priority_dir [i] = direct_ration_random (nearest_allies_vec.x, nearest_allies_vec.y);
				} else if (skills [i].GetComponent<Click_skill>().Name == "Swing") {
					priority_dir [i] = direct_ration_random (nearest_allies_vec.x, nearest_allies_vec.y);
				} else if (skills [i].GetComponent<Click_skill>().Name == "Spur") {
					if(Mathf.Abs(nearest_allies_vec.x) >= 2 && Mathf.Abs(nearest_allies_vec.y) >= 2)priority_dir [i] = random_dir(new int[2]{(nearest_allies_vec.x > 0) ? 0 : 2, (nearest_allies_vec.y > 0) ? 3 : 1});
					else if(Mathf.Abs(nearest_allies_vec.x) >= 2)priority_dir [i] = random_dir((nearest_allies_vec.x > 0) ? 0 : 2);
					else if(Mathf.Abs(nearest_allies_vec.y) >= 2)priority_dir [i] = random_dir((nearest_allies_vec.y > 0) ? 3 : 1);
				}
			}

		} else if (nearest_allies_vec.z == 2) {
			//若為直線
			if (Mathf.Abs (nearest_allies_vec.x) == 2 || Mathf.Abs (nearest_allies_vec.y) == 2) {
				//第一格採(3:1)進行Attack/Spur
				set_skill ((random(0.75f)) ? "Attack" : "Spur", 0);

				priority_dir [0] = random_dir (face_dir(nearest_allies_vec));

				//在第2~3格擇一(1:1)擺上Forward
				int random_position = Random.Range(1, 3);
				set_skill ("Forward", random_position);

				//方向向男主以外方向隨機(1:1:1)
				priority_dir[random_position] = relative_random_dir(face_dir(nearest_allies_vec), 0, 1, 1, 1);

				//剩下隨機擺放
				for (int i = 1; i < 5; i++) {
					if (i == random_position)continue;
					set_random_skill (i);
					if (skills [i].GetComponent<Click_skill>().Name == "Attack") {
						//Attack方向為(2:1:1)敵人方向/垂直方向
						priority_dir [i] = relative_random_dir(face_dir(nearest_allies_vec),2, 1, 0, 1);
					} else if (skills [i].GetComponent<Click_skill>().Name == "Spur"){
						//Spur為向敵人方向
						priority_dir [i] = priority_dir[0];
					} else if (skills [i].GetComponent<Click_skill>().Name == "Forward"){
						//Forward方向徹底隨機
						priority_dir [i] = random_dir (new int[0]);
					} else if (skills [i].GetComponent<Click_skill>().Name == "Swing"){
						//Swing僅面向敵人方向
						priority_dir [i] = priority_dir[0];
					}
				}
			} 

			//若為斜對角
			else {
				//第2~3須有一個Forward，方向徹底隨機
				int random_position = Random.Range(1, 3);
				set_skill ("Forward", random_position);
				priority_dir[random_position] = random_dir(new int[0]);

				//將Spur以外的棋全部放上棋盤
				//get 4 random skill
				for (int i = 0; i < 5; i++) {
					if (i == random_position)continue;
					Debug.Log (i);
					set_random_skill ("Spur", i);
					if (skills [i].GetComponent<Click_skill>().Name == "Forward") {
						//Forward方向徹底隨機
						priority_dir [i] = random_dir(new int[0]);
					} else{
						//攻擊方向僅面向男主
						priority_dir [i] = direct_ration_random (nearest_allies_vec.x, nearest_allies_vec.y);
					}
				}
			}

		} else if (nearest_allies_vec.z == 1) {
			//第一格以(6:3:1)進行Swing/Attack/Spur
			set_skill ((random(0.6f)) ? "Swing" : (random(0.75f)) ? "Attack" : "Spur", 0);

			priority_dir [0] = random_dir (face_dir(nearest_allies_vec));

			//若為Spur
			if(skills[0].GetComponent<Click_skill>().Name == "Spur"){
				//擇後三格隨機放上Forward*2、Swing
				int random_position = Random.Range(1, 4);
				set_skill ("Swing", random_position);

				//Swing方向面向敵人
				priority_dir[random_position] = random_dir (face_dir(nearest_allies_vec));

				for(int i=1;i<4;i++){
					//Forward方向為任何不是面向敵人方向
					if(i == random_position)continue;
					set_skill ("Forward", 1);
					priority_dir[i] = relative_random_dir(face_dir(nearest_allies_vec),0, 1, 1, 1);
				}

				//最後一格Attack
				set_skill ("Attack", 4);

				//方向為隨機且非背向敵人(3個方向1:1:1)
				priority_dir[4] = relative_random_dir(face_dir(nearest_allies_vec),1, 1, 0, 1);
			}

			//若非Spur

			else {
				//將Spur以外的隨機擺放
				for (int i = 1; i < 5; i++) {
					set_random_skill ("Spur", i);
					if (skills [i].GetComponent<Click_skill>().Name == "Forward") {
						//Forward為任何非面向敵人方向
						priority_dir [i] = relative_random_dir(face_dir(nearest_allies_vec),0, 1, 1, 1);
					} else if (skills [i].GetComponent<Click_skill>().Name == "Attack") {
						//Attack為3個方向(1:1:1)
						priority_dir [i] = relative_random_dir(face_dir(nearest_allies_vec),1, 1, 0, 1);
					} else if (skills [i].GetComponent<Click_skill>().Name == "Swing") {
						//Swing為(8:1:1)
						priority_dir [i] = relative_random_dir(face_dir(nearest_allies_vec),8, 1, 0, 1);
					}
				}
			}
		}
	}
}

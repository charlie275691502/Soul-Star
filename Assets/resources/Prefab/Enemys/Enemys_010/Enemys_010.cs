using UnityEngine;
using System.Collections;

public class Enemys_010 : AI {
	override public void specific_AI(){

		int move_position;
		int atk_position = 10;
		int shoot_position2;
		bool light_atk = true;
		//遠距時先丟真空劍舞；中距先衝(反向)；近距先砍
		if (furthest_allies_vec.z > 3) {
			atk_position = 0;
			set_skill ("Sword Dance", 0);

			move_position = get_random_position(1, 4);
			set_skill ("Bump", move_position);
			shoot_position2 = move_position + 1;
			if (shoot_position2 != 5) {
				set_skill ("Kill", shoot_position2);
				light_atk = false;
			}
		} else if (furthest_allies_vec.z == 3) {
			move_position = 0;
			set_skill ("Bump", move_position);
			shoot_position2 = 1;
			set_skill (random (0.5f) ? "Sword Dance" : "Kill", shoot_position2);
		} else {
			move_position = get_random_position(1, 2);
			set_skill ("Bump", move_position);
			shoot_position2 = move_position+1;
			set_skill ("Kill", shoot_position2);
		}
		//填空&調方向
		for (int i = 0; i < 5; i++) {
			if (i == move_position) {
				if (i != 0) {
					priority_dir [i] = random_dir (face_dir(nearest_allies_vec));
					nearest_allies_vec= vec_update (nearest_allies_vec, 1, priority_dir [i].dir [0]);
					furthest_allies_vec= vec_update (furthest_allies_vec, 1, priority_dir [i].dir [0]);
					continue;
				}
				priority_dir [i] = random_dir ((face_dir(furthest_allies_vec)+2)%4);
				nearest_allies_vec= vec_update (nearest_allies_vec, 1, priority_dir [i].dir [0]);
				furthest_allies_vec= vec_update (furthest_allies_vec, 1, priority_dir [i].dir [0]);
				continue;
			}
			if (i==shoot_position2) {
				priority_dir [i] = random_dir (face_dir(furthest_allies_vec));
				continue;
			}
			if (i==atk_position) {
				priority_dir [i] = random_dir (face_dir(furthest_allies_vec));
				continue;
			}			
			if (!light_atk) {
				set_random_skill ("Light Smite", i);
			} else {
				set_random_skill (new string[2] {"Sword Dance","Kill"}, i);
			}
//			if (!has_skill ("Swing"))set_skill ("Light Smite", i);
//			else if (!has_skill ("Light Smite") || !light_atk)set_skill ("Swing", i);
//			else if (amount_of_skill ("Swing") == 2)	set_skill (random (0.66f) ? "Swing" : "Light Smite", i);
//			else set_skill (random (0.5f) ? "Swing" : "Light Smite", i);
				
			if(nearest_allies_vec.z>2)
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
			else 
				priority_dir [i] = relative_random_dir (face_dir (nearest_allies_vec), 6, 1, 0, 1);
		}


	}

}

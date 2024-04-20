using UnityEngine;
using System.Collections;

public class Enemys_018 : AI {
	override public void specific_AI(){

		int move_position = 0;
		int move_position2 = get_random_position (3, 4);
		int fire_position3 = 0;
		if (nearest_allies_vec.z > 3) {
			set_skill ("衝刺", move_position);
			set_skill ("前進", move_position2);
			priority_dir [0] = random_dir (face_dir(nearest_allies_vec));
			priority_dir [move_position2] = random_dir (face_dir(nearest_allies_vec));
			nearest_allies_vec= vec_update (nearest_allies_vec, 2, priority_dir [0].dir [0]);
		} else if (nearest_allies_vec.z == 3) {
			set_skill ("輕擊", 0);
			set_skill ("前進", move_position2);
			if(enemy.mp >= 10){
				fire_position3 = get_random_position (3, 4);
				set_skill ("焰霧射擊", fire_position3);
				priority_dir [fire_position3] = relative_random_dir (face_dir (nearest_allies_vec), 6, 1, 0, 1);
			}
			priority_dir [0] = random_dir (face_dir(nearest_allies_vec));
			priority_dir [move_position2] = random_dir (face_dir(nearest_allies_vec));
		} else if (nearest_allies_vec.z == 2) {
			set_skill ("橫斬", 0);
			set_skill ("前進", move_position2);
			if(enemy.mp >= 10){
				fire_position3 = get_random_position (1, 4);
				set_skill ("焰霧射擊", fire_position3);
				priority_dir [fire_position3] = relative_random_dir (face_dir (nearest_allies_vec), 9, 1, 0, 1);
			}
			priority_dir [0] = random_dir (face_dir(nearest_allies_vec));
			priority_dir [move_position2] = random_dir ();
		} else {
			if(enemy.mp >= 10 && random(0.66f)){
				fire_position3 = 0;
				set_skill ("焰霧射擊", fire_position3);
				priority_dir [fire_position3] = random_dir (face_dir(nearest_allies_vec));
			} else{
				set_skill ("重擊", 0);
				priority_dir [0] = random_dir (face_dir(nearest_allies_vec));
			}
			move_position2 = 4;
			set_skill ("前進", move_position2);
			priority_dir [move_position2] = random_dir (face_dir(nearest_allies_vec));
		}

		for (int i = 1; i < 5; i++) {
			if (i==move_position2) {
				nearest_allies_vec= vec_update (nearest_allies_vec, 1, priority_dir [i].dir [0]);
				continue;
			}
			if (i == fire_position3) {
				continue;
			}
				
			if (has_skill ("重擊") && random (0.1f)) set_skill ("重擊", i);
			else set_random_skill (new string[3] {"重擊","衝刺","焰霧射擊"}, i);

			if(nearest_allies_vec.z>2)
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
			else 
				priority_dir [i] = relative_random_dir (face_dir (nearest_allies_vec), 6, 1, 0, 1);
		}


	}
}
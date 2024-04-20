using UnityEngine;
using System.Collections;

public class Enemys_013 : AI {
	override public void specific_AI(){
		
		int move_position=0;
		int move_position2 = get_random_position (3, 4);
		if (nearest_allies_vec.z > 3) {
			set_skill ("衝刺", move_position);
			set_skill ("前進", move_position2);
			priority_dir [0] = random_dir (face_dir(nearest_allies_vec));
			priority_dir [move_position2] = random_dir (face_dir(nearest_allies_vec));
			nearest_allies_vec= vec_update (nearest_allies_vec, 2, priority_dir [0].dir [0]);
		} else if (nearest_allies_vec.z == 3) {
			set_skill ("輕擊", 0);
			set_skill ("前進", move_position2);
			priority_dir [0] = random_dir (face_dir(nearest_allies_vec));
			priority_dir [move_position2] = random_dir (face_dir(nearest_allies_vec));
		} else if (nearest_allies_vec.z == 2) {
			set_skill ("橫斬", 0);
			set_skill ("前進", move_position2);
			priority_dir [0] = random_dir (face_dir(nearest_allies_vec));
			priority_dir [move_position2] = random_dir ();
		} else {
			set_skill ("重擊", 0);
			move_position2 = 4;
			set_skill ("前進", move_position2);
			priority_dir [0] = random_dir (face_dir(nearest_allies_vec));
			priority_dir [move_position2] = random_dir (face_dir(nearest_allies_vec));
		}

		for (int i = 1; i < 5; i++) {
			if (i==move_position2) {
				nearest_allies_vec= vec_update (nearest_allies_vec, 1, priority_dir [i].dir [0]);
				continue;
			}
			if (has_skill ("重擊") && random (0.1f)) set_skill ("重擊", i);
			else set_random_skill (new string[2] {"重擊","衝刺"}, i);

			if(nearest_allies_vec.z>2)
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
			else 
				priority_dir [i] = relative_random_dir (face_dir (nearest_allies_vec), 6, 1, 0, 1);
		}


	}

}
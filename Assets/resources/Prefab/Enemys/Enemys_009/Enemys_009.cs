using UnityEngine;
using System.Collections;

public class Enemys_009 : AI {

	override public void specific_AI(){

		//判定距離，近距時在1~2格優先丟範圍技。
		int atk_position = 0;
		if (nearest_allies_vec.z < 2) {
			atk_position = get_random_position(0, 1);
			set_skill ("Spore Blast", atk_position);
		}
		//丟衝撞沖臉。
		int move_position2 = get_random_position(atk_position, 3);
		set_skill ("Bump", move_position2);
		if (has_skill ("Spore Blast")) {
			atk_position = move_position2 + 1;
			set_skill("Spore Blast", atk_position);

		}
		for (int i = 0; i < 5; i++) {
			if (i==atk_position) {
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
				continue;
			}
			if (i==move_position2) {
				
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
				nearest_allies_vec= vec_update (nearest_allies_vec, 1, priority_dir [i].dir [0]);
				continue;
			}
			set_skill("Knock", i);
			if(nearest_allies_vec.z>2)
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
			else 
				priority_dir [i] = relative_random_dir (face_dir (nearest_allies_vec), 3, 1, 0, 1);
		}



	}
}
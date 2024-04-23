using UnityEngine;
using System.Collections;

public class Enemys_008 : AI {


	override public void specific_AI(){

		//在1~5格隨機Forward和衝撞，方向先不管。
		int move_position = get_random_position(0, 2);
		set_skill (random (0.5f) ? "Forward" : "Bump", move_position);
		int move_position2 = get_random_position(move_position, 4);
		set_skill (!has_skill("Bump")? "Forward" : "Bump", move_position2);

		//接下來重第一個行動開始填空格。移動&衝撞前後分別判斷距離，遠：面向敵人方向；近：機率打面向及側向。
		for (int i = 0; i < 5; i++) {
			if (!can_use [i]) {
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
				nearest_allies_vec= vec_update (nearest_allies_vec, 1, priority_dir [i].dir [0]);
				continue;
			}
			set_skill("Knock", i);
			if (nearest_allies_vec.z > 3)
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
			else
				priority_dir [i] = relative_random_dir (face_dir (nearest_allies_vec), 3, 1, 0, 1);
		}

	}
}
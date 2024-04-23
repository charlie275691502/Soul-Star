using UnityEngine;
using System.Collections;

public class Enemys_012 : AI {
	override public void specific_AI(){
		//在2~5格隨機前進，方向先不管。
		int move_position = get_random_position(1, 4);
		set_skill ("Slow Move", move_position);

		//接下來重第一個行動開始填空格。移動前後分別判斷距離，遠：面向敵人方向；近：20%機率打側向。
		for (int i = 0; i < 5; i++) {
			if (!can_use [i]) {
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
				nearest_allies_vec= vec_update (nearest_allies_vec, 1, priority_dir [i].dir [0]);
				continue;
			}
			set_random_skill(i);
			if(nearest_allies_vec.z>2)
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
			else 
				priority_dir [i] = relative_random_dir (face_dir (nearest_allies_vec), 6, 1, 0, 1);
		}
	}

}

using UnityEngine;
using System.Collections;

public class Enemys_016: AI {
	override public void specific_AI(){
		
		int move_position=0;
		int move_position2=0;
		bool long_move = false;
		//判距離放招
		if (nearest_allies_vec.z > 2) {
				set_skill ((enemy.ep >= 4)?"Refill":"Forward", move_position2);
				long_move = true;
		}
		if (nearest_allies_vec.z == 1 && enemy.ep >= 10)set_skill ("Energy Absorb", 0);
		if (has_skill ("Forward")) {
			move_position = get_random_position (2, 4);
			set_skill ("Forward", move_position);
		}

		//填空填向
		for (int i = 0; i < 5; i++) {
			if (!can_use [i]) {
				if(i==0){
					if(!has_skill("Refill")){
						priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
						nearest_allies_vec= vec_update (nearest_allies_vec, 2, priority_dir [i].dir [0]);
						continue;
					}
					if (!has_skill ("Energy Absorb")) {
						priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
						continue;
					}
					priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
					nearest_allies_vec= vec_update (nearest_allies_vec, 1, priority_dir [i].dir [0]);
					continue;
				}
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
				nearest_allies_vec= vec_update (nearest_allies_vec, 1, priority_dir [i].dir [0]);
				continue;
			}

			if(has_skill("Dance"))set_skill("Dance",i);
			else if (has_skill ("Energy Absorb")&& enemy.ep >= 10) set_skill(random(0.9f)?(has_skill ("Light Smite")?"Light Smite":"Blank"):"Energy Absorb",i);
			else if (has_skill ("Light Smite")) set_skill("Light Smite",i);
			else set_skill("Blank",i);

			if(nearest_allies_vec.z>2)
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
			else 
				priority_dir [i] = relative_random_dir (face_dir (nearest_allies_vec), 2, 1, 0, 1);
		}


	}
}
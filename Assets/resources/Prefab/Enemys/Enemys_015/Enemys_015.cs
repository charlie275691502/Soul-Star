using UnityEngine;
using System.Collections;

public class Enemys_015 : AI {
	override public void specific_AI(){
		
		int move_position=0;
		int move_position2=0;
		bool long_move = false;
		//判距離放招
		if (nearest_allies_vec.z > 2) {
				set_skill ((enemy.ep >= 4)?"快速補位":"前進", move_position2);
				long_move = true;
		}
		if (nearest_allies_vec.z == 1 && enemy.ep >= 10)set_skill ("能量偷竊", 0);
		if (has_skill ("前進")) {
			move_position = get_random_position (2, 4);
			set_skill ("前進", move_position);
		}

		//填空填向
		for (int i = 0; i < 5; i++) {
			if (!can_use [i]) {
				if(i==0){
					if(!has_skill("快速補位")){
						priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
						nearest_allies_vec= vec_update (nearest_allies_vec, 2, priority_dir [i].dir [0]);
						continue;
					}
					if (!has_skill ("能量偷竊")) {
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

			if(has_skill("嗡嗡揮舞"))set_skill("嗡嗡揮舞",i);
			else if (has_skill ("能量偷竊")&& enemy.ep >= 10) set_skill(random(0.9f)?(has_skill ("輕擊")?"輕擊":"空白技能"):"能量偷竊",i);
			else if (has_skill ("輕擊")) set_skill("輕擊",i);
			else set_skill("空白技能",i);

			if(nearest_allies_vec.z>2)
				priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
			else 
				priority_dir [i] = relative_random_dir (face_dir (nearest_allies_vec), 2, 1, 0, 1);
		}


	}
}
using UnityEngine;
using System.Collections;

public class Enemys_005 : AI {

	override public void specific_AI(){
		can_use = new bool[5]{true, true, true, true, true};
		for (int i = 0; i < ai_skills.Length; i++)ai_skills [i].amount = ai_skills [i].init_amount;
		now_loc = enemy.loc;
		//在第2~5格隨機選擇一個前進，方向(1:4:2:3/2:1:3:4/1:1:4:4/1:6:1:2)。
		int random_position = get_random_position(1, 4);
		set_skill ("前進", random_position);
		if(find_map_color() == 'R')priority_dir[random_position] = relative_random_dir(0, 4, 2, 3, 1);
		if(find_map_color() == 'Y')priority_dir[random_position] = relative_random_dir(0, 1, 3, 4, 2);
		if(find_map_color() == 'B')priority_dir[random_position] = relative_random_dir(0, 1, 4, 4, 1);
		if(find_map_color() == 'G')priority_dir[random_position] = relative_random_dir(0, 6, 1, 2, 1);

		//若別人沒有人有受到灼傷效果，且MP>=12，有(60%/80%/0%/40%)。在第(1~2/2~3/不討論/1~3)格隨機擇一放上然火彈，方向指向最接近敵方。
		if (!is_allies_have_buff(Buff_Type.灼傷) && enemy.mp >= 12) {
			random_position = -1;
			if(find_map_color() == 'R' && random(0.6f))random_position = get_random_position(0, 1);
			if(find_map_color() == 'Y' && random(0.8f))random_position = get_random_position(1, 2);
			if(find_map_color() == 'B' && random(0.0f))your_mom_so_fat();
			if(find_map_color() == 'G' && random(0.4f))random_position = get_random_position(0, 2);
			if (random_position != -1) {
				set_skill ("然火彈", random_position);
				priority_dir [random_position] = random_dir (face_oblique_dir (nearest_allies_vec));
			}
		}

		//EP>=10且第五格行動尚未決定的時候，有(20%/50%/0%/30%)的機率在第五個行動放上重擊，方向向最接近敵人方向(兩個方向1:1)。若最接近敵人有灼傷效果則機率變成(40%/100%/15%/50%)。
		if (enemy.ep >= 10 && can_use [4]) { 
			if (nearest_allies.buffs.has_buff (Buff_Type.灼傷)) {
				if ((find_map_color () == 'R' && random (0.4f)) ||
					(find_map_color () == 'Y' && random (1.0f)) ||
					(find_map_color () == 'B' && random (0.15f)) ||
					(find_map_color () == 'G' && random (0.5f))) {
					set_skill ("重擊", 4);
					priority_dir [4] = random_dir (face_dir (nearest_allies_vec));
				}
			} else {
				if ((find_map_color () == 'R' && random (0.2f)) ||
					(find_map_color () == 'Y' && random (0.5f)) ||
					(find_map_color () == 'B' && random (0.0f)) ||
					(find_map_color () == 'G' && random (0.3f))) {
					set_skill ("重擊", 4);
					priority_dir [4] = random_dir (face_dir (nearest_allies_vec));
				}
			}
		}

		//接下來重第一個行動開始填空格。刺擊:左刺:右刺(6:1:3/5:2:3/4:1:5/4:5:1)。方向優先不要戳到障礙物，面向敵人方向。
		for (int i = 0; i < 5; i++) {
			if (!can_use [i])continue;
				 if (!has_skill("刺擊") && !has_skill("左刺")) set_skill("右刺", i);
			else if (!has_skill("左刺") && !has_skill("右刺")) set_skill("刺擊", i);
			else if (!has_skill("刺擊") && !has_skill("右刺")) set_skill("左刺", i);
			  else if (!has_skill("刺擊")){
				if (find_map_color () == 'R')set_skill (random (0.25f  ) ? "左刺" : "右刺", i);
				if (find_map_color () == 'Y')set_skill (random (0.4f   ) ? "左刺" : "右刺", i);
				if (find_map_color () == 'B')set_skill (random (0.1666f) ? "左刺" : "右刺", i);
				if (find_map_color () == 'G')set_skill (random (0.8333f) ? "左刺" : "右刺", i);
			} else if (!has_skill("左刺")){
				if (find_map_color () == 'R')set_skill (random (0.6666f) ? "刺擊" : "右刺", i);
				if (find_map_color () == 'Y')set_skill (random (0.625f ) ? "刺擊" : "右刺", i);
				if (find_map_color () == 'B')set_skill (random (0.4444f) ? "刺擊" : "右刺", i);
				if (find_map_color () == 'G')set_skill (random (0.8f   ) ? "刺擊" : "右刺", i);
			} else if (!has_skill("右刺")){
				if (find_map_color () == 'R')set_skill (random (0.8571f) ? "刺擊" : "左刺", i);
				if (find_map_color () == 'Y')set_skill (random (0.7143f) ? "刺擊" : "左刺", i);
				if (find_map_color () == 'B')set_skill (random (0.8f   ) ? "刺擊" : "左刺", i);
				if (find_map_color () == 'G')set_skill (random (0.4444f) ? "刺擊" : "左刺", i);
			} else {
				if (find_map_color () == 'R')set_skill (random (0.6f) ? "刺擊" : random (0.25f  ) ? "左刺" : "右刺", i);
				if (find_map_color () == 'Y')set_skill (random (0.5f) ? "刺擊" : random (0.4f   ) ? "左刺" : "右刺", i);
				if (find_map_color () == 'B')set_skill (random (0.4f) ? "刺擊" : random (0.1666f) ? "左刺" : "右刺", i);
				if (find_map_color () == 'G')set_skill (random (0.4f) ? "刺擊" : random (0.8333f) ? "左刺" : "右刺", i);
			}	
			//方向向最接近敵人方向
			priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
		}
	}



	//找最接近敵人距離

}

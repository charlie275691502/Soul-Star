using UnityEngine;
using System.Collections;

public class Enemys_004 : AI {
	override public void specific_AI(){
		//EP>=7時有60%進這一段
		if (enemy.ep >= 7 && random (0.6f)) {
			//在第1~4格隨機選擇一個Forward
			int random_position = get_random_position (0, 3);
			set_skill ("Forward", random_position);

			//(方向2:7:0:1/1:1:6:2/0:1:7:2/1:2:1:6)
			if(find_map_color() == 'R')priority_dir[random_position] = relative_random_dir(0, 7, 0, 1, 2);
			if(find_map_color() == 'Y')priority_dir[random_position] = relative_random_dir(0, 1, 6, 2, 1);
			if(find_map_color() == 'B')priority_dir[random_position] = relative_random_dir(0, 1, 7, 2, 0);
			if(find_map_color() == 'G')priority_dir[random_position] = relative_random_dir(0, 2, 1, 6, 1);

			//(方向)
//			aI.priority_dir[random_position] = 

			//並且在下一格放上”Air Arrow”，方向向左
			set_skill("Air Arrow", random_position + 1);
			priority_dir [random_position + 1] = random_dir (2);
		} 
		
		//EP<7或剩餘的40%
		else {
			
			//紅色格50%不Forward
			if (find_map_color () == 'Y' && random (0.3f));
			else {
				int random_position = -1;
				//若最接近敵人距離>=3則僅在第4/5個行動
				if (nearest_allies_vec.z >= 3) random_position = get_random_position(3, 4);

				//若距離<3則在第1~5個行動隨機選擇一個進行Forward
				else random_position = get_random_position(0, 4);
				set_skill ("Forward", random_position);

				//方向(3:5:1:1/1:1:5:3/0:2:6:2/0:5:0:5)
				if(find_map_color() == 'R')priority_dir[random_position] = relative_random_dir(0, 5, 1, 1, 3);
				if(find_map_color() == 'Y')priority_dir[random_position] = relative_random_dir(0, 1, 5, 3, 1);
				if(find_map_color() == 'B')priority_dir[random_position] = relative_random_dir(0, 2, 6, 2, 0);
				if(find_map_color() == 'G')priority_dir[random_position] = relative_random_dir(0, 5, 0, 5, 0);
			}
		}

		//接下來如果與最近敵人距離<3，(0%/60%/70%/80%)在隨機一有效行動格放上”震盪箭。異”
		if (nearest_allies_vec.z < 3) {
			if( (find_map_color () == 'R' && random (0.0f)) ||
				(find_map_color () == 'Y' && random (0.6f)) ||
				(find_map_color () == 'B' && random (0.7f)) ||
				(find_map_color () == 'G' && random (0.8f))) {
				int random_position = get_random_position (0, 4, 2);
				set_skill ("Shock Arrow Special", random_position);

				//方向向直觀敵人接近方向。
				priority_dir [random_position] = random_dir (face_dir (nearest_allies_vec));
			}
		}

		//接下來從第一個行動開始填上Triple Shots/Flat Shot(1:9/3:7/7:3/2:8)
		for (int i = 0; i < 5; i++) {
			if (!can_use [i] || (!has_skill("Triple Shots") && !has_skill("Flat Shot")))continue;
			if (!has_skill("Triple Shots")) set_skill("Flat Shot", i);
			else if (!has_skill("Flat Shot")) set_skill("Triple Shots", i);
			else {
				if (find_map_color () == 'R')set_skill (random (0.1f) ? "Triple Shots" : "Flat Shot", i);
				if (find_map_color () == 'Y')set_skill (random (0.3f) ? "Triple Shots" : "Flat Shot", i);
				if (find_map_color () == 'B')set_skill (random (0.7f) ? "Triple Shots" : "Flat Shot", i);
				if (find_map_color () == 'G')set_skill (random (0.2f) ? "Triple Shots" : "Flat Shot", i);
			}

			//方向向最接近敵人方向
			priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
		}
	}
}

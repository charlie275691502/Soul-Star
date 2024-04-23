using UnityEngine;
using System.Collections;

public class Enemys_003 : AI {
	override public void specific_AI(){
		//EP>=7時有80%進這一段
		if (enemy.ep >= 7 && random (0.8f)) {
			//在第3~4格隨機選擇一個Forward(方向先上，不能的話右/8:0:0:2/0:7:3:0/ 2:8:0:0)
			int move_position = get_random_position(2, 3);
			set_skill ("Forward", move_position);
			if(find_map_color() == 'R')priority_dir[move_position] = random_dir(3, 0);
			if(find_map_color() == 'Y')priority_dir[move_position] = relative_random_dir(0, 0, 0, 2, 8);
			if(find_map_color() == 'B')priority_dir[move_position] = relative_random_dir(0, 7, 3, 0, 0);
			if(find_map_color() == 'G')priority_dir[move_position] = relative_random_dir(0, 8, 0, 0, 2);

			//並且在下一格放上”Wind Arrow”，方向向最接近敵人之座標差較大的方向(如起始位置為向左)
			set_skill ("Wind Arrow", move_position + 1);
			priority_dir [move_position + 1] = random_dir (2, 1);
		} 

		//EP<7或剩餘的20%
		else {
			//黃色格30%不Forward
			if (find_map_color () == 'Y' && random (0.3f));
			else {
				int move_position = -1;
				//若最接近敵人距離>=3則僅在第4/5個行動
				if (nearest_allies_vec.z >= 3) move_position = get_random_position(3, 4);

				//若距離<3則在第1~5個行動隨機選擇一個進行Forward
				else move_position = get_random_position(0, 4);
				set_skill ("Forward", move_position);
				if(find_map_color() == 'R')priority_dir[move_position] = relative_random_dir(0, 1, 0, 0, 0);
				if(find_map_color() == 'Y')priority_dir[move_position] = relative_random_dir(0, 1, 8, 0, 1);
				if(find_map_color() == 'B')priority_dir[move_position] = relative_random_dir(0, 1, 6, 3, 0);
				if(find_map_color() == 'G')priority_dir[move_position] = relative_random_dir(0, 3, 2, 4, 1);
			}
		}

		//接下來如果與最近敵人距離<(2&重衛兵已死亡/不放/3/2)，(25%/0%/40%/100%)在隨機一有效行動格放上震盪箭。強(佔兩個行動)
		if((find_map_color () == 'R' && random (0.25f) && nearest_allies_vec.z < 2 && (gameController.enemys_prefab.Length < 3 || gameController.enemys [2].hp <= 0)) ||
		   (find_map_color () == 'Y' && random (0.0f)) ||
			(find_map_color () == 'B' && random (0.4f) && nearest_allies_vec.z < 3) ||
			(find_map_color () == 'G' && random (1.0f) && nearest_allies_vec.z < 2)) {
			int move_position = get_random_position (0, 4, 2);
			set_skill ("Shock Arrow Strong", move_position);

			//方向向直觀敵人接近方向。
			priority_dir [move_position] = random_dir (face_dir (nearest_allies_vec));
		}


		//接下來從第一個行動開始填上Long Shot/Flat Shot(7:3/3:7/1:1/2:8)
		for (int i = 0; i < 5; i++) {
			if (!can_use [i] || (!has_skill("Long Shot") && !has_skill("Flat Shot")))continue;
				 if (!has_skill("Long Shot")) set_skill("Flat Shot", i);
			else if (!has_skill("Flat Shot")) set_skill("Long Shot", i);
			else {
				if (find_map_color () == 'R')set_skill (random (0.7f) ? "Long Shot" : "Flat Shot", i);
				if (find_map_color () == 'Y')set_skill (random (0.3f) ? "Long Shot" : "Flat Shot", i);
				if (find_map_color () == 'B')set_skill (random (0.5f) ? "Long Shot" : "Flat Shot", i);
				if (find_map_color () == 'G')set_skill (random (0.2f) ? "Long Shot" : "Flat Shot", i);
			}
			//方向向最接近敵人方向
			priority_dir [i] = random_dir (face_dir (nearest_allies_vec));
		}

//		default_skill ();
	}
}

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Click_skill))]
public class Click_skill_editor : Editor {

	private bool folder_on = false;
	private bool machine_attacks_range_folder = false;
	Click_skill click_skill;

	void OnEnable(){
		folder_on = true;
		click_skill = (Click_skill)target;
	}

	public override void OnInspectorGUI(){

		DrawDefaultInspector ();
		folder_on = EditorGUILayout.Foldout(folder_on, "Skill_detail");
		if (!folder_on ) return;
		click_skill.skill_detail.show_time = EditorGUILayout.FloatField ("     Show_time", click_skill.skill_detail.show_time);
		click_skill.skill_detail.skill_type = (Skill_Type)EditorGUILayout.EnumPopup ("     Clas", click_skill.skill_detail.skill_type);
		switch (click_skill.skill_detail.skill_type) {
		case Skill_Type.Move:
			click_skill.skill_detail.move_Type = (Move_Type)EditorGUILayout.EnumPopup ("     Type", click_skill.skill_detail.move_Type);
			break;
		case Skill_Type.Machine:
			click_skill.skill_detail.machine_Type = (Machine_Type)EditorGUILayout.EnumPopup ("     Type", click_skill.skill_detail.machine_Type);
			switch (click_skill.skill_detail.machine_Type) {
			case Machine_Type.pure_damage:
				click_skill.skill_detail.only_slash_once = EditorGUILayout.Toggle ("     Only_slash_once", click_skill.skill_detail.only_slash_once);
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				click_skill.skill_detail.attack_delay = EditorGUILayout.FloatField ("     Attack_delay", click_skill.skill_detail.attack_delay);
				break;
			case Machine_Type.move_damage:
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				click_skill.skill_detail.attack_delay = EditorGUILayout.FloatField ("     Attack_delay", click_skill.skill_detail.attack_delay);
				break;
			case Machine_Type.move_success_damage:
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				click_skill.skill_detail.attack_delay = EditorGUILayout.FloatField ("     Attack_delay", click_skill.skill_detail.attack_delay);
				break;
			case Machine_Type.random_damage:
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				click_skill.skill_detail.random_amount = EditorGUILayout.IntField ("     Random_amount", click_skill.skill_detail.random_amount);
				click_skill.skill_detail.attack_delay = EditorGUILayout.FloatField ("     Attack_delay", click_skill.skill_detail.attack_delay);
				break;
			case Machine_Type.refill:
				click_skill.skill_detail.refill_mp = EditorGUILayout.IntField ("     Refill_mp", click_skill.skill_detail.refill_mp);
				click_skill.skill_detail.refill_ep = EditorGUILayout.IntField ("     Refill_ep", click_skill.skill_detail.refill_ep);
				break;
			case Machine_Type.Dance:
				click_skill.skill_detail.attack_delay = EditorGUILayout.FloatField ("     Attack_delay", click_skill.skill_detail.attack_delay);
				break;
			case Machine_Type.拍擊:
				click_skill.skill_detail.only_slash_once = EditorGUILayout.Toggle ("     Only_slash_once", click_skill.skill_detail.only_slash_once);
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				click_skill.skill_detail.attack_delay = EditorGUILayout.FloatField ("     Attack_delay", click_skill.skill_detail.attack_delay);
				break;
			}
				
			machine_attacks_range_folder = EditorGUILayout.Foldout (machine_attacks_range_folder, "     Attacks_range");
			if (!machine_attacks_range_folder)break;
			int pre_size = click_skill.skill_detail.attacks_range_size;
			click_skill.skill_detail.attacks_range_size = EditorGUILayout.IntField ("     Size", click_skill.skill_detail.attacks_range_size);


			Vector3[] new_vec = click_skill.skill_detail.attacks_range;
			click_skill.skill_detail.attacks_range = new Vector3[click_skill.skill_detail.attacks_range_size];
			for (int i = 0; i < Mathf.Min (pre_size, click_skill.skill_detail.attacks_range_size); i++)click_skill.skill_detail.attacks_range [i] = new_vec [i];


			for(int i=0;i<click_skill.skill_detail.attacks_range_size;i++)click_skill.skill_detail.attacks_range[i] = EditorGUILayout.Vector3Field ("", click_skill.skill_detail.attacks_range[i]);
			break;

		case Skill_Type.Magic:
			click_skill.skill_detail.magic_Type = (Magic_Type)EditorGUILayout.EnumPopup ("     Type", click_skill.skill_detail.magic_Type);
			switch (click_skill.skill_detail.magic_Type) {
			case Magic_Type.pure_damage:
				click_skill.skill_detail.only_slash_once = EditorGUILayout.Toggle ("     Only_slash_once", click_skill.skill_detail.only_slash_once);
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				click_skill.skill_detail.attack_delay = EditorGUILayout.FloatField ("     Attack_delay", click_skill.skill_detail.attack_delay);
				break;
			case Magic_Type.effect_damage:
				click_skill.skill_detail.only_slash_once = EditorGUILayout.Toggle ("     Only_slash_once", click_skill.skill_detail.only_slash_once);
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				click_skill.skill_detail.buff_Type = (Buff_Type)EditorGUILayout.EnumPopup ("     Buff_Type", click_skill.skill_detail.buff_Type);
				click_skill.skill_detail.para1 = EditorGUILayout.IntField ("     Para1", click_skill.skill_detail.para1);
				click_skill.skill_detail.turn = EditorGUILayout.IntField ("     Turn", click_skill.skill_detail.turn);
				click_skill.skill_detail.attack_delay = EditorGUILayout.FloatField ("     Attack_delay", click_skill.skill_detail.attack_delay);
				break;
			case Magic_Type.knockback:
				click_skill.skill_detail.only_slash_once = EditorGUILayout.Toggle ("     Only_slash_once", click_skill.skill_detail.only_slash_once);
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				click_skill.skill_detail.hit_wall_deal_damage = EditorGUILayout.FloatField ("     Hit_wall_deal_damage", click_skill.skill_detail.hit_wall_deal_damage);
				click_skill.skill_detail.hit_wall_cancel_skill = EditorGUILayout.Toggle ("     Hit_wall_cancel_skill", click_skill.skill_detail.hit_wall_cancel_skill);
				click_skill.skill_detail.attack_delay = EditorGUILayout.FloatField ("     Attack_delay", click_skill.skill_detail.attack_delay);
				break;
			case Magic_Type.危險感應:
				click_skill.skill_detail.refill_mp = EditorGUILayout.IntField ("     Refill_mp", click_skill.skill_detail.refill_mp);
				click_skill.skill_detail.refill_ep = EditorGUILayout.IntField ("     Refill_ep", click_skill.skill_detail.refill_ep);
				break;
			case Magic_Type.石療:
				click_skill.skill_detail.refill_hp = EditorGUILayout.IntField ("     Refill_hp", click_skill.skill_detail.refill_hp);
				break;
			}

			machine_attacks_range_folder = EditorGUILayout.Foldout (machine_attacks_range_folder, "     Attacks_range");
			if (!machine_attacks_range_folder)break;
			pre_size = click_skill.skill_detail.attacks_range_size;
			click_skill.skill_detail.attacks_range_size = EditorGUILayout.IntField ("     Size", click_skill.skill_detail.attacks_range_size);


			new_vec = click_skill.skill_detail.attacks_range;
			click_skill.skill_detail.attacks_range = new Vector3[click_skill.skill_detail.attacks_range_size];
			for (int i = 0; i < Mathf.Min (pre_size, click_skill.skill_detail.attacks_range_size); i++)click_skill.skill_detail.attacks_range [i] = new_vec [i];


			for(int i=0;i<click_skill.skill_detail.attacks_range_size;i++)click_skill.skill_detail.attacks_range[i] = EditorGUILayout.Vector3Field ("", click_skill.skill_detail.attacks_range[i]);
			break;
		case Skill_Type.Mix:
			click_skill.skill_detail.mix_Type = (Mix_Type)EditorGUILayout.EnumPopup ("     Type", click_skill.skill_detail.mix_Type);
			switch (click_skill.skill_detail.mix_Type) {
			case Mix_Type.pure_damage:
				click_skill.skill_detail.only_slash_once = EditorGUILayout.Toggle ("     Only_slash_once", click_skill.skill_detail.only_slash_once);
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				click_skill.skill_detail.attack_delay = EditorGUILayout.FloatField ("     Attack_delay", click_skill.skill_detail.attack_delay);
				break;
			case Mix_Type.buff_damage:
				click_skill.skill_detail.only_slash_once = EditorGUILayout.Toggle ("     Only_slash_once", click_skill.skill_detail.only_slash_once);
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				click_skill.skill_detail.buff_Type = (Buff_Type)EditorGUILayout.EnumPopup ("     Buff_Type", click_skill.skill_detail.buff_Type);
				click_skill.skill_detail.para1 = EditorGUILayout.IntField ("     Para1", click_skill.skill_detail.para1);
				click_skill.skill_detail.turn = EditorGUILayout.IntField ("     Turn", click_skill.skill_detail.turn);
				click_skill.skill_detail.attack_delay = EditorGUILayout.FloatField ("     Attack_delay", click_skill.skill_detail.attack_delay);
				break;
			case Mix_Type.影穿:
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				break;
			}

			machine_attacks_range_folder = EditorGUILayout.Foldout (machine_attacks_range_folder, "     Attacks_range");
			if (!machine_attacks_range_folder)break;
			pre_size = click_skill.skill_detail.attacks_range_size;
			click_skill.skill_detail.attacks_range_size = EditorGUILayout.IntField ("     Size", click_skill.skill_detail.attacks_range_size);


			new_vec = click_skill.skill_detail.attacks_range;
			click_skill.skill_detail.attacks_range = new Vector3[click_skill.skill_detail.attacks_range_size];
			for (int i = 0; i < Mathf.Min (pre_size, click_skill.skill_detail.attacks_range_size); i++)click_skill.skill_detail.attacks_range [i] = new_vec [i];


			for(int i=0;i<click_skill.skill_detail.attacks_range_size;i++)click_skill.skill_detail.attacks_range[i] = EditorGUILayout.Vector3Field ("", click_skill.skill_detail.attacks_range[i]);

			break;
		case Skill_Type.Item:
			click_skill.skill_detail.item_Type = (Item_Type)EditorGUILayout.EnumPopup ("     Type", click_skill.skill_detail.item_Type);
			break;
		
		case Skill_Type.Passive:
			click_skill.skill_detail.passive_Type = (Passive_Type)EditorGUILayout.EnumPopup ("     Type", click_skill.skill_detail.passive_Type);
			click_skill.skill_detail.passive_skill_launch_time = (Passive_skill_launch_time)EditorGUILayout.EnumPopup ("     Passive_skill_launch_time", click_skill.skill_detail.passive_skill_launch_time);
			switch (click_skill.skill_detail.passive_Type) {
			case Passive_Type.吞噬:
				click_skill.skill_detail.refill_hp = EditorGUILayout.IntField ("     Refill_hp", click_skill.skill_detail.refill_hp);
				break;
			case Passive_Type.岩石裝甲:
				break;
			case Passive_Type.鏡像:
				break;
			case Passive_Type.首次攻擊額外傷害:
				click_skill.skill_detail.dmg_rate = EditorGUILayout.FloatField ("     Dmg_rate", click_skill.skill_detail.dmg_rate);
				break;
			}
			break;
		}
	}
}

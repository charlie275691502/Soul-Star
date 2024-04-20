using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Detail_manager : MonoBehaviour {
	public DataController dataController;
	public Character_window_controller character_window_controller;

	public SpriteRenderer headphoto;
	public Text character_name_text;
	public Text lv_text;
	public Text exp_text;
	public Text soul_text;
	public Text hp_text;
	public Text mp_text;
	public Text ep_text;
	public Text psy_atk_text;
	public Text psy_def_text;
	public Text spell_atk_text;
	public Text spell_def_text;
	public Text technique_text;
	public Text intelligence_text;
	public Text agility_text;

	public void Awake(){
		dataController = GameObject.Find ("DataController").GetComponent<DataController> ();
	}

	public void display_character_data(int order){
		CharacterData characterData = dataController.data.characterData [order];


		headphoto.sprite = characterData.poster;
		character_name_text.text = characterData.Name;
		lv_text.text = characterData.lv.ToString();
		exp_text.text = characterData.exp.ToString();
		soul_text.text = characterData.soul.ToString();

		int hp = characterData.hp;
		int mp = characterData.mp;
		int ep = characterData.ep;
		int psy_atk = characterData.psy_atk;
		int psy_def = characterData.psy_def;
		int mag_atk = characterData.mag_atk;
		int mag_def = characterData.mag_def;
		int TEC = characterData.TEC;
		int INT = characterData.INT;
		int AGI = characterData.AGI;


		for (int i=0;i< character_window_controller.equip_manager.slots.Length;i++) {
			if (character_window_controller.equip_manager.slots_equips[i] == null) continue;
			Equip equip = character_window_controller.equip_manager.slots_equips [i].GetComponent<Click_equip>().equip;
			hp += equip.hp;
			mp += equip.mp;
			ep += equip.ep;
			psy_atk += equip.psy_atk;
			psy_def += equip.psy_def;
			mag_atk += equip.mag_atk;
			mag_def += equip.mag_def;
			TEC += equip.TEC;
			INT += equip.INT;
			AGI += equip.AGI;
		}


		hp_text.text = hp.ToString();
		mp_text.text = mp.ToString();
		ep_text.text = ep.ToString();
		psy_atk_text.text = psy_atk.ToString();
		psy_def_text.text = psy_def.ToString();
		spell_atk_text.text = mag_atk.ToString();
		spell_def_text.text = mag_def.ToString();
		technique_text.text = TEC.ToString();
		intelligence_text.text = INT.ToString();
		agility_text.text = AGI.ToString();

	}
}

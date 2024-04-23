using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public enum Equip_type{
	LongSword,
	ShortSword,
	Clip,
	Crystal,
	Core,
	Shell,
	Grip,
	Rings,
	Potion,
	Loots
}

public enum Character_location{
	South,
	North,
	Unavailable
}

[System.Serializable]
public class CharacterData{
	public int id; // save
	public string Name;
	public Sprite avatar;
	public Sprite poster;
	public int lv; // save
	public int exp; // save
	public int soul; // save

	// load from config //

	public int hp;
	public int mp;
	public int ep;
	public int psy_atk;
	public int psy_def;
	public int mag_atk;
	public int mag_def;
	public int TEC;
	public int INT;
	public int AGI;
	public Equip_type[] equip_tpyps;

	//---------------------//

//	public Job job;
	public Character_location character_location; // save
	public bool is_main_character; // save
	public int skill_set_length;
	public int[] skill_set;
	public int equip_length;
	public int[] equip_set;

	public void new_data(int id, Character_location character_location, Config config){
		this.id = id;
		Name = config.characters_name [id];
		avatar = config.character_avatar [id];
		poster = config.character_poster [id];
		lv = 1;
		exp = 0;
		soul = 3;

		hp = config.character_lv_data[id].lv[lv-1].hp;
		mp = config.character_lv_data[id].lv[lv-1].mp;
		ep = config.character_lv_data[id].lv[lv-1].ep;
		psy_atk = config.character_lv_data[id].lv[lv-1].psy_atk;
		psy_def = config.character_lv_data[id].lv[lv-1].psy_def;
		mag_atk = config.character_lv_data[id].lv[lv-1].mag_atk;
		mag_def = config.character_lv_data[id].lv[lv-1].mag_def;
		TEC = config.character_lv_data[id].lv[lv-1].TEC;
		INT = config.character_lv_data[id].lv[lv-1].INT;
		AGI = config.character_lv_data[id].lv[lv-1].AGI;
		equip_tpyps = config.character_Eqiup_type[id].equip_types;

		this.character_location = character_location;
		is_main_character = true;

		skill_set_length = config.skill_set_config[id].array.Length;
		skill_set = config.skill_set_config [id].array;
		equip_length = 3;
		equip_set = new int[3]{0, 0, 0};
	}

	public void save_data(string name){
		PlayerPrefs.SetInt (name + "id", id);
		PlayerPrefs.SetInt (name + "lv", lv);
		PlayerPrefs.SetInt (name + "exp", exp);
		PlayerPrefs.SetString (name + "character_location", character_location.ToString());
		PlayerPrefs.SetInt (name + "soul", soul);
		PlayerPrefs.SetInt (name + "is_main_character", is_main_character ? 1 : 0);

		PlayerPrefs.SetInt (name + "skill_set_length", skill_set.Length);
		for(int i=0;i<skill_set.Length;i++) PlayerPrefs.SetInt (name + "skill_set" + i.ToString(), skill_set[i]);
		PlayerPrefs.SetInt (name + "equip_set_length", equip_set.Length);
		for(int i=0;i<equip_set.Length;i++) PlayerPrefs.SetInt (name + "equip_set" + i.ToString(), equip_set[i]);
	}

	public void load_data(string name, Config config){
		id = PlayerPrefs.GetInt (name + "id");
		Name = config.characters_name [id];
		avatar = config.character_avatar [id];
		poster = config.character_poster [id];
		lv = PlayerPrefs.GetInt (name + "lv");
		exp = PlayerPrefs.GetInt (name + "exp");
		soul = PlayerPrefs.GetInt (name + "soul");

		hp = config.character_lv_data[id].lv[lv-1].hp;
		mp = config.character_lv_data[id].lv[lv-1].mp;
		ep = config.character_lv_data[id].lv[lv-1].ep;
		psy_atk = config.character_lv_data[id].lv[lv-1].psy_atk;
		psy_def = config.character_lv_data[id].lv[lv-1].psy_def;
		mag_atk = config.character_lv_data[id].lv[lv-1].mag_atk;
		mag_def = config.character_lv_data[id].lv[lv-1].mag_def;
		TEC = config.character_lv_data[id].lv[lv-1].TEC;
		INT = config.character_lv_data[id].lv[lv-1].INT;
		AGI = config.character_lv_data[id].lv[lv-1].AGI;
		equip_tpyps = config.character_Eqiup_type[id].equip_types;

		//job
		character_location = (Character_location) System.Enum.Parse( typeof( Character_location ), PlayerPrefs.GetString (name + "character_location"));
		is_main_character = (PlayerPrefs.GetInt (name + "is_main_character") == 1);

		skill_set_length = PlayerPrefs.GetInt (name + "skill_set_length");
		skill_set = new int[skill_set_length];
		for(int i=0;i<skill_set_length;i++) skill_set[i] = PlayerPrefs.GetInt (name + "skill_set" + i.ToString());
		equip_length = PlayerPrefs.GetInt (name + "equip_set_length");
		equip_set = new int[equip_length];
		for(int i=0;i<equip_length;i++) equip_set[i] = PlayerPrefs.GetInt (name + "equip_set" + i.ToString());
	}

	public void Add_exp(int x, Config config){
		exp += x;
		while (exp >= config.exp_module [lv]) {
			exp -= config.exp_module [lv];
			lv++;
			hp = config.character_lv_data[id].lv[lv-1].hp;
			mp = config.character_lv_data[id].lv[lv-1].mp;
			ep = config.character_lv_data[id].lv[lv-1].ep;
			psy_atk = config.character_lv_data[id].lv[lv-1].psy_atk;
			psy_def = config.character_lv_data[id].lv[lv-1].psy_def;
			mag_atk = config.character_lv_data[id].lv[lv-1].mag_atk;
			mag_def = config.character_lv_data[id].lv[lv-1].mag_def;
			TEC = config.character_lv_data[id].lv[lv-1].TEC;
			INT = config.character_lv_data[id].lv[lv-1].INT;
			AGI = config.character_lv_data[id].lv[lv-1].AGI;
			equip_tpyps = config.character_Eqiup_type[id].equip_types;
		}
	}

	public void add_skill(int id){
		skill_set [id-1]++;
	}

	public void refetch_equip(string s, int[] ids){
		equip_set = new int[ids.Length];
		for (int i = 0; i < equip_set.Length; i++) equip_set [i] = ids [i];

		save_data (s);
	}
}

//第n章節 是否完成任務的bool陣列
[System.Serializable]
public class Chapter_mission_completeds{
	public int mission_completeds_length;
	public bool[] mission_completeds;
}

[System.Serializable]
public class Data{
	[HideInInspector] public int order;

	[HideInInspector] public int main_progress;
	[HideInInspector] public int chapter_mission_completeds_length;
	[HideInInspector] public Chapter_mission_completeds[] chapter_mission_completeds;
	[HideInInspector] public int characterDataLength;
	[HideInInspector] public CharacterData[] characterData;
	[HideInInspector] public int equip_in_bag_Length;
	[HideInInspector] public int[] equip_in_bag;

	public void save_data(){
		PlayerPrefs.SetInt (order.ToString () + "main_progress", main_progress);

		PlayerPrefs.SetInt (order.ToString () + "chapter_mission_completeds_length", chapter_mission_completeds.Length);
		for (int i = 0; i < chapter_mission_completeds.Length; i++) {
			PlayerPrefs.SetInt (order.ToString () + "chapter_mission_completeds" + i.ToString () + "mission_completeds_length", chapter_mission_completeds[i].mission_completeds.Length);
			for (int j = 0; j < chapter_mission_completeds [i].mission_completeds.Length; j++) {
				PlayerPrefs.SetInt (order.ToString () + "chapter_mission_completeds" + i.ToString () + "mission_completeds" + j.ToString (), (chapter_mission_completeds [i].mission_completeds [j]) ? 1 : 0);
			}
		}

		PlayerPrefs.SetInt (order.ToString () + "characterDataLength", characterData.Length);
		for(int i=0;i<characterData.Length;i++)characterData[i].save_data (order.ToString () + i.ToString() + "characterData");

		PlayerPrefs.SetInt (order.ToString () + "equip_in_bag_Length", equip_in_bag.Length);
		for (int i = 0; i < equip_in_bag.Length; i++) PlayerPrefs.SetInt (order.ToString () + i.ToString() + "equip_in_bag", equip_in_bag[i]);
	}

	public void load_data(){
		main_progress = PlayerPrefs.GetInt (order.ToString () + "main_progress");
		characterDataLength = PlayerPrefs.GetInt (order.ToString () + "characterDataLength");

		chapter_mission_completeds_length = PlayerPrefs.GetInt (order.ToString () + "chapter_mission_completeds_length");
		chapter_mission_completeds = new Chapter_mission_completeds[chapter_mission_completeds_length];
		for (int i = 0; i < chapter_mission_completeds_length; i++) {
			chapter_mission_completeds [i] = new Chapter_mission_completeds ();
			chapter_mission_completeds[i].mission_completeds_length = PlayerPrefs.GetInt (order.ToString () + "chapter_mission_completeds" + i.ToString () + "mission_completeds_length");
			chapter_mission_completeds[i].mission_completeds = new bool[chapter_mission_completeds[i].mission_completeds_length];
			for (int j = 0; j < chapter_mission_completeds [i].mission_completeds_length; j++) {
				chapter_mission_completeds [i].mission_completeds [j] = (PlayerPrefs.GetInt (order.ToString () + "chapter_mission_completeds" + i.ToString () + "mission_completeds" + j.ToString ())==1) ? true : false;
			}
		}

		characterData = new CharacterData[characterDataLength];
		for (int i = 0; i < characterDataLength; i++) {
			characterData [i] = new CharacterData ();
			characterData [i].load_data (order.ToString () + i.ToString () + "characterData", GameObject.Find ("DataController").GetComponent<DataController> ().config);
		}
	

		equip_in_bag_Length = PlayerPrefs.GetInt (order.ToString () + "equip_in_bag_Length");
		equip_in_bag = new int[equip_in_bag_Length];
		for (int i = 0; i < equip_in_bag_Length; i++) equip_in_bag[i] = PlayerPrefs.GetInt (order.ToString () + i.ToString() + "equip_in_bag");
	}

	public void delete_data(){
		main_progress = 0;
		characterDataLength = 0;
		save_data ();
	}

	public void new_data(){
		Debug.Log ("successfully new a character data");
		main_progress = 1; // beginner = 1

		chapter_mission_completeds = new Chapter_mission_completeds[1];
		chapter_mission_completeds[0] = new Chapter_mission_completeds();
		chapter_mission_completeds[0].mission_completeds = new bool[]{ false, false, false, false, false, false, false, false, false, false, false }; // 11

		characterData = new CharacterData[1] {new CharacterData()};
		Config config = GameObject.Find ("DataController").GetComponent<DataController> ().config;
		characterData [0].new_data (1, Character_location.North, config);

		equip_in_bag = new int[1]{1};

		save_data ();
	}

	public void add_character(int id){
		characterDataLength = PlayerPrefs.GetInt (order.ToString () + "characterDataLength");
		characterData = new CharacterData[characterDataLength + 1];
		for (int i = 0; i < characterDataLength; i++) {
			characterData [i] = new CharacterData ();
			characterData [i].load_data (order.ToString () + i.ToString () + "characterData", GameObject.Find ("DataController").GetComponent<DataController> ().config);
		}
		characterData [characterDataLength] = new CharacterData ();
		characterData [characterDataLength].new_data(id, Character_location.North, GameObject.Find ("DataController").GetComponent<DataController> ().config);
		save_data ();
	}

	public void add_equip(int id){
		equip_in_bag_Length = PlayerPrefs.GetInt (order.ToString () + "equip_in_bag_Length");
		equip_in_bag = new int[equip_in_bag_Length + 1];
		for (int i = 0; i < equip_in_bag_Length; i++) equip_in_bag[i] = PlayerPrefs.GetInt (order.ToString () + i.ToString() + "equip_in_bag");

		equip_in_bag [equip_in_bag_Length] = id;
		save_data ();
	}
}

[System.Serializable]
public class Character_lv_data{
	public CharacterData[] lv;
}

[System.Serializable]
public class StageInformation{
	public int stage_chapter;
	public int stage_number;
	public string stage_name = "";
	public string suggest_element = "";
	public int team_limit_number;
	public string[] team_limit;
	public string stage_introduction = "";
	public bool has_start_story;
	public bool has_end_story;
	public Victory_condition victory_condition;
	public Defeat_condition defeat_condition;
	public Vector3[] enemys_config;
	public int exp;
	public Vector2[] drop_equips_config;

	public StageInformation(string[] informations){
		FieldInfo[] fields = this.GetType ().GetFields ();
		int ink = 1;
		for(int i=0;i<fields.Length;i++){
			if (fields [i].Name == "stage_chapter") {
				string[] temp = informations [ink++].Split ('-');
				stage_chapter = int.Parse(temp [0]);
				stage_number = int.Parse (temp [1]);
				i++;
				continue;
			}
			if(fields [i].Name == "team_limit"){
				team_limit = informations[ink++].Split(' ');
				continue;
			}
			if (fields [i].Name == "stage_introduction") {
				string temp = "";
				while (ink < informations.Length && informations [ink++] != "@") temp += informations [ink-1] + "\n";
				stage_introduction = temp;
				continue;
			}
			if (fields [i].Name == "has_start_story") {
				string[] temp = informations [ink++].Split (' ');
				has_start_story = (temp [0] == "y");
				has_end_story = (temp [1] == "y");
				i++;
				continue;
			}
			if (fields [i].Name == "victory_condition") {
				victory_condition = (Victory_condition)System.Enum.Parse (typeof(Victory_condition), informations [ink++]);
				defeat_condition = (Defeat_condition)System.Enum.Parse (typeof(Defeat_condition), informations [ink++]);
				i++;
				continue;
			}
			if (fields [i].Name == "enemys_config") {
				int length = int.Parse(informations [ink++]);
				enemys_config = new Vector3[length];
				for (int j = 0; j < length; j++) {
					string[] temp = informations [ink++].Split (' ');
					enemys_config [j] = new Vector3 (int.Parse(temp[0]), int.Parse(temp[1]), int.Parse(temp[2]));
				}
				continue;
			}
			if (fields [i].Name == "drop_equips_config") {
				int length = int.Parse(informations [ink++]);
				drop_equips_config = new Vector2[length];
				for (int j = 0; j < length; j++) {
					string[] temp = informations [ink++].Split (' ');
					drop_equips_config [j] = new Vector2 (int.Parse(temp[0]), int.Parse(temp[1]));
				}
				continue;
			}
			if (fields [i].GetValue(this).GetType () == typeof(int))
				fields [i].SetValue (this, int.Parse(informations[ink++]));
			else 
				fields[i].SetValue (this, informations[ink++]);
		}
	}
}

[System.Serializable]
public class Stage_loading_data{
	public int[] allies_order;
}

[System.Serializable]
public class Skill_set_config{
	public int[] array;
}

[System.Serializable]
public class Character_Eqiup_type{
	public Equip_type[] equip_types;
}


[System.Serializable]
public class Config{
	[HideInInspector] public int[] exp_module;
	public TextAsset exp_module_text;
	public string[] characters_name;
	public TextAsset[] character_lv_text;
	[HideInInspector] public Character_lv_data[] character_lv_data;
	public Skill_set_config[] skill_set_config;
	public Sprite[] character_avatar;
	public Sprite[] character_poster;
	public Character_Eqiup_type[] character_Eqiup_type;
	public TextAsset stage_information_text;
	[HideInInspector] public List<StageInformation> stageInformation = new List<StageInformation>();
}

public class DataController : MonoBehaviour {
	public bool cheat_mod;
	public Config config;
	public Data data;
	public Stage_loading_data stage_loading_data;
	public StageInformation stageInformation;

	void Awake(){
		DontDestroyOnLoad (gameObject);
		string[] exp_data = config.exp_module_text.text.Split ('\n');
		int length = exp_data.Length;
		config.exp_module = new int[length];
		for (int i = 0; i < length; i++) config.exp_module[i] = int.Parse(exp_data[i]);

		//express 角色數值資訊
		config.character_lv_data = new Character_lv_data[config.character_lv_text.Length];
		for (int i = 1; i < config.character_lv_text.Length; i++) {
			string[] character_numbers = config.character_lv_text[i].text.Split ('\n');
			config.character_lv_data [i] = new Character_lv_data ();
			config.character_lv_data [i].lv = new CharacterData[character_numbers.Length];
			for (int j=0;j<character_numbers.Length;j++) {
				string[] temp = character_numbers[j].Split(' ');
				config.character_lv_data[i].lv[j] = new CharacterData();
				config.character_lv_data[i].lv[j].hp = int.Parse(temp[0]);
				config.character_lv_data[i].lv[j].mp = int.Parse(temp[1]);
				config.character_lv_data[i].lv[j].ep = int.Parse(temp[2]);
				config.character_lv_data[i].lv[j].psy_atk = int.Parse(temp[3]);
				config.character_lv_data[i].lv[j].psy_def = int.Parse(temp[4]);
				config.character_lv_data[i].lv[j].mag_atk = int.Parse(temp[5]);
				config.character_lv_data[i].lv[j].mag_def = int.Parse(temp[6]);
				config.character_lv_data[i].lv[j].TEC = int.Parse(temp[7]);
				config.character_lv_data[i].lv[j].INT = int.Parse(temp[8]);
				config.character_lv_data[i].lv[j].AGI = int.Parse(temp[9]);
			}
		}

		// express 關卡資訊

		string[] stage_source_datas = config.stage_information_text.text.Split('#');
		foreach (string stage_source_data in stage_source_datas) {
//			Debug.Log (stage_source_data);
			string[] stage_datas = stage_source_data.Split ('\n');
			config.stageInformation.Add (new StageInformation(stage_datas));
		}
	}
}

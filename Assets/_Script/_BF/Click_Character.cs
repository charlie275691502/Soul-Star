using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Buff_Type{
	灼傷,
	標記,
	感應,
	攻防提升,
	機械減傷,
	魔法減傷,
	提昇首次攻擊
}

public class Buff{
	public Buff_Type buff_type;
	public float para1;
	public int turn;

	public Buff(Buff_Type abuff_type, float apara1, int aturn){
		buff_type = abuff_type;
		para1 = apara1;
		turn = aturn;
	}
}

[System.Serializable]
public class Buffs{
	public Buff[] buff = new Buff[0];

	public void Add(Buff new_buff){
		if (has_buff (new_buff.buff_type)) for (int i = 0; i < buff.Length; i++)if (buff [i].buff_type == new_buff.buff_type) {
			buff [i] = new_buff;
			return;
		}
		Buff[] temp = new Buff[buff.Length];
		for (int i = 0; i < buff.Length; i++)temp [i] = buff [i];
		buff = new Buff[buff.Length + 1];
		for (int i = 0; i < buff.Length - 1; i++)buff [i] = temp [i];
		buff [buff.Length - 1] = new_buff;
	}

	public void Remove(Buff_Type remove_buff_type){
		int ink = buff.Length;
		foreach (Buff b in buff) {
			if (b.buff_type == remove_buff_type)
				ink--;
		}

		Buff[] temp = new Buff[buff.Length];
		for (int i = 0; i < buff.Length; i++)temp [i] = buff [i];
		buff = new Buff[ink];
		ink = 0;
		for (int i = 0; i < temp.Length; i++)if(temp[i].buff_type != remove_buff_type)buff [ink++] = temp [i];
	}

	public bool has_buff(Buff_Type buff_type){
		for(int i=0;i<buff.Length;i++)if(buff[i].buff_type == buff_type)return true;
		return false;
	}

	public float buff_para(Buff_Type buff_type){
		for(int i=0;i<buff.Length;i++)if(buff[i].buff_type == buff_type)return buff[i].para1;
		return 0;
	}

	public void end_turn(Character character){
		if (has_buff (Buff_Type.灼傷))character.deal_dmg (-1, Damage_Type.真實, Property.火, null);


		int ink = 0;
		Buff[] temp = new Buff[buff.Length];
		for (int i = 0; i < buff.Length; i++) if (--buff [i].turn > 0) temp [ink++] = buff [i];
		buff = new Buff[ink];
		for (int i = 0; i < ink; i++) buff [i] = temp [i];
	}
}

[System.Serializable]
public enum Character_camp{
	allies,
	enemys,
	obstacles
}

[System.Serializable]
public class Character{
	[HideInInspector] public GameObject gmo;
	[HideInInspector] public int index;
	[HideInInspector] public int order;
	[HideInInspector] public Character_camp character_camp;
	[HideInInspector] public Vector3 loc;
	public int lv;
	[HideInInspector] public int hp;
	[HideInInspector] public int mp;
	[HideInInspector] public int ep;
	public Sprite skillsbox_image;
	public Sprite job_icon;
	public string Name;
	public Property property;
	public int max_hp;
	public int max_mp;
	public int max_ep;
	public int psy_atk;
	public int psy_def;
	public int mag_atk;
	public int mag_def;
	public int TEC;
	public int INT;
	public int AGI;
	public int hp_regain;
	public int mp_regain;
	public int ep_regain;
	public Sprite head;
	public Image hp_bar;
	public Transform image;
	[HideInInspector] public GameObject[] skills;
	public GameObject[] skills_prefab;
	public GameObject[] init_skills;
	[HideInInspector] public Buffs buffs;
	[HideInInspector] public int this_turn_damage;

	[HideInInspector] public GameController gameController;

	public void regain(){
		if(hp_regain > 0)gameController.StartCoroutine(add_hp (hp_regain));
		add_mp (mp_regain);
		add_ep (ep_regain);
		buffs.end_turn (this);
		this_turn_damage = 0;
	}

	public void deal_dmg(float dmg_rate, Damage_Type attack_damage_type, Property attack_damage_property, Character attacker){
		int dmg = adjust_damage(dmg_rate, attack_damage_type, attack_damage_property, attacker);
		gameController.StartCoroutine(add_hp(-dmg));
	}

	public void deal_Dance_dmg(Character attacker){
		int dmg = (int)Mathf.Min((float)(attacker.psy_atk + psy_atk- TEC - psy_def) * (float)max_hp, 0.17f * (float)max_hp);
		gameController.StartCoroutine(add_hp(-dmg));
	}

	public IEnumerator add_hp(int x){
		if(character_camp != Character_camp.obstacles){
			GameObject temp = (GameObject) GameObject.Instantiate (gameController.acts.jump_damage, gmo.transform.position, Quaternion.identity);
			temp.GetComponent<jump_damage> ().damage = -x;
			hp = Mathf.Clamp(hp + x, 0, max_hp);
			hp_bar.fillAmount = ((float)hp / (float)max_hp) * (gameController.acts.hp_rate.y - gameController.acts.hp_rate.x) + gameController.acts.hp_rate.x;
			if (gameController.details.now_character.character_camp == character_camp && gameController.details.now_character.order == order) {
				gameController.details.detail_hp_bar.fillAmount = ((float)hp / (float)max_hp);
				gameController.details.hp_text.text = hp.ToString();
				gameController.details.max_hp_text.text = max_hp.ToString();
			}
			if (hp == 0) {
				gameController.acts.someone_die = true;
				gameController.StartCoroutine (died_animate());

				if(Name == "輻射香菇")gameController.trigger_all_passive_skill (Passive_skill_launch_time.立即, "吞噬");
			}
			yield return null;
		}
	}

	public void add_mp(int x){
		mp = Mathf.Clamp(mp + x, 0, max_mp); 
		if (gameController.details.now_character.character_camp == character_camp && gameController.details.now_character.order == order){
			gameController.details.detail_mp_bar.fillAmount = ((float)mp / (float)max_mp);
			gameController.details.mp_text.text = mp.ToString();
			gameController.details.max_mp_text.text = max_mp.ToString();
		}
	}

	public void add_ep(int x){
		ep = Mathf.Clamp(ep + x, 0, max_ep);
		if (gameController.details.now_character.character_camp == character_camp && gameController.details.now_character.order == order){
			gameController.details.detail_ep_bar.fillAmount = ((float)ep / (float)max_ep);
			gameController.details.ep_text.text = ep.ToString();
			gameController.details.max_ep_text.text = max_ep.ToString();
		}
	}

	public int adjust_damage(float dmg_rate, Damage_Type attack_damage_type, Property attack_damage_property, Character attacker){

		float dmg = 0;

		//psy_atk*技能傷害*(1+技術差*0.01)-enemy_psy_def*(1-技術差*0.01))*{2^(lv/10}*(ＢＵＦＦ)
		if (attack_damage_type == Damage_Type.機械)
			dmg = ( (float)attacker.psy_atk * dmg_rate * (1.0f + (attacker.TEC - TEC) * 0.01f) - (float)psy_def * (1.0f - (attacker.TEC - TEC) * 0.01f) ) * Mathf.Pow(2.0f, (float)attacker.lv / 10.0f); // * buff

		//{spell_atk*技能傷害*(1+智力差*0.01)-enemy_spell_def*(1-智力差*0.01)}*{2^(lv/10}*(ＢＵＦＦ)*(剋屬1.25 被剋屬0.9)
		if (attack_damage_type == Damage_Type.魔法)
			dmg = ( (float)attacker.mag_atk * dmg_rate * (1.0f + (attacker.INT - INT) * 0.01f) - (float)mag_def * (1.0f - (attacker.INT - INT ) * 0.01f) ) * Mathf.Pow(2.0f, (float)attacker.lv / 10.0f) * gameController.Compute_counter_property(attack_damage_property, property); // * buff

		//{psy_atk*技能傷害*(1+技術差*0.01)+spell_atk*技能傷害*(1+智力差*0.01)}*{2^(lv/10}*(ＢＵＦＦ)}*0.5
		if (attack_damage_type == Damage_Type.混合)
			dmg = (( (float)attacker.psy_atk * dmg_rate * (1.0f + (attacker.TEC - TEC) * 0.01f) + (float)attacker.mag_atk * dmg_rate * (1.0f + (attacker.INT + INT) * 0.01f) ) * Mathf.Pow(2.0f, (float)attacker.lv / 10.0f)) /* *buff */ * 0.5f;

		if(attacker.buffs.has_buff(Buff_Type.提昇首次攻擊)){
			dmg *= attacker.buffs.buff_para(Buff_Type.提昇首次攻擊);
			attacker.buffs.Remove(Buff_Type.提昇首次攻擊);
		}
		if(attack_damage_type == Damage_Type.機械 && buffs.has_buff(Buff_Type.機械減傷))dmg *= (1.0f-buffs.buff_para(Buff_Type.機械減傷));
		if(attack_damage_type == Damage_Type.魔法 && buffs.has_buff(Buff_Type.魔法減傷))dmg *= (1.0f-buffs.buff_para(Buff_Type.魔法減傷));
		
		Debug.Log (attacker.Name + " " +  dmg.ToString());
		//攻擊者減少攻擊
//		if (attacker_gmo != null) {
//			Character attacker = attacker_gmo.GetComponent<Click_Character> ().character;
//			if(attacker.buffs.find_buff(Buff_Type.標記))x = (int)((float)x * 0.8f);
//		}

		//防禦者減少傷害
//		int real_def = def;
//		if (buffs.find_buff (Buff_Type.感應))real_def += 20;
//		if (skill_Type == Skill_Type.Machine) {
//			if (!buffs.find_buff (Buff_Type.灼傷))x = (int)((float)x * (1.0f - (float)real_def / 100.0f));
//		}

		
		this_turn_damage -= (int)dmg;
		return (int)dmg;
	}

	bool all_died(Character[] characters){
		foreach(Character character in characters)if(character.hp > 0)return false;
		return true;
	}

	bool any_main_character_died(Character[] characters){
		foreach(Character character in characters)if(character.index <= 3 && character.hp <= 0)return true;
		return false;
	}

	public IEnumerator died_animate(){
		GameObject big_frame = character_camp == Character_camp.allies ? gameController.allies_big_frames [order] : gameController.enemys_big_frames [order];
		GameObject shines = character_camp == Character_camp.allies ? gameController.allies_shines [order] : gameController.enemys_shines [order];
		shines.SetActive (false);

		foreach (Transform shine in shines.transform) {
			GameObject skill = gameController.the_skill_in_frame (shine);
			if(skill != null)GameObject.Destroy (skill);
		}
		if (character_camp == Character_camp.allies) gameController.allies_head [order].SetActive (false);
		else gameController.enemys_head [order].SetActive (false);
		big_frame.SetActive(false);

		for (float i = 0; i < gameController.acts.die_delay; i += Time.deltaTime) {
			foreach (Transform child in image) {
				if(child.gameObject.GetComponent<Image> () != null)child.gameObject.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1 - (i / gameController.acts.die_delay));
				if(child.gameObject.GetComponent<SpriteRenderer> () != null)child.gameObject.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 1 - (i / gameController.acts.die_delay));
			}
			yield return null;
		}
		MonoBehaviour.Instantiate (gameController.acts.die_light, gmo.transform.position, Quaternion.identity);
		GameObject.Destroy (gmo);

		if (character_camp == Character_camp.enemys && all_died (gameController.enemys) && ( gameController.dataController.stageInformation.victory_condition == Victory_condition.DefeatAllEnemy || gameController.dataController.stageInformation.victory_condition == Victory_condition.SurviveFor10Turns)) {
			if (gameController.has_second_enemy) {
				gameController.has_second_enemy = false;
				gameController.acts.game_over = false;
				while(!gameController.can_edit)yield return null;
				gameController.init_second_enemys_character ();
			} else {
				gameController.StartCoroutine(gameController.victory_ani());
			}
		}

		if (character_camp == Character_camp.allies && any_main_character_died (gameController.allies) && gameController.dataController.stageInformation.defeat_condition == Defeat_condition.AnyAllyDied) {
				gameController.StartCoroutine(gameController.defeat_ani());
		}
	}
}

public class Click_Character: MonoBehaviour {
	public Character character;
	private GameController gameController;
	[HideInInspector] public Color color;

	void Start(){
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		character.gmo = gameObject;
		character.gameController = gameController;
	}

	void OnMouseDown(){
		if(character.character_camp == Character_camp.allies)gameController.details.change (gameController.allies [character.order]);
		else gameController.details.change (gameController.enemys [character.order]);
	}
}

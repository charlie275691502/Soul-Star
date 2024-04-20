using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class detail_box_controller : MonoBehaviour {
	public Transform folder;
	public GameObject background_gmo;
	private Vector2 background_init_position;
	private Vector2 background_init_scale;
	private float line_distance;
	private Vector2 mp_init_position;
	private float mp_distance;
	private float mp_ep_distance;
	public Skill_Type skill_type;
	public string skill_name;
	public int mp;
	public int ep;
	public string skill_desc;
	public int speed;
	public Text skill_name_text;
	public Text mp_text;
	public Text ep_text;
	public Text skill_detail_text;
	public Text priority_text;

	void Awake(){
		background_init_position = new Vector2 (7.4f, 9f);
		background_init_scale = new Vector2 (7.7f, 1.8f);
		line_distance = 0.7f;
		mp_init_position = new Vector2 (15.7f, 7.1f);
		mp_distance = 1.0f;
		mp_ep_distance = 2.2f;
	}

	// Use this for initialization
	void Start () {
		Click_skill click_skill = transform.parent.GetComponent<Click_skill> ();
		skill_type = click_skill.skill_detail.skill_type;
		skill_name = click_skill.Name;
		mp = click_skill.mp;
		ep = click_skill.ep;
		skill_desc = click_skill.skill_desc;
		speed = click_skill.speed;


		GameObject background = (GameObject)Instantiate (background_gmo, Vector2.zero, Quaternion.identity);
		background.transform.parent = folder;
		background.transform.localPosition = background_init_position;
		background.transform.localScale = background_init_scale + new Vector2 (0, ((skill_desc.Length - 1) / 15) * line_distance);
		skill_name_text.text = skill_name;
		mp_text.transform.localPosition = mp_init_position + new Vector2(mp_distance * skill_name.Length, 0.0f);
		mp_text.text = (mp == 0) ? "" : mp.ToString () + "MP";
		ep_text.transform.localPosition = mp_init_position + new Vector2(mp_distance * skill_name.Length + ((mp == 0) ? 0 : mp_ep_distance) + ((0 < mp && mp < 10) ? -0.5f : 0), 0.0f);
		ep_text.text = (ep == 0) ? "" : ep.ToString () + "EP";
		skill_detail_text.text = skill_desc;
		priority_text.text = (speed <= click_skill.gameController.acts.prioiriy_low_up_bound) ? "優先度低" : (speed <= click_skill.gameController.acts.prioiriy_mid_up_bound) ? "優先度中" : "優先度高";
		if (skill_type == Skill_Type.Move)   skill_name_text.color = new Color(208.0f / 256.0f, 146.0f / 256.0f, 256.0f / 256.0f, 1.0f);
		if (skill_type == Skill_Type.Machine)skill_name_text.color = new Color(221.0f / 256.0f,  33.0f / 256.0f,  33.0f / 256.0f, 1.0f);
		if (skill_type == Skill_Type.Magic)  skill_name_text.color = new Color(141.0f / 256.0f, 218.0f / 256.0f,  91.0f / 256.0f, 1.0f);
		if (skill_type == Skill_Type.Mix)    skill_name_text.color = new Color( 73.0f / 256.0f, 180.0f / 256.0f, 225.0f / 256.0f, 1.0f);
		if (skill_type == Skill_Type.Item)   skill_name_text.color = new Color(246.0f / 256.0f, 165.0f / 256.0f,  79.0f / 256.0f, 1.0f);
	}
}

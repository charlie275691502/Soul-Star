using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settlement_imformation_manager : MonoBehaviour {
	public GameController gameController;
	public DataController dataController;

	public float avatar_position_y;
	public float equip_dis;
	public GameObject[] character_gmo;
	public SpriteRenderer[] avatar_sr;
	public Text[] lv_text;
	public Image[] exp_image;

	public Transform drop_folder;

	void Awake(){
		gameController = GameObject.Find ("GameController").GetComponent<GameController> ();
		dataController = GameObject.Find ("DataController").GetComponent<DataController> ();
	}

	int add_exp;
	void Start(){
		int length = dataController.stageInformation.drop_equips_config.Length;
		
		//先處理掉落物

		//首通
		if (!dataController.data.chapter_mission_completeds [dataController.stageInformation.stage_chapter - 1].mission_completeds [dataController.stageInformation.stage_number - 1]) {
			add_exp = dataController.stageInformation.exp;
			drop_folder.localPosition = new Vector3 (-equip_dis * (float)(length - 1) / 2.0f, -15, 0);
			for (int i = 0; i < length; i++) {
				int order = (int)dataController.stageInformation.drop_equips_config [i].x;
				string order_string = ((order > 99) ? "" : (order > 9) ? "0" : "00") + order.ToString ();
				GameObject gmo = Instantiate ((GameObject)Resources.Load ("Prefab/Equips/Equips_" + order_string, typeof(GameObject)), Vector3.zero, Quaternion.identity);
				gmo.transform.parent = drop_folder;
				gmo.transform.localPosition = new Vector3 (i, 0, 0) * equip_dis;
				gmo.GetComponent<Click_equip> ().equip.order = order;
				gmo.GetComponent<Click_equip> ().equip.equip_type = (Equip_type)(order / 100);
				gmo.GetComponent<Click_equip> ().equip.number = (int)dataController.stageInformation.drop_equips_config [i].y;

				if (901 <= order && order <= 920) {
					dataController.data.add_character (int.Parse (gmo.GetComponent<Click_equip> ().equip.desc));
				} else if (921 <= order && order <= 999) {
					dataController.data.characterData [dataController.stage_loading_data.allies_order [0]].add_skill (int.Parse (gmo.GetComponent<Click_equip> ().equip.desc));
				} else {
					dataController.data.add_equip (order);
				}
			}
		} else {
			add_exp = (int)((float)dataController.stageInformation.exp * 0.7f);
		}



		length = dataController.stage_loading_data.allies_order.Length;
		character_gmo [0].SetActive (length > 0);
		character_gmo [1].SetActive (length > 1);
		character_gmo [2].SetActive (length > 2);
		if (length == 1) {
			character_gmo [0].transform.localPosition = new Vector3 (0, avatar_position_y, 0);
		} else if (length == 2) {
			character_gmo [0].transform.localPosition = new Vector3 (-4, avatar_position_y, 0);
			character_gmo [1].transform.localPosition = new Vector3 (4, avatar_position_y, 0);
		} else if (length == 3) {
			character_gmo [0].transform.localPosition = new Vector3 (-8, avatar_position_y, 0);
			character_gmo [1].transform.localPosition = new Vector3 (0, avatar_position_y, 0);
			character_gmo [2].transform.localPosition = new Vector3 (8, avatar_position_y, 0);
		}

		for (int i = 0; i < length; i++) {
			lv_text [i].text = dataController.data.characterData [dataController.stage_loading_data.allies_order[i]].lv.ToString();
			avatar_sr [i].sprite = dataController.data.characterData [dataController.stage_loading_data.allies_order[i]].avatar;
		}

		dataController.data.chapter_mission_completeds [dataController.stageInformation.stage_chapter - 1].mission_completeds [dataController.stageInformation.stage_number - 1] = true;
		if (/*是主線*/ dataController.stageInformation.stage_number <= 6)dataController.data.main_progress = dataController.stageInformation.stage_chapter * 100 + dataController.stageInformation.stage_number;

		StartCoroutine (Add_exp ());
	}

	IEnumerator Add_exp(){
		int length = dataController.stage_loading_data.allies_order.Length;
		int[] lv = new int[length];
		float[] exp = new float[length];
		float[] max_exp = new float[length];
		float time = gameController.acts.exp_fill_up_time;
		float eaps = (float)add_exp / time; // exp_add_per_sec
		for (int i = 0; i < length; i++) {
			lv [i] = dataController.data.characterData [dataController.stage_loading_data.allies_order[i]].lv;
			exp[i] = (float)dataController.data.characterData [dataController.stage_loading_data.allies_order[i]].exp;
			max_exp [i] = (float)dataController.config.exp_module [lv [i]];
		}

		for (int j = 0; j < length; j++) {
			exp_image [j].fillAmount = exp [j] / max_exp [j];
		}

		for (int i = 0; i < length; i++) dataController.data.characterData [dataController.stage_loading_data.allies_order[i]].Add_exp(add_exp, dataController.config);
		dataController.data.save_data();

		yield return new WaitForSeconds (0.3f);

		for (float i = 0; i < time; i += Time.deltaTime) {
			for (int j = 0; j < length; j++) {
				exp [j] += eaps * Time.deltaTime;
				exp_image [j].fillAmount = exp [j] / max_exp [j];
				if (exp_image [j].fillAmount == 1) {
					exp [j] -= max_exp [j];
					lv [j]++;
					max_exp[j] = (float)dataController.config.exp_module [lv [j]];
					lv_text [j].text = lv [j].ToString();
				}
			}
			yield return null;
		}

		for (int i = 0; i < length; i++) {
			lv_text [i].text = dataController.data.characterData [dataController.stage_loading_data.allies_order [i]].lv.ToString ();
			exp_image[i].fillAmount = (float)dataController.data.characterData [dataController.stage_loading_data.allies_order [i]].exp / (float)dataController.config.exp_module [dataController.data.characterData [dataController.stage_loading_data.allies_order [i]].lv];
		}
	}
}

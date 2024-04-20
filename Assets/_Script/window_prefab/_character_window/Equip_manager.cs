using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equip_manager : MonoBehaviour {
	[HideInInspector] public DataController dataController;
	public Character_window_controller character_window_controller;
	public Equip_detail_manager equip_detail_manager;
	public Transform[] bags;
	public Transform[] slots;
	private int characterData_order;

	[HideInInspector] public GameObject[] equips;
	[HideInInspector] public GameObject[] slots_equips;

	// Use this for initialization
	void Awake () {
		dataController = GameObject.Find ("DataController").GetComponent<DataController> ();
	}

	public void display_equip(int x){
		foreach (GameObject gmo in GameObject.FindGameObjectsWithTag("Equips")) Destroy (gmo);

		characterData_order = x;
		CharacterData characterData = dataController.data.characterData [x];
		for (int i = 0; i < slots.Length; i++) slots[i].Find("type").gameObject.GetComponent<Text>().text = characterData.equip_tpyps[i].ToString();

		int length = dataController.data.equip_in_bag.Length;
		equips = new GameObject[length];
		for (int i = 0; i < length; i++) {
			int order = dataController.data.equip_in_bag [i];
			string order_string = ((order > 99) ? "" : (order > 9) ? "0" : "00") + order.ToString ();
			equips [i] = Instantiate ((GameObject)Resources.Load ("Prefab/Equips/Equips_" + order_string, typeof(GameObject)), Vector3.zero, Quaternion.identity);
			equips [i].transform.parent = bags [i];
			equips [i].transform.localPosition = Vector3.zero;
			equips [i].transform.localScale = Vector3.one;
			equips [i].GetComponent<Click_equip> ().equip.order = order;
			equips [i].GetComponent<Click_equip> ().equip.index = i;
			equips [i].GetComponent<Click_equip> ().equip.equip_type = (Equip_type)(order / 100);
			equips [i].GetComponent<Click_equip> ().equip_manager = this;
			equips [i].GetComponent<Click_equip> ().home = equips [i].transform.position;
			equips [i].GetComponent<Click_equip> ().home_parent = bags [i];
		}

		int length2 = slots.Length;
		slots_equips = new GameObject[length2];
		for(int i=0;i<length2;i++){
			int order = characterData.equip_set [i];
			if (order == 0) continue;
			slots [i].Find ("裝備底圖").gameObject.SetActive (false);
			string order_string = ((order > 99) ? "" : (order > 9) ? "0" : "00") + order.ToString ();
			slots_equips [i] = Instantiate ((GameObject)Resources.Load ("Prefab/Equips/Equips_" + order_string, typeof(GameObject)), Vector3.zero, Quaternion.identity);
			slots_equips [i].transform.parent = slots [i];
			slots_equips [i].transform.localPosition = Vector3.zero;
			slots_equips [i].transform.localScale = Vector3.one;
			slots_equips [i].GetComponent<Click_equip> ().equip.order = order;
			slots_equips [i].GetComponent<Click_equip> ().equip.index = i + length;
			slots_equips [i].GetComponent<Click_equip> ().equip.equip_type = (Equip_type)(order / 100);
			slots_equips [i].GetComponent<Click_equip> ().equip_manager = this;
			slots_equips [i].GetComponent<Click_equip> ().home = bags [i+length].transform.position;
			slots_equips [i].GetComponent<Click_equip> ().home_parent = bags [i+length];
		}

		character_window_controller.detail_manager.display_character_data (character_window_controller.now_character_order);
	}

	public void Refresh_character(bool save){
		int[] ids = new int[slots.Length];
		for (int i = 0; i < slots.Length; i++) {
			ids [i] = (Find_equip_in_slot (slots [i]) != null) ? Find_equip_in_slot (slots [i]).order : 0;
			slots_equips[i] = (Find_equip_gmo_in_slot (slots [i]) != null) ? Find_equip_gmo_in_slot (slots [i]) : null;
		}


		if (save) {
			// save data
			string s = dataController.data.order.ToString () + characterData_order.ToString () + "characterData";
			dataController.data.characterData[characterData_order].refetch_equip (s, ids);

			// save bag stat
			int ink = 0;
			for (int i = 0; i < bags.Length; i++) if (Find_equip_in_slot (bags [i]) != null) ink++;
			dataController.data.equip_in_bag = new int[ink];
			ink = 0;
			for (int i = 0; i < bags.Length; i++)
				if (Find_equip_in_slot (bags [i]) != null)
					dataController.data.equip_in_bag[ink++] = Find_equip_in_slot (bags [i]).order;
			
			dataController.data.save_data ();
		}

		character_window_controller.detail_manager.display_character_data (character_window_controller.now_character_order);
	}

	public GameObject Find_equip_gmo_in_slot(Transform t){
		foreach (Transform child in t) {
			if (child.GetComponent<Click_equip> () != null) {
				return child.gameObject;
			}
		}
		return null;
	}

	public Equip Find_equip_in_slot(Transform t){
		foreach (Transform child in t) {
			if (child.GetComponent<Click_equip> () != null) {
				return child.GetComponent<Click_equip> ().equip;
			}
		}
		return null;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_window_controller : MonoBehaviour {
	[HideInInspector] public MainCityController mainCityController;
	[HideInInspector] public DataController dataController;
	public Detail_manager detail_manager;
	public Equip_manager equip_manager;
	public Transform bag_character_folder;
	public GameObject switch_bag_character;
	public float bag_dis;
	public int now_character_order;

	void Start () {
		mainCityController = GameObject.Find ("MainCityController").GetComponent<MainCityController> ();
		dataController = GameObject.Find ("DataController").GetComponent<DataController> ();
		Setup_bag_character ();
		now_character_order = 0;
		equip_manager.display_equip (now_character_order);
		change_parent_has_window_prefab (true);
	}

	public void switch_character(int order){
		now_character_order = order;
		equip_manager.display_equip (now_character_order);
	}

	public void change_parent_has_window_prefab(bool boolean){
		mainCityController.has_window_prefab = boolean;
	}

	void Setup_bag_character(){
		int length = dataController.data.characterData.Length;
		bag_character_folder.localPosition -= new Vector3 ((length - 1) * bag_dis / 2.0f, 0, 0);
		for (int i = 0; i < length; i++) {
			// 判斷是不是南北不對
			GameObject gmo = Instantiate (switch_bag_character, new Vector3(), Quaternion.identity);
			gmo.transform.parent = bag_character_folder;
			gmo.transform.localPosition = new Vector3 (i, 0, 0) * bag_dis;
			gmo.transform.localScale *= 7;
			gmo.GetComponent<SpriteRenderer> ().sprite = dataController.data.characterData [i].avatar;
			gmo.GetComponent<Switch_bag_character> ().order = i;
		}
	}
}

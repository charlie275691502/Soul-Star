using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Equip{
	[HideInInspector] public int index; // 存檔的順序
	[HideInInspector] public int order;
	public string Name;
	public int number;
	[HideInInspector] public Equip_type equip_type;
	public string desc;

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
}

public class Click_equip : MonoBehaviour {
	public Equip equip;

	public Text name_text;
	public Equip_manager equip_manager;
	[HideInInspector] public Transform[] posible_slots;
	[HideInInspector] public Vector3 home;
	[HideInInspector] public Transform home_parent;

	void Start(){
		name_text.text = equip.Name;
	}

	void OnMouseDown(){
		if(GameObject.Find ("Equip_detail") == null)return;
		GameObject.Find ("Equip_detail").GetComponent<Equip_detail_manager> ().display_equip_detail (equip, GetComponent<SpriteRenderer>().sprite);
		GetComponent<SpriteRenderer>().sortingLayerName = "toppest-1";
	}

	void OnMouseDrag(){
		if(GameObject.Find ("Equip_detail") == null)return;

		Vector3 pos =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
		pos = new Vector3 (pos.x, pos.y, 0);

		Transform slot;
		if ((slot = is_in_any_slot (pos)) != null) {
			transform.position = slot.position;
			transform.parent = slot;
			slot.Find ("裝備底圖").gameObject.SetActive (false);
		} else if (is_in_home (pos)) {
			transform.position = home;
			transform.parent = home_parent;
		} else {
			transform.position = pos;
			transform.parent = equip_manager.gameObject.transform;
		}
		equip_manager.Refresh_character (false);
	}

	void OnMouseUp(){
		if(GameObject.Find ("Equip_detail") == null)return;
		GetComponent<SpriteRenderer>().sortingLayerName = "jump_out_box";

		if (is_in_any_slot (transform.position) == null) {
			transform.position = home;
			transform.parent = home_parent;
		}
		equip_manager.Refresh_character (true);
	}

	Transform is_in_any_slot(Vector3 pos){
		foreach (Transform slot in equip_manager.slots) {
			if(Mathf.Abs(slot.position.x - pos.x) < 1.0f && Mathf.Abs(slot.position.y - pos.y) < 1.0f && slot.Find("type").gameObject.GetComponent<Text>().text == equip.equip_type.ToString() && (equip_manager.Find_equip_in_slot(slot) == null || equip_manager.Find_equip_in_slot(slot).index == equip.index)){
				return slot;
			}
		}
		return null;
	}

	bool is_in_home(Vector3 pos){
		return Mathf.Abs (home.x - pos.x) < 1.0f && Mathf.Abs (home.y - pos.y) < 1.0f;
	}
}

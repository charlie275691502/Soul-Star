using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equip_detail_manager : MonoBehaviour {
	public SpriteRenderer avatar_sr;
	public Text name_text;
	public Text equip_type_text;
	public Text desc_text;

	public void display_equip_detail(Equip equip, Sprite equip_avatar){
		avatar_sr.sprite = equip_avatar;
		name_text.text = equip.Name;
		equip_type_text.text = equip.equip_type.ToString ();
		desc_text.text = equip.desc;
	}
}

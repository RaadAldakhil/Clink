using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour {

	public void onClick(){
		GameUtil.volume = GameUtil.volume>0?0:1;
		string onOff = GameUtil.volume>0?"On":"Off";
		GetComponent<Image> ().sprite = Resources.Load<Sprite>("Textures/sound" + onOff);
	}
}

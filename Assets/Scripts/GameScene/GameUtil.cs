using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUtil : MonoBehaviour {

	public static int volume = 1;
	public static string[] specials = {"Delete column", "Delete row", "Rotate"};
	public static bool isDown;
	public static bool onTurn;
	public static bool isGameOn;
	public static bool isAnimating;
	public static int special1, special2;
	public static int selectorId; //0 is hidden, 1 is disk, 2 is deleteCol, 3 is deleteRow
	public static IntVector2 currentSpecial;
	public static GameObject resetButton, specialButton;

	private static AudioSource audioSource;
	private static Text notificationField;

	public static void reset(){
		isDown = false;
		onTurn = false;
		isGameOn = false;
		isAnimating = false;
		setNotification ("");
		special1 = 0;
		special2 = 0;
		resetButton.SetActive (false);
		specialButton.SetActive (false);
	}

	public static int positionToRealWorld(int pos){
		return pos * 10 - 30;
	}

	public static int positionToArray(float pos){
		int n = RoundNum ((int)pos);
		return (n + 30) / 10;
	}

	public static IntVector2 mousePosition(){
		var pos3d = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		int x = limitMousePos(pos3d.x);
		int y = limitMousePos(pos3d.y+7);
		return new IntVector2 (x, y);
	}

	public static int limitMousePos(float f){
		int n = RoundNum ((int)f);
		if (n > 30)
			return 30;
		else if (n < -30)
			return -30;
		else return n;
	}

	public static bool MouseInRack(){
		var mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		var x = mousePos.x;
		var y = mousePos.y+7;
		return x > -35 && y> -35 && x < 35 && y < 35;
	}

	public static bool mouseClickInRack(){
		return Input.GetMouseButton (0) && MouseInRack () &&!isAnimating;
	}

	public static int RoundNum(float f){
		int i = (int)f;
		return ((i + (i > 0 ? 5 : -5)) / 10) * 10;
	}

	//Plays sound with name (without extension) in folder Sounds
	public static void playOneShot(string name){
		if (audioSource == null) {
			audioSource = GameObject.Find ("Audio").GetComponent<AudioSource> ();
		}
		var clip = Resources.Load<AudioClip>("Sounds/" + name);
		audioSource.PlayOneShot (clip, GameUtil.volume);
	}

	//Sets the notification text field's text
	public static void setNotification(string text){
		if (notificationField == null) {
			notificationField = GameObject.Find ("Notification").gameObject.GetComponent<Text>();
		}
		notificationField.text = text;
	}

	//Delegate managing classes
	public static void setResetHandler(ResetFunction myFunction){
		var button = GameObject.Find ("ResetButton").GetComponent<ResetButton> ();
		button.ResetCallback += myFunction;
	}
	public static void setSpecialHandler(SpecialFunction myFunction){
		var button = GameObject.Find ("SpecialButton").GetComponent<SpecialButton> ();
		button.SpecialCallback += myFunction;
	}
}

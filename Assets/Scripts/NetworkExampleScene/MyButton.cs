using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MyButton : MonoBehaviour, DataDelegate {

	private InputField inputField;
	private Text textField;
	private Button button;

	void Start(){
		inputField = GameObject.Find ("InputField").GetComponent<InputField>();
		textField = GameObject.Find ("OutputField").GetComponent<Text> ();
		button = GameObject.Find("TheButton").GetComponent<Button> ();

		NetworkManager.addDelegate (this);
		NetworkManager.checkTurn ();
	}

	// Use this for initialization
	public void onClick(){
		string text = inputField.text;
		textField.text = textField.text + text + "\n";
		inputField.text = "";

		NetworkManager.postMove (text,false);
		NetworkManager.endTurn ();

		button.interactable = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onDataReceived(JSONObject data){
		textField.text += data.ToString () + "\n";
	}

	public void onTurnReceived(){
		button.interactable = true;
	}

	public void onTurnCheckReceived(bool isOnTurn){
		button.interactable = isOnTurn;
	}

	public void onTurnEnded(){
		
	}
}

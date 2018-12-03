using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour{

	public delegate void OnDataReceived (JSONObject data);
	public static event OnDataReceived DataCallback;

	public delegate void OnTurnCheckReceived (bool isOnTurn);
	public static event OnTurnCheckReceived TurnCheckCallback;

	public delegate void OnTurnReceived ();
	public static event OnTurnReceived TurnCallback;

	private static string checkTurnUrl = "localhost/checkTurn.php";
	private static string makeMoveUrl = "localhost/makeMove.php";
	private static string waitForTurnUrl = "localhost/waitForTurn.php";
	private static string endTurnUrl = "localhost/endTurn.php";

	private static NetworkManager singleton;

	private static NetworkManager getInstance() {
		if (singleton == null) {
			var obj = new GameObject ("NetworkManager");
			singleton = obj.AddComponent<NetworkManager> ();
		}
		return singleton;
	}

	public static void addDelegate(DataDelegate myDelegate){
		DataCallback += myDelegate.onDataReceived;
		TurnCallback += myDelegate.onTurnReceived;
		TurnCheckCallback += myDelegate.onTurnCheckReceived;
	}

	//Checks if it's the player's turn, result returned by delegate TurnCallback
	public static void checkTurn(){
		WWW www = new WWW(checkTurnUrl);
		getInstance().StartCoroutine(getInstance().checkTurn(www));
	}

	public static void postSpecial(int specialId,int id, bool endTurn){
		JSONObject j = new JSONObject ();
		j.AddField ("special", specialId);
		j.AddField ("id", id);
		postMove (j.Print (), endTurn);
	}

	public static void postDropDisk(int column, bool endTurn){
		JSONObject j = new JSONObject ();
		j.AddField ("dropDisk", column);
		postMove (j.Print (), endTurn);
	}

	public static void postNewSpecial(int x,int y){
		JSONObject j = new JSONObject ();
		j.AddField ("newSpecial", 1);
		j.AddField ("x", x);
		j.AddField ("y", y);
		postMove (j.Print (), false);
	}

	//Posts the player's move, param is a json encoded string, no result
	public static void postMove(string data, bool endTurn){
		WWWForm form = new WWWForm();
		form.AddField("input",data);
		if(endTurn)
			form.AddField("endTurn",1);
		WWW www = new WWW(makeMoveUrl,form);
		getInstance().StartCoroutine(getInstance().makeMoveResponse(www));
	}

	public static void endTurn(){
		WWW www = new WWW(endTurnUrl);
	}

	IEnumerator checkTurn(WWW www){
		yield return www;
		// check for errors
		if (www.error == null)
		{
			JSONObject j = new JSONObject (www.text);

			if (TurnCheckCallback != null) {
				TurnCheckCallback (j ["onTurn"].i==1);
			}

			if (j ["onTurn"].i<1) {
				WWW www1 = new WWW(waitForTurnUrl);
				getInstance().StartCoroutine(WaitForTurn(www1));
			}
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}

	IEnumerator makeMoveResponse(WWW www){
		yield return www;
		// check for errors
		if (www.error == null)
		{
			Debug.Log("Makemoveresponse:" + www.text);
			WWW www1 = new WWW(waitForTurnUrl);
			getInstance().StartCoroutine(WaitForTurn(www1));
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}

	IEnumerator WaitForTurn(WWW www){
		yield return www;
		// check for errors
		if (www.error == null)
		{
			Debug.Log ("WaitForTurnResponse:" + www.text);
			JSONObject j = new JSONObject (www.text);
			if (j.HasField ("data") && DataCallback != null) {
				JSONObject data = j ["data"];
				Debug.Log ("Calling data back");
				DataCallback (data[data.Count - 1]);
			}

			if (j ["status"].i == 0) {
				WWW www1 = new WWW (waitForTurnUrl);
				getInstance().StartCoroutine (WaitForTurn (www1));
			} else if (j["status"].i == 1){
				TurnCallback ();
			}
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

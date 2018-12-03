using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaitroomManager : MonoBehaviour {

	public GameObject leaveButton;

	private const string joinWaitlistUrl = "localhost/joinWaitlist.php";
	private const string leaveWaitlistUrl = "localhost/leaveWaitlist.php";
	//https://web.cs.manchester.ac.uk/mbyx4ev2/X5101Lab
	WWW www;
	bool hasLeft;

	// Use this for initialization
	void Start () {
		www = new WWW (joinWaitlistUrl);
		StartCoroutine (joinWaitlistResponse());
	}

	public void leaveClicked(){
		hasLeft = true;
		WWW www1 = new WWW (leaveWaitlistUrl);
		SceneManager.LoadScene ("Welcome");
	}

	IEnumerator joinWaitlistResponse(){
		yield return www;
		// check for errors
		if (www.error == null)
		{
			Debug.Log (www.text);
			JSONObject j = new JSONObject (www.text);

			if (j ["status"].i == 0 && !hasLeft) {
				www = new WWW (joinWaitlistUrl);
				StartCoroutine (joinWaitlistResponse ());
			} else if (j ["status"].i == 1) {
				var initializer = new GameObject ("Initializer");
				var script = initializer.AddComponent<Initializer> ();
				script.id = 0;
				SceneManager.LoadScene ("Game");
			}
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}
}

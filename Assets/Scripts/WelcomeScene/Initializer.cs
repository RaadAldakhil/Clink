using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Initializer : MonoBehaviour {

	public int id;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (SceneManager.GetActiveScene ().name.Equals ("Game")) {
			switch (id) {
			case 0:
				Debug.Log ("Multiplayer");
				GameObject.Find ("GameManager").AddComponent<OnlineMultiplayer> ();
				break;
			case 1:
				Debug.Log ("Offline Multiplayer");
				GameObject.Find ("GameManager").AddComponent<OfflineMultiplayer> ();
				break;
			}
			Destroy (this);
		}
	}
}

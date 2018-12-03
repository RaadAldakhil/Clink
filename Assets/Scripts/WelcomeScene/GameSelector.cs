using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSelector : MonoBehaviour {

	public void onClick(int id){
		if (id == 0) {
			SceneManager.LoadScene ("Waitroom");
		} else {
			var initializer = new GameObject ("Initializer");
			var script = initializer.AddComponent<Initializer> ();
			script.id = id;
			SceneManager.LoadScene ("Game");
		}

	}
}

using UnityEngine;
using System.Collections;

public class SpecialBlinker : MonoBehaviour {

	bool isVisible;

	public static GameObject createBlinker(){
		Object prefab = (GameObject)Resources.Load("Prefabs/SpecialBlinker");
		GameObject clone = (GameObject)GameObject.Instantiate (prefab);
		return clone;
	}

	// Use this for initialization
	void Start () {
		StartCoroutine (updateBlinker ());
	}

	IEnumerator updateBlinker(){
		var renderer = GetComponent<SpriteRenderer> ();
		float t = 0f;
		while (t<1) {
			t += Time.deltaTime/0.5f;
			var col = renderer.color;
			col.a = isVisible ? 1 - t : t;
			renderer.color = col;
			yield return null;
		}
		isVisible = !isVisible;
		StartCoroutine (updateBlinker ());
	}
}

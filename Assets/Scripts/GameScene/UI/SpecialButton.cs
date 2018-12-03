using UnityEngine;
using System.Collections;

public delegate void SpecialFunction ();

public class SpecialButton : MonoBehaviour {

	public event SpecialFunction SpecialCallback;

	public void onClick(){
		SpecialCallback ();
	}
}

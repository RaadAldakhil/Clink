using UnityEngine;
using System.Collections;

public delegate void ResetFunction ();

public class ResetButton : MonoBehaviour {

	public event ResetFunction ResetCallback;

	public void onClick(){
		ResetCallback ();
	}
}

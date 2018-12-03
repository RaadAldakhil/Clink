using UnityEngine;
using System.Collections;

public delegate void RotateFunction ();

public class RotateButton : MonoBehaviour {

	public event RotateFunction RotateCallback;

	public void onClick(){
		RotateCallback ();
	}
}

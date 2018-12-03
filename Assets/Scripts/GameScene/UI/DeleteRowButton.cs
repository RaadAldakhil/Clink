using UnityEngine;
using System.Collections;

public delegate void DeleteRowFunction ();

public class DeleteRowButton : MonoBehaviour {

	public event DeleteRowFunction DeleteRowCallback;

	public void onClick(){
		DeleteRowCallback ();
	}
}

using UnityEngine;
using System.Collections;

public delegate void DeleteFunction ();

public class DeleteButton : MonoBehaviour {

	public event DeleteFunction DeleteCallback;

	public void onClick(){
		DeleteCallback ();
	}
}

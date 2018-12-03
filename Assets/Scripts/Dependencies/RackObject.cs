using UnityEngine;
using System.Collections;

public class RackObject{
	public int id = 0;
	public GameObject disk;
	public GameObject rackPiece;

	public RackObject(int id, GameObject disk, GameObject rackPiece){
		this.id = id;
		this.disk = disk;
		this.rackPiece = rackPiece;
	}
}

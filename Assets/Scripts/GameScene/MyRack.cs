using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void diskDropped (IntVector2 pos);
public delegate void rackRotated ();
public delegate void columnDeleted ();
public delegate void rowDeleted (int row);

public class MyRack : MonoBehaviour {

	public event diskDropped DiskDroppedCallback;
	public event rackRotated RackRotatedCallback;
	public event columnDeleted ColumnDeletedCallback;
	public event rowDeleted RowDeletedCallback;

	public GameObject prefab;
	public GameObject container;
	public GameObject selector;

	private static MyRack singleton;

	public static MyRack getInstance(){
		return singleton;
	}

	void Awake(){
		singleton = this;
	}

	public void showDiskSelector(){
		var renderer = selector.GetComponent<SpriteRenderer> ();
		renderer.sprite = Resources.Load<Sprite>("Textures/pieceWhite");
		renderer.color = GameUtil.onTurn?Color.red:Color.yellow;
		selector.transform.localScale = new Vector3 (14, 14, 1);
		selector.SetActive (true);
		GameUtil.selectorId = 1;
	}

	public void showDeleteColumnSelector(){
		var renderer = selector.GetComponent<SpriteRenderer> ();
		renderer.sprite = Resources.Load<Sprite>("Textures/Arrow");
		renderer.color = Color.white;
		selector.transform.localScale = new Vector3 (3.9f, 3.9f, 1.0f);
		selector.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		selector.SetActive (true);
		GameUtil.selectorId = 2;
	}

	public void showDeleteRowSelector(){
		var renderer = selector.GetComponent<SpriteRenderer> ();
		renderer.sprite = Resources.Load<Sprite>("Textures/Arrow");
		renderer.color = Color.white;
		selector.transform.localScale = new Vector3 (3.9f, 3.9f, 1.0f);
		selector.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 90));
		selector.SetActive (true);
		GameUtil.selectorId = 3;
	}

	public void hideSelector(){
		var renderer = selector.GetComponent<SpriteRenderer> ();
		renderer.color = Color.white;
		selector.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		selector.SetActive (false);
		GameUtil.selectorId = 0;
	}

	public void createSpecial(IntVector2 coordinates){
		if (GameUtil.currentSpecial.x != -1) {
			destroyRackBlinker(GameUtil.currentSpecial);
		}
		addRackBlinker (coordinates);
		GameUtil.currentSpecial = coordinates;
	}

	void destroyRackBlinker(IntVector2 pos){
		var rackPiece = RackUtil.myRack [pos.x] [pos.y].rackPiece;
		if (rackPiece.transform.childCount > 0) {
			var blinker = rackPiece.transform.GetChild(0);
			if(blinker!=null) Destroy (blinker.gameObject);
		}
	}

	void addRackBlinker(IntVector2 pos){
		var rackPiece = RackUtil.myRack [pos.x] [pos.y].rackPiece;
		var blinker = SpecialBlinker.createBlinker ();
		blinker.transform.SetParent(rackPiece.transform);
		blinker.transform.localPosition = Vector3.zero;
	}

	public void deleteColumn(int column){
		GameUtil.isAnimating = true;
		for (int i = 0; i < RackUtil.SIZE; i++) {
			var disk = RackUtil.myRack [i] [column].disk;
			RackUtil.deleteDisk (i, column);
			Destroy (disk);
		}
		StartCoroutine( ColumnDeleteTask());
		GameUtil.playOneShot ("Special activate");
	}

	IEnumerator ColumnDeleteTask(){
		yield return new WaitForSeconds(0.5f);
		ColumnDeletedCallback();
	}

	public void deleteRow(int row){
		for (int i = 0; i < RackUtil.SIZE; i++) {
			var disk = RackUtil.myRack [row] [i].disk;
			RackUtil.deleteDisk (row, i);
			Destroy (disk);
		}
		GameUtil.isAnimating = true;
		StartCoroutine (RowDeleteTask(row, 0.5f));
		GameUtil.playOneShot ("InGameSound");
		GameUtil.playOneShot ("Special activate");
	}

	IEnumerator RowDeleteTask(int row, float inTime) {
		RackUtil.shiftRackDown ();

		for(var t = 0f; t < 1; t += Time.deltaTime/inTime) {
			for (int i = 0; i < RackUtil.SIZE; i++) {
				for (int j = 0; j <RackUtil.SIZE; j++) {
					if (RackUtil.myRack [i] [j].id != 0)
						updateRackItemBy (i, j, t);
				}
			}
			yield return null;
		}
		RowDeletedCallback (row);
	}

	public void deleteAllDisks(){
		for (int i = 0; i < RackUtil.SIZE; i++) {
			for (int j = 0; j < RackUtil.SIZE; j++) {
				var disk = RackUtil.myRack [i] [j].disk;
				RackUtil.deleteDisk (i, j);
				Destroy (disk);
			}
		}
	}

	public void resetRack(){
		deleteAllDisks ();
		hideSelector ();
	}

	/*DROP DISK*/

	public IntVector2 dropDisk(){
		int column = ((int)selector.transform.localPosition.x+30)/10;
		return dropDisk(column);
	}

	public IntVector2 dropDisk(int column){
		var clone = GameObject.Instantiate (prefab);
		clone.transform.SetParent(container.transform);
		clone.transform.localPosition = new Vector2(column*10-30,41);
		clone.GetComponent<SpriteRenderer> ().color = GameUtil.onTurn?Color.red:Color.yellow;
		var id = GameUtil.onTurn ? 1 : 2;
		var coordinates = RackUtil.registerDisk(column,id,clone);
		GameUtil.isAnimating = true;
		StartCoroutine (updateRackItem (coordinates.x, coordinates.y));
		GameUtil.playOneShot ("InGameSound");
		return coordinates;
	}

	IEnumerator updateRackItem(int i, int j){
		float t = 0f;
		while (t<1) {
			t += (Time.deltaTime / 0.5f)/((RackUtil.SIZE-i)/(float)RackUtil.SIZE);
			updateRackItemBy (i, j, t);
			yield return null;
		}

		GameUtil.isAnimating = false;
		DiskDroppedCallback (new IntVector2(i,j));
	}

	public void updateRackItemBy(int i, int j, float t){
		var pos = new Vector2 (j*10-30,i * 10 - 30);
		var disk = RackUtil.myRack [i] [j].disk;
		disk.transform.localPosition = Vector3.Lerp (disk.transform.localPosition, pos, t);
	}

	/*ROTATE BOARD*/

	public void rotateBoard(){
		GameUtil.isAnimating = true;
		StartCoroutine (RotateRack(new Vector3(0,0,90), 1));
		GameUtil.playOneShot ("InGameSound");
	}

	IEnumerator RotateRack(Vector3 angle, float inTime) {
		var fromAngle = container.transform.rotation;
		var toAngle = Quaternion.Euler(angle);

		RackUtil.shiftRackToLeft ();

		for(var t = 0f; t < 1; t += Time.deltaTime/inTime) {
			container.transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
			for (int i = 0; i < RackUtil.SIZE; i++) {
				for (int j = 0; j <RackUtil.SIZE; j++) {
					if (RackUtil.myRack [i] [j].id != 0)
						updateRackItemBy (i, j, t);
				}
			}
			yield return null;
		}
		container.transform.rotation = Quaternion.Euler(angle);

		//Rotates the array 90 degrees
		RackUtil.transposeRack ();
		RackUtil.flipRackColumns ();

		normalizePositionsAfterRotation ();
		GameUtil.isAnimating = false;
		RackRotatedCallback ();
	}

	/*UPDATE SELECTOR*/

	public void updateSelectorPosition(){
		IntVector2 pos = GameUtil.mousePosition ();
		var currentPosition = selector.transform.localPosition;
		currentPosition.x = GameUtil.selectorId==3 ? -41 : pos.x;
		currentPosition.y = GameUtil.selectorId==3 ? pos.y : 41;
		selector.transform.localPosition = currentPosition;
	}

	public void normalizePositionsAfterRotation(){
		//Take out disks and rackImage from container
		foreach (GameObject disk in RackUtil.activeDisks) {
			disk.transform.SetParent(transform);
		}
		var rackImage = container.transform.Find ("RackImage");
		rackImage.transform.SetParent (transform);

		//Set container rotation to zero
		container.transform.rotation = Quaternion.Euler (Vector3.zero);

		//Put disks and rackImage back into container
		foreach (GameObject disk in RackUtil.activeDisks) {
			disk.transform.SetParent(container.transform);
		}
		rackImage.transform.SetParent (container.transform);

		//Normalize current special position after rotation
		var pos = GameUtil.currentSpecial;
		pos = new IntVector2 (pos.y, RackUtil.SIZE - pos.x - 1);
		GameUtil.currentSpecial = pos;
	}		
}

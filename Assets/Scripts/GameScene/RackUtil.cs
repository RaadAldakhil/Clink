using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Manages disk data and position

public class RackUtil : MonoBehaviour {

	public static RackObject[][] myRack;
	public static List<GameObject> activeDisks;
	public static int SIZE = 7;

	public static void initRacks(){
		myRack = new RackObject[SIZE][];
		for (int i = 0; i < SIZE; i++) {
			myRack [i] = new RackObject[SIZE];
			for (int j = 0; j < SIZE; j++) {
				var rackPiece = GameObject.Find ("RackImage").transform.Find (""+j).transform.Find (""+i);
				myRack [i] [j] = new RackObject (0, null, rackPiece.gameObject);
			}
		}
		activeDisks = new List<GameObject> ();
	}

	public static void deleteDisk(int row, int column){
		myRack [row] [column].id = 0;
		activeDisks.Remove (myRack [row] [column].disk);
		myRack [row] [column].disk = null;
	}

	public static IntVector2 registerDisk(int column, int id, GameObject disk){
		for (int i = 0; i < SIZE; i++) {
			if (myRack [i] [column].id == 0) {
				myRack [i] [column].id = id;
				myRack [i] [column].disk = disk;
				activeDisks.Add (disk);
				return new IntVector2 (i, column);
			}
		}
		return new IntVector2();
	}

	public static void registerDisk(int row, int column , int id, GameObject disk){
		myRack [row] [column].id = id;
		myRack [row] [column].disk = disk;
		activeDisks.Add (disk);
	}

	public static int checkWin(){
		string unwraped = "";
		//Unwrap diagonals
		for (int slice = 0; slice < 2 * SIZE - 1; ++slice) {
			int z = slice < SIZE ? 0 : slice - SIZE + 1;
			string d1 = "", d2 = "";
			for (int j = z; j <= slice - z; ++j) {
				d1 += myRack[j][slice - j].id;
				d2 += myRack [SIZE - slice + j - 1] [j].id;
			}
			unwraped += d1 + "," + d2 + ",";
		}
		unwraped +=",";

		//Unwrap horizontals and verticals
		for (int i = 0; i < SIZE; i++) {
			string h = "", v = "";
			for (int j = 0; j < SIZE; j++) {
				h += myRack [i] [j].id;
				v += myRack [j] [i].id;
			}
			unwraped += h + "," + v + ",";
		}
		if (unwraped.IndexOf ("1111") >= 0)
			return 1;
		else if (unwraped.IndexOf ("2222") >= 0)
			return 2;
		return 0;
	}

	public static void shiftRackToLeft(){
		for (int i = 0; i < SIZE; i++) {
			for (int j = 1, k = 0; j <SIZE && k<SIZE; j++) {
				if (myRack [i] [k].id != 0) {
					k++;
				} else if (myRack [i] [j].id != 0) {
					myRack [i] [k].id = myRack [i] [j].id;
					myRack [i] [j].id = 0;
					myRack [i] [k].disk = myRack [i] [j].disk;
					myRack [i] [j].disk = null;
					k++;
				}
			}
		}
	}

	public static void shiftRackDown(){
		for (int i = 0; i < SIZE; i++) {
			for (int j = 1, k = 0; j <SIZE && k<SIZE; j++) {
				if (myRack [k] [i].id != 0) {
					k++;
				} else if (myRack [j] [i].id != 0) {
					myRack [k] [i].id = myRack [j] [i].id;
					myRack [j] [i].id = 0;
					myRack [k] [i].disk = myRack [j] [i].disk;
					myRack [j] [i].disk = null;
					k++;
				}
			}
		}
	}

	public static void transposeRack(){
		for (int i = 0; i < SIZE; i++) {
			for (int j = 0; j <i; j++) {
				var temp = myRack [i] [j];
				myRack[i][j]= myRack [j] [i];
				myRack [j] [i] = temp;
			}
		}
	}

	public static void flipRackColumns(){
		for (int i = 0; i < SIZE; i++) {
			for (int j = 0; j <SIZE/2; j++) {
				var temp = myRack [i] [j];
				myRack[i][j]= myRack [i] [SIZE-1-j];
				myRack [i] [SIZE-1-j] = temp;
			}
		}
	}

	public static void printArray(){
		string a = "";
		for (int i = SIZE-1; i >= 0; i--) {
			for (int j = 0; j <SIZE; j++) {
				a += myRack [i] [j].id + " ";
			}
			a+="\n";
		}
		Debug.Log (a);
	}

	public static bool isColumnFull(int column){
		return myRack [SIZE-1] [column].id != 0;
	}
		
}

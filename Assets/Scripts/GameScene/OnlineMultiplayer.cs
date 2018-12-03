using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

public class OnlineMultiplayer : MonoBehaviour,DataDelegate {

  protected MyRack rack;
  public const int specialDeleteColumn = 1;
  public const int specialDeleteRow = 2;
  public const int specialRotateRack = 3;

  // Use this for initialization
  void Start () {
    RackUtil.initRacks ();
    rack = MyRack.getInstance ();
    GameUtil.specialButton = GameObject.Find ("SpecialButton");
    GameUtil.resetButton = GameObject.Find ("ResetButton");

    //Handling delegates
	NetworkManager.addDelegate(this);
    rack.DiskDroppedCallback += diskDropped;
    rack.RackRotatedCallback += rackRotated;
    rack.ColumnDeletedCallback += columnDeleted;
    rack.RowDeletedCallback += rowDeleted;
    GameUtil.setResetHandler (resetPressed);
    GameUtil.setSpecialHandler (specialPressed);
    setupGame ();
  }
  
  // Update is called once per frame
  void Update () {
    if (!GameUtil.isGameOn) return;
    rack.updateSelectorPosition();
    updateSpecialButton ();
    if (GameUtil.mouseClickInRack ())
      handleMouseClick ();
  }

  protected void handleMouseClick(){
    if (GameUtil.selectorId == 1) {
      dropDisk ();
    } else if (GameUtil.selectorId == 2) {
      deleteColumn ();
    } else if (GameUtil.selectorId == 3) {
      deleteRow ();
    }
  }

  protected void playerWon(int player){
    GameUtil.setNotification("Player " + player + " Won");
    GameUtil.playOneShot ("Win");
    GameUtil.isGameOn = false;
    GameUtil.resetButton.SetActive (true);
  }

  protected void resetPressed(){
    rack.resetRack();
    setupGame ();
  }

  protected void setupGame(){
    GameUtil.reset ();
    GameUtil.isGameOn = true;
	NetworkManager.checkTurn ();
  }

  protected void dropDisk(){
    var column = (GameUtil.mousePosition ().x + 30) / 10;
    if(!RackUtil.isColumnFull(column)){
      rack.hideSelector ();
      var pos = rack.dropDisk (column);
      onDiskDown (pos);
      GameUtil.onTurn = false;
      NetworkManager.postDropDisk(column,true); 
    }
  }

  protected void onDiskDown(IntVector2 pos){
    int id = RackUtil.checkWin ();
    if (id > 0) {
      playerWon (id);
    } else {
      if (pos == GameUtil.currentSpecial) {
        //activateDelete();
        if (GameUtil.onTurn) {
          GameUtil.special1 = UnityEngine.Random.Range (1, 4);
        } else {
          GameUtil.special2 = UnityEngine.Random.Range (1, 4);
        }
        GameUtil.playOneShot ("SpecialPickUp");
		IntVector2 v = newSpecialLocation ();
		NetworkManager.postNewSpecial (v.x, v.y);
		rack.createSpecial (v);
      }
    }
  }

  protected void updateSpecialButton(){
    int special = GameUtil.special1;
    if (special > 0) {
      Text t = GameUtil.specialButton.transform.Find ("Text").GetComponent<Text>();
      t.text = GameUtil.specials [special - 1];
      GameUtil.specialButton.SetActive (true);
    } else {
      GameUtil.specialButton.SetActive (false);
    }
  }

  //Called after disk's animation finishes
  protected void diskDropped(IntVector2 pos){
    //Future implementation
  }

  protected void deleteColumn(){
    var column = (GameUtil.mousePosition ().x + 30) / 10;
    rack.deleteColumn (column);
    rack.showDiskSelector ();
    NetworkManager.postSpecial(specialDeleteColumn,column,false);
  }

  protected void deleteRow(){
    var row = (GameUtil.mousePosition ().y + 30) / 10;
    rack.deleteRow (row);
    rack.showDiskSelector ();
    NetworkManager.postSpecial(specialDeleteRow,row,false);
  }

  protected void deletePressed(){
    GameUtil.playOneShot ("SpecialPickUp");
    rack.updateSelectorPosition ();
    rack.showDeleteColumnSelector ();
  }

  protected void deleteRowPressed(){
    GameUtil.playOneShot ("SpecialPickUp");
    rack.updateSelectorPosition ();
    rack.showDeleteRowSelector ();
  }

  protected void specialPressed(){
    int special = GameUtil.onTurn ? GameUtil.special1 : GameUtil.special2;
    switch (special) {
    case 1:
      deletePressed ();
      break;
    case 2: 
      deleteRowPressed ();
      break;
    case 3:
      rotatePressed ();
      break;
    }
    if (GameUtil.onTurn) {
      GameUtil.special1 = 0;
    } else {
      GameUtil.special2 = 0;
    }
  }

  protected void rotatePressed(){
    GameUtil.playOneShot ("Special activate");
    rack.rotateBoard ();
    NetworkManager.postSpecial(specialRotateRack,0,false);
  }

  //Called after rack's animation finishes
  protected void rackRotated(){
	IntVector2 v = newSpecialLocation ();
	NetworkManager.postNewSpecial (v.x, v.y);
    rack.createSpecial (v);
    int id = RackUtil.checkWin ();
    if (id > 0) {
      playerWon (id);
    }
  }

  //Calledd after column's deletion finishes
  protected void columnDeleted(){
    GameUtil.isAnimating = false;
  }

  //Called after row's deletion finishes
  protected void rowDeleted(int row){
    GameUtil.isAnimating = false;
  }

  //Creates new location for the special
  protected IntVector2 newSpecialLocation(){
    int x, y;
    do {
      x = UnityEngine.Random.Range (0, 7);
      y = UnityEngine.Random.Range (0, 7);
    } while(RackUtil.myRack [x] [y].id != 0);
    return new IntVector2 (x, y);
  }

  public void opponentDropDisk(int column){
    if(!RackUtil.isColumnFull(column)){
      rack.hideSelector ();
      GameUtil.onTurn = false;
      var pos = rack.dropDisk (column);
      onDiskDown (pos);
    }
  }

  public void opponentDeleteColumn(int column){
    rack.deleteColumn (column);
  }

  public void opponentDeleteRow(int row){
    rack.deleteRow (row);
  }

  public void opponentRotateRack(){
    GameUtil.playOneShot ("Special activate");
    rack.rotateBoard ();
  }

  public void onDataReceived(JSONObject data){
	if (data.HasField ("dropDisk")) {
		opponentDropDisk ((int)data ["dropDisk"].i);
	} else if (data.HasField ("special")) {
		if (data ["special"].i == specialDeleteColumn) {
			opponentDeleteColumn ((int)data ["id"].i);
		} else if (data ["special"].i == specialDeleteRow) {
			opponentDeleteRow ((int)data ["id"].i);
		} else if (data ["special"].i == specialRotateRack) {
			opponentRotateRack ();
		}
	} else if (data.HasField ("newSpecial")) {
			rack.createSpecial (new IntVector2((int)data["x"].i, (int)data["y"].i));
	}
  }

  public void onTurnReceived(){
    GameUtil.onTurn = true;
    rack.showDiskSelector();
  }

  public void onTurnCheckReceived(bool isOnTurn){
    GameUtil.onTurn = isOnTurn;
		if (isOnTurn) {
			rack.showDiskSelector ();
			IntVector2 v = newSpecialLocation ();
			NetworkManager.postNewSpecial (v.x, v.y);
			NetworkManager.postNewSpecial (v.x, v.y);
			rack.createSpecial (v);		
		}
      
  }
}
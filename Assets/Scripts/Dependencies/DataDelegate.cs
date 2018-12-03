using UnityEngine;
using System.Collections;

public interface DataDelegate{
	//Executes when data is received
	void onDataReceived (JSONObject data); 
	//Executes when it is now the current player's turn
	void onTurnReceived (); 
	//Executes when response is received from checkTurn method
	void onTurnCheckReceived(bool isOnTurn);
}

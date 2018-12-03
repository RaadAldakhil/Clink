using UnityEngine;
using System.Collections;

public struct IntVector2{
	public int x,y;

	public IntVector2(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public static bool operator ==(IntVector2 v1, IntVector2 v2) {
		return v1.x==v2.x && v1.y==v2.y;
	}

	public static bool operator !=(IntVector2 v1, IntVector2 v2) {
		return !(v1 == v2);
	}
}

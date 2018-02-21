using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex
{
	private static Vector2Int[] axialDirections =
	{
		Vector2Int.down, Vector2Int.down + Vector2Int.right, Vector2Int.right,
		Vector2Int.up, Vector2Int.up + Vector2Int.left, Vector2Int.left
	};



	public int x;
	public int y;
}

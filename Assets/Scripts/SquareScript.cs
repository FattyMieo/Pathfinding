using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareScript : MonoBehaviour
{
	private Square _position;
	public Square position
	{
		get { return _position; }
		set
		{
			_position = value;

			transform.localPosition = new Vector3
			(
				(_position.x + 0.5f - (BoardManager.instance.boardSize / 2.0f)) * BoardManager.instance.spacingWidth,
				(_position.y + 0.5f - (BoardManager.instance.boardSize / 2.0f)) * BoardManager.instance.spacingHeight,
				0.0f
			);
		}
	}

	//Debug
	public int x;
	public int y;

	// Use this for initialization
	void Start ()
	{
		x = position.x;
		y = position.y;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}

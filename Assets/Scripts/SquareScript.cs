using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareScript : TileScript
{
	protected Square _position = new Square();
	public Square position
	{
		get { return _position; }
		set
		{
			_position = value;

			CalibratePosition();

			x = _position.x;
			y = _position.y;
		}
	}

	// Use this for initialization
	void Awake ()
	{
		x = position.x;
		y = position.y;
	}

	protected override void CalibratePosition()
	{
		float offsetX = (_position.x + 0.5f - (BoardManager.instance.boardSize / 2.0f)) * BoardManager.instance.spacingWidth;
		float offsetY = (_position.y + 0.5f - (BoardManager.instance.boardSize / 2.0f)) * BoardManager.instance.spacingHeight;
		transform.localPosition = new Vector3(offsetX, offsetY, 0.0f);
	}
}

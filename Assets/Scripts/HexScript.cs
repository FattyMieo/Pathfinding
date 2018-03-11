using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexScript : TileScript
{
	protected Hex _position = new Hex();
	public Hex position
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
		Vector3 offsetX = new Vector3(BoardManager.instance.spacingWidth, -BoardManager.instance.spacingHeight / 2.0f, 0.0f) * _position.x;
		Vector3 offsetY = new Vector3(0.0f, -BoardManager.instance.spacingHeight, 0.0f) * _position.y;
		transform.localPosition = offsetX + offsetY;
	}
}

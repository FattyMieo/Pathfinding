using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexScript : TileScript<Hex>
{
	// Use this for initialization
	private new void Start ()
	{
		base.Start();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	protected override void CalibratePosition()
	{
		Vector3 offsetX = new Vector3(BoardManager.instance.spacingWidth, -BoardManager.instance.spacingHeight / 2.0f, 0.0f) * _position.x;
		Vector3 offsetY = new Vector3(0.0f, -BoardManager.instance.spacingHeight, 0.0f) * _position.y;
		transform.localPosition = offsetX + offsetY;
	}
}

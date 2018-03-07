using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareScript : TileScript<Square>
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
		float offsetX = (_position.x + 0.5f - (BoardManager.instance.boardSize / 2.0f)) * BoardManager.instance.spacingWidth;
		float offsetY = (_position.y + 0.5f - (BoardManager.instance.boardSize / 2.0f)) * BoardManager.instance.spacingHeight;
		transform.localPosition = new Vector3(offsetX, offsetY, 0.0f);
	}
}

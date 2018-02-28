using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexScript : MonoBehaviour
{
	private AxialHex _position;
	public AxialHex position
	{
		get { return _position; }
		set
		{
			_position = value;

			Vector3 offsetX = new Vector3(BoardManager.instance.spacingWidth, -BoardManager.instance.spacingHeight / 2.0f, 0.0f) * _position.x;
			Vector3 offsetY = new Vector3(0.0f, -BoardManager.instance.spacingHeight, 0.0f) * _position.y;
			transform.localPosition = offsetX + offsetY;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileScript<T> : MonoBehaviour where T : Tile
{
	protected T _position;
	public T position
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

	//Debug
	[Header("Position")]
	public int x;
	public int y;

	[Header("Tile Data")]
	public bool hasSelected;
	public int movementCost;

	// Use this for initialization
	protected void Start ()
	{
		x = position.x;
		y = position.y;
	}

	protected abstract void CalibratePosition();

	public virtual void BeginSearch()
	{
		hasSelected = false;
		movementCost = Mathf.Infinity;
	}
}

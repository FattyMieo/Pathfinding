using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileState
{
	Empty = 0,	//White

	Unchecked,	//Light Blue
	Checked,	//Orange
	Path,		//Yellow

	Origin,			//Red
	Destination,	//Green
	Obstacle,		//White

	Total
}

public abstract class TileScript : MonoBehaviour
{
	//Debug
	[Header("Position")]
	public int x;
	public int y;

	[Header("Tile Data")]
	public bool _isChecked;
	public bool isChecked
	{
		get
		{
			if(state != TileState.Origin && state != TileState.Destination && !_isObstacle)
			{
				if(_isChecked)
					state = TileState.Checked;
				else
					state = TileState.Unchecked;
			}
			return _isChecked; 
		}
		set
		{
			_isChecked = value;
			if(state != TileState.Origin && state != TileState.Destination && !_isObstacle)
			{
				if(_isChecked)
					state = TileState.Checked;
				else
					state = TileState.Unchecked;
			}
		}
	}
	public bool _isObstacle;
	public bool isObstacle
	{
		get
		{
			return _isObstacle; 
		}
		set
		{
			_isObstacle = value;
			if(state != TileState.Origin && state != TileState.Destination)
			{
				if(_isObstacle)
					state = TileState.Obstacle;
				else
					state = TileState.Empty;
			}
		}
	}
	public float movementCost;
	public TileScript parentTile;

	[Header("Tile Status")]
	public TileState state;

	[Header("Editor")]
	public SpriteRenderer rend;
	public float h;

	protected abstract void CalibratePosition();

	protected virtual void OnGUI()
	{
		switch(state)
		{
			case TileState.Empty:
				rend.color = Color.white;
				break;
			case TileState.Unchecked:
				rend.color = Color.Lerp(Color.Lerp(Color.blue, Color.white, 0.25f), Color.white, 0.5f);
				break;
			case TileState.Checked:
				rend.color = Color.Lerp(Color.Lerp(Color.red, Color.yellow, 0.5f), Color.white, 0.5f);
				break;
			case TileState.Path:
				rend.color = Color.Lerp(Color.yellow, Color.white, 0.5f);
				break;
			case TileState.Origin:
				rend.color = Color.green;
				break;
			case TileState.Destination:
				rend.color = Color.red;
				break;
			case TileState.Obstacle:
				rend.color = Color.gray;
				break;
		}
	}

	void OnMouseOver()
	{
		//Do not edit while pathfinding is in progress
		if(PathfindingManager.instance.isRunning) return;

		if(Input.GetMouseButton(0))
		{
			if(UIManager.instance.isOrigin)
			{
				if(this.isObstacle) return;
				UIManager.instance.lastTile.state = TileState.Empty;
				this.state = TileState.Origin;
				PathfindingManager.instance.origin = this;
				UIManager.instance.lastTile = PathfindingManager.instance.origin;
			}
			else if(UIManager.instance.isDestination)
			{
				if(this.isObstacle) return;
				UIManager.instance.lastTile.state = TileState.Empty;
				this.state = TileState.Destination;
				PathfindingManager.instance.destination = this;
				UIManager.instance.lastTile = PathfindingManager.instance.destination;
			}
			else if(UIManager.instance.isFilling)
			{
				isObstacle = true;
			}
			else if(UIManager.instance.isErasing)
			{
				isObstacle = false;
			}
			else
			{
				if(state == TileState.Origin)
				{
					UIManager.instance.isOrigin = true;
					UIManager.instance.lastTile = this;
				}
				else if(state == TileState.Destination)
				{
					UIManager.instance.isDestination = true;
					UIManager.instance.lastTile = this;
				}
				else
				{
					UIManager.instance.isFilling = !isObstacle;
					UIManager.instance.isErasing = isObstacle;
				}
			}
		}
	}
}

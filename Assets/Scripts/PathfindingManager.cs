using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathfindingStage
{
	Preparation = 0,
	SearchingOpenList,
	RetracePath
}

public enum PathfindingAlgorithm
{
	Dijkstra = 0,
	AStar
}

public class PathfindingManager : MonoBehaviour
{
	//Singleton
	private static PathfindingManager _instance;
	public static PathfindingManager instance
	{
		get { return _instance; }
	}

	[Header("Status")]
	public bool isRunning = false;
	public PathfindingStage stage;

	[Header("Settings")]
	public bool checkDiagonals;

	[Header("Time")]
	public float timeDelay = 0.5f;
	public float timePassed = 0.0f;

	[Header("Editor")]
	public LineRenderer retraceLine;

	//Pathfinding
	int originX, originY, destineX, destineY;
	TileScript[,] tileBoard;
	TileScript origin;
	TileScript destination;
	TileType tileType;
	List<TileScript> openList;
	List<TileScript> retraceList;

	//Listeners
	void Awake()
	{
		//Singleton
		if(_instance == null)
			_instance = this;
		else
			Destroy(this.gameObject);
	}

	void Start()
	{
		retraceLine.gameObject.SetActive(false);
	}

	void Update()
	{
		if(isRunning)
		{
			timePassed += Time.deltaTime;
			if(timePassed >= timeDelay)
			{
				switch(stage)
				{
					case PathfindingStage.Preparation:
						Preparation();
						break;
					case PathfindingStage.SearchingOpenList:
						SearchingOpenList();
						break;
					case PathfindingStage.RetracePath:
						RetracePath();
						break;
				}

				timePassed = 0.0f;
			}
		}
	}

	public void InitPoints()
	{
		if(BoardManager.instance.tileType == TileType.Square)
		{
			origin = BoardManager.instance.GetTile(Mathf.RoundToInt(BoardManager.instance.boardSize / 4.0f) - 1, Mathf.FloorToInt(BoardManager.instance.boardSize / 2.0f));
			origin.state = TileState.Origin;

			destination = BoardManager.instance.GetTile(Mathf.RoundToInt(BoardManager.instance.boardSize / 4.0f * 3.0f), Mathf.FloorToInt(BoardManager.instance.boardSize / 2.0f));
			destination.state = TileState.Destination;
		}
		else if(BoardManager.instance.tileType == TileType.Hex)
		{
			origin = BoardManager.instance.GetTile(-Mathf.RoundToInt(BoardManager.instance.boardSize / 2.0f), 0);
			origin.state = TileState.Origin;

			destination = BoardManager.instance.GetTile(Mathf.RoundToInt(BoardManager.instance.boardSize / 2.0f), 0);
			destination.state = TileState.Destination;
		}
	}

	public void FindDijkstraPath(int originX, int originY, int destineX, int destineY)
	{
		this.originX = originX;
		this.originY = originY;
		this.destineX = destineX;
		this.destineY = destineY;

		stage = PathfindingStage.Preparation;
		isRunning = true;
	}

	void Preparation()
	{
		//Clear Retrace Line
		retraceLine.gameObject.SetActive(false);

		//Begin Search (Reset everything in the board)
		tileBoard = BoardManager.instance.tileBoard;

		for(int i = 0; i < tileBoard.GetLength(0); i++)
		{
			for(int j = 0; j < tileBoard.GetLength(1); j++)
			{
				if(tileBoard[i, j] == null) continue;
				tileBoard[i, j].isChecked = false;
				tileBoard[i, j].movementCost = Mathf.Infinity;
				tileBoard[i, j].parentTile = null;
				if(tileBoard[i, j].isObstacle) continue;
				tileBoard[i, j].state = TileState.Empty;
			}
		}

		//Define origin & destination tiles
		origin = BoardManager.instance.GetTile(originX, originY);
		destination = BoardManager.instance.GetTile(destineX, destineY);

		//Set states
		origin.state = TileState.Origin;
		destination.state = TileState.Destination;

		//Get used tile type
		tileType = BoardManager.instance.tileType;

		//Set up open list
		openList = new List<TileScript>();

		//Set origin's movement cost to 0 and add to open list
		origin.movementCost = 0.0f;
		openList.Add(origin);

		//Go to next stage
		stage = (PathfindingStage)((int)stage + 1);
	}

	void SearchingOpenList()
	{
		//While open list is not empty
		if(openList.Count > 0 && !destination.isChecked)
		{
			//Set current tile as first in the list
			TileScript currentTile = openList[0];

			//Find tile with smallest movement cost
			for(int i = 1; i < openList.Count; i++)
			{
				if(openList[i].movementCost < currentTile.movementCost)
				{
					currentTile = openList[i];
				}
			}

			//Remove current tile from open list
			currentTile.isChecked = true;
			openList.Remove(currentTile);

			if(tileType == TileType.Square)
			{
				//Get neighbours of current tile
				Square s = ((SquareScript)currentTile).position;
				Square[] neighbours = s.GetNeighbours();

				for(int i = 0; i < neighbours.Length; i++)
				{
					//Check if coordinate is valid
					if
					(
						neighbours[i].y < 0 || neighbours[i].y >= tileBoard.GetLength(0) ||
						neighbours[i].x < 0 || neighbours[i].x >= tileBoard.GetLength(1)
					)
					{
						continue;
					}

					//Transform coordinate back to TileScript
					TileScript neighbour = BoardManager.instance.GetTile(neighbours[i].x, neighbours[i].y);

					//Skip if neighbour doesn't exist
					if(neighbour == null) continue;
					//Skip if neighbour is an obstacle
					if(neighbour.isObstacle) continue;
					//Skip if neighbour is checked
					if(neighbour.isChecked) continue;

					//New current tile's neighbour movement cost
					float newCost = currentTile.movementCost + s.GetCost(i);

					//If new cost is smaller, replace it and set parent tile
					if(newCost < neighbour.movementCost)
					{
						neighbour.movementCost = newCost;
						neighbour.parentTile = currentTile;

						if(!openList.Contains(neighbour))
						{
							openList.Add(neighbour);
						}
					}
				}
			}
			else if(tileType == TileType.Hex)
			{
				//Get neighbours of current tile
				Hex h = ((HexScript)currentTile).position;
				Hex[] neighbours = h.GetNeighbours();

				for(int i = 0; i < neighbours.Length; i++)
				{
					//Check if coordinate is valid
					if
					(
						neighbours[i].ToArrayPos(BoardManager.instance.boardSize).y < 0 || neighbours[i].ToArrayPos(BoardManager.instance.boardSize).y >= tileBoard.GetLength(0) ||
						neighbours[i].ToArrayPos(BoardManager.instance.boardSize).x < 0 || neighbours[i].ToArrayPos(BoardManager.instance.boardSize).x >= tileBoard.GetLength(1)
					)
					{
						continue;
					}

					//Transform coordinate back to TileScript
					TileScript neighbour = BoardManager.instance.GetTile(neighbours[i].x, neighbours[i].y);

					//Skip if neighbour doesn't exist
					if(neighbour == null) continue;
					//Skip if neighbour is an obstacle
					if(neighbour.isObstacle) continue;
					//Skip if neighbour is checked
					if(neighbour.isChecked) continue;

					//New current tile's neighbour movement cost
					float newCost = currentTile.movementCost + h.GetCost(i);

					//If new cost is smaller, replace it and set parent tile
					if(newCost < neighbour.movementCost)
					{
						neighbour.movementCost = newCost;
						neighbour.parentTile = currentTile;

						if(!openList.Contains(neighbour))
						{
							openList.Add(neighbour);
						}
					}
				}
			}
		}
		else
		{
			//Go to next stage
			stage = (PathfindingStage)((int)stage + 1);
		}
	}

	void RetracePath()
	{
		//Set up retrace list
		List<TileScript> retraceList = new List<TileScript>();
		List<Vector3> posList = new List<Vector3>();

		posList.Add(destination.transform.position);

		TileScript retraceTile = destination.parentTile;

		while(retraceTile != null && retraceTile != origin)
		{
			retraceTile.state = TileState.Path;
			retraceList.Add(retraceTile);
			posList.Add(retraceTile.transform.position);
			retraceTile = retraceTile.parentTile;
		}

		posList.Add(origin.transform.position);

		//Draw Retrace Line
		retraceLine.positionCount = posList.Count;
		retraceLine.SetPositions(posList.ToArray());
		retraceLine.gameObject.SetActive(true);

		//Go to next stage
		isRunning = false;
	}

	public void FindDijkstraPath(TileScript origin, TileScript destination)
	{
		FindDijkstraPath(origin.x, origin.y, destination.x, destination.y);
	}
}

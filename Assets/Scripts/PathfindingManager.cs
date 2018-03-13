using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathfindingStage
{
	Preparation = 0,
	SearchingOpenList,
	RetracePath,
	EndPhase
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
	public bool isPaused = false;
	public PathfindingStage stage;

	[Header("Settings")]
	public PathfindingAlgorithm algorithm;
	public bool checkDiagonals;
	[Range(1, 10)]
	public int weight = 1;

	[Header("Time")]
	public float timeDelay = 0.01f;
	public float timePassed = 0.0f;

	[Header("Editor")]
	public LineRenderer retraceLine;

	//Pathfinding
	TileScript[,] tileBoard;
	public TileScript origin;
	public TileScript destination;
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
			if(!isPaused)
			{
				timePassed += Time.deltaTime;
				if(timePassed >= timeDelay)
				{
					Step();
					timePassed = 0.0f;
				}
			}
		}
	}

	public void Step()
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
			case PathfindingStage.EndPhase:
				break;
		}
	}

	[ContextMenu("Run Pathfinding")]
	public void RunPathfinding()
	{
		PathfindingManager.instance.FindPath();
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

	public void FindPath(int originX, int originY, int destineX, int destineY)
	{
		//Define origin & destination tiles
		origin = BoardManager.instance.GetTile(originX, originY);
		destination = BoardManager.instance.GetTile(destineX, destineY);

		FindPath();
	}

	public void FindPath(TileScript origin, TileScript destination)
	{
		//Define origin & destination tiles
		this.origin = origin;
		this.destination = destination;

		FindPath();
	}

	public void FindPath()
	{
		stage = PathfindingStage.Preparation;
		isRunning = true;
		isPaused = false;
	}

	public void CleanBoard()
	{
		//Clear Retrace Line
		retraceLine.gameObject.SetActive(false);

		//Reset everything in the board
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

		//Set states
		origin.state = TileState.Origin;
		destination.state = TileState.Destination;
	}

	void Preparation()
	{
		//Clean Board
		CleanBoard();

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
				float newHeuristic = 0.0f;
				float curHeuristic = 0.0f;

				if(algorithm == PathfindingAlgorithm.AStar)
				{
					if(tileType == TileType.Square)
					{
						Square newSqr = ((SquareScript)openList[i]).position;
						Square curSqr = ((SquareScript)currentTile).position;

						newHeuristic = newSqr.GetHeuristic(destination.x, destination.y);
						curHeuristic = curSqr.GetHeuristic(destination.x, destination.y);
					}
					else if(tileType == TileType.Hex)
					{
						Hex newHex = ((HexScript)openList[i]).position;
						Hex curHex = ((HexScript)currentTile).position;

						newHeuristic = newHex.GetHeuristic(destination.x, destination.y);
						curHeuristic = curHex.GetHeuristic(destination.x, destination.y);
					}

					newHeuristic *= weight;
					curHeuristic *= weight;
				}

				if(openList[i].movementCost + newHeuristic < currentTile.movementCost + curHeuristic)
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
		stage = (PathfindingStage)((int)stage + 1);
	}
}

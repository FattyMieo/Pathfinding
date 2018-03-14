using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
	Square = 0,
	Hex,

	Total
}

public class BoardManager : MonoBehaviour
{
	//Singleton
	private static BoardManager _instance;
	public static BoardManager instance
	{
		get { return _instance; }
	}

	//Variables
	[Header("Debug")]
	public bool debugMode = false;
	[ContextMenuItem("Update Board", "UpdateBoard")]
	public bool constantUpdateMode = false;

	[Header("Prefabs")]
	public GameObject[] tilePrefab;

	[Header("Tile Settings")]
	public TileType tileType;
	[Range(0.0f, 3.0f)]
	public float tileSize = 1.0f;
	[Range(3, 100)]
	public int _boardSize = 20;
	public int boardSize
	{
		get
		{
			if(tileType == TileType.Hex) return Mathf.FloorToInt(_boardSize / 2.0f) + 1;
			else return _boardSize;
		}
		set
		{
			_boardSize = value;
		}
	}

	public TileScript[,] tileBoard
	{
		get
		{
			if(tileType == TileType.Square) return squareBoard;
			else if(tileType == TileType.Hex) return hexBoard;
			return null;
		}
		set
		{
			if(tileType == TileType.Square) squareBoard = value;
			else if(tileType == TileType.Hex) hexBoard = value;
		}
	}
	public TileScript[,] squareBoard;	//Can be flatstened to 1D for easier display
	public TileScript[,] hexBoard;		//Can be flattened to 1D for easier display

	[Header("Editor")]
	public Transform squareBoardParent;
	public Transform hexBoardParent;

	//Getters
	public float tileWidth
	{
		get
		{
			switch(tileType)
			{
				case TileType.Square:
					return tileSize;
				case TileType.Hex:
					return tileSize;
			}
			return tileSize;
		}
	}

	public float tileHeight
	{
		get
		{
			switch(tileType)
			{
				case TileType.Square:
					return tileSize;
				case TileType.Hex:
					return MathExtension.sqrt3 / 2.0f * tileWidth;
			}
			return tileSize;
		}
	}

	public float spacingWidth
	{
		get
		{
			switch(tileType)
			{
				case TileType.Square:
					return tileWidth;
				case TileType.Hex:
					return tileWidth * 3.0f / 4.0f;
			}
			return tileWidth / 2.0f;
		}
	}

	public float spacingHeight
	{
		get
		{
			switch(tileType)
			{
				case TileType.Square:
					return tileHeight;
				case TileType.Hex:
					return tileHeight;
			}
			return tileHeight / 2.0f;
		}
	}

	//Listeners
	void Awake()
	{
		//Singleton
		if(_instance == null)
			_instance = this;
		else
			Destroy(this.gameObject);
	}

	void Start ()
	{
		UpdateBoard();
	}

	void Update ()
	{
		if(constantUpdateMode)
			UpdateBoard();
	}

	//Functions
	public void CreateBoard(int size)
	{
		switch(tileType)
		{
			case TileType.Square:
				CreateSquareBoard(size);
				break;
			case TileType.Hex:
				CreateHexBoard(size);
				break;
		}
	}

	public void CreateSquareBoard(int size)
	{
		if (size <= 0) return;

		int prevSize = 0; //squareBoard == null _WHEN_ prevRadius <= 0

		if(tileBoard != null)
			prevSize = tileBoard.GetLength(0);

		if(size < prevSize)
		{
			for (int i = 0; i < prevSize; i++)
			{
				for (int j = 0; j < prevSize; j++)
				{
					if(i >= size || j >= size)
					{
						//Destroy
						if(debugMode) Debug.Log(j + "," + i + " is destroyed");
						Destroy(tileBoard[i, j].gameObject);
					}
				}
			}
		}

		SquareScript[,] newSquareBoard = new SquareScript[size, size];

		//Array search
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				if(i < prevSize && j < prevSize)
				{
					//Migrate
					newSquareBoard[i, j] = (SquareScript)tileBoard[i, j];
					if(debugMode) Debug.Log("squareBoard[" + i + "," + j + "] can be taken from old array[" + i + "," + j + "]");
				}
				else
				{
					//Instantiate
					newSquareBoard[i, j] = Instantiate(tilePrefab[(int)tileType]).GetComponent<SquareScript>();
					newSquareBoard[i, j].transform.parent = squareBoardParent;
					if(debugMode) Debug.Log("squareBoard[" + i + "," + j + "] is instantiated");
				}

				//Set Scale
				newSquareBoard[i, j].transform.localScale = Vector3.one * tileSize;
				newSquareBoard[i, j].position = new Square(j, i);

				newSquareBoard[i, j].isChecked = false;
				newSquareBoard[i, j].isObstacle = false;
				newSquareBoard[i, j].movementCost = Mathf.Infinity;
				newSquareBoard[i, j].parentTile = null;
				newSquareBoard[i, j].state = TileState.Empty;
			}
		}

		//Save new board
		tileBoard = newSquareBoard;
		return;
	}

	public void CreateHexBoard(int radius)
	{
		if (radius <= 0) return;

		int prevRadius = 0; //hexBoard == null _WHEN_ prevRadius <= 0

		if(tileBoard != null)
			prevRadius = (tileBoard.GetLength(0) - 1) / 2 + 1;

		if(radius < prevRadius)
		{
			for (int i = 0; i < prevRadius; i++)
			{
				Hex nextHex = AxialHex.GetDirection(4) * i;
				for(int dir = 0; dir < 6; dir++)
				{
					for (int j = 0; j < i; j++)
					{
						if(i >= radius)
						{
							//Destroy
							if(debugMode) Debug.Log(nextHex.ToArrayPos(prevRadius).x + "," + nextHex.ToArrayPos(prevRadius).y + " is destroyed");
							Destroy(tileBoard[nextHex.ToArrayPos(prevRadius).y, nextHex.ToArrayPos(prevRadius).x].gameObject);

							nextHex = nextHex.GetNeighbour(dir);
						}
					}
				}
			}
		}

		HexScript[,] newHexBoard = new HexScript[(radius - 1) * 2 + 1, (radius - 1) * 2 + 1];

		//Center piece
		if(0 < prevRadius)
		{
			//Migrate
			newHexBoard[0, 0] = (HexScript)tileBoard[0, 0];
			if(debugMode) Debug.Log("hexBoard[0,0] can be taken from old array[0,0]");
		}
		else
		{
			//Instantiate
			newHexBoard[0, 0] = Instantiate(tilePrefab[(int)tileType]).GetComponent<HexScript>();
			newHexBoard[0, 0].transform.parent = hexBoardParent;
			if(debugMode) Debug.Log("hexBoard[0,0] is instantiated");
		}

		//Set Scale
		newHexBoard[0, 0].transform.localScale = Vector3.one * tileSize;
		newHexBoard[0, 0].position = new Hex(0, 0);

		newHexBoard[0, 0].isChecked = false;
		newHexBoard[0, 0].isObstacle = false;
		newHexBoard[0, 0].movementCost = Mathf.Infinity;
		newHexBoard[0, 0].parentTile = null;
		newHexBoard[0, 0].state = TileState.Empty;

		//Ring search
		for (int i = 1; i < radius; i++)
		{
			Hex nextHex = AxialHex.GetDirection(4) * i;
			for(int dir = 0; dir < 6; dir++)
			{
				for (int j = 0; j < i; j++)
				{
					Hex arrayPos = nextHex.ToArrayPos(radius);
					Hex oldArrayPos = nextHex.ToArrayPos(prevRadius);

					if(i < prevRadius)
					{
						//Migrate
						newHexBoard[arrayPos.y, arrayPos.x] = (HexScript)tileBoard[oldArrayPos.y, oldArrayPos.x];
						if(debugMode) Debug.Log("hexBoard[" + arrayPos.y + "," + arrayPos.x + "] can be taken from old array[" + oldArrayPos.y + "," + oldArrayPos.x + "]");
					}
					else
					{
						//Instantiate
						newHexBoard[arrayPos.y, arrayPos.x] = Instantiate(tilePrefab[(int)tileType]).GetComponent<HexScript>();
						newHexBoard[arrayPos.y, arrayPos.x].transform.parent = hexBoardParent;
						//newHexBoard[nextHex.ToArrayPos(radius).y, nextHex.ToArrayPos(radius).x].position = new AxialHex(nextHex.y, nextHex.x);
						if(debugMode) Debug.Log("hexBoard[" + arrayPos.y + "," + arrayPos.x + "] is instantiated");
					}

					//Set Scale
					newHexBoard[arrayPos.y, arrayPos.x].transform.localScale = Vector3.one * tileSize;
					newHexBoard[arrayPos.y, arrayPos.x].position = new Hex(nextHex.x, nextHex.y);

					newHexBoard[arrayPos.y, arrayPos.x].isChecked = false;
					newHexBoard[arrayPos.y, arrayPos.x].isObstacle = false;
					newHexBoard[arrayPos.y, arrayPos.x].movementCost = Mathf.Infinity;
					newHexBoard[arrayPos.y, arrayPos.x].parentTile = null;
					newHexBoard[arrayPos.y, arrayPos.x].state = TileState.Empty;

					nextHex = nextHex.GetNeighbour(dir);
				}
			}
		}

		//Save new board
		tileBoard = newHexBoard;
		return;
	}

	public void UpdateBoard(bool skipInitPoint = false)
	{
		CreateBoard(boardSize);

		switch(tileType)
		{
			case TileType.Square:
				squareBoardParent.gameObject.SetActive(true);
				hexBoardParent.gameObject.SetActive(false);
				break;
			case TileType.Hex:
				hexBoardParent.gameObject.SetActive(true);
				squareBoardParent.gameObject.SetActive(false);
				break;
		}

		PathfindingManager.instance.retraceLine.gameObject.SetActive(false);

		if(!skipInitPoint)
			PathfindingManager.instance.InitPoints();
	}

	public TileScript GetTile(int x, int y)
	{
		if(tileType == TileType.Square)
		{
			return tileBoard[y, x];
		}
		else if(tileType == TileType.Hex)
		{
			Hex h = new Hex(x, y);
			h = h.ToArrayPos(boardSize);
			return tileBoard[h.y, h.x];
		}

		return null;
	}
}

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
	[Header("Connections")]
	public Transform squareBoardParent;
	public Transform hexBoardParent;

	[Header("Debug")]
	public bool debugMode = false;
	[ContextMenuItem("Update Board", "UpdateBoard")]
	public bool constantUpdateMode = false;

	[Header("Prefabs")]
	public GameObject[] tilePrefab;

	[Header("Settings")]
	public TileType tileType;
	[Range(0.0f, 3.0f)]
	public float tileSize = 1.0f;
	[Range(1, 50)]
	public int boardSize = 5;

	private SquareScript[,] squareBoard; //Can be flattened to 1D for easier display
	private HexScript[,] hexBoard; //Can be flattened to 1D for easier display

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

		if(squareBoard != null)
			prevSize = squareBoard.GetLength(0);

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
						Destroy(squareBoard[i, j].gameObject);
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
					newSquareBoard[i, j] = squareBoard[i, j];
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
			}
		}

		//Save new board
		squareBoard = newSquareBoard;
		return;
	}

	public void CreateHexBoard(int radius)
	{
		if (radius <= 0) return;

		int prevRadius = 0; //hexBoard == null _WHEN_ prevRadius <= 0

		if(hexBoard != null)
			prevRadius = (hexBoard.GetLength(0) - 1) / 2 + 1;

		if(radius < prevRadius)
		{
			for (int i = 0; i < prevRadius; i++)
			{
				AxialHex nextHex = AxialHex.GetDirection(4) * i;
				for(int dir = 0; dir < 6; dir++)
				{
					for (int j = 0; j < i; j++)
					{
						if(i >= radius)
						{
							//Destroy
							if(debugMode) Debug.Log(nextHex.ToArrayPos(prevRadius).x + "," + nextHex.ToArrayPos(prevRadius).y + " is destroyed");
							Destroy(hexBoard[nextHex.ToArrayPos(prevRadius).y, nextHex.ToArrayPos(prevRadius).x].gameObject);

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
			newHexBoard[0, 0] = hexBoard[0, 0];
			if(debugMode) Debug.Log("hexBoard[0,0] can be taken from old array[0,0]");
		}
		else
		{
			//Instantiate
			newHexBoard[0, 0] = Instantiate(tilePrefab[(int)tileType]).GetComponent<HexScript>();
			newHexBoard[0, 0].transform.parent = hexBoardParent;
			newHexBoard[0, 0].position = new AxialHex(0, 0);
			if(debugMode) Debug.Log("hexBoard[0,0] is instantiated");
		}
		//Set Scale
		newHexBoard[0, 0].transform.localScale = Vector3.one * tileSize;

		//Ring search
		for (int i = 1; i < radius; i++)
		{
			AxialHex nextHex = AxialHex.GetDirection(4) * i;
			for(int dir = 0; dir < 6; dir++)
			{
				for (int j = 0; j < i; j++)
				{
					AxialHex arrayPos = nextHex.ToArrayPos(radius);
					AxialHex oldArrayPos = nextHex.ToArrayPos(prevRadius);

					if(i < prevRadius)
					{
						//Migrate
						newHexBoard[arrayPos.y, arrayPos.x] = hexBoard[oldArrayPos.y, oldArrayPos.x];
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
					newHexBoard[arrayPos.y, arrayPos.x].position = new AxialHex(nextHex.x, nextHex.y);

					nextHex = nextHex.GetNeighbour(dir);
				}
			}
		}

		//Save new board
		hexBoard = newHexBoard;
		return;
	}

	public void UpdateBoard()
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
	}
}

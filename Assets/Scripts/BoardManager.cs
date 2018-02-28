using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	public bool constantUpdateMode = false;

	[Header("Prefabs")]
	public GameObject tilePrefab;

	[Header("Settings")]
	[Range(0.0f, 3.0f)]
	public float tileSize = 1.0f;

	[Range(1, 50)]
	public int boardRadius = 5;

	private HexScript[,] hexBoard; //Can be flattened to 1D for easier display

	//Getters
	public float tileWidth
	{
		get { return tileSize; }
	}
	public float tileHeight
	{
		get { return MathExtension.sqrt3 / 2.0f * tileWidth; }
	}

	public float spacingWidth
	{
		get { return tileWidth * 3.0f / 4.0f; }
	}
	public float spacingHeight
	{
		get { return tileHeight; }
	}

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
		CreateBoard(boardRadius);
	}

	void Update ()
	{
		if(constantUpdateMode)
			UpdateBoard();
	}

	public void CreateBoard(int radius)
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
			newHexBoard[0, 0] = Instantiate(tilePrefab).GetComponent<HexScript>();
			newHexBoard[0, 0].transform.parent = this.transform;
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
						newHexBoard[arrayPos.y, arrayPos.x] = Instantiate(tilePrefab).GetComponent<HexScript>();
						newHexBoard[arrayPos.y, arrayPos.x].transform.parent = this.transform;
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

	[ContextMenu("Create Board")]
	public void UpdateBoard()
	{
		CreateBoard(boardRadius);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
	private static BoardManager _instance;
	public static BoardManager instance
	{
		get { return _instance; }
	}

	public GameObject tilePrefab;

	public float tileSize = 1.0f;

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

	public int boardRadius = 5;

	//public List<AxialHex> hexBoard;
	public HexScript[,] hexBoard;

	void Awake()
	{
		if(_instance == null)
			_instance = this;
		else
			Destroy(this.gameObject);
	}

	// Use this for initialization
	void Start ()
	{
		//Init();
		CreateBoard(boardRadius);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	/*
	public void Init()
	{
		tileWidth = tileSize;
		tileHeight = MathExtension.sqrt3 / 2.0f * tileWidth;

		spacingWidth = tileWidth * 3.0f / 4.0f;
		spacingHeight = tileHeight;
	}
	*/

	[ContextMenu("Create Board")]
	public void UpdateBoard()
	{
		CreateBoard(boardRadius);
	}

	public void CreateBoard(int radius)
	{
		//Destroy code here
		//hexBoard.Reconfigure (boardRadius);
		//hexBoard.Clear();

		if (radius <= 0)
		{
			return;
		}

		HexScript[,] newHexBoard = new HexScript[(radius - 1) * 2 + 1, (radius - 1) * 2 + 1];

		//Center piec
		if(hexBoard != null)
		{
			newHexBoard[0, 0] = hexBoard[0, 0];
		}
		else
		{
			newHexBoard[0, 0] = Instantiate(tilePrefab).GetComponent<HexScript>();
			newHexBoard[0, 0].transform.parent = this.transform;
			newHexBoard[0, 0].position = new AxialHex(0, 0);
		}

		if (radius <= 1)
		{
			return;
		}

		//Ring search
		if(hexBoard != null)
		{
			for (int i = 1; i < radius; i++)
			{
				AxialHex nextHex = AxialHex.GetDirection(4) * i;
				for(int dir = 0; dir < 6; dir++)
				{
					for (int j = 0; j < i; j++)
					{
						if((i - 1) * 2 + 1 < hexBoard.GetLength(0))
						{
							newHexBoard[nextHex.ToArrayPos(radius).y, nextHex.ToArrayPos(radius).x] = hexBoard[nextHex.ToArrayPos(radius).y, nextHex.ToArrayPos(radius).x];
							Debug.Log(nextHex.ToArrayPos(radius).x + "," + nextHex.ToArrayPos(radius).y + " can be taken from old array");
						}
						else
						{
							Destroy(hexBoard[nextHex.ToArrayPos(radius).y, nextHex.ToArrayPos(radius).x].gameObject);
							newHexBoard[nextHex.ToArrayPos(radius).y, nextHex.ToArrayPos(radius).x] = Instantiate(tilePrefab).GetComponent<HexScript>();
							newHexBoard[nextHex.ToArrayPos(radius).y, nextHex.ToArrayPos(radius).x].transform.parent = this.transform;
							newHexBoard[nextHex.ToArrayPos(radius).y, nextHex.ToArrayPos(radius).x].position = new AxialHex(nextHex.y, nextHex.x);
							Debug.Log(nextHex.ToArrayPos(radius).x + "," + nextHex.ToArrayPos(radius).y + " is destroyed and reinstantiated");
						}

						nextHex = nextHex.GetNeighbour(dir);
					}
				}
			}
		}
		else
		{
			for (int i = 1; i < radius; i++)
			{
				AxialHex nextHex = AxialHex.GetDirection(4) * i;
				for(int dir = 0; dir < 6; dir++)
				{
					for (int j = 0; j < i; j++)
					{
						newHexBoard[nextHex.ToArrayPos(radius).y, nextHex.ToArrayPos(radius).x] = Instantiate(tilePrefab).GetComponent<HexScript>();
						newHexBoard[nextHex.ToArrayPos(radius).y, nextHex.ToArrayPos(radius).x].transform.parent = this.transform;
						newHexBoard[nextHex.ToArrayPos(radius).y, nextHex.ToArrayPos(radius).x].position = new AxialHex(nextHex.y, nextHex.x);
						Debug.Log(nextHex.ToArrayPos(radius).x + "," + nextHex.ToArrayPos(radius).y + " is instantiated");

						nextHex = nextHex.GetNeighbour(dir);
					}
				}
			}
		}

		hexBoard = newHexBoard;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
	public GameObject tilePrefab;

	public float tileSize = 1.0f;

	private float tileWidth;
	private float tileHeight;

	private float spacingWidth;
	private float spacingHeight;

	public int boardSize = 5;

	public List<AxialHex> hexBoard;

	// Use this for initialization
	void Start ()
	{
		Init();
		CreateBoard();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Init()
	{
		tileWidth = tileSize;
		tileHeight = MathExtension.sqrt3 / 2.0f * tileWidth;

		spacingWidth = tileWidth * 3.0f / 4.0f;
		spacingHeight = tileHeight;
	}

	[ContextMenu("Create Board")]
	public void CreateBoard()
	{
		//Destroy code here
		hexBoard.Clear ();

		if (boardSize <= 0)
		{
			return;
		}

		//Center piec
		hexBoard.Add (new AxialHex (0, 0));

		if (boardSize <= 1)
		{
			return;
		}

		//Ring
		for (int i = 1; i < boardSize; i++)
		{
			for(int dir = 0; dir < 6; dir++)
			{
				for (int j = 0; j < i; j++)
				{
					hexBoard.Add(hexBoard[hexBoard.Count - 1].GetNeighbour(dir));
				}
				dir++;
			}
		}
	}
}

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

	// Use this for initialization
	void Start ()
	{
		Init();
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

	public void GetNeighbours(int x, int y, int direction)
	{
		axialDirections[direction]
	}

	public void CreateBoard()
	{
		for(int i = 0; i < boardSize; i++)
		{
			for(int j = 0; j < i * 6; j++)
			{

			}
		}
	}
}

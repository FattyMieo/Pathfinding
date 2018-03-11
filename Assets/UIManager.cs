using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	//Singleton
	private static UIManager _instance;
	public static UIManager instance
	{
		get { return _instance; }
	}

	[Header("Test only")]
	public TileScript origin;
	public TileScript destination;

	//Listeners
	void Awake()
	{
		//Singleton
		if(_instance == null)
			_instance = this;
		else
			Destroy(this.gameObject);
	}

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	[ContextMenu("Run Dijkstra")]
	public void RunDijkstra()
	{
		PathfindingManager.instance.FindDijkstraPath(origin, destination);
	}
}

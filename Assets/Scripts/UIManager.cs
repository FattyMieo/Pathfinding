using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	//Singleton
	private static UIManager _instance;
	public static UIManager instance
	{
		get { return _instance; }
	}

	[Header("Options Panel")]
	public Dropdown boardType;
	public Slider size;
	public InputField sizeText;
	public Dropdown algorithm;
	public Slider weight;
	public InputField weightText;
	public Toggle diagonal;
	public Toggle crossGaps;
	public Slider speed;
	public InputField speedText;

	[Header("Control Panel")]
	public Button startBtn;
	public Text startBtnText;
	public Button stepBtn;
	public Button stopBtn;
	public Text stopBtnText;

	[Header("Mouse")]
	public bool isFilling;
	public bool isErasing;
	public TileScript lastTile;
	public bool isOrigin;
	public bool isDestination;

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
		boardType.value 	= (int)BoardManager.instance.tileType;
		size.value 			= BoardManager.instance.boardSize;
		sizeText.text 		= size.value.ToString();
		algorithm.value 	= (int)PathfindingManager.instance.algorithm;
		weight.value 		= PathfindingManager.instance.weight;
		weightText.text 	= weight.value.ToString();
		diagonal.isOn 		= PathfindingManager.instance.checkDiagonals;
		crossGaps.isOn		= PathfindingManager.instance.canCrossDiagonalGaps;
		speed.value 		= 1.0f / PathfindingManager.instance.timeDelay;
		speedText.text 		= speed.value.ToString("##.000");
	}

	// Update is called once per frame
	void Update ()
	{
		startBtn	.interactable = !(PathfindingManager.instance.isRunning && PathfindingManager.instance.stage == PathfindingStage.EndPhase);
		stepBtn		.interactable = PathfindingManager.instance.isRunning && PathfindingManager.instance.isPaused && PathfindingManager.instance.stage != PathfindingStage.EndPhase;
		stopBtn		.interactable = true;

		boardType	.interactable = !PathfindingManager.instance.isRunning;
		size		.interactable = !PathfindingManager.instance.isRunning;
		sizeText	.interactable = !PathfindingManager.instance.isRunning;
		algorithm	.interactable = !PathfindingManager.instance.isRunning;
		weight		.interactable = !PathfindingManager.instance.isRunning;
		weightText	.interactable = !PathfindingManager.instance.isRunning;
		diagonal	.interactable = !PathfindingManager.instance.isRunning;
		crossGaps	.interactable = !PathfindingManager.instance.isRunning;

		weight		.gameObject.SetActive(PathfindingManager.instance.algorithm	 == PathfindingAlgorithm.AStar);
		weightText	.gameObject.SetActive(PathfindingManager.instance.algorithm	 == PathfindingAlgorithm.AStar);
		diagonal	.gameObject.SetActive(BoardManager.instance.tileType		 == TileType.Square);
		crossGaps	.gameObject.SetActive(BoardManager.instance.tileType		 == TileType.Square && PathfindingManager.instance.checkDiagonals);

		//Mouse
		if(Input.GetMouseButtonUp(0))
		{
			isFilling = false;
			isErasing = false;

			lastTile = null;
			isOrigin = false;
			isDestination = false;
		}
	}

	void OnGUI()
	{
		startBtnText.text = (!PathfindingManager.instance.isRunning ? "Start Search" : (!PathfindingManager.instance.isPaused ? "Pause Search" : "Resume Search"));
		stopBtnText.text = (PathfindingManager.instance.isRunning && PathfindingManager.instance.stage == PathfindingStage.EndPhase ? "Clear Search" : (!PathfindingManager.instance.isRunning ? "Clear Board" : "Stop Search"));
	}

	public void OnBoardTypeChange()
	{
		if(!PathfindingManager.instance.isRunning)
		{
			BoardManager.instance.tileType = (TileType)boardType.value;
			BoardManager.instance.UpdateBoard();
		}
	}

	public void OnBoardSizeChange(bool isText)
	{
		int result;

		if(!isText)
		{
			result = (int)size.value;
			sizeText.text = result.ToString();
		}
		else
		{
			if(int.TryParse(sizeText.text, out result))
				size.value = result;
			else
				result = (int)size.value;
		}

		if(!PathfindingManager.instance.isRunning)
		{
			BoardManager.instance.boardSize = result;
			BoardManager.instance.tileSize = 10.0f / BoardManager.instance._boardSize;
			PathfindingManager.instance.retraceLine.startWidth = 2.0f / BoardManager.instance._boardSize;
			PathfindingManager.instance.retraceLine.endWidth = 2.0f / BoardManager.instance._boardSize;
			BoardManager.instance.UpdateBoard();
		}
	}

	public void OnAlgorithmChange()
	{
		if(!PathfindingManager.instance.isRunning)
			PathfindingManager.instance.algorithm = (PathfindingAlgorithm)algorithm.value;
	}

	public void OnWeightChange(bool isText)
	{
		int result;

		if(!isText)
		{
			result = (int)weight.value;
			weightText.text = result.ToString();
		}
		else
		{
			if(int.TryParse(weightText.text, out result))
				weight.value = result;
			else
				result = (int)weight.value;
		}

		if(!PathfindingManager.instance.isRunning)
			PathfindingManager.instance.weight = result;
	}

	public void OnDiagonalToggle()
	{
		if(!PathfindingManager.instance.isRunning)
			PathfindingManager.instance.checkDiagonals = diagonal.isOn;
	}

	public void OnCrossGapsToggle()
	{
		if(!PathfindingManager.instance.isRunning)
			PathfindingManager.instance.canCrossDiagonalGaps = crossGaps.isOn;
	}

	public void OnSpeedChange(bool isText)
	{
		int result;

		if(!isText)
		{
			result = (int)speed.value;
			speedText.text = result.ToString("##.000");
		}
		else
		{
			if(int.TryParse(speedText.text, out result))
				speed.value = result;
			else
				result = (int)speed.value;
		}

		PathfindingManager.instance.timeDelay = 1.0f / result;
	}

	public void OnStartBtnPress()
	{
		if(!PathfindingManager.instance.isRunning)
			PathfindingManager.instance.FindPath();
		else
			PathfindingManager.instance.isPaused = !PathfindingManager.instance.isPaused;
	}

	public void OnStepBtnPress()
	{
		if(PathfindingManager.instance.isRunning && PathfindingManager.instance.isPaused)
			PathfindingManager.instance.Step();
	}

	public void OnStopBtnPress()
	{
		if(PathfindingManager.instance.isRunning)
		{
			PathfindingManager.instance.isRunning = false;
			PathfindingManager.instance.CleanBoard();
		}
		else
		{
			BoardManager.instance.UpdateBoard(true);

			PathfindingManager.instance.origin.state = TileState.Origin;
			PathfindingManager.instance.destination.state = TileState.Destination;
		}
	}
}

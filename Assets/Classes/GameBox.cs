using UnityEngine;
using System.Collections;

public class GameBox : MonoBehaviour {
	
	// Block
	public Transform shape;
	private string[] shapes = {"T","L","J","S","Z","O","I"};
	private Transform currentBlock;
	private Block blockScript;
	private Vector3 spawnPoint;
	private string randomBlock;
	private UserInput userInput;
	private bool killing = false;

	// Brick
	public Transform brick;
	public Transform currentBrick;
	
	void Start () {
		userInput = transform.Find("/UserInput").GetComponent<UserInput>();
		spawnPoint = new Vector3(2f, 9f, 0f);
		StartCoroutine(SpawnBlock());
		
		// Register Touch Events
		userInput.TapTouchEvent += new TouchEventHandler(TapTouch);
		
		userInput.DownTouchEvent += new TouchEventHandler(DownTouch);
		userInput.QuickDownTouchEvent += new TouchEventHandler(QuickDownTouch);
		
		userInput.RightTouchEvent += new TouchEventHandler(RightTouch);
		userInput.QuickRightTouchEvent += new TouchEventHandler(QuickRightTouch);
		
		userInput.LeftTouchEvent += new TouchEventHandler(LeftTouch);
		userInput.QuickLeftTouchEvent += new TouchEventHandler(QuickLeftTouch);
		
		userInput.SwipedTouchEvent += new TouchEventHandler(SwipedTouch);
	}
	
	// --------- Touch Events -----------------------------------------------
	void SwipedTouch(TouchInput touchInput)
	{
		//swipeEnded = true;
		//lastPosition = touchInput.touchPosition;
	}
	
	void QuickRightTouch(TouchInput touchInput)
	{
		print ("QuickRightTouch");
		blockScript.StartCoroutine(blockScript.CoQuickRightTouch(touchInput));
	}
	
	void RightTouch(TouchInput touchInput)
	{
		print ("RightTouch");
		blockScript.StartCoroutine(blockScript.CoRightTouch(touchInput));
	}
	
	void QuickLeftTouch(TouchInput touchInput)
	{
		print ("QuickLeftTouch");
		blockScript.StartCoroutine(blockScript.CoQuickLeftTouch(touchInput));
	}
	
	void LeftTouch(TouchInput touchInput)
	{
		print ("LeftTouch");
		StartCoroutine(blockScript.CoLeftTouch(touchInput));
	}
	
	void QuickDownTouch(TouchInput touchInput)
	{
		//blockScript.StartCoroutine(blockScript.CoQuickDownTouch(touchInput));
	}
	
	void DownTouch(TouchInput touchInput)
	{
		print ("DownTouch");
		blockScript.StartCoroutine(blockScript.CoDownTouch(touchInput));
	}
	
	void TapTouch(TouchInput touchInput)
	{
		blockScript.StartCoroutine(blockScript.CoTapTouch(touchInput));
	}
	
	IEnumerator SpawnBlock()
	{
		yield return new WaitForSeconds(3f);
		killing = false;

		randomBlock = shapes[Random.Range(0,7)];
		//randomBlock = shapes[2];
		
		// Create Block
		currentBlock = Instantiate(shape.transform.Find(randomBlock) , spawnPoint, Quaternion.identity) as Transform;
		currentBlock.parent = this.transform;
		currentBlock.name = randomBlock;
		
		
		// Intialize Block
		blockScript = currentBlock.GetComponent<Block>();
		print ("Spawning " + currentBlock.name);
		blockScript.BlockDestroy += new BlockDestroyEventHandler(CleanBlock);
		
		userInput.ClearInput();
		userInput.LockInput(false);
	}
	
	IEnumerator CleanBlock()
	{
		userInput.LockInput(true);
		userInput.ClearInput();
		
		currentBlock.GetComponent<Block>().BlockDestroy -= new BlockDestroyEventHandler(CleanBlock);
		yield return new WaitForFixedUpdate();
		// Replace block bricks with simple game world bricks
		for(int i = 0; i < 4; i++)
			CreateBrick(currentBlock.GetComponent<Block>().blockBricks[i].transform.position);
		
		//killing = true;
		
		Destroy(currentBlock.gameObject);
		
		// Make a new block
		StartCoroutine(SpawnBlock());
		print("Cleaned");
	}
	
	void CreateBrick(Vector3 location)
	{
		currentBrick = Instantiate(brick,location,Quaternion.identity) as Transform;
		currentBrick.name = "Brick";
		currentBrick.parent = this.transform;
		currentBrick.GetComponent<Collider>().isTrigger = true;
	}

	
	void OnGUI() {
		
		GUIStyle style = new GUIStyle();
		style.fontSize = 40;
		GUI.Label(new Rect(700,10,700,40), " kill="+killing, style );	
	}
	
	void OnDestroy()
	{
		// Detach Touch Events
		userInput.TapTouchEvent -= new TouchEventHandler(TapTouch);
		
		userInput.DownTouchEvent -= new TouchEventHandler(DownTouch);
		userInput.QuickDownTouchEvent -= new TouchEventHandler(QuickDownTouch);
			
		userInput.RightTouchEvent -= new TouchEventHandler(RightTouch);
		userInput.QuickRightTouchEvent -= new TouchEventHandler(QuickRightTouch);
		
		userInput.LeftTouchEvent -= new TouchEventHandler(LeftTouch);
		userInput.QuickLeftTouchEvent -= new TouchEventHandler(QuickLeftTouch);
	}

}




	/*
	bool TouchIsNear()
	{
		if(Mathf.Abs(touchPosition.x-currentBlock.transform.position.x) < 1.5 && touchPosition.y<currentBlock.transform.position.y)
		{
			return true;
		}
		else
			return false;
	}*/
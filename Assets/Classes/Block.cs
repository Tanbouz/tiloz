using UnityEngine;
using System.Collections;

public delegate IEnumerator BlockDestroyEventHandler();

public class Block : MonoBehaviour {
	
	public event BlockDestroyEventHandler BlockDestroy;

	private bool blockFallPause = false;
	private bool blockFallStop = true;
	
	public float rotateBy = 90f;
	public RotateType rotateType;
	public enum RotateType {NoRotate , Single , Quad};

	private Transform rotateAxis;
	private Transform segments;
	private Transform ghost;
	public Transform[] blockBricks;
	
	public int rightCollided = 0;
	public int leftCollided = 0;
	public int downCollided = 0;
	public bool downCollidedNew = true;
	public bool ghostCollided = false;
	
	private static Vector3 stepRight = new Vector3(0.5f,0f,0f);
	private static Vector3 stepLeft = new Vector3(-0.5f,0f,0f);
	private static Vector3 stepDown = new Vector3(0f,-0.5f,0f);
	
	private UserInput userInput;
	private bool swipeEnded = false;
	private Vector3 lastPosition;
	
	void Start () 
	{
		userInput = transform.Find("/UserInput").GetComponent<UserInput>();
		rotateAxis = transform.Find("RotateAxis");
		segments = transform.Find("Segments");
		ghost = transform.Find("Ghost");
		blockBricks = new Transform[segments.childCount];

		for(int i = 0 ; i < segments.childCount ; i++)
		{
			blockBricks[i] = segments.GetChild(i);
		}
		
		
		switch(rotateType)
		{
		case RotateType.NoRotate:
			rotateBy = 0;
			break;
			
		case RotateType.Single:
			rotateBy = -90;
			break;
			
		case RotateType.Quad:
			rotateBy = 90;
			break;
		}
		
		
		// Allow the block to start falling
		BlockFallStart();
		

	}
	
	void MoveBlock(Vector3 translation)
	{
		//print("R "+rightCollided+" L "+leftCollided+" D "+downCollided);
		if(rightCollided > 0 && translation.x > 0 )
			translation.x = 0;
		if(leftCollided > 0 && translation.x < 0)
			translation.x = 0;
		if(downCollided > 0)
		{
			translation.y = 0;
			if(downCollidedNew == true)
			{
				downCollidedNew = false;
				print("TryingToDestroy");
				StartCoroutine(Grace());
			}
		}
		else
			downCollidedNew = true;

		transform.Translate(translation);
		//print (rightCollided+" "+leftCollided+" "+downCollided+"  "+Time.time);
	}
	
	IEnumerator Grace()
	{
		yield return new WaitForSeconds(0.5f);
		if(downCollided > 0 && !downCollidedNew)
			StartCoroutine(DestroyBlock());
		else
			downCollidedNew = true;
	}
	
	IEnumerator DestroyBlock()
	{
		yield return null;
		// Invoke block destroy event
		if(BlockDestroy != null)
		{
			swipeEnded = true;
			BlockFallStop();
			StartCoroutine(BlockDestroy());	
		}
	}
	
	void RotateBlock()
	{
		if(rotateType == RotateType.Single)
			rotateBy = rotateBy * -1;
		if(rotateType != RotateType.NoRotate)
		{
			segments.RotateAround(rotateAxis.transform.position,Vector3.forward,rotateBy);
			blockBricks[0].GetChild(0).transform.Rotate(new Vector3(0,0,-rotateBy),Space.Self);
			blockBricks[1].GetChild(0).transform.Rotate(new Vector3(0,0,-rotateBy),Space.Self);
			blockBricks[2].GetChild(0).transform.Rotate(new Vector3(0,0,-rotateBy),Space.Self);
			blockBricks[3].GetChild(0).transform.Rotate(new Vector3(0,0,-rotateBy),Space.Self);
		}
	}
	
	
	// ---------------- Co routines -------------------------------------------------
	
	// ---- Tap ----
	public IEnumerator CoTapTouch(TouchInput touchInput)
	{
		ghost.RotateAround(rotateAxis.transform.position,Vector3.forward,rotateBy);
		yield return new WaitForFixedUpdate();
		if(!ghostCollided)
		{
			RotateBlock();
		}
		else
			ghost.RotateAround(rotateAxis.transform.position,Vector3.forward,-rotateBy);
		
		userInput.ClearInput();
		
		ghostCollided = false;
	}
	
	// -------- Right --------
	
	// ---- Quick Right Touch ----
	public IEnumerator CoQuickRightTouch(TouchInput touchInput)
	{
		swipeEnded = false;
		userInput.LockInput(true);
		//yield return new WaitForFixedUpdate();
		if(rightCollided == 0)
		{
			for(int m = 0; m<4 ; m++)
			{
				yield return null;
				MoveBlock(new Vector3(0.1f,0f,0f));	
			}
			MoveBlock(new Vector3(0.1f,0f,0f));
		}
		userInput.LockInput(false);
		userInput.ClearInput();
	}

	// ---- Right Touch ----
	public IEnumerator CoRightTouch(TouchInput touchInput)
	{
		swipeEnded = false;
		int i = 0;
		int deltaMove = Mathf.Abs((int)(touchInput.touchBeganPosition.x - touchInput.touchPosition.x));
		
		while(!swipeEnded)
		{
			yield return new WaitForFixedUpdate();
			deltaMove = Mathf.Abs((int)(touchInput.touchBeganPosition.x - touchInput.touchPosition.x));
			while(i < deltaMove)
			{
				yield return new WaitForFixedUpdate();
				print ("Moving right");
				MoveBlock(stepRight);
				i++;
			}
			
		}
		/*
		while(!swipeEnded)
		{
			yield return new WaitForFixedUpdate();
			if(transform.position.x < touchInput.touchPosition.x)
				MoveBlock(stepRight);	
		}*/
	}
	

	// -------- Left --------
	
	// ---- Quick Left Touch ----
	public IEnumerator CoQuickLeftTouch(TouchInput touchInput)
	{
		swipeEnded = false;
		yield return new WaitForFixedUpdate();
		userInput.ClearInput();
		MoveBlock(stepLeft);	
	}
	
	// ---- Left Touch ----
	public IEnumerator CoLeftTouch(TouchInput touchInput)
	{
		swipeEnded = false;
		
		int i = 0;
		int deltaMove = Mathf.Abs((int)(touchInput.touchBeganPosition.x - touchInput.touchPosition.x));
		
		while(!swipeEnded)
		{
			yield return new WaitForFixedUpdate();
			deltaMove = Mathf.Abs((int)(touchInput.touchBeganPosition.x - touchInput.touchPosition.x));
			while(i < deltaMove)
			{
				yield return new WaitForFixedUpdate();
				MoveBlock(stepLeft);
				i++;
			}
		}
		
		/*
		while(!swipeEnded)
		{
			yield return new WaitForFixedUpdate();
			if(transform.position.x > touchInput.touchPosition.x)
				MoveBlock(stepLeft);	
		}*/
	}
	
	// -------- Down --------
	
	// ---- Quick Down Touch ----
	public IEnumerator coQuickDownTouch(TouchInput touchInput)
	{
		yield return new WaitForFixedUpdate();
		//userInput.LockInput(true);
		//BlockFallStop();
		//StartCoroutine(QuickFall());
	}

	// ---- Down Touch ----
	public IEnumerator CoDownTouch(TouchInput touchInput)
	{
		/*
		swipeEnded = false;
		while(!swipeEnded)
		{
			yield return new WaitForFixedUpdate();
			//if(transform.position.y > touchInput.touchPosition.y)
				MoveBlock(stepDown);
		}*/
		
		swipeEnded = false;
		
		int i = 0;
		int deltaMove = Mathf.Abs((int)(touchInput.touchBeganPosition.y - touchInput.touchPosition.y));
		
		while(!swipeEnded)
		{
			yield return new WaitForFixedUpdate();
			deltaMove = Mathf.Abs((int)(touchInput.touchBeganPosition.y - touchInput.touchPosition.y));
			while(i < deltaMove)
			{
				yield return new WaitForFixedUpdate();
				MoveBlock(stepDown);
				i++;
			}
		}
	}
	
	//void QuickFall()
		
	IEnumerator QuickFall()
	{
		while(downCollided == 0)
		{
			yield return new WaitForFixedUpdate();
			MoveBlock(stepDown);
		}
	}


	
	// Block Fall -----------------------------------------------
	IEnumerator BlockFall()
	{
		while(!blockFallStop)
		{
			if(!blockFallPause)
			{
				yield return new WaitForFixedUpdate();
				MoveBlock(stepDown);
			}
			yield return new WaitForSeconds(2f);	
		}
	}
	
	public void BlockFallStart()
	{
		if(blockFallStop)
		{
			blockFallStop = false;
			StartCoroutine(BlockFall());
		}
	}
	
	public void BlockFallResume()
	{
		blockFallPause = false;
	}
	
	public void BlockFallPause()
	{
		blockFallPause = true;
	}
	
	public void BlockFallStop()
	{
		blockFallStop = true;
	}
}

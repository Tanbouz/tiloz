using UnityEngine;
using System.Collections;

public class TouchInput{
	
	// +Public ------------------------------
	
	// TOUCH STATUS
	// * Touch public status values
	// * Intial status "clear"
	// * Stores previous status when the touch ends (Ex. touch ends, status = swiped , previous = down swipe)
	public enum TouchStatus {Clear, RightSwipe, LeftSwipe, UpSwipe, DownSwipe,
		QuickRightSwipe, QuickLeftSwipe, QuickUpSwipe, QuickDownSwipe, Swiped, Tapped , Deadzone};
	
	public TouchStatus touchStatus = TouchStatus.Clear;
	private TouchStatus touchStatusTemp = TouchStatus.Clear;
	public TouchStatus previousTouchStatus;
	
	// TOUCH TIMING
	// * Total time touch took from start to end
	// * Specifies how long a quick touch is
	// * Quick touch flag
	public float touchTime = 0;
	public float touchQuickTime = 0.3f;
	public bool touchQuick = false;
	
	// * Starting position of a touch
	public Vector2 touchBeganPosition;
	public Vector3 touchPosition;
	
	// -Public ------------------------------
	
	
	// +Private -----------------------------
	
	// POSITION
	// * Game world height in game units
	// * Game world width in game units
	// * Position of touch as game world coordinates
	private float worldHeight;
	private float worldWidth;
	private Vector3 worldSpace;
	
	// Difference in current and previous positions
	private float deltaX = 0;
	private float deltaY = 0;
	
	// Previous positions
	private float prePosX = 0;
	private float prePosY = 0;
	
	// The sum of changes/deltas in the touch position
	private float accumulatorX = 0;
	private float accumulatorY = 0;
	
	// Percetage deviation of the touch with respect to the world size (height/width)
	private float deviationPercX = 0;
	private float deviationPercY = 0;
	
	// Sensitivity determines when a touch becomes a swipe
	private int sensitivityX = 1;
	private int sensitivityY = 1;
	
	// Comfort determines how free (by distance) a user's finger can deviate for the same swipe type
	private int comfortX = 1;
	private int comfortY = 2;
	
	// Direction Change of touch 
	// * Set how sensitive it is to touch direction change
	// * Direction change tracker
	private int directionChangeSensitivity = 7;
	private int directionChange = 3;
	
	// -Private -----------------------------
	
	public TouchInput()
	{
		// Convert screen sizes to world sizes
		worldHeight = Camera.main.orthographicSize*2;
		worldWidth = Camera.main.aspect*worldHeight;
		
		touchBeganPosition = new Vector2(0,0);
	}
	
	public Vector2 CaptureTouch(Touch currentTouch)
	{
		// Get and Normalize Screen Coordinates to World Coordinates
		worldSpace = Camera.main.ScreenToWorldPoint(new Vector3(currentTouch.position.x,currentTouch.position.y,0));
		
		
		switch(currentTouch.phase)
		{
			//--------------------------------------------------------------------
			case TouchPhase.Began:
			
				// Set starting touch position
				touchBeganPosition = (Vector2)worldSpace;
				// Intialize previous positions with starting touch position
				prePosX = touchBeganPosition.x;
				prePosY = touchBeganPosition.y;
				// Reset values
				prePosX = 0;
				prePosY = 0;
				accumulatorX = 0;
				accumulatorY = 0;
				directionChange = directionChangeSensitivity;
				touchTime = 0;
				touchQuick = false;
				previousTouchStatus = TouchStatus.Clear;
				break;
		
			//--------------------------------------------------------------------
			case TouchPhase.Moved:
				
				// If touch has deviated too much off original direction , reset values to detect new direction
				if(directionChange < 0)
				{
					accumulatorX = 0;
					accumulatorY = 0;
					directionChange = directionChangeSensitivity;
				}
			
				// Calculate delta change in world units for each axis
				deltaX = worldSpace.x - prePosX; // Delta = Current World position - Previous world position
				deltaY = worldSpace.y - prePosY;
				
				// Update previous position with current position
				prePosX = worldSpace.x; 
				prePosY = worldSpace.y;
				
				// Add up changes in touch position (motion)
				accumulatorX += deltaX;
				accumulatorY += deltaY;
			
				// Calculate percentage deviation of the touch with respect to game world size
				deviationPercX = (accumulatorX/worldWidth*100);
		    	deviationPercY = (accumulatorY/worldHeight*100);
				
				// Detect changes in touch direction
				if(deltaX < 0 && touchStatus == TouchStatus.RightSwipe) // If delta is negative (going left) and current direction is right
					directionChange--;
				else if(deltaX > 0 && touchStatus == TouchStatus.LeftSwipe)
					directionChange--;
					
				
				// The value of both devaitions determines the nature of the swipe (vertical or horizontal)
				// If deviation in the X axis is less than the comfort AND the % deviation in Y is greater than sensitiviity then Vertical Swipe.
				if(Mathf.Abs(deviationPercX) < comfortX && Mathf.Abs(deviationPercY) > sensitivityY )
					{
						// The sign of the devaition value determines whether the vertical swipe is up or down
						if(deviationPercY > 0)
							touchStatusTemp = TouchStatus.UpSwipe;
						else
							touchStatusTemp = TouchStatus.DownSwipe;
					}
				// Horizontal
				else if(Mathf.Abs(deviationPercY) < comfortY && Mathf.Abs(deviationPercX) > sensitivityX )
					{
						if(deviationPercX > 0)
							touchStatusTemp = TouchStatus.RightSwipe;
						else
							touchStatusTemp = TouchStatus.LeftSwipe;
					}
				// Neither vertical or horizontal
				else if(Mathf.Abs(deviationPercY) > comfortY && Mathf.Abs(deviationPercX) > comfortX)
				{
					accumulatorX = 0;
					accumulatorY = 0;
					directionChange = directionChangeSensitivity;
				}
				
				if(touchTime > touchQuickTime)
					touchStatus = touchStatusTemp;
			
				touchTime += Time.deltaTime;
				break;
			
			//--------------------------------------------------------------------
			case TouchPhase.Stationary:
			
				touchTime += Time.deltaTime;
				break;
			
			//--------------------------------------------------------------------
			case TouchPhase.Ended:
			
				previousTouchStatus = touchStatus;
				// Tap or Swipe?
				if(Mathf.Abs(touchBeganPosition.x-worldSpace.x) < 0.5f && Mathf.Abs(touchBeganPosition.y-worldSpace.y) < 0.5f )
					touchStatus = TouchStatus.Tapped;
				
				else if(touchTime < touchQuickTime)
						switch(touchStatusTemp)
						{
							case TouchStatus.DownSwipe:
								touchStatus = TouchStatus.QuickDownSwipe;
								break;
							case TouchStatus.UpSwipe:
								touchStatus = TouchStatus.QuickUpSwipe;
								break;
							case TouchStatus.RightSwipe:
								touchStatus = TouchStatus.QuickRightSwipe;
								break;
							case TouchStatus.LeftSwipe:
								touchStatus = TouchStatus.QuickLeftSwipe;
								break;
						}
				else
					touchStatus = TouchStatus.Swiped;
				break;
			
			default:
				break;
		}
		
		// Return current touch position
		touchPosition = new Vector2(worldSpace.x,worldSpace.y);
		return touchPosition;
	}
	
	public void ClearTouchStatus()
	{
		touchStatus = TouchStatus.Clear;
		touchQuick = false;
	}		
}

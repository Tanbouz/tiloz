using UnityEngine;
using System.Collections;

public delegate void TouchEventHandler(TouchInput touchInput);

public class UserInput : MonoBehaviour {
	
	// Touch
	public event TouchEventHandler TapTouchEvent;
	
	public event TouchEventHandler RightTouchEvent;
	public event TouchEventHandler QuickRightTouchEvent;
	
	public event TouchEventHandler LeftTouchEvent;
	public event TouchEventHandler QuickLeftTouchEvent;
	
	//public event TouchEventHandler UpTouchEvent;
	//public event TouchEventHandler QuickUpTouchEvent;
	
	public event TouchEventHandler DownTouchEvent;
	public event TouchEventHandler QuickDownTouchEvent;
	
	public event TouchEventHandler SwipedTouchEvent;
	
	public TouchInput touchInput;
	public Touch currentTouch;
	public bool inputLocked = false;
	public Vector3 touchPosition;
	public TouchInput.TouchStatus preTouchStatus;
	
	void Start()
	{
		touchInput = new TouchInput();
	}
	
	void Update () {
		if(Input.touchCount > 0)
		{
			preTouchStatus = touchInput.touchStatus;
			touchPosition = touchInput.CaptureTouch(Input.GetTouch(0));
			if(preTouchStatus != touchInput.touchStatus && !inputLocked)
				StartCoroutine(TouchEventUpdate());
		}
	}
	
	IEnumerator TouchEventUpdate()
	{
		//yield return new WaitForFixedUpdate();
		yield return null;
			
		switch(touchInput.touchStatus)
		{
			
		//--------------------------------------------------------------------
		case TouchInput.TouchStatus.Tapped:
			if(TapTouchEvent != null)
				TapTouchEvent(touchInput);
			break;
			
		//--------------------------------------------------------------------	
		case TouchInput.TouchStatus.DownSwipe:
			if(DownTouchEvent != null)
				DownTouchEvent(touchInput);
			break;
			
		case TouchInput.TouchStatus.QuickDownSwipe:
			if(QuickDownTouchEvent != null)
				QuickDownTouchEvent(touchInput);
			break;
			
		//--------------------------------------------------------------------	
		case TouchInput.TouchStatus.RightSwipe:
			if(RightTouchEvent != null)
				RightTouchEvent(touchInput);
			break;
			
		case TouchInput.TouchStatus.QuickRightSwipe:
			if(QuickRightTouchEvent != null)
				QuickRightTouchEvent(touchInput);
			break;
			
		//--------------------------------------------------------------------
		case TouchInput.TouchStatus.LeftSwipe:
			if(LeftTouchEvent != null)
				LeftTouchEvent(touchInput);
			break;
		
		case TouchInput.TouchStatus.QuickLeftSwipe:
			if(QuickLeftTouchEvent != null)
				QuickLeftTouchEvent(touchInput);
			break;
			

		//--------------------------------------------------------------------
		case TouchInput.TouchStatus.Swiped:
			if(SwipedTouchEvent != null)
				SwipedTouchEvent(touchInput);
			break;
		}
		
	}
	
	public void LockInput(bool lockInput)
	{
		inputLocked = lockInput;
	}
	
	public void ClearInput()
	{
		touchInput.ClearTouchStatus();
	}
	
	void OnGUI() {
		
		GUIStyle style = new GUIStyle();
		style.fontSize = 40;
		if(Input.touchCount > 0)
			GUI.Label(new Rect(10,10,700,40), " Swipe="+touchInput.touchStatus, style );	
		else
			GUI.Label(new Rect(10,10,700,40), " Swipe="+touchInput.touchStatus, style );	
	}
}

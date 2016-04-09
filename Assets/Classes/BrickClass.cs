using UnityEngine;
using System.Collections;

 
public class BrickClass : MonoBehaviour {
	
	Block block;
	
	void Awake(){
		block = transform.root.GetComponent<Block>();
	}
	
	void OnTriggerEnter() {

		//print(name+" Enter Time = "+Time.time + " *");
		if(block != null)
		{
		switch(name)
			{
			case "DOWN":
				block.downCollided++;
				break;
			case "RIGHT":
				block.rightCollided++;
				break;
			case "LEFT":
				block.leftCollided++;
				break;
			}
		}
    }
	
	void OnTriggerExit() 
	{
		StartCoroutine(TriggerExitDelayed());
    }
	
	IEnumerator TriggerExitDelayed()
	{
		yield return null;
		if(block != null)
		{
		switch(name)
			{
			case "RIGHT":
				block.rightCollided--;
				break;
			case "LEFT":
				block.leftCollided--;
				break;
			case "DOWN":
				block.downCollided--;
				break;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class GhostBlock : MonoBehaviour {

	Block block;
	
	void Awake(){
		block = transform.root.GetComponent<Block>();
	}
	
	void OnTriggerEnter() {
		print ("cookies");
		if(block != null)
		{
			block.ghostCollided = true;
		}
    }
}

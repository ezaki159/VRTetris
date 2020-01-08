using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTable : MonoBehaviour {

	// Use this for initialization
	private void Start() {
		GameMaster.GM.GameEnded += Kill;
	}
	
	private void Kill() {
		var tableBlocks = transform.GetChild(0);
		foreach (Transform block in tableBlocks) {
			block.GetComponent<BasicCube>().Kill();
		}

		transform.GetChild(1).GetComponent<TetrisGridCreator>().Kill();
		Destroy(gameObject, 5.0f);
	}
}

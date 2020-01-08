using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoGroundKill : GroundKill {
	public GameObject FXIndicatorPrefab;
	private GameObject FXIndicatorInstance;
	
	protected override void OutOfBounds() {
		GameMaster.GM.Score -= 5;
		Destroy(gameObject);
	}

	protected override void Kill() {
		foreach (Transform child in transform) 
			child.GetComponent<BasicCube>().Kill();
		GameMaster.GM.Lives--;
		if (FXIndicatorInstance != null)
			Destroy(FXIndicatorInstance.gameObject);
		Destroy(gameObject, 5.0f);
	}
	
	protected override float TimeToLive() {
		return 10f;
	}

	protected override void OnGroundEnterEffect() {
		if (FXIndicatorInstance == null)
			FXIndicatorInstance = Instantiate(FXIndicatorPrefab, transform.position, FXIndicatorPrefab.transform.rotation);
	}

	protected override void OnGroundExitEffect() {
		if (FXIndicatorInstance != null)
			Destroy(FXIndicatorInstance);
	}
}

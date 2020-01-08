using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeGroundKill : GroundKill {
	
	protected override void OutOfBounds() {
		Destroy(gameObject);
	}

	protected override void Kill() {
		GetComponent<Pickaxe>().Kill();
	}
	
	protected override float TimeToLive() {
		return 6f;
	}

	protected override void OnGroundEnterEffect() { }

	protected override void OnGroundExitEffect() { }
}

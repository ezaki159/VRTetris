using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScript : MonoBehaviour {
	private void Start() {
		GameMaster.GM.GameEnded += GameOver;
	}

	private void GameOver() {
		for (int i = 2; i < 5; i++) {
			transform.GetChild(i).gameObject.SetActive(false);
		}

		transform.GetChild(5).gameObject.SetActive(true);
	}
}

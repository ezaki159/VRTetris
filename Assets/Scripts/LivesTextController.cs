using UnityEngine;
using UnityEngine.UI;

public class LivesTextController : MonoBehaviour {
	private Text _text;
	// Use this for initialization
	private void Start () {
		_text = GetComponent<Text>();
		GameMaster.GM.LivesChanged += UpdateLivesText;
		GameMaster.GM.Lives = GameMaster.GM.InitialLives;
	}

	private void UpdateLivesText(int newLives) {
		_text.text = string.Format("Lives:   {0}", newLives);
	}
}

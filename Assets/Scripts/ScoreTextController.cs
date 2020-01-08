using UnityEngine;
using UnityEngine.UI;

public class ScoreTextController : MonoBehaviour {
	private Text _text;
	// Use this for initialization
	private void Start () {
		_text = GetComponent<Text>();
		GameMaster.GM.ScoreChanged += UpdateScoreText;
	}

	private void UpdateScoreText(int newScore) {
		_text.text = string.Format("Score: {0}", newScore);
	}
}

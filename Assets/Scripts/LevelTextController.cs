using UnityEngine;
using UnityEngine.UI;

public class LevelTextController : MonoBehaviour {
	private Text _text;

	private void Start () {
		_text = GetComponent<Text>();
		GameMaster.GM.LevelChanged += UpdateLevelText;
		GameMaster.GM.Level = GameMaster.GM.InitialLevel;
	}

	private void UpdateLevelText(int newLevel) {
		_text.text = string.Format("Level:   {0}", newLevel);
	}
}

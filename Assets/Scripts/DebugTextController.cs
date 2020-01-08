using UnityEngine;
using UnityEngine.UI;

public class DebugTextController : MonoBehaviour {
	private Text _text;

	// Use this for initialization
	private void Start() {
		_text = gameObject.GetComponent<Text>();
		if (_text == null)
			Destroy(this);
	}

	public void OnEnable() {
		Application.logMessageReceived += LogMessage;
	}

	public void OnDisable() {
		Application.logMessageReceived -= LogMessage;
	}

	public void LogMessage(string message, string stackTrace, LogType type) {
		if (message == null)
			message = "";
		_text.text = message + "\n" + _text.text;
	}
}

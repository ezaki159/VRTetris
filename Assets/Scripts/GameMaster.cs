using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster GM;
     
    [Tooltip("Number of tetromino cubes in a line (x axis)")]
	public int LineSize = 5;
	
	[Tooltip("Number of tetromino cubes in a column (z axis)")]
	public int ColumnSize = 5;
	
	[Tooltip("Height of the Tetris area (y axis, in tetromino cube units)")]
	public int AreaHeight = 5;

	[Tooltip("Size of a tetromino cube unit")]
	public float TetrominoCubeSize = 0.1f;
	
	[SerializeField, Tooltip("Current player Score")] 
	private int _score;

	[Tooltip("Initial lives for the player")]
	public int InitialLives = 5;

	[SerializeField, Tooltip("Current lives")]
	private int _lives;
	
	[Tooltip("Initial level of the game")]
	public int InitialLevel = 0;
	
	[Tooltip("Current level")]
	private int _level;
	
	[Tooltip("Tetromino Pieces")]
	public GameObject[] TetrominoPrefabs;

	[Tooltip("Possible Tetromino Colors")]
	public Material[] colors;
	
	public delegate void ScoreChangedDelegate(int score);
	public event ScoreChangedDelegate ScoreChanged;

	public delegate void LivesChangedDelegate(int lives);
	public event LivesChangedDelegate LivesChanged;
	
	public delegate void LevelChangedDelegate(int level);
	public event LevelChangedDelegate LevelChanged;
	
	public delegate void KillEveryoneDelegate();
	public event KillEveryoneDelegate GameEnded;

	public int Score {
		get { return _score; }
		set {
			_score = value;
			OnScoreChanged(_score);
		}
	}

	public int Lives {
		get { return _lives; }
		set {
			_lives = value;
			OnLivesChanged(_lives);
			if (_lives == 0)
				EndGame();
		}
	}

	public int Level {
		get { return _level; }
		set {
			_level = value;
			OnLevelChanged(_level);
		}
	}

	private void Start() {
		Lives = InitialLives;
		Level = InitialLevel;
		Score = 0;
	}

	private void EndGame() {
		OnGameEnd();
	}
	
	protected virtual void OnGameEnd() {
		var handler = GameEnded;
		if (handler != null) {
			handler();
		}
	}

	protected virtual void OnScoreChanged(int newScore) {
		var handler = ScoreChanged;
		if (handler != null) {
			handler(newScore);
		}
	}

	protected virtual void OnLivesChanged(int lives) {
		var handler = LivesChanged;
		if (handler != null) {
			handler(lives);
		}
	}
	
	protected virtual void OnLevelChanged(int level) {
		var handler = LevelChanged;
		if (handler != null) {
			handler(level);
		}
	}

	private void Awake(){
        GM = this;
    }

	private void Update() {
		if (Input.GetKeyDown(KeyCode.K)) {
			OnGameEnd();
		}
	}
}

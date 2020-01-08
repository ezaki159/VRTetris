using System.Collections;
using UnityEngine;

public class TetrominoSpawner : MonoBehaviour {
    public GameObject[] TretrominoPrefabs;
    public Material[] Colors;
    public float SpawnRate;
    public float Side;
    public float Height;

    public int Level {
        get { return _level; }
        set {
            _level = value;
            Debug.Log("Level was changed to " + _level);
            CancelInvoke();
            var spawnInterval = 32 / Mathf.Pow(2, _level);
            InvokeRepeating("SpawnTetromino", 1.0f, spawnInterval);
        }
    }

    private GameObject _nextTetromino;

    private Vector3[] _directions;
    public Transform NextTetrominoHolder;
    
    [SerializeField]
    private int _level;

    public float LevelIncreaseTime;

    // Use this for initialization
    private void Start() {
        _directions = new[] {
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(-1.0f, 0.0f, 0.0f),
            new Vector3(0.0f, -1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, -1.0f)
        };
        _nextTetromino = CreateTetromino();
        InvokeRepeating("SpawnTetromino", 1.0f, SpawnRate);
        StartCoroutine(IncreaseLevel());
    }

    private IEnumerator IncreaseLevel() {
        yield return new WaitForSeconds(LevelIncreaseTime);
        Level++;
        if (Level < 5)
            StartCoroutine(IncreaseLevel());
    }

    private void SpawnTetromino() {
        Destroy(_nextTetromino.GetComponent<NextTetrominoRotator>());
        _nextTetromino.GetComponent<Rigidbody>().isKinematic = false;
        PositionTetromino(_nextTetromino.transform);
        _nextTetromino = CreateTetromino();
    }

    private void PositionTetromino(Transform tetromino) {
        tetromino.parent = transform;
        var dir = _directions[Random.Range(0, _directions.Length)];
        var x = Random.Range(-Side, Side);
        var z = Random.Range(-Side, Side);
        tetromino.position = new Vector3(x, Height, z);
        tetromino.rotation = Quaternion.LookRotation(dir, new Vector3(0.0f, 1.0f, 0.0f));
    }

    private GameObject CreateTetromino() {
        var t = TretrominoPrefabs[Random.Range(0, TretrominoPrefabs.Length)];
        var color = Colors[Random.Range(0, Colors.Length)];

        var piece = Instantiate(t, NextTetrominoHolder);
        piece.AddComponent<TetrominoGroundKill>();
        piece.GetComponent<Rigidbody>().drag = 20;
        piece.GetComponent<Rigidbody>().isKinematic = true;
        piece.AddComponent<NextTetrominoRotator>();
        foreach (Transform child in piece.transform) {
            child.GetComponent<Renderer>().material = color;
        }

        return piece;
    }
}
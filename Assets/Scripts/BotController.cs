using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

public class BotController : MonoBehaviour {
    [Tooltip("Prefab to instantiate the rare pickaxe pickup")]
    public GameObject PickaxePrefab;

    [Tooltip("Percentage chance for the pickaxe to spawn")]
    public int PickaxeChance = 5;

    [Tooltip("Position to spawn and grab the next piece from")]
    public Transform NextPiecePosition;

    [Tooltip("Duration of the initial carrying animation")]
    public float InitialAnimationDuration;

    [Tooltip("Length of the side of the piece drop area")]
    public float Side;

    [Tooltip("Height from which to drop the pieces")]
    public float Height;

    [Tooltip("Maximum difficulty level")] public int MaxLevel = 4;

    [Tooltip("Amount to subtract from the initial duration for each difficulty level")]
    public int DifficultyStep = 10;

    [Tooltip("Duration of each difficulty level")]
    public int LevelDuration = 60;

    [SerializeField, Tooltip("Current difficulty level")]
    private int _level = 0;

    [SerializeField, Tooltip("Offset of the piece being carried")]
    private Vector3 _pickUpOffset = Vector3.down * 0.4f;

    private Vector3 _endPoint;
    private Transform _startPoint;
    private float _elapsedTime;
    private float _animationDuration;
    private bool _carrying;
    private GameObject _nextPiece;
    private GameObject _heldPiece;
    public GameObject BlockMarioPrefab;
    public GameObject FXIndicatorPrefab;


    public int Level {
        get { return _level; }
        set {
            _level = value;
            _animationDuration = InitialAnimationDuration - value * DifficultyStep;
            GameMaster.GM.Level = _level;
        }
    }

    // Use this for initialization
    private void Start() {
        Physics.IgnoreLayerCollision(12, 13, true);
        _carrying = false;
        _nextPiece = CreatePiece();
        _endPoint = NextPiecePosition.position;
        _elapsedTime = 0;
        _animationDuration = InitialAnimationDuration;
        _level = GameMaster.GM.InitialLevel;
        GameMaster.GM.GameEnded += Kill;
        StartCoroutine(IncreaseLevel());
    }

    private IEnumerator IncreaseLevel() {
        yield return new WaitForSeconds(LevelDuration);
        if (Level < MaxLevel) {
            Level++;
            StartCoroutine(IncreaseLevel());
        }
    }

    // Update is called once per frame
    private void Update() {
        if (transform.parent.position == _endPoint) {
            if (_carrying) {
                DropPiece();
                _endPoint = NextPiecePosition.position;
                _carrying = false;
                _elapsedTime = 0;
                return;
            }
        }

        _elapsedTime += Time.deltaTime;
        transform.parent.position =
            Vector3.Slerp(transform.parent.position, _endPoint, _elapsedTime / _animationDuration);

        transform.rotation = Quaternion.LookRotation(_endPoint - transform.position);
        transform.rotation = Quaternion.Euler(-90.0f, transform.rotation.eulerAngles.y + 90.0f, 0.0f);
    }

    private void DropPiece() {
        _heldPiece.transform.parent = null;
        _heldPiece.GetComponent<Rigidbody>().isKinematic = false;
        var interactable = _heldPiece.GetComponent<Interactable>();
        interactable.highlightOnHover = true;
        interactable.hideHandOnAttach = true;
        interactable.enabled = true;
        _heldPiece.GetComponent<Throwable>().enabled = true;
        if (_heldPiece.CompareTag("Pickaxe")) {
            _heldPiece.GetComponent<Pickaxe>().enabled = true;
            _heldPiece.GetComponent<Pickaxe>().ActivateFX();
        }
        else {
            _heldPiece.GetComponent<TetrominoSnapController>().enabled = true;
        }
        _heldPiece = null;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Container") && !_carrying) {
            GrabPiece(_nextPiece);
            _nextPiece = CreatePiece();
            _carrying = true;
            _elapsedTime = 0;
        }
    }

    private GameObject CreatePiece() {
        GameObject piece;
        if (Random.Range(0, 101) < PickaxeChance) {
            piece = CreatePickaxe();
        }
        else {
            piece = CreateTetromino();
        }
        
        var interactable = piece.GetComponent<Interactable>();
        interactable.highlightOnHover = false;
        interactable.hideHandOnAttach = false;
        interactable.enabled = false;
        piece.GetComponent<Throwable>().enabled = false;

        piece.GetComponent<Rigidbody>().drag = 20;
        piece.GetComponent<Rigidbody>().isKinematic = true;
        piece.AddComponent<NextTetrominoRotator>();

        return piece;
    }

    private GameObject CreatePickaxe() {
        var piece = Instantiate(PickaxePrefab, NextPiecePosition);
        piece.GetComponent<Pickaxe>().enabled = false;
        piece.AddComponent<PickaxeGroundKill>();
        return piece;
    }

    private void GrabPiece(GameObject piece) {
        var x = Random.Range(-Side, Side);
        var z = Random.Range(-Side, Side);

        Destroy(piece.GetComponent<NextTetrominoRotator>());

        piece.transform.localRotation = Quaternion.identity;
        piece.transform.localPosition = _pickUpOffset;

        _endPoint = new Vector3(x, Height, z);

        piece.transform.parent = transform.parent;
        _heldPiece = piece;
    }

    private GameObject CreateTetromino() {
        var tetrominoPrefabs = GameMaster.GM.TetrominoPrefabs;
        var t = tetrominoPrefabs[Random.Range(0, tetrominoPrefabs.Length)];
        var colors = GameMaster.GM.colors;
        var color = colors[Random.Range(0, colors.Length)];

        var piece = Instantiate(t, NextPiecePosition);
        piece.GetComponent<TetrominoSnapController>().enabled = false;
        var groundKill = piece.AddComponent<TetrominoGroundKill>();
        groundKill.FXIndicatorPrefab = FXIndicatorPrefab;
        foreach (Transform child in piece.transform) {
            child.GetComponent<Renderer>().material = color;
            child.gameObject.layer = 12;
        }

        return piece;
    }

    public void Kill() {
        Instantiate(BlockMarioPrefab, transform.position, BlockMarioPrefab.transform.rotation);
        Destroy(transform.parent.gameObject);
    }
}
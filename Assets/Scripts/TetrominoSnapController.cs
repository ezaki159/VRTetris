using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TetrominoSnapController : MonoBehaviour
{
    private Transform _tableTransform;
    private bool _needsSnapping;
    private float _snappingY;
    private Rigidbody _rb;
    private GameObject _ghost;
    private int _gridUnitsTouching = 0;
    private bool _inHand;
    private bool _placed = false;
    private AudioSource _audioSource;
    private bool _done = false;
    
    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        var throwable = GetComponent<Throwable>();
        if (throwable != null) {
            throwable.onPickUp.AddListener(PickedUp);
            throwable.onDetachFromHand.AddListener(Dropped);
        }
    }

    private void PickedUp() {
        if (_placed)
            return;
        
        _rb.drag = 0;
        _inHand = true;
        foreach (Transform child in transform) 
            child.gameObject.layer = 11;
        
        _needsSnapping = _gridUnitsTouching != 0;
        
        GetComponent<GroundKill>().OnPickUp();
    }

    private void Dropped() {
        if (_placed)
            return;
        
        _inHand = false;
        
        if (_ghost != null && _gridUnitsTouching != 0) {
            _placed = true;
            Destroy(GetComponent<Throwable>());
            Destroy(GetComponent<VelocityEstimator>());
            Destroy(GetComponent<Interactable>());
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<BoxCollider>().size *= 0.51f;
                child.gameObject.layer = 14;
            }

            _rb.freezeRotation = true;        
            transform.position = _ghost.transform.position;
            transform.rotation = _ghost.transform.rotation;
        }
        else 
            foreach (Transform child in transform) 
                child.gameObject.layer = 0;      
        
        if (_ghost != null)
            DestroyGhost();
    }

    private void DestroyGhost() {
        Destroy(_ghost);
        _ghost = null;
        MakeVisible(gameObject);
    }

    private void OnCollisionEnter(Collision other) {
        if (!_done && _placed && other.gameObject.name == "Table") {
            _audioSource.Play();
            _done = true;
            SnapObjectPosition(gameObject);
            Destroy(GetComponent<TetrominoSnapController>());
            Destroy(GetComponent<Rigidbody>()); 
            if (_ghost != null)
                DestroyGhost();
            
            foreach (Transform child in transform) {
                var gridCreator = _tableTransform.gameObject.GetComponent<TetrisGridCreator>();
                var renderer = child.GetComponent<Renderer>();
                    
                var initialPosition = _tableTransform.InverseTransformPoint(child.position);
                var localPosition = initialPosition / GameMaster.GM.TetrominoCubeSize;
                localPosition = new Vector3(
                    Mathf.Round(localPosition.x),
                    Mathf.Round(localPosition.y),
                    Mathf.Round(localPosition.z)
                );
                Material material = renderer.material;
                renderer.enabled = false;
                gridCreator.AddBlock((int) localPosition.x, (int) localPosition.y, (int) localPosition.z, material);
            }
            Destroy(gameObject, 0.3f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _tableTransform = other.transform.parent.parent;
        _needsSnapping = _inHand;
        _gridUnitsTouching++;
    }

    private void OnTriggerExit(Collider other) {
        if (--_gridUnitsTouching == 0) {
            DestroyGhost();
        }
    }

    private void FixedUpdate() {
        if (_placed) {
            _rb.velocity = new Vector3(0, -2, 0);
            SnapObjectRotation(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (_needsSnapping && _inHand) {
            var ghostCreated = false;
            if (_ghost == null) {
                CreateGhost();
                ghostCreated = true;
            }
            _needsSnapping = false;
            _ghost.transform.position = transform.position;
            _ghost.transform.rotation = transform.rotation;
            SnapObjectRotation(_ghost);
            SnapObjectPosition(_ghost);
            CheckGhost();
            if (_ghost != null && ghostCreated)
                Pulse(GetComponent<Interactable>().attachedToHand);
        }
    }

    private void CheckGhost() {
        bool valid = true;
        if (_ghost != null) {
            foreach (Transform child in _ghost.transform) {
                var initialPosition = _tableTransform.InverseTransformPoint(child.position);
                var localPosition = initialPosition / GameMaster.GM.TetrominoCubeSize;
                localPosition = new Vector3(
                    Mathf.Round(localPosition.x),
                    Mathf.Round(localPosition.y),
                    Mathf.Round(localPosition.z)
                );
                if (!_tableTransform.gameObject.GetComponent<TetrisGridCreator>().CheckGridUnit((int) localPosition.x,
                    (int) localPosition.y, (int) localPosition.z))
                    valid = false;
            }
            if (!valid)
                DestroyGhost();
        }
    }
    
    private void Pulse(Hand hand)
    {
            StartCoroutine(LongVibration(hand, 0.05f, 3999));
    }
 
 
    private static IEnumerator LongVibration(Hand hand, float length, ushort strength)
    {
        for(float i = 0.0f; i < length; i += Time.deltaTime)
        {
            hand.TriggerHapticPulse(strength);
            yield return null; //every single frame for the duration of "length" you will vibrate at "strength" amount
        }
    }

    private void CreateGhost() {
        _ghost = Instantiate(gameObject);
        foreach (Transform child in _ghost.transform) {
            Destroy(child.gameObject.GetComponent<Collider>());
        }

        Destroy(_ghost.GetComponent<TetrominoSnapController>());
        Destroy(_ghost.GetComponent<Throwable>());
        Destroy(_ghost.GetComponent<VelocityEstimator>());
        Destroy(_ghost.GetComponent<Interactable>());
        Destroy(_ghost.GetComponent<Rigidbody>());
        MakeTransparent(gameObject);
    }

    private static void MakeTransparent(GameObject go) {
        SetAlpha(go, 0.7f);
    }

    private static void MakeVisible(GameObject go) {
        SetAlpha(go, 1.0f);
    }

    private static void SetAlpha(GameObject go, float alpha) {
        foreach (Transform child in go.transform) {
            var blockRenderer = child.GetComponent<Renderer>();
            var color = blockRenderer.material.color;
            color.a = alpha;
            blockRenderer.material.color = color;
        }
    }

    private void SnapObjectPosition(GameObject gObject) {
        var blockSize = GameMaster.GM.TetrominoCubeSize;
        var cube = gObject.transform.GetChild(0);
        var initialPosition = _tableTransform.InverseTransformPoint(cube.transform.position);
        var localPosition = initialPosition / blockSize;
        localPosition = new Vector3(
            Mathf.Round(localPosition.x),
            Mathf.Round(localPosition.y),
            Mathf.Round(localPosition.z)
        );
        localPosition *= blockSize;
        gObject.transform.position = _tableTransform.TransformPoint(_tableTransform.InverseTransformPoint(gObject.transform.position) - (initialPosition - localPosition));
    }

    private void SnapObjectRotation(GameObject gObject)
    {
        var localRotation = Quaternion.Inverse(_tableTransform.rotation) * gObject.transform.localRotation;
        var rot = _tableTransform.rotation * Quaternion.Euler(PieceOrientation.ClosestValidOrientation(localRotation));
        gObject.transform.rotation = rot;
    }
}

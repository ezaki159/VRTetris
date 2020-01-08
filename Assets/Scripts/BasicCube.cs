using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasicCube : MonoBehaviour {
    public GameObject prefab;
    private ParticleSystem system;
    private GameObject systemOBJ;
    private AudioSource audioSource;

    void Start() {
        systemOBJ = Instantiate(prefab);
        system = systemOBJ.GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null) {
            audioSource.spatialize = true;
        }
        system.GetComponent<ParticleSystemRenderer>().material = GetComponent<Renderer>().material;
        system.transform.parent = transform;
    }

    public void Kill() {
        if (audioSource != null) {
            audioSource.Play();
        }

        var emitParams = new ParticleSystem.EmitParams {
            position = transform.position,
            velocity = new Vector3(Random.Range(-1.0f, 1.0f), 10.0f, Random.Range(-1.0f, 1.0f)),
            angularVelocity3D = new Vector3(Random.Range(-90f, 90.0f), 90, Random.Range(-90.0f, 90.0f))
        };
        system.Emit(emitParams, 1);
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        Destroy(gameObject, 5.0f);
    }
}
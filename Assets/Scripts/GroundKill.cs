using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class GroundKill : MonoBehaviour {

	private float accumulatedTime = 0;
	private bool grounded = false;
	private bool dead = false;

	public float offset = 0.008f;
	private Vector3 initialScale;
	private float step = 0;
	private Rigidbody rb;

	void Start() {
		initialScale = transform.localScale;
		rb = GetComponent<Rigidbody>();
	}

	void Update() {
		if (dead)
			return;

		if (transform.position.y < -1) {
			OutOfBounds();
			return;
		}

		if (grounded) {
			accumulatedTime += Time.deltaTime;
			transform.localScale = initialScale + new Vector3(offset, offset, offset) * Mathf.Cos(step);
			step = (step + accumulatedTime * 4 * Time.deltaTime) % (2 * Mathf.PI);

			if (accumulatedTime > TimeToLive()) {
				dead = true;
				Kill();	
			}
		}

		else {
			accumulatedTime = 0;
		}
	}

	protected abstract void OutOfBounds();

	protected abstract void Kill();

	protected abstract float TimeToLive();

	protected abstract void OnGroundEnterEffect();

	protected abstract void OnGroundExitEffect();

	private void OnCollisionEnter(Collision other) {
		if ((other.transform.parent != null && other.transform.parent.name == "Plane")) {
			grounded = true;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			OnGroundEnterEffect();
		}
	}

	public void OnPickUp() {
		grounded = false;
		rb.constraints = RigidbodyConstraints.None;
		OnGroundExitEffect();
	}
}

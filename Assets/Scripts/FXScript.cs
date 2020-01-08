using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXScript : MonoBehaviour {
	private Vector3 initScale;
	public float rate = 5;
	private float step = 0;
	public float offset = 0.4f;
	private Vector3 mask = new Vector3(1, 0, 1);
	

	// Use this for initialization
	void Start () {
		initScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.identity;
		transform.localScale = initScale + mask * offset * Mathf.Cos(step);
		step = (step + rate * Time.deltaTime) % (2 * Mathf.PI);
	}
}

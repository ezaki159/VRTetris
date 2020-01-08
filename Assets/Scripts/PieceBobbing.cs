using UnityEngine;

public class PieceBobbing : MonoBehaviour {
	public bool reverse = false;
	public float magnitude = 2;
	public float rate = 4.5f;
	private Vector3 initPosition;
	private float step = 0;

	// Use this for initialization
	void Start () {
		initPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		int i = reverse ? -1 : 1;
		transform.position = initPosition + Vector3.up * magnitude * Mathf.Cos(step) * i;
		step = (step + rate * Time.deltaTime) % (2 * Mathf.PI);
	}
}

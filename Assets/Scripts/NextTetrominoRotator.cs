using UnityEngine;

public class NextTetrominoRotator : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		var newRotation = transform.rotation;
		newRotation.eulerAngles += (Vector3.forward + Vector3.up) * 90 * Time.deltaTime;
		transform.rotation = newRotation;
	}
}

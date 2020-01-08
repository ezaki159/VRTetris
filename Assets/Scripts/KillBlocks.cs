using UnityEngine;

public class KillBlocks : MonoBehaviour {
    private bool done = false;

    private void LateUpdate() {
        if (!done) {
            Kill();
            done = true;
        }
    }

    public void Kill() {
        foreach (Transform block in transform)
            block.gameObject.GetComponent<BasicCube>().Kill();
        Destroy(gameObject, 5.0f);
    }
}
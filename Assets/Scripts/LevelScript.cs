using UnityEngine;
using Valve.VR;

public class LevelScript : MonoBehaviour {
    public GameObject cubePrefab;
    private int height;
    public int childCount;

    public void AddBlock(int x, int z, Material color) {
        var block = Instantiate(cubePrefab);
        block.layer = 10;
        block.tag = "PlacedBlock";
        GameObject gridUnit = GetGridUnitByCoords(x, z);
        gridUnit.GetComponent<Collider>().enabled = false;
        block.transform.parent = gridUnit.transform;
        block.transform.localPosition = Vector3.zero;
        block.transform.localRotation = Quaternion.identity;
        block.GetComponent<Renderer>().material = color;
        childCount++;
    }

    public void CreateGrid() {
        for (var z = 0; z < GameMaster.GM.ColumnSize; z++) {
            for (var x = 0; x < GameMaster.GM.LineSize; x++) {
                var gridUnit = new GameObject(string.Format("Block {0}x{1}", x, z));
                gridUnit.transform.parent = transform;
                gridUnit.transform.localScale = Vector3.one * GameMaster.GM.TetrominoCubeSize;
                gridUnit.transform.localPosition = new Vector3(x, 0.0f, z) * GameMaster.GM.TetrominoCubeSize;
                var gridUnitCollider = gridUnit.AddComponent<BoxCollider>();
                gridUnitCollider.isTrigger = true;
            }
        }
    }

    public void SetHeight(int h) {
        height = h;
        transform.localPosition = new Vector3(0.0f, h * GameMaster.GM.TetrominoCubeSize, 0.0f);
    }

    public void Lower() {
        int y = Mod(height - 1, GameMaster.GM.AreaHeight);
        SetHeight(y);
    }

    private int Mod(int x, int m) {
        return (x % m + m) % m;
    }


    public void Activate(float x, float z) {
        GetGridUnitByCoords(x, z).SetActive(true);
    }

    public GameObject GetGridUnitByCoords(float x, float z) {
        return transform.GetChild((int) (x + GameMaster.GM.LineSize * z)).gameObject;
    }

    public void KillLevel() {
        foreach (Transform child in transform) {
            if (child.childCount > 0) {
                Transform block = child.GetChild(0);
                block.gameObject.GetComponent<BasicCube>().Kill();
                childCount--;
                child.GetComponent<Collider>().enabled = true;
            }
        }
    }

    public bool IsComplete() {
        var size = GameMaster.GM.ColumnSize * GameMaster.GM.LineSize;
        return childCount >= size;
    }

    public void PlaceRandomBlocks(int n) {
        var colors = GameMaster.GM.colors;
        int added = 0;
        int x, z;
        while (added < n) {
            x = Random.Range(0, GameMaster.GM.ColumnSize);
            z = Random.Range(0, GameMaster.GM.LineSize);
            var c = colors[Random.Range(0, colors.Length)];

            if (GetGridUnitByCoords(x, z).transform.childCount == 0) {
                AddBlock(x, z, c);
                added++;
            }
        }
    }
}
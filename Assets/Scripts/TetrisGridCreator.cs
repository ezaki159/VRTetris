using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TetrisGridCreator : MonoBehaviour {
    public GameObject blockPrefab;
    public GameObject botObject;
    private BotController _botController;
    public int Height = 5;
    public int RandomBlockCount = 30;
    public int randomHeight;

    private void Start() {
        
        for (var y = 0; y < Height; y++) {
            var level = new GameObject(string.Format("LevelScript {0}", y));
            level.transform.parent = transform;
            var ls = level.gameObject.AddComponent<LevelScript>();
            ls.cubePrefab = blockPrefab;
            ls.SetHeight(y);
            ls.CreateGrid();
            if (SceneManager.GetActiveScene().buildIndex == 1 && y < randomHeight) {
                ls.PlaceRandomBlocks(RandomBlockCount/randomHeight);      
            }
        }

        _botController = botObject.GetComponent<BotController>();
    }


    private void Update() {
        var completedLevels = new List<Transform>();
        foreach (Transform level in transform) {
            var ls = level.GetComponent<LevelScript>();
            if (ls == null)
                continue;
            if (ls.IsComplete()) {
                completedLevels.Add(level);
            }
        }

        foreach (var level in completedLevels) {
            var ls = level.GetComponent<LevelScript>();
            ls.KillLevel();
            LowerLevels(level.GetSiblingIndex());
            var topHeight = GameMaster.GM.AreaHeight - 1;
            ls.SetHeight(topHeight);
            level.SetSiblingIndex(topHeight);
        }

        var scores = new[] {0, 40, 100, 300, 1200};
        var newScore = scores[completedLevels.Count];
        if (newScore == 0) return;
        GameMaster.GM.Score += newScore * (_botController.Level + 1);
    }

    void LowerLevels(int index) {
        foreach (Transform level in transform) {
            if (level.GetSiblingIndex() > index) {
                LevelScript ls = level.GetComponent<LevelScript>();
                if (ls == null)
                    continue;
                ls.Lower();
            }
        }
    }

    public bool CheckGridUnit(int x, int y, int z) { // checks if grid unit is available to place blocks
        if (x < 0 || y < 0 || z < 0 || x >= GameMaster.GM.ColumnSize || y >= GameMaster.GM.AreaHeight ||
            z >= GameMaster.GM.LineSize)
            return false;

        return transform.GetChild(y).GetComponent<LevelScript>().GetGridUnitByCoords(x, z).GetComponent<Collider>()
            .enabled;
    }

    public void AddBlock(int x, int y, int z, Material color) {
        transform.GetChild(y).GetComponent<LevelScript>().AddBlock(x, z, color);
    }

    public void Kill() {
        foreach (Transform level in transform) {
            level.GetComponent<LevelScript>().KillLevel();
            
        }
    }
}
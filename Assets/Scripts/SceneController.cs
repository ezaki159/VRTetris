using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public string Scene1;
    public string Scene2;
    public string Scene3;

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SceneManager.LoadScene(Scene1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SceneManager.LoadScene(Scene2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SceneManager.LoadScene(Scene3);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
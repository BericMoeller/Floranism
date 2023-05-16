using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public int CurrentLevel = 0;
    public GameObject worldObject;
    public World worldScript;
    public string level1;
    public string level2;
    public string level3;
    private List<string> levels;
    private void Start()
    {
        levels = new List<string>();
        levels.Add(level1);
        levels.Add(level2);
        levels.Add(level3);
        worldScript = worldObject.GetComponent<World>();
    }
    public static void LoadLevel(string level)
    {
        Debug.Log("Attempting to load " + level);
        SceneManager.LoadScene(level);
    }
    public static void QuitGame()
    {
        Debug.Log("Attempting to quit...");
        Application.Quit();
    }
    public static void WinGame(string winner)
    {
        LoadLevel("Menu");
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    public void MoveUp()
    {
        CurrentLevel++;
        string newLevel = levels[CurrentLevel];
        worldScript.UpdateLevel(newLevel);
    }
}

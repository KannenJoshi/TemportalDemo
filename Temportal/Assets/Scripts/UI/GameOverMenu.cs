using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// https://www.youtube.com/watch?v=76WOa6IU_s8&list=WL&index=2
public class GameOverMenu : MonoBehaviour
{
    public static bool IsGameOver;

    private static GameObject _gameOverScreen;

    private void Start()
    {
        IsGameOver = false;
        _gameOverScreen = transform.GetChild(0).gameObject;
    }

    public static void GameOver()
    {
        IsGameOver = true;
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
        _gameOverScreen.SetActive(true);
    }
    
    public void StartGame()
    {
        IsGameOver = false;
        _gameOverScreen.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void MainMenu()
    {
        _gameOverScreen.SetActive(false);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUITTING...");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// https://www.youtube.com/watch?v=76WOa6IU_s8&list=WL&index=2
public class MainMenu : MonoBehaviour
{
    [SerializeField] private string firstLevel;
    [SerializeField] private GameObject options;

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevel, LoadSceneMode.Single);
    }

    public void OpenOptions()
    {
        options.SetActive(true);
    }

    public void CloseOptions()
    {
        options.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUITTING...");
    }
}

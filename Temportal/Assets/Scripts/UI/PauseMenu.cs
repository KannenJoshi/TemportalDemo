using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private static bool _isPaused;

    public static bool IsPaused
    {
        get => _isPaused;
        set
        {
            _isPaused = value;
            if (value)
                Pause();
            else
            {
                UnPause();
            }
        }
    }

    private static GameObject _pauseScreen;

    private static float _oldTimeScale = 1f;
    
    private void Awake()
    {
        _pauseScreen = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        UnPause();
    }

    public void ResumeGame()
    {
        UnPause();
        StartCoroutine(HideMouse());
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUITTING...");
    }

    private static void Pause()
    {
        _isPaused = true;
        _oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
        _pauseScreen.SetActive(true);
    }

    private static void UnPause()
    {
        _isPaused = false;
        Time.timeScale = _oldTimeScale;
                
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _pauseScreen.SetActive(false);
    }

    IEnumerator HideMouse()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
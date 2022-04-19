using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    
    [SerializeField] private GameObject options;

    private static float _oldTimeScale;
    
    private void Awake()
    {
        _pauseScreen = transform.GetChild(0).gameObject;
    }

    public void ResumeGame()
    {
        UnPause();
        StartCoroutine(HideMouse());
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
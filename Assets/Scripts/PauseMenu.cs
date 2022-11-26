using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] 
    private LevelLoader _levelLoader;
    
    [SerializeField]
    private GameObject _pauseMenu;

    private bool _isPaused;

    private void Awake()
    {
        _pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace)) {
           _isPaused = !_isPaused;
           PauseGame();
        }
    }

    void PauseGame() {
        if(_isPaused)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            _pauseMenu.SetActive(true);
        }
        else 
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            _pauseMenu.SetActive(false);
        }
    }

    public void ResumeGame() {
       Time.timeScale = 1;
       _pauseMenu.SetActive(false);
    }

    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit() {
        Application.Quit();
    }

    public void Restart() {
        Time.timeScale = 1;
        _pauseMenu.SetActive(false);
        _levelLoader.ReloadCurrentLevel();
    }

}

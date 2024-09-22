using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu;
    public Button quitButton;
    private bool paused = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Debug.Log("Pause Menu Start()");
        quitButton.onClick.AddListener(quitGame);
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                unpauseGame();
            }
            else // if (!paused)
            {
                pauseGame();
            }
        }
    }

    private void pauseGame()
    {
        // desbloqueamos raton
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // pausamos
        paused = true;
        Time.timeScale = 0;
        // renderizamos el menu
        pauseMenu.SetActive(true);
    }

    private void unpauseGame()
    {
        // bloqueamos raton
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // despausamos
        paused = false;
        Time.timeScale = 1;
        // derenderizamos el menu
        pauseMenu.SetActive(false);
    }
    
    
    public void quitGame()
    {
        Debug.Log("Quit pressed");
        Application.Quit();
        Debug.Log("Application.Quit() called");
    }
    
    
}

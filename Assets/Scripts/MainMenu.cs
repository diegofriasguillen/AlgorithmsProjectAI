using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1f;
    }
    public void Play()
    {
        SceneManager.LoadScene("Level1");
        Time.timeScale = 1f;

    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
        Time.timeScale = 1f;  // Reestablecer el tiempo a normal en caso de que venga de una pausa
    }

    public void PlayLevel1()
    {
        LoadLevel("Level1");
    }

    public void PlayLevel2()
    {
        LoadLevel("Level2");
    }

    public void Quit()
    {
        Application.Quit();
    }
}

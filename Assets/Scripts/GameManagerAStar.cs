using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerAStar : MonoBehaviour
{
    public static GameManagerAStar Instance { get; private set; }
    public GameObject gameCompletePanelAStar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ShowGameCompleteAStar()
    {
        if(gameCompletePanelAStar != null)
        {
            gameCompletePanelAStar.SetActive(true);
            Time.timeScale = 0f;
        }
        
    }

}

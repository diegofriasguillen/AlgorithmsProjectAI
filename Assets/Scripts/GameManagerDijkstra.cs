using UnityEngine;

public class GameManagerDijkstra : MonoBehaviour
{
    public static GameManagerDijkstra Instance { get; private set; }
    public GameObject gameCompletePanelDisjktra;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    public void ShowGameCompleteDijkstra()
    {
        if (gameCompletePanelDisjktra != null)
        {
            gameCompletePanelDisjktra.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}

using UnityEngine;

public class screenManager : MonoBehaviour
{
    public static screenManager Instance;

    public GameObject victoryScreen;
    public GameObject defeatScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowVictoryScreen()
    {
        victoryScreen.SetActive(true);
    }

    public void ShowDefeatScreen()
    {
        defeatScreen.SetActive(true);
    }
}
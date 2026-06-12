
using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaGameOver : MonoBehaviour
{
    public GameObject PanelGameWin; 
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PanelGameWin.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("LevelDesignImplementation");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

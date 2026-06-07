
using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaGameOver : MonoBehaviour
{
    public GameObject PanelGameOver; 
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(true);
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

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button playButton;
    public Button optionsButton;
    public Button quitButton;

    void Start()
    {
        // Se os botões não estiverem atribuídos no Inspector, tenta achar pelos nomes
        if (playButton == null) playButton = GameObject.Find("PlayButton")?.GetComponent<Button>();
        if (optionsButton == null) optionsButton = GameObject.Find("OptionsButton")?.GetComponent<Button>();
        if (quitButton == null) quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();

        // Adiciona os listeners
        if (playButton != null) playButton.onClick.AddListener(PlayGame);
        if (optionsButton != null) optionsButton.onClick.AddListener(OpenOptions);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }

    public void PlayGame()
    {
        // Carrega a cena do jogo
        SceneManager.LoadScene("LevelDesignImplementation");
    }

    public void OpenOptions()
    {
        // Por enquanto não faz nada
        Debug.Log("Botão de Opções clicado!");
    }

    public void QuitGame()
    {
        // Fecha a aplicação
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}

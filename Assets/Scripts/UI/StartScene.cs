using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    [SerializeField] GameObject deactivatePanel;
    private Canvas canvas;
    private Button buttonStart;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        buttonStart = GetComponent<Button>();
    }


    public void aoClicarBotao()
    {
        deactivatePanel.SetActive(false);
    }

}

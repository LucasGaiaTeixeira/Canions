using UnityEngine;
using TMPro; // Necessário se você usar TextMeshPro para a interface

public class JogoTimer : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private TextMeshProUGUI textoTimer; // Arraste seu texto da UI aqui

    private float tempoDecorrido;
    private bool cronometroAtivo = true;

    void Start()
    {
        tempoDecorrido = 0f;
        cronometroAtivo = true;
    }

    void Update()
    {
        if (cronometroAtivo)
        {
            tempoDecorrido += Time.deltaTime;
            AtualizarInterface();
        }
    }

    // Método que calcula minutos e segundos e mostra na tela
    void AtualizarInterface()
    {
        if (textoTimer == null) return;

        int minutos = Mathf.FloorToInt(tempoDecorrido / 60f);
        int segundos = Mathf.FloorToInt(tempoDecorrido % 60f);

        // Formata para string ex: 02:05
        textoTimer.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    // O barco vai chamar esse método para travar o tempo!
    public void PararCronometro()
    {
        cronometroAtivo = false;
        Debug.Log($"Cronômetro parado no tempo: {textoTimer.text}");
    }
}
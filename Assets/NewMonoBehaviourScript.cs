using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoTransicao : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject imagemFinal;

    void Start()
    {
        // Garante que a imagem começa desativada
        imagemFinal.SetActive(false);
        
        // Inscreve no evento que avisa quando o vídeo termina
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Desativa o Video Player e o vídeo da tela
        videoPlayer.gameObject.SetActive(false);
        
        // Ativa a imagem estática
        imagemFinal.SetActive(true);
    }
}

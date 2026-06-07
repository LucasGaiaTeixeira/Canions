using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuGenerator : MonoBehaviour
{
    [MenuItem("Tools/Gerar Menu Principal")]
    public static void GenerateMainMenu()
    {


        // 1. Criação do Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Adiciona o script MainMenuManager
        canvasObj.AddComponent<MainMenuManager>();

        // 2. Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f); // Cinza escuro provisório
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 1);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // 3. Título CARRANCADA
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(canvasObj.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "CARRANCADA";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 120;
        titleText.alignment = TextAnchor.MiddleCenter;
        
        Color purpleColor;
        ColorUtility.TryParseHtmlString("#BC13FE", out purpleColor);
        titleText.color = purpleColor;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(1000, 200);
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -200);

        // Função local para criar botões
        void CreateMenuButton(string name, string textStr, float yPos)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(canvasObj.transform, false);
            Image btnImage = btnObj.AddComponent<Image>();
            
            Color blueNeon;
            ColorUtility.TryParseHtmlString("#1F51FF", out blueNeon);
            btnImage.color = blueNeon;

            Button btn = btnObj.AddComponent<Button>();

            RectTransform btnRect = btnObj.GetComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(400, 80);
            btnRect.anchorMin = new Vector2(0.5f, 0.5f);
            btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.anchoredPosition = new Vector2(0, yPos);

            // Texto do botão
            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            Text txt = txtObj.AddComponent<Text>();
            txt.text = textStr;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 40;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;

            RectTransform txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;
        }

        // 4. Criação dos botões (pegando do meio da tela para baixo)
        CreateMenuButton("PlayButton", "PLAY", 0);
        CreateMenuButton("OptionsButton", "OPTIONS", -100);
        CreateMenuButton("QuitButton", "QUIT", -200);

        Debug.Log("Menu Principal gerado com sucesso! Verifique a hierarquia da sua cena.");
    }
}

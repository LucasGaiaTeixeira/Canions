using System;
using UnityEngine;

public class DistorcionWater : MonoBehaviour
{
    //[SerializeField] private Material waterMaterial;
    //[SerializeField] private float valorDistortion;

    //void Update()
    //{
    //    waterDistorcion();
    //    if (waterMaterial.GetFloat("Vector1_F6B82B81") > 0.018)
    //    {
    //        waterMaterial.SetFloat("Vector1_F6B82B81", 0.016f);
    //    }
    //}

    //public void waterDistorcion()
    //{
    //    waterMaterial.SetFloat("Vector1_F6B82B81", waterMaterial.GetFloat("Vector1_F6B82B81") + valorDistortion);
    //}

    [SerializeField] private Material waterMaterial;

    [Header("Configurações de Oscilação")]
    [SerializeField] private float valorMinimo = 0.016f;
    [SerializeField] private float valorMaximo = 0.018f;
    [SerializeField] private float velocidade = 0.002f; // Controla a velocidade do vai e vem

    // Guardamos o ID da propriedade para o Unity processar mais rápido que por String
    private int shaderPropertyID;

    void Start()
    {
        // Boa prática: Transformar a string em ID melhora a performance no Update
        shaderPropertyID = Shader.PropertyToID("Vector1_F6B82B81");
    }

    void Update()
    {
        if (waterMaterial != null)
        {
            // 1. O Time.time * velocidade faz o valor aumentar constantemente com o tempo
            // 2. O Mathf.PingPong garante que esse valor varie estritamente entre 0 e a distância (max - min)
            float distancia = valorMaximo - valorMinimo;
            float oscilacao = Mathf.PingPong(Time.time * velocidade, distancia);

            // 3. Somamos o valor mínimo para que o resultado final fique entre 0.016 e 0.018
            float valorFinal = valorMinimo + oscilacao;

            // Aplica suavemente no Shader
            waterMaterial.SetFloat(shaderPropertyID, valorFinal);
        }
    }
}

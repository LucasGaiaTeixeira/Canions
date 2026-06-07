using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStonesSky : MonoBehaviour
{
    [Header("Configurações de Posição")]
    [SerializeField] private List<Transform> PointSpawn; // Coloque os seus 53 pontos aqui no Inspector

    [Header("Prefab da Pedra")]
    [SerializeField] private GameObject StonePrefab;

    [Header("Configurações de Tempo")]
    [SerializeField] private float tempoDeEspera = 2.0f; // Tempo para as 6 pedras mudarem de lugar

    // Lista interna para armazenar as 6 pedras criadas no início
    private List<GameObject> pedrasInstanciadas = new List<GameObject>();

    void Start()
    {
        // 1. Cria as 6 pedras iniciais no começo do jogo
        InstanciarPedrasIniciais();

        // 2. Inicia a rotina que vai ficar movendo as 6 pedras pelos 53 pontos infinitamente
        StartCoroutine(RotinaMoverTodasAsPedras());
    }

    void InstanciarPedrasIniciais()
    {
        // Criamos uma lista temporária para não spawnar duas pedras no mesmo lugar logo no Start
        List<Transform> pontosDisponiveis = new List<Transform>(PointSpawn);

        // Loop fixo para criar exatamente 6 pedras
        for (int i = 0; i < 6; i++)
        {
            if (pontosDisponiveis.Count == 0) break;

            // Sorteia um ponto inicial dos 53 disponíveis
            int indiceAleatorio = Random.Range(0, pontosDisponiveis.Count);
            Transform pontoSorteado = pontosDisponiveis[indiceAleatorio];

            // Instancia a pedra
            GameObject novaPedra = Instantiate(StonePrefab, pontoSorteado.position, pontoSorteado.rotation);

            // Guarda na lista para podermos mover no futuro
            pedrasInstanciadas.Add(novaPedra);

            // Remove o ponto usado para a próxima pedra nascer em outro lugar
            pontosDisponiveis.RemoveAt(indiceAleatorio);
        }
    }

    IEnumerator RotinaMoverTodasAsPedras()
    {
        while (true)
        {
            // Espera os 2 segundos
            yield return new WaitForSeconds(tempoDeEspera);

            // Criamos uma cópia nova dos 53 pontos a cada ciclo.
            // Isso serve para garantir que as 6 pedras escolham lugares únicos nesta rodada!
            List<Transform> pontosLivresDestaRodada = new List<Transform>(PointSpawn);

            // Passamos por cada uma das 6 pedras criadas e mudamos o transform delas
            foreach (GameObject pedra in pedrasInstanciadas)
            {
                if (pedra != null && pontosLivresDestaRodada.Count > 0)
                {
                    // Sorteia um dos pontos que ainda estão livres para esta pedra
                    int indiceAleatorio = Random.Range(0, pontosLivresDestaRodada.Count);
                    Transform pontoDestino = pontosLivresDestaRodada[indiceAleatorio];

                    // Teletransporta o transform da pedra atual para o novo ponto sorteado
                    pedra.transform.position = pontoDestino.position;
                    pedra.transform.rotation = pontoDestino.rotation;

                    // Se a pedra usar física, limpamos o movimento antigo dela para ela cair reto do novo local
                    Rigidbody rb = pedra.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }

                    // IMPORTANTE: Remove o ponto da lista temporária para que NENHUMA das outras 5 pedras
                    // seja enviada para esse mesmo local no mesmo segundo!
                    pontosLivresDestaRodada.RemoveAt(indiceAleatorio);
                }
            }

            Debug.Log("As 6 pedras foram teletransportadas para novas posições aleatórias!");
        }
    }
}
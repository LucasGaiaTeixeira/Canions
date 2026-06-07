using System.Collections;
using UnityEngine;

public class BoatController : BoatInputs
{
    [Header("Coisas Principais")]
    private Rigidbody rigBoat;
    public bool boatControllerAll = true;
    public AudioSource backgroundMusic;
    public AudioSource audioFishEatBoat;

    [Header("Movimentação do Barco")]
    [SerializeField] private float speedBoat;
    [SerializeField] private float speedBoatRotation;
    [SerializeField] private bool boatController;

    [Header("Atributos do Barco")]
    [SerializeField] private int maxLife = 2;
    private int currentLife;
    [SerializeField] private GameObject playerBody;
    private Renderer boatRenderer;

    [Header("SISTEMA DE TEMPO")]
    [SerializeField] private JogoTimer scriptTimer; // Arraste o objeto que tem o JogoTimer aqui!

    [Header("PEIXE DE TRÁS (Movido por Alvos)")]
    [SerializeField] private GameObject fishBack; // O peixe gigante de trás (FORA do barco na Hierarchy)
    [SerializeField] private Transform alvoPeixeTrasLonge; // Objeto vazio "Alvo_PeixeTras_Longe" (DENTRO do barco)
    [SerializeField] private Transform alvoPeixeTrasPerto; // Objeto vazio "Alvo_PeixeTras_Perto" (DENTRO do barco)
    [SerializeField] private float velocidadSuavizacao = 5f;

    [Header("PEIXE DE BAIXO (Apenas Animação)")]
    [SerializeField] private GameObject fishDown; // O peixe de baixo (DENTRO do barco na Hierarchy)
    private Animator animatorFishDown; // Componente Animator do peixe de baixo

    [Header("Configuração de Arrasto (Inércia)")]
    [SerializeField] private float boatInWater;
    [SerializeField] private float boatOutWater;

    [Header("Pulo e Super Pulo")]
    private bool canJump;
    [SerializeField] private Vector3 forceJumpNormal;
    [SerializeField] private float forceJumpVerticalMax;
    [SerializeField] private float forceJumpHorizontalMax;
    [SerializeField] private float timeChargeMax;
    private float timeJumpNormal = 0.35f;
    private float timeCharging;

    [Header("Cor do Barco")]
    [SerializeField] public Color originalColor;

    private Coroutine corrotinaMovimentoPeixe;
    private Coroutine corrotinaRegeneracao;

    void Start()
    {
        rigBoat = GetComponent<Rigidbody>();
        boatRenderer = GetComponent<Renderer>();

        currentLife = maxLife;

        // Configura o peixe de trás para ignorar física (ele vai seguir os alvos por script)
        if (fishBack != null)
        {
            Rigidbody rbBack = fishBack.GetComponent<Rigidbody>();
            if (rbBack != null) rbBack.isKinematic = true;

            // Coloca o peixe de trás no alvo de longe logo no início
            if (alvoPeixeTrasLonge != null)
            {
                fishBack.transform.position = alvoPeixeTrasLonge.position;

                // Inicializa a rotação travando X e Z em 0
                Vector3 rotAlvo = alvoPeixeTrasLonge.eulerAngles;
                fishBack.transform.rotation = Quaternion.Euler(0f, rotAlvo.y, 0f);
            }
        }

        // Configura o peixe de baixo (ele começa desativado aguardando o fim do jogo)
        if (fishDown != null)
        {
            animatorFishDown = fishDown.GetComponent<Animator>();
            fishDown.SetActive(false); // Começa desligado
        }
    }

    void FixedUpdate()
    {
        if (boatControllerAll)
        {
            if (boatController)
            {
                // 1. MOVIMENTO PARA FRENTE E PARA TRÁS
                if (Mathf.Abs(posicaoBoat.z) > 0.01f)
                {
                    Vector3 forcaMotor = transform.forward * posicaoBoat.z * speedBoat;
                    rigBoat.AddForce(forcaMotor * Time.fixedDeltaTime, ForceMode.VelocityChange);
                }

                // 2. ROTAÇÃO REALISTA PARA OS LADOS
                if (Mathf.Abs(posicaoBoat.x) > 0.01f)
                {
                    Vector3 torqueLeme = transform.up * posicaoBoat.x * speedBoatRotation;
                    rigBoat.AddTorque(torqueLeme * Time.fixedDeltaTime, ForceMode.VelocityChange);
                }
            }

            if (boatCanceledSuperJump)
            {
                boatCanceledSuperJump = false; // Consome o input

                if (canJump)
                {
                    canJump = false; // Está no ar

                    // CENÁRIO A: Toque rápido -> Pulo Normal
                    if (timeCharging <= timeJumpNormal)
                    {
                        Debug.Log("Pulo Normal executado!");
                        rigBoat.AddForce(forceJumpNormal, ForceMode.Impulse);
                    }
                    // CENÁRIO B: Segurou o botão -> Super Pulo Proporcional
                    else
                    {
                        Debug.Log("SUPER PULO DETONADO!");

                        float porcentagemCarga = timeCharging / timeChargeMax;

                        float forcaVerticalFinal = forceJumpVerticalMax * porcentagemCarga;
                        float forcaHorizontalFinal = forceJumpHorizontalMax * porcentagemCarga;

                        Vector3 direcaoSuperPulo = (transform.up * forcaVerticalFinal) + (transform.forward * forcaHorizontalFinal);
                        rigBoat.AddForce(direcaoSuperPulo, ForceMode.Impulse);
                    }
                }

                // Reseta o cronômetro para o próximo clique
                timeCharging = 0f;
            }
        }
    }

    void Update()
    {
        if (boatControllerAll)
        {
            // Sistema de carregamento do Super Pulo
            if (boatController && boatChardingSuperJump && canJump)
            {
                timeCharging += Time.deltaTime;
                timeCharging = Mathf.Clamp(timeCharging, 0f, timeChargeMax);

                if (timeCharging > timeJumpNormal)
                {
                    Debug.Log($"Carregando Super Pulo: {Mathf.Round((timeCharging / timeChargeMax) * 100)}%");
                }
            }

            // SEGUIR EM TEMPO REAL (Garante que o peixe acompanha o barco em movimento)
            if (fishBack != null)
            {
                // Escolhe dinamicamente o alvo com base na vida atual
                Transform alvoAtual = (currentLife == maxLife) ? alvoPeixeTrasLonge : alvoPeixeTrasPerto;

                if (alvoAtual != null)
                {
                    // 1. Move a posição continuamente até o alvo ativo
                    fishBack.transform.position = Vector3.Lerp(fishBack.transform.position, alvoAtual.position, Time.deltaTime * velocidadSuavizacao);

                    // 2. Calcula a rotação desejada, mas isola APENAS o eixo Y (X e Z viram 0)
                    Vector3 eulerAlvo = alvoAtual.eulerAngles;
                    Quaternion rotacaoApenasY = Quaternion.Euler(0f, eulerAlvo.y, 0f);

                    // Aplica a rotação suave apenas para os lados
                    fishBack.transform.rotation = Quaternion.Slerp(fishBack.transform.rotation, rotacaoApenasY, Time.deltaTime * velocidadSuavizacao);
                }
            }
        }
    }

    public void takeDamage()
    {
        if (!boatControllerAll || currentLife <= 0) return;

        currentLife--;

        // 1. PRIMEIRA BATIDA: Peixe de trás muda o alvo para perto e começa a perseguir colado
        if (currentLife == 1)
        {
            Debug.Log("Barco danificado! O peixe de trás está se aproximando do motor...");

            if (corrotinaMovimentoPeixe != null) StopCoroutine(corrotinaMovimentoPeixe);
            corrotinaMovimentoPeixe = StartCoroutine(MoverPeixeTrasParaAlvo(alvoPeixeTrasPerto));

            // Inicia o cronômetro de 5 segundos para recuperar a vida
            if (corrotinaRegeneracao != null) StopCoroutine(corrotinaRegeneracao);
            corrotinaRegeneracao = StartCoroutine(RotinaRegenerarVida());
        }
        // 2. SEGUNDA BATIDA (Vida 0): Peixe de trás recua e o de baixo ativa a animação de ataque
        else if (currentLife <= 0)
        {
            Debug.Log("Vida zerada! Ativando animação de ataque do peixe de baixo!");
            if (corrotinaRegeneracao != null) StopCoroutine(corrotinaRegeneracao);

            // COMANDO CRUCIAL: Manda o cronômetro parar de contar o tempo IMEDIATAMENTE!
            if (scriptTimer != null)
            {
                scriptTimer.PararCronometro();
            }

            // Afasta o peixe de trás de volta para o alvo longe
            if (corrotinaMovimentoPeixe != null) StopCoroutine(corrotinaMovimentoPeixe);
            corrotinaMovimentoPeixe = StartCoroutine(MoverPeixeTrasParaAlvo(alvoPeixeTrasLonge));

            // Executa a sequência de fim de jogo com a animação do peixe de baixo
            StartCoroutine(RotinaFimDeJogoComAnimacao());
        }
    }

    private IEnumerator MoverPeixeTrasParaAlvo(Transform alvo)
    {
        if (fishBack == null || alvo == null) yield break;

        float tempo = 0;
        Vector3 posicaoInicial = fishBack.transform.position;
        Quaternion rotacaoInicial = fishBack.transform.rotation;

        Vector3 eulerAlvo = alvo.eulerAngles;
        Quaternion rotacaoAlvoApenasY = Quaternion.Euler(0f, eulerAlvo.y, 0f);

        while (tempo < 1f)
        {
            tempo += Time.deltaTime * velocidadSuavizacao;

            fishBack.transform.position = Vector3.Lerp(posicaoInicial, alvo.position, tempo);
            fishBack.transform.rotation = Quaternion.Slerp(rotacaoInicial, rotacaoAlvoApenasY, tempo);

            yield return null;
        }
    }

    private IEnumerator RotinaRegenerarVida()
    {
        yield return new WaitForSeconds(5f);
        currentLife = maxLife;

        if (corrotinaMovimentoPeixe != null) StopCoroutine(corrotinaMovimentoPeixe);
        corrotinaMovimentoPeixe = StartCoroutine(MoverPeixeTrasParaAlvo(alvoPeixeTrasLonge));

        Debug.Log("Vida regenerada! O peixe de trás recuou.");
    }

    private IEnumerator RotinaFimDeJogoComAnimacao()
    {
        boatControllerAll = false; // Bloqueia totalmente os comandos do barco
        boatController = false;

        if (fishDown != null)
        {
            fishDown.SetActive(true); // Liga o objeto do peixe embaixo do barco

            if (animatorFishDown != null)
            {
                animatorFishDown.SetTrigger("atack");
                yield return new WaitForSeconds(0.5f);
                audioFishEatBoat.Play();
            }
        }

        yield return new WaitForSeconds(2.1f);

        Debug.Log("GAME OVER: O jogador foi devorado!");
        if (playerBody != null) Destroy(playerBody);
        backgroundMusic.Stop();
        
        Destroy(gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GroundBoat"))
        {
            canJump = true;
            rigBoat.linearDamping = boatInWater;
            boatController = true;
        }
        else if (collision.gameObject.CompareTag("Stone"))
        {
            takeDamage();
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("GroundBoat"))
        {
            boatController = false;
            rigBoat.linearDamping = boatOutWater;
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Obstacles") || collision.gameObject.CompareTag("Stone"))
        {
            takeDamage();
        }
    }
}
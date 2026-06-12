using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;

public class BoatController : BoatInputs
{
    [SerializeField] private PlayerMoviment ScriptPlayerMoviment;

    [Header("Coisas Principais")]
    private Rigidbody rigBoat;
    public bool boatControllerAll = true;

    public AudioSource audioFishEatBoat;
    public GameObject PanelGameOver2;

    [Header("Movimentação do Barco")]
    [SerializeField] private float speedBoat;
    [SerializeField] private float speedBoatRotation;
    [SerializeField] private bool boatController;
    private bool boatCanMove;

    [Header("Atributos do Barco")]
    [SerializeField] private int maxLife = 2;
    private int currentLife;
    [SerializeField] private GameObject playerBody;
    private Renderer boatRenderer;


    [Header("SISTEMA DE TEMPO")]
    [SerializeField] private JogoTimer scriptTimer; // Arraste o objeto que tem o JogoTimer aqui!

    [Header("PEIXE DE TRÁS (Movido por Alvos)")]
    [SerializeField] private GameObject fishBack; // DEVE FICAR FORA DO BARCO NA HIERARCHY
    [SerializeField] private Transform alvoPeixeTrasLonge; // Objeto vazio (DENTRO do barco)
    [SerializeField] private Transform alvoPeixeTrasPerto; // Objeto vazio (DENTRO do barco)
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
        //ScriptPlayerMoviment = GetComponent<PlayerMoviment>();
        rigBoat = GetComponent<Rigidbody>();
        boatRenderer = GetComponent<Renderer>();

        currentLife = maxLife;

        // Configura o peixe de trás para ignorar física
        if (fishBack != null)
        {
            Rigidbody rbBack = fishBack.GetComponent<Rigidbody>();
            if (rbBack != null) rbBack.isKinematic = true;

            if (alvoPeixeTrasLonge != null)
            {
                fishBack.transform.position = alvoPeixeTrasLonge.position;
                Vector3 rotAlvo = alvoPeixeTrasLonge.eulerAngles;
                fishBack.transform.rotation = Quaternion.Euler(0f, rotAlvo.y, 0f);
            }
        }

        if (fishDown != null)
        {
            animatorFishDown = fishDown.GetComponent<Animator>();
            fishDown.SetActive(false); // Começa desligado
        }
        
    }

    void FixedUpdate()
    {
        if (boatCanMove)
        {

            if (boatControllerAll)
            {
                if (boatController)
                {
                    if (Mathf.Abs(posicaoBoat.z) > 0.01f)
                    {
                        Vector3 forcaMotor = transform.forward * posicaoBoat.z * speedBoat;
                        rigBoat.AddForce(forcaMotor * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }

                    if (Mathf.Abs(posicaoBoat.x) > 0.01f)
                    {
                        Vector3 torqueLeme = transform.up * posicaoBoat.x * speedBoatRotation;
                        rigBoat.AddTorque(torqueLeme * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                }

                if (boatCanceledSuperJump)
                {
                    boatCanceledSuperJump = false;

                    if (canJump)
                    {
                        canJump = false;

                        if (timeCharging <= timeJumpNormal)
                        {
                            rigBoat.AddForce(forceJumpNormal, ForceMode.Impulse);
                        }
                        else
                        {
                            float porcentagemCarga = timeCharging / timeChargeMax;
                            float forcaVerticalFinal = forceJumpVerticalMax * porcentagemCarga;
                            float forcaHorizontalFinal = forceJumpHorizontalMax * porcentagemCarga;

                            Vector3 direcaoSuperPulo = (transform.up * forcaVerticalFinal) + (transform.forward * forcaHorizontalFinal);
                            rigBoat.AddForce(direcaoSuperPulo, ForceMode.Impulse);
                        }
                    }
                    timeCharging = 0f;
                }
            }
        }
    }

    void Update()
    {
        if (ScriptPlayerMoviment.boatCanMove)
        {
            boatCanMove = true;
        }
        if (boatControllerAll)
        {
            if (boatController && boatChardingSuperJump && canJump)
            {
                timeCharging += Time.deltaTime;
                timeCharging = Mathf.Clamp(timeCharging, 0f, timeChargeMax);
            }

            // SEGUIR EM TEMPO REAL COM SEGURANÇA
            if (fishBack != null)
            {
                Transform alvoAtual = (currentLife == maxLife) ? alvoPeixeTrasLonge : alvoPeixeTrasPerto;

                if (alvoAtual != null)
                {
                    fishBack.transform.position = Vector3.Lerp(fishBack.transform.position, alvoAtual.position, Time.deltaTime * velocidadSuavizacao);

                    Vector3 eulerAlvo = alvoAtual.eulerAngles;
                    Quaternion rotacaoApenasY = Quaternion.Euler(0f, eulerAlvo.y, 0f);
                    fishBack.transform.rotation = Quaternion.Slerp(fishBack.transform.rotation, rotacaoApenasY, Time.deltaTime * velocidadSuavizacao);
                }
            }
        }
        Debug.Log(ScriptPlayerMoviment.boatCanMove);
    }

    public void takeDamage()
    {
        if (!boatControllerAll || currentLife <= 0) return;

        currentLife--;

        if (currentLife == 1)
        {
            Debug.Log("Barco danificado! O peixe de trás está a aproximar-se...");

            if (corrotinaMovimentoPeixe != null) StopCoroutine(corrotinaMovimentoPeixe);
            corrotinaMovimentoPeixe = StartCoroutine(MoverPeixeTrasParaAlvo(alvoPeixeTrasPerto));

            if (corrotinaRegeneracao != null) StopCoroutine(corrotinaRegeneracao);
            corrotinaRegeneracao = StartCoroutine(RotinaRegenerarVida());
        }
        else if (currentLife <= 0)
        {
            Debug.Log("Vida zerada! Ativando animação de ataque do peixe de baixo!");
            if (corrotinaRegeneracao != null) StopCoroutine(corrotinaRegeneracao);

            if (scriptTimer != null)
            {
                scriptTimer.PararCronometro();
            }

            if (corrotinaMovimentoPeixe != null) StopCoroutine(corrotinaMovimentoPeixe);
            corrotinaMovimentoPeixe = StartCoroutine(MoverPeixeTrasParaAlvo(alvoPeixeTrasLonge));

            StartCoroutine(RotinaFimDeJogoComAnimacao());
        }
    }

    private IEnumerator MoverPeixeTrasParaAlvo(Transform alvo)
    {
        // Se o peixe foi destruído por erro, para a corrotina imediatamente para não dar erro no console
        if (fishBack == null || alvo == null) yield break;

        float tempo = 0;
        Vector3 posicaoInicial = fishBack.transform.position;
        Quaternion rotacaoInicial = fishBack.transform.rotation;

        Vector3 eulerAlvo = alvo.eulerAngles;
        Quaternion considerandoApenasY = Quaternion.Euler(0f, eulerAlvo.y, 0f);

        while (tempo < 1f)
        {
            // Proteção caso o peixe suma durante o trajeto
            if (fishBack == null) yield break;

            tempo += Time.deltaTime * velocidadSuavizacao;

            fishBack.transform.position = Vector3.Lerp(posicaoInicial, alvo.position, tempo);
            fishBack.transform.rotation = Quaternion.Slerp(rotacaoInicial, considerandoApenasY, tempo);

            yield return null;
        }
    }

    private IEnumerator RotinaRegenerarVida()
    {
        yield return new WaitForSeconds(5f);
        currentLife = maxLife;

        if (corrotinaMovimentoPeixe != null) StopCoroutine(corrotinaMovimentoPeixe);
        corrotinaMovimentoPeixe = StartCoroutine(MoverPeixeTrasParaAlvo(alvoPeixeTrasLonge));

        Debug.Log("Vida regenerada! O peixe de trás afastou-se.");
    }

    private IEnumerator RotinaFimDeJogoComAnimacao()
    {
        boatControllerAll = false;
        boatController = false;

        if (fishDown != null)
        {
            fishDown.SetActive(true);

            if (animatorFishDown != null)
            {
                animatorFishDown.SetTrigger("atack");

                yield return new WaitForSeconds(0.5f);
                audioFishEatBoat.Play();
                yield return new WaitForSeconds(2f);
                ChamarTelaGameOver();
            }
        }

        yield return new WaitForSeconds(2.1f);

        Debug.Log("GAME OVER: O jogador foi devorado!");
        if (playerBody != null) Destroy(playerBody);

        Destroy(gameObject);
    }
    
    public void ChamarTelaGameOver()
    {

        PanelGameOver2.SetActive(true);
        
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
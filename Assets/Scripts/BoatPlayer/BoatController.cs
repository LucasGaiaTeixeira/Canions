using System.Collections;
using UnityEngine;

public class BoatController : BoatInputs
{

    [Header("coisas principais")]
    private Rigidbody rigBoat;
    public bool boatControllerAll = true;

   // public static BoatController boatControllerScript;


    [Header("movimentação do barco")]
    [SerializeField] private float speedBoat;
    [SerializeField] private float speedBoatRotation;
    [SerializeField] private bool boatController;
    


    [Header("atributos do barco")]
    [SerializeField] private int vida;
    [SerializeField] private GameObject playerBody;
    private Renderer boatRenderer;


    [Header("configuração de arrasto no caso inercia")]
    [SerializeField] private float boatInWater;
    [SerializeField] private float boatOutWater;

    [Header("pulo e super pulo")]
    private bool canJump;
    [SerializeField] private Vector3 forceJumpNormal;
    [SerializeField] private float forceJumpVerticalMax;
    [SerializeField] private float forceJumpHorizontalMax;
    [SerializeField] private float timeChargeMax;
    private float timeJumpNormal = 0.35f;
    private float timeCharging;

    [Header("cor do barco")]
    [SerializeField]public Color originalColor;


    void Start()
    {
        rigBoat = GetComponent<Rigidbody>();
        boatRenderer = GetComponent<Renderer>();
        //boatControllerScript = this;
    }

    void FixedUpdate()
    {
        if (boatControllerAll)
        {
            if (boatController)
            {
                //// 1. MOVIMENTO PARA FRENTE E PARA TRÁS (Usando o eixo Z do seu Vector3)
                //if (Mathf.Abs(posicaoBoat.z) > 0.01f)
                //{
                //    // Criamos uma força empurrando sempre para onde a frente do barco aponta (transform.forward)
                //    Vector3 forcaMotor = transform.forward * posicaoBoat.z * speedBoat;
                //    rigBoat.AddForce(forcaMotor * Time.fixedDeltaTime, ForceMode.Force);
                //}

                //// 2. ROTAÇÃO REALISTA PARA OS LADOS (Usando o eixo X do seu Vector3)
                //if (Mathf.Abs(posicaoBoat.x) > 0.01f)
                //{
                //    // Criamos um torque (força de rotação) no eixo Y do mundo (transform.up)
                //    Vector3 torqueLeme = transform.up * posicaoBoat.x * speedBoatRotation;
                //    rigBoat.AddTorque(torqueLeme * Time.fixedDeltaTime, ForceMode.Force);
                //}
                // 1. MOVIMENTO PARA FRENTE E PARA TRÁS
                if (Mathf.Abs(posicaoBoat.z) > 0.01f)
                {
                    // Mudamos para VelocityChange (ignora a massa e aplica velocidade instantânea)
                    Vector3 forcaMotor = transform.forward * posicaoBoat.z * speedBoat;
                    rigBoat.AddForce(forcaMotor * Time.fixedDeltaTime, ForceMode.VelocityChange);
                }

                // 2. ROTAÇÃO REALISTA PARA OS LADOS
                if (Mathf.Abs(posicaoBoat.x) > 0.01f)
                {
                    // Mudamos para VelocityChange na rotação também
                    Vector3 torqueLeme = transform.up * posicaoBoat.x * speedBoatRotation;
                    rigBoat.AddTorque(torqueLeme * Time.fixedDeltaTime, ForceMode.VelocityChange);
                }
            }

            if (boatCanceledSuperJump)
            {
                boatCanceledSuperJump = false; // Consome o input

                if (canJump)
                {
                    canJump = false; // Remove a permissão de pular (está no ar)

                    // CENÁRIO A: Toque rápido -> Pulo Normal
                    if (timeCharging <= timeJumpNormal)
                    {
                        Debug.Log("Pulo Normal executado!");
                        // Aplica o pulo normal reto para cima
                        rigBoat.AddForce(forceJumpNormal, ForceMode.Impulse);
                    }
                    // CENÁRIO B: Segurou o botão -> Super Pulo Proporcional
                    else
                    {
                        Debug.Log("SUPER PULO DETONADO!");

                        // Calcula a porcentagem com base nos 5 segundos máximos
                        float porcentagemCarga = timeCharging / timeChargeMax;

                        float forcaVerticalFinal = forceJumpVerticalMax * porcentagemCarga;
                        float forcaHorizontalFinal = forceJumpHorizontalMax * porcentagemCarga;

                        // Lança para cima e para a frente do barco
                        Vector3 direcaoSuperPulo = (transform.up * forcaVerticalFinal) + (transform.forward * forcaHorizontalFinal);
                        rigBoat.AddForce(direcaoSuperPulo, ForceMode.Impulse);
                    }
                }

                // Sempre reseta o cronômetro para o próximo clique
                timeCharging = 0f;
            }
        }
    }

    void Update()
    {
        Debug.Log(boatControllerAll);
        if(vida <= 0)
        {
            Destroy(gameObject);
            Debug.Log("barco destroido");
            Destroy(playerBody);
        }

        if (boatController && boatChardingSuperJump && canJump)
        {
            timeCharging += Time.deltaTime;
            timeCharging = Mathf.Clamp(timeCharging, 0f, timeChargeMax);

            // Só exibe o log de carga se o jogador passou do tempo de um pulo normal
            if (timeCharging > timeChargeMax)
            {
                Debug.Log($"Carregando Super Pulo: {Mathf.Round((timeCharging / timeChargeMax) * 100)}%");
            }
        }
    }

    

    public void takeDamage()
    {
        StartCoroutine(damageColor());
        vida--;
    }

    public IEnumerator damageColor()
    {
        boatRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        boatRenderer.material.color = originalColor;
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
}

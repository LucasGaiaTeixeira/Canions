using System.Collections;
using UnityEngine;

public class BoatController : BoatInputs
{
    private Rigidbody rigBoat;
     


    [Header("movimentação do barco")]
    [SerializeField] private float speedBoat;
    [SerializeField] private float speedBoatRotation;
    [SerializeField] private bool boatController;
    [SerializeField] private Vector3 alcancePulo;


    [Header("atributos do barco")]
    [SerializeField] private int vida;
    [SerializeField] private GameObject playerBody;
    private Renderer boatRenderer;
    private bool canJump;

    [Header("cor do barco")]
    [SerializeField]public Color originalColor;


    void Start()
    {
        rigBoat = GetComponent<Rigidbody>();
        boatRenderer = GetComponent<Renderer>();
       
    }

    void FixedUpdate()
    {
        if (boatController)
        {
            // 1. MOVIMENTO PARA FRENTE E PARA TRÁS (Usando o eixo Z do seu Vector3)
            if (Mathf.Abs(posicaoBoat.z) > 0.01f)
            {
                // Criamos uma força empurrando sempre para onde a frente do barco aponta (transform.forward)
                Vector3 forcaMotor = transform.forward * posicaoBoat.z * speedBoat;
                rigBoat.AddForce(forcaMotor * Time.fixedDeltaTime, ForceMode.Force);
            }

            // 2. ROTAÇÃO REALISTA PARA OS LADOS (Usando o eixo X do seu Vector3)
            if (Mathf.Abs(posicaoBoat.x) > 0.01f)
            {
                // Criamos um torque (força de rotação) no eixo Y do mundo (transform.up)
                Vector3 torqueLeme = transform.up * posicaoBoat.x * speedBoatRotation;
                rigBoat.AddTorque(torqueLeme * Time.fixedDeltaTime, ForceMode.Force);
            }
        }

        if (boatJump && canJump)
        {
            canJump = false;
            rigBoat.AddForce(alcancePulo, ForceMode.Impulse);
            
        }
        boatJump = false;
        
    }

    void Update()
    {
        if(vida <= 0)
        {
            Destroy(gameObject);
            Debug.Log("barco destroido");
            Destroy(playerBody);
        }
    }

    public void OnTriggerEnter(Collider collison)
    {
        if (collison.gameObject.CompareTag("Player"))
        {
            boatController = true;
        }else if (collison.gameObject.CompareTag("Stone"))
        {
            takeDamage();
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
        }
    }
}

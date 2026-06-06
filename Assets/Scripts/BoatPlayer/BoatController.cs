using System.Collections;
using UnityEngine;

public class BoatController : BoatInputs
{
    private CharacterController boat;
    [SerializeField] private float speedBoat;
    [SerializeField] private float speedBoatRotation;
    private bool boatController;

    [SerializeField] private int vida;
    [SerializeField] private GameObject playerBody;
    private Renderer boatRenderer;

    [SerializeField]public Color originalColor;


    void Start()
    {
        boat = GetComponent<CharacterController>();
        boatRenderer = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        if (boatController)
        {
            boat.Move(new Vector3(posicaoBoat.x, 0, posicaoBoat.z) * speedBoat);
            Quaternion positionRotation = Quaternion.LookRotation(posicaoBoat);
            transform.rotation = Quaternion.Slerp(transform.rotation, positionRotation, speedBoatRotation);
        }
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

    
}

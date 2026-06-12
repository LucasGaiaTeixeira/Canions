using UnityEngine;

public class PlayerMoviment : PlayerInputs
{
    [SerializeField] private CharacterController playerControlle;
    
    [SerializeField] Transform boatPlayerPosition;
    [SerializeField] private float speed;
    [SerializeField] public bool playerController = true;
    public bool boatCanMove = false;
    

    void Start()
    {
        playerControlle = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {   
        if (playerController && playerControlle != null) {
            // CORREÇÃO: Multiplicar por speed E por Time.fixedDeltaTime para movimento suave e correto
            Vector3 direcaoMovimento = new Vector3(position.x, position.y - 1, position.z) * speed;
            playerControlle.Move(direcaoMovimento * Time.fixedDeltaTime);
        }
    }


    private void OnTriggerEnter(Collider collision)
    {
        
        
        if (collision.gameObject.CompareTag("Boat"))
        {
            playerController = false;
            boatCanMove = true;

            // CORREÇÃO: Desativa o CharacterController para não brigar com o posicionamento
            if (playerControlle != null) playerControlle.enabled = false;

            // Transforma o Player em FILHO do barco. Assim, onde o barco for, o player vai junto!
            transform.SetParent(collision.transform);

            // Reseta a posição local para bater exatamente com o banco/assento do barco
            transform.localPosition = boatPlayerPosition.localPosition;
            transform.localRotation = boatPlayerPosition.localRotation;

            Debug.Log("Jogador entrou no barco: " + collision.gameObject.name);
        }
    }
}
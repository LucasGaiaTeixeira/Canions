using UnityEngine;


public class PlayerMoviment : PlayerInputs
{
    [SerializeField] private CharacterController playerControlle;
    private float gravity = -9.78f;
    [SerializeField] Transform boatPlayerPosition;
    [SerializeField] private float speed;
    [SerializeField] public bool playerController = true;
    [SerializeField] public GameObject collisionObject;
    private bool PlayerStayBoat = false;
    public static PlayerMoviment Instance;


    void Start()
    {
        playerControlle = GetComponent<CharacterController>();
        Instance = this;
    }

    private void Update()
    {
        if (PlayerStayBoat)
        {
            transform.position = boatPlayerPosition.position;
        }
    }

    private void FixedUpdate()
    {   
        if (playerController) {
            playerControlle.Move(new Vector3(position.x, gravity, position.z) * speed);
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        collisionObject = collision.gameObject;
        if (collision.gameObject.CompareTag("Boat"))
        {
            transform.position = boatPlayerPosition.position;
            playerController = false;
            PlayerStayBoat = true;
            Debug.Log(collision.gameObject.name);
        }
    }

}

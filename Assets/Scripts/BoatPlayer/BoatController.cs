using System;
using UnityEngine;

public class BoatController : BoatInputs
{
    private CharacterController boat;
    [SerializeField] private float speedBoat;
    [SerializeField] private float speedBoatRotation;
    private bool boatController;
    private PlayerMoviment instanciaPlayerToBoat;

    void Start()
    {
        boat = GetComponent<CharacterController>();
        instanciaPlayerToBoat = GetComponent<PlayerMoviment>();
        
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

    private void Update()
    {
        if (instanciaPlayerToBoat)
        {
            boatController = false;
        }
        else
        {
            boatController = true;
        }
    }
}

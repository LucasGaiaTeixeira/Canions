using UnityEngine;
using UnityEngine.InputSystem;

public class BoatInputs : MonoBehaviour
{
    protected Vector3 posicaoBoat;

    public void BoatInputMover(InputAction.CallbackContext context)
    {
        posicaoBoat = context.ReadValue<Vector3>();
    }

}

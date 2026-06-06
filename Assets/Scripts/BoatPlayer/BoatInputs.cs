using UnityEngine;
using UnityEngine.InputSystem;

public class BoatInputs : MonoBehaviour
{
    protected Vector3 posicaoBoat;
    protected bool boatJump;

    public void BoatInputMover(InputAction.CallbackContext context)
    {
        posicaoBoat = context.ReadValue<Vector3>();
    }

    public void BoatInputJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            boatJump = true;
        }
    }
}

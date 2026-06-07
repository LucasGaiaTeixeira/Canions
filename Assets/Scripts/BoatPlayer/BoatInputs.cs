using UnityEngine;
using UnityEngine.InputSystem;

public class BoatInputs : MonoBehaviour
{
    protected Vector3 posicaoBoat;
    protected bool boatChardingSuperJump;
    protected bool boatCanceledSuperJump;

    public void BoatInputMover(InputAction.CallbackContext context)
    {
        posicaoBoat = context.ReadValue<Vector3>();
    }

    public void BoatInputJump(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            boatChardingSuperJump = true;
            boatCanceledSuperJump = false;
        }
        else if (context.canceled)
        {
            boatChardingSuperJump = false;
            boatCanceledSuperJump = true;
        }
    }
}

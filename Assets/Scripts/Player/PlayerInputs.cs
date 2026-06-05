using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    protected Vector3 position;
    protected bool enter;

    public void Movimento(InputAction.CallbackContext context)
    {
        position = context.ReadValue<Vector3>();
    }
    
    public void EnterBoat(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            enter = true;
        }
    }
}

using ParkourFPS;
using UnityEngine;

[RequireComponent(typeof(PlayerControllerScript))]
public class PlayerMovement : MonoBehaviour
{
    public bool dashing;
    public bool swinging;
    public bool climbing;

    public bool grounded
    {
        get
        {
            PlayerControllerScript controller = GetComponent<PlayerControllerScript>();
            return controller != null && controller.IsGrounded();
        }
    }
}

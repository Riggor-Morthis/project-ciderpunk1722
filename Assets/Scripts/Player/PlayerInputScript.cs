using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputScript : MonoBehaviour
{
    #region Variables
    public Vector2 movementInput { get; private set; } //L'input de mouvement au sol
    public Vector2 mouseInput { get; private set; } //L'input pour bouger notre camera
    public float switchShoulderInput { get; private set; } //Est-ce qu'on doit changer le cote de la camera ?
    public float jumpInput { get; private set; } //Est-ce qu'on veut sauter ?
    #endregion

    #region PublicMethods
    public void OnMovement(InputValue iv) => movementInput = iv.Get<Vector2>();
    public void OnMouseLook(InputValue iv) => mouseInput = iv.Get<Vector2>();
    public void OnSwitchShoulder(InputValue iv) => switchShoulderInput = iv.Get<float>();
    public void OnJump(InputValue iv) => jumpInput = iv.Get<float>();
    #endregion
}

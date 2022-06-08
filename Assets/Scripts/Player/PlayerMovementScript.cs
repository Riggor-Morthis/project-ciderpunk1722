using UnityEngine;
using System.Collections;

public class PlayerMovementScript : MonoBehaviour
{
    #region Variables
    [SerializeField, Tooltip("Le script qui capture les inputs joueur")]
    private PlayerInputScript playerInput;

    [Space]

    [SerializeField, Tooltip("Le rigidbody du joueur pour pouvoir le deplacer")]
    private Rigidbody playerRigidbody;
    [SerializeField, Tooltip("Le point de pivot de notre camera")]
    private Transform playerBrain;

    [Space]

    [SerializeField, Tooltip("Le layer utilise par le sol pour signaler que c'est le sol")]
    private LayerMask groundLayer;
    [SerializeField, Tooltip("La position utilisee pour check si les pieds du personnage sont au sol ou non")]
    private Transform feetContact;

    private Vector2 movementInput = Vector2.zero; //Les mouvements au clavir du joueur
    private float jumpInputC; //La vrai valeur de l'input de saut
    private bool jumpInputI = false; //La valeur convertie afin d'etre utilisable dans notre systeme

    private const float movementSpeed = 5f; //La vitesse de deplacement du personnage joueur
    private Vector3 destinationPoint; //La ou on veut aller avec notre personnage

    private const float groundCheckRadius = 0.06f; //Le rayon dans lequel on verifie si on est au contact du sol ou pas
    private bool isGrounded = false; //Est-ce qu'on est au sol ou pas ?
    private const float upGravity = -15f, downGravity = -30f, downDownGravity = -45f, downDownDownGravity = -90f; //Les differentes gravites, selon la velocite de notre personnage
    private const float jumpVelocity = 6f; //La puissance de saut de notre personnage
    private float currentDownVelocity = 0f; //Notre velocite verticale actuelle
    #endregion

    #region FixedUpdate
    private void FixedUpdate()
    {
        //Initialisation de variables utiles
        GetInputs();
        //La gravite c'est un sujet serieux
        CheckGround();
        ApplyGravity();

        //On se bouge que si on a des instructions
        if (movementInput != Vector2.zero) MovePlayer();

        //On ne saute que si on a des instructions
        //Lock et unlock pour le ButtonDown
        if (jumpInputC > 0 && !jumpInputI)
        {
            jumpInputI = true;
            Jump();
        }
        else if (jumpInputC == 0 && jumpInputI) jumpInputI = false;
    }
    #endregion

    #region PrivateMethods
    /// <summary>
    /// Simplement pour recuperer les inputs du joueur
    /// </summary>
    private void GetInputs()
    {
        movementInput = playerInput.movementInput;
        jumpInputC = playerInput.jumpInput;
    }

    /// <summary>
    /// Permet de savoir si on est au sol ou non
    /// </summary>
    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(feetContact.position, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// Pour bouger le joueur relativement a sa position actuelle de camera
    /// </summary>
    private void MovePlayer()
    {
        //On s'assure que les strafes ne sont pas trop rapides
        movementInput.Normalize();

        //On calcule une destination en fonction du "tout droit" de la camera et de nos inputs actuels, puis on l'applique
        destinationPoint = playerRigidbody.position + playerBrain.forward * movementInput.y * movementSpeed * Time.fixedDeltaTime + playerBrain.right * movementInput.x * movementSpeed * Time.fixedDeltaTime;
        playerRigidbody.MovePosition(destinationPoint);
    }

    /// <summary>
    /// Applique les effets d'une gravite custom a notre joueur
    /// </summary>
    private void ApplyGravity()
    {
        //Si on est pas au sol, il faut tomber
        if (!isGrounded)
        {
            //On a pas encore atteint notre vitesse terminale
            if (playerRigidbody.velocity.y > downDownDownGravity)
            {
                //On utilise 4 valeurs de gravite differente pour simuler un saut avec un bon gamefeel
                if (playerRigidbody.velocity.y > 0) currentDownVelocity += upGravity * Time.fixedDeltaTime;
                else if (playerRigidbody.velocity.y > -jumpVelocity) currentDownVelocity += downGravity * Time.fixedDeltaTime;
                else if (playerRigidbody.velocity.y > downGravity) currentDownVelocity += downDownGravity * Time.fixedDeltaTime;
                else currentDownVelocity += downDownDownGravity * Time.fixedDeltaTime;
            }
            //Le cap
            else if (playerRigidbody.velocity.y < downDownDownGravity) currentDownVelocity = downDownDownGravity;

            //On applique les effets
            playerRigidbody.velocity = new Vector3(0f, currentDownVelocity, 0f);
        }
        //Permet de remettre a zero nos valeurs importantes
        else if (currentDownVelocity != 0f)
        {
            currentDownVelocity = 0f;
            playerRigidbody.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Permet de faire sauter le joueur
    /// </summary>
    private void Jump()
    {
        if (isGrounded)
        {
            currentDownVelocity = jumpVelocity;
            playerRigidbody.velocity = new Vector3(0f, currentDownVelocity, 0f);
        }
    }
    #endregion
}

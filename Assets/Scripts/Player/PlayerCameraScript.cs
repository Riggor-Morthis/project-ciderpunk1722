using UnityEngine;
using System.Collections;

public class PlayerCameraScript : MonoBehaviour
{
    #region Variables
    [SerializeField, Tooltip("Le script qui capture les inputs joueur")]
    private PlayerInputScript playerInput;

    [Space]

    [SerializeField, Tooltip("Le point de pivot horizontal pour notre camera")]
    private Transform playerBrain;
    [SerializeField, Tooltip("Le point de pivot vertical pour notre camera")]
    private Transform cameraPivot;
    [SerializeField, Tooltip("Le transform de la camera, pour bouger sa profondeur")]
    private Transform mainCamera;

    private Vector2 mouseInput = Vector2.zero; //Les mouvements de souris du joueur
    private float switchShoulderInputC = 0f; //Le vrai input de changement de camera
    private bool switchShoulderInputI = false; //L'interpretation de ce changement pour le rendre exploitable par ce programme

    private const float verticalSensitivity = 60f, horizontalSensitivity = 80f; //La sensibilite de la camera dans chacun des axes
    private const float minVerticalRotation = -80f, maxVerticalRotation = 80f;
    private float currentHorizontalRotation = 0f, currentVerticalRotation = 0f, currentCameraDepth = -6f; //La rotation actuelle selon chacun des deux axes, et la distance entre la camera et nous actuellement

    private const float maxShoulderDepth = 0.5f, minShoulderDepth = -0.5f; //Les positions maximales de notre camera d'un point de vue laterale
    private float currentShoulderDepth; //La position actuelle de notre camera
    private const float shoulderSwitchingSpeed = 5.5f; //La vitesse a laquelle la camera va de gauche a droite
    private int switchShoulderDirection = 1; //Dans quel sens on veut emmener notre camera
    private bool isSwitchingShoulder = false; //Histoire de pas lancer une meme co-routine plusieurs fois de suite
    #endregion

    #region Awake
    private void Awake()
    {
        //On applique les changements basiques pour instancier correctement notre camera
        playerBrain.localRotation = Quaternion.Euler(0, currentHorizontalRotation, 0);
        cameraPivot.localRotation = Quaternion.Euler(currentVerticalRotation, 0, 0);
        mainCamera.localPosition = new Vector3(0, 0, currentCameraDepth);
        currentShoulderDepth = maxShoulderDepth;
        cameraPivot.localPosition = new Vector3(currentShoulderDepth, cameraPivot.localPosition.y, 0f);
    }
    #endregion

    #region Update
    private void Update()
    {
        GetInputs();
        //On ne met a jour l'affichage que si on a eu des inputs
        if (mouseInput.x != 0) RotateHorizontal();
        if (mouseInput.y != 0) RotateVertical();

        //On change le cote de l'epaule
        //On a un systeme de locks pour simuler le OnButtonDown
        if (switchShoulderInputC > 0 && !switchShoulderInputI)
        {
            switchShoulderInputI = true;
            PrepareSwitchingShoulders();
        }
        else if (switchShoulderInputC == 0 && switchShoulderInputI) switchShoulderInputI = false;
    }
    #endregion

    #region PrivateMethods
    /// <summary>
    /// Simplement pour recuperer les inputs joueur
    /// </summary>
    private void GetInputs()
    {
        mouseInput = playerInput.mouseInput;
        switchShoulderInputC = playerInput.switchShoulderInput;
    }

    /// <summary>
    /// Permet de calculer notre rotation actuelle selon Y
    /// </summary>
    private void RotateHorizontal()
    {
        //On calcule notre rotation
        currentHorizontalRotation += mouseInput.x * horizontalSensitivity * Time.deltaTime;

        //On applique notre rotation
        playerBrain.localRotation = Quaternion.Euler(0, currentHorizontalRotation, 0);
    }

    /// <summary>
    /// Permet de calculer notre rotation actuelle selon X
    /// </summary>
    private void RotateVertical()
    {
        //On incremente notre rotation actuelle, et on s'assure qu'elle reste entre les bonnes bornes
        currentVerticalRotation += mouseInput.y * verticalSensitivity * Time.deltaTime;
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minVerticalRotation, maxVerticalRotation);

        //On peut utiliser cette valeur pour calculer la profondeur actuelle de notre camera
        if (currentVerticalRotation > 0) currentCameraDepth = -(3f + 3f * (1 - (currentVerticalRotation / maxVerticalRotation)));
        else currentCameraDepth = -(1f + 5f * (1 - (currentVerticalRotation / minVerticalRotation)));

        //On peut appliquer nos donnees
        cameraPivot.localRotation = Quaternion.Euler(currentVerticalRotation, 0, 0);
        mainCamera.localPosition = new Vector3(0, 0, currentCameraDepth);
    }

    /// <summary>
    /// Change les variables selon le ordres du joueur et lance la coroutine si il faut
    /// </summary>
    private void PrepareSwitchingShoulders()
    {
        switchShoulderDirection = -switchShoulderDirection;
        if (!isSwitchingShoulder) StartCoroutine(SwitchTheShoulders());
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Permet de changer la camera de cote
    /// </summary>
    private IEnumerator SwitchTheShoulders()
    {
        //Lock
        isSwitchingShoulder = true;

        //On agit tant qu'on est pas hors des bords
        while (currentShoulderDepth <= maxShoulderDepth && currentShoulderDepth >= minShoulderDepth)
        {
            currentShoulderDepth += switchShoulderDirection * shoulderSwitchingSpeed * Time.deltaTime;
            cameraPivot.localPosition = new Vector3(currentShoulderDepth, cameraPivot.localPosition.y, 0f);
            yield return null;
        }

        //On snap
        if (currentShoulderDepth > maxShoulderDepth) currentShoulderDepth = maxShoulderDepth;
        else currentShoulderDepth = minShoulderDepth;
        cameraPivot.localPosition = new Vector3(currentShoulderDepth, cameraPivot.localPosition.y, 0f);

        //Unlock
        isSwitchingShoulder = false;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelRotationScript : MonoBehaviour
{
    #region Variables
    [SerializeField, Tooltip("Le script qui capture les inputs joueur")]
    private PlayerInputScript playerInput;

    [Space]

    [SerializeField, Tooltip("Les modeles 3D du personnage")]
    private Transform models;
    [SerializeField, Tooltip("Le point de pivot de notre camera")]
    private Transform playerBrain;

    private Vector2 movementInput = Vector2.zero; //Les mouvements au clavier du joueur
    private float targetModelsAngle = 0, currentModelsAngle = 0, modelsAngleStep; //L'angle qu'on veut donner a nos modeles, l'angle actuel, et le nombre de degres qu'on va faire a cette frame
    private float rotationAlpha, rotationBeta, rotationDirection; //Les deux sens de rotation possible pour re-orienter le modele, et le meilleur qu'on choisit
    private const float fastRotationSpeed = 720f, slowRotationSpeed = 360f; //L'angle maximale qu'on peut parcourir en une seule frame
    #endregion

    #region Update
    private void Update()
    {
        GetInputs();
        //On tourne les modeles en update pour que ca soit plus esthetique
        RotateModels();
    }
    #endregion

    #region PrivateMethods
    /// <summary>
    /// Simplement pour recuperer les inputs du joueur
    /// </summary>
    private void GetInputs()
    {
        movementInput = playerInput.movementInput;
    }

    /// <summary>
    /// Tourne les modeles dans le sens du deplacement progressivement lorsqu'on a des inputs
    /// </summary>
    private void RotateModels()
    {
        //On ne calcule pas l'angle si on a pas d'inputs
        if (movementInput != Vector2.zero)
        {
            //On commence par calculer l'angle qu'on veut atteindre
            targetModelsAngle = Vector3.SignedAngle(Vector3.forward, playerBrain.forward * movementInput.y + playerBrain.right * movementInput.x, Vector3.up);
        }

        //On ne se tourne pas si on a deja la bonne orientation
        if (currentModelsAngle != targetModelsAngle)
        {
            //On reste dans les bonnes bornes
            if (targetModelsAngle > 180) targetModelsAngle -= 360;
            else if (targetModelsAngle < -180) targetModelsAngle += 360;
            if (currentModelsAngle > 180) currentModelsAngle -= 360;
            else if (currentModelsAngle < -180) currentModelsAngle += 360;

            //On calcule le bon sens de rotation
            rotationAlpha = (targetModelsAngle - currentModelsAngle);
            rotationBeta = rotationAlpha < 0 ? 360 + rotationAlpha : rotationAlpha - 360;
            rotationDirection = Mathf.Abs(rotationAlpha) < Mathf.Abs(rotationBeta) ? rotationAlpha : rotationBeta;

            //On calcule le nombre de degres pour cette frame
            if (Mathf.Abs(rotationDirection) > 60) modelsAngleStep = fastRotationSpeed * Time.deltaTime * Mathf.Sign(rotationDirection);
            else modelsAngleStep = slowRotationSpeed * Time.deltaTime * Mathf.Sign(rotationDirection);

            //On snap a la bonne place si la step est trop grande
            if (Mathf.Abs(targetModelsAngle - currentModelsAngle) < Mathf.Abs(modelsAngleStep)) currentModelsAngle = targetModelsAngle;
            else currentModelsAngle += modelsAngleStep;

            //On applique la rotation
            models.localRotation = Quaternion.Euler(0, currentModelsAngle, 0);
        }
    }
    #endregion
}

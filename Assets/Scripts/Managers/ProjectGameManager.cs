using UnityEngine;

public class ProjectGameManager : MonoBehaviour
{
    #region Variables
    public static ProjectGameManager instance { get; private set; } //L'instance de ce singleton
    #endregion

    #region Awake
    private void Awake()
    {
        //On s'assure qu'on part pas entre chaque scene
        DontDestroyOnLoad(this.gameObject);

        //On s'assure qu'on est la seule instance
        if (instance != null && instance != this) Destroy(this);
        else instance = this;

        //On peut maintenant faire des trucs vraiment utile
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion
}

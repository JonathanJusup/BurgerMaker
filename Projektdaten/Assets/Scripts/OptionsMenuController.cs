using UnityEngine;

/// <summary>
/// Controller for options menu. Option menu must be a singleton to keep
/// a connection to the Sound Manager singleton instance and the relevant
/// UI elements and Listeners.
/// 
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class OptionsMenuController : MonoBehaviour {
    /// <summary>
    /// Singleton instance.
    /// </summary>
    private static OptionsMenuController instance;
    
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}

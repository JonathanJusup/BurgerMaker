using UnityEngine;

/// <summary>
/// GameOver Menu base class. Both Standard and Hell's Kitchen
/// gamemode are using variants of a GameOver Menu to show
/// relevant score data on GameOver.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class GameOverMenu : MonoBehaviour {
    /// <summary>
    /// GameOver Menu reference.
    /// </summary>
    private GameObject gameOverMenu;


    /// <summary>
    /// Start is called before the first frame update.
    /// Initially hides GameOver menu.
    /// </summary>
    public void Start() {
        this.gameOverMenu = this.transform.GetChild(0).gameObject;
        HideMenu();
    }

    /// <summary>
    /// Shows GameOver menu
    /// </summary>
    public void ShowMenu() {
        gameOverMenu.SetActive(true);
    }

    /// <summary>
    /// Hides GameOver menu
    /// </summary>
    public void HideMenu() {
        gameOverMenu.SetActive(false);
    }
}
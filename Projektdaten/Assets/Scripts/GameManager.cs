using System;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Game Manager class for controlling the flow of the game and loading scenes.
/// 
/// @author Prince Lare-Lantone (cgt104645),  Jonathan El Jusup (cgt104707)
/// </summary>
public class GameManager : MonoBehaviour {
    /// <summary>
    /// Ground reference for cleanliness score
    /// </summary>
    [SerializeField] private GroundController ground;
    
    /// <summary>
    /// GameOver UI Menu
    /// </summary>
    [SerializeField] private GameOverMenu gameOverMenu;

    /// <summary>
    /// GameOver audio clip
    /// </summary>
    [SerializeField] private AudioClip gameOverClip;

    /// <summary>
    /// Standard background music audio clip
    /// </summary>
    [SerializeField] private AudioClip bgmClip;

    /// <summary>
    /// Hell's Kitchen gamemode background music audio clip
    /// </summary>
    [SerializeField] private AudioClip bgmHellClip;
    
    /// <summary>
    /// Max orders for the Standard gamemode.
    /// </summary>
    public static int maxOrders = 5;
    
    /// <summary>
    /// Current count of fulfilled orders.
    /// </summary>
    private int orderCount = 0;
    
    /// <summary>
    /// Accumulated scores to average later.
    /// </summary>
    private float scoreSum = 0.0f;

    /// <summary>
    /// Minimum quality threshold for Hell's Kitchen gamemode.
    /// </summary>
    public static float minRequiredQuality = 0.7f;
    
    /// <summary>
    /// Current gamemode.
    /// </summary>
    public Gamemode gamemode;

    
    /// <summary>
    /// Start is called before the first frame update.
    /// Sets appropriate gamemode and play background music
    /// </summary>
    private void Start() {
        switch (SceneManager.GetActiveScene().buildIndex) {
            case 0:
                gamemode = Gamemode.Lobby;
                SoundManager.getInstance.StopBackgroundMusic();
                SoundManager.getInstance.PlayBackgroundMusic(bgmClip, 0.1f);

                break;
            case 1:
                gamemode = Gamemode.Standard;
                SoundManager.getInstance.StopBackgroundMusic();
                SoundManager.getInstance.PlayBackgroundMusic(bgmClip, 0.1f);
                break;
            case 2:
                SoundManager.getInstance.StopBackgroundMusic();
                SoundManager.getInstance.PlayBackgroundMusic(bgmHellClip, 0.1f);
                gamemode = Gamemode.HellsKitchen;
                break;
        }
    }

    /// <summary>
    /// Updates the score based on gamemode.
    /// </summary>
    /// <param name="score">score</param>
    /// <exception cref="ArgumentOutOfRangeException">In case of unsupported gamemode</exception>
    public void UpdateScore(float score) {
        switch (gamemode) {
            case Gamemode.Lobby:
                //No score in the lobby
                break;
            case Gamemode.Standard:
                //Increase order count and accumulate score
                orderCount++;
                scoreSum += score;

                //If all orders are fulfilled, trigger GameOver
                if (orderCount == maxOrders) {
                    TriggerGameOver();
                }

                break;
            case Gamemode.HellsKitchen:
                //Increase order count
                orderCount++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Triggers the game over, based on the gamemode.
    /// Standard gamemode:
    ///     Average all scores and take cleanliness into account
    ///     Shows GameOver menu
    ///     Plays audio clip
    /// 
    /// Hell's Kitchen gamemode:
    ///     Shows GameOver menu with number of fulfilled orders
    ///     Plays GameOver audio clip
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">In case of unsupported gamemode</exception>
    public void TriggerGameOver() {
        switch (gamemode) {
            case Gamemode.Standard:
                float averageScore = this.scoreSum / maxOrders;
                float cleanlinessScore = 1.0f - ground.CalcDirtiness();

                float overallScore = averageScore * 0.8f + cleanlinessScore * 0.2f;
                ((GameOverStandardMenu)gameOverMenu).SetGameOverScore(averageScore, cleanlinessScore, overallScore);
                gameOverMenu.ShowMenu();
                SoundManager.getInstance.PlaySoundEffect(gameOverClip, SoundManager.getInstance.transform, 1.0f);
                break;
            case Gamemode.HellsKitchen:
                ((GameOverHellsKitchenMenu)gameOverMenu).SetGameOverScore(orderCount);
                gameOverMenu.ShowMenu();
                SoundManager.getInstance.PlaySoundEffect(gameOverClip, SoundManager.getInstance.transform, 1.0f);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Loads the lobby scene.
    /// </summary>
    public void ReturnToLobby() {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Loads the Standard gamemode scene.
    /// </summary>
    public void PlayStandard() {
        SceneManager.LoadScene(1);
    }
    
    /// <summary>
    /// Loads the Hell's Kitchen gamemode.
    /// </summary>
    public void PlayHellsKitchen() {
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Resets the scene by reloading it.
    /// </summary>
    public void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    public void ExitApplication() {
        Application.Quit();
    }
}
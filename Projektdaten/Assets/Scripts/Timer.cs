using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Timer class for hell's kitchen gamemode. The timer decreases continuously.
/// When a burger order is completed and scored, the timer resets, but becomes
/// slightly faster. To prevent exploiting the timer function, by delivering a
/// burger of poor quality, a penalty. The timer is visualized by a UI progressBar.
/// 
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class Timer : MonoBehaviour {
    /// <summary>
    /// Current timer which increases continuously
    /// </summary>
    private float timeLeft;
    
    /// <summary>
    /// Start timer which decreases, when fulfilling an order
    /// </summary>
    [SerializeField] private float timer = 10.0f;

    //________________________________________________________________________________________
    
    /// <summary>
    /// Factor, that resets timer with a fraction of the original timer
    /// </summary>
    [SerializeField] private float penaltyFactor = 0.6f;
    
    /// <summary>
    /// Fail streak counter for consecutive poor burger scoring
    /// </summary>
    private int failStreak = 0;
    
    /// <summary>
    /// Game manager instance
    /// </summary>
    private GameManager gm;
    
    //________________________________________________________________________________________
    
    /// <summary>
    /// Progress bar UI element to visualize the remaining time
    /// </summary>
    [SerializeField] private Slider progressBar;
    
    /// <summary>
    /// Progress bar UI fill element to visualize the remaining time
    /// </summary>
    [SerializeField] private Image progressBarFill;
    
    /// <summary>
    /// Progress text UI to visualize the remaining time
    /// </summary>
    [SerializeField] private TextMeshProUGUI progressText;
    
    /// <summary>
    /// Green color to color the timer progress bar
    /// </summary>
    private Color colorGreen = Color.green;
    
    /// <summary>
    /// Red color to color the timer progress bar
    /// </summary>
    private Color colorRed = Color.red;
    
    //________________________________________________________________________________________
    
    /// <summary>
    /// Warning audio clip, when little time is left
    /// </summary>
    [SerializeField] private AudioClip warningClip;
    
    /// <summary>
    /// Flag, if warning audio clip already has been played
    /// </summary>
    private bool warningPlayed = false;

    
    
    // Start is called before the first frame update
    void Start() {
        timeLeft = timer;
        progressBar.value = 1.0f;
        this.gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0.0f) {
            //Decrease timeLeft, as time goes on
            progressBar.value = GetTimerProgress();
            progressText.SetText(Math.Floor(timeLeft).ToString());
            progressBarFill.color = Color.Lerp(colorRed, colorGreen, progressBar.value);
            timeLeft -= Time.deltaTime;
        } else {
            //Trigger GameOver & deactivate self
            gm.TriggerGameOver();
            this.enabled = false;
        }

        //Play warning, if remaining time is less than 20%
        if (GetTimerProgress() < 0.2f && !warningPlayed) {
            SoundManager.getInstance.PlaySoundEffect(warningClip, transform, 1.0f);
            warningPlayed = true;
        }
    }

    /// <summary>
    /// Everytime, an order is fulfilled, set new timer.
    /// New timer is slightly decreased.
    /// </summary>
    public void SetNewTimerAndReset(bool penalty) {
        timer *= 0.9f;  //Decrease new timer by 10%
        
        if (penalty) {
            failStreak++;
            
            //Repeated failure increases penalty Factor
            timeLeft = timer * (Mathf.Pow(penaltyFactor, failStreak));
        } else {
            //Reset timer and failStreak normally
            timeLeft = timer;
            failStreak = 0;
        }
        
    }
    
    /// <summary>
    /// Calculates ratio of timeLeft and timer between 0.0f - 1.0f.
    /// </summary>
    /// <returns>ratio of timeLeft and Timer</returns>
    public float GetTimerProgress() {
        return timeLeft / timer;
    }
}

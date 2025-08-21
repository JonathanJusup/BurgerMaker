using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Bell controller class for handling the bell. A bell can be triggered,
/// when the player touches it with its hand. When the bell is rung, it
/// triggers the burger evaluation, if its possible. Evaluation is blocked
/// by EvaluationController, if not both burger and order are present on the
/// evaluation area.
///
/// Hell's Kitchen Gamemode:
/// The bell is connected to the timer. If an evaluation was successful, the
/// timer will be reset. If the timer runs low, the player will be warned with
/// an audio clip.
/// 
/// @author Prince Lare-Lantone (cgt104645), Jonathan El Jusup (cgt104707)
/// </summary>
public class BellController : MonoBehaviour {
    /// <summary>
    /// Evaluation controller, which enables the evaluation, triggered by the bell.
    /// </summary>
    [SerializeField] private EvaluationController eval;

    /// <summary>
    /// GameManager instance.
    /// </summary>
    private GameManager gm;

    /// <summary>
    /// Bell ring audio clip
    /// </summary>
    [SerializeField] private AudioClip bell;

    /// <summary>
    /// Score menu UI Reference
    /// </summary>
    [SerializeField] private ScoreMenu scoreMenu;

    //________________________________________________________________________________________

    /// <summary>
    /// Timer reference (only relevant in Hell's Kitchen Gamemode)
    /// </summary>
    [SerializeField] private Timer timer = null;

    /// <summary>
    /// Gordon Ramsay Controller Reference (only relevant in Hell's Kitchen Gamemode)
    /// </summary>
    [SerializeField] private GordonRamsayController gordonRamsay;

    /// <summary>
    /// List of gordon ramsay insult audio clips to play randomly.
    /// </summary>
    [SerializeField] private List<AudioClip> penaltyAudioClips;


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start() {
        this.gm = FindObjectOfType<GameManager>();
    }

    /// <summary>
    /// On collision with player hand, ring the bell
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hand")) {
            RingBell();
        }
    }

    /// <summary>
    /// Rings the bell by playing a ring audio clip and evaluates the burger, if possible.
    /// To evaluate a burger, the burger and order are taken from the evaluation area. The
    /// resulting score is saved.
    ///
    /// Hell's Kitchen Gamemode:
    /// Additionally a timer will be reset, when a burger was evaluated. If the score doesn't
    /// meet a certain threshold, a penalty comes into play. The burger will be shot by lasers
    /// originating from Gordon's eyes and a random gordon ramsay insult is played.
    /// </summary>
    public void RingBell() {
        SoundManager.getInstance.PlaySoundEffect(bell, transform, 0.5f);
        if (!eval.CanEvaluate()) {
            return;
        }

        Stack burger = eval.GetBurger();
        Order order = eval.GetOrder();

        ScoringSystem.ScoreData scoreData = ScoringSystem.EvaluateBurger(burger.ingredients, order);
        scoreMenu.SetScore(scoreData);
        eval.ResetEvaluationArea();

        gm.UpdateScore(scoreData.overallScore);

        //If timer is assigned, also execute hells Kitchen logic 
        if (timer != null) {
            //If quality was subpar, reset timer with penalty
            bool hasPenalty = scoreData.overallScore < GameManager.minRequiredQuality;
            timer.SetNewTimerAndReset(hasPenalty);

            if (hasPenalty) {
                //Laser the burger and play a random insult audio clip
                gordonRamsay.TriggerLaser(burger.transform);
                AudioClip insult = penaltyAudioClips[Random.Range(0, penaltyAudioClips.Count - 1)];
                SoundManager.getInstance.PlaySoundEffect(insult, transform, 0.5f);
            }
        }
    }
}
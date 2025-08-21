using TMPro;
using UnityEngine;

/// <summary>
/// GameOver Standard Menu class is a specialization
/// of the base GameOver menu base class. Average score,
/// cleanliness score and overall score are set here.
/// 
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class GameOverStandardMenu : GameOverMenu {
    /// <summary>
    /// Average Score of all previous orders text UI element
    /// </summary>
    [SerializeField] private TextMeshProUGUI averageScore;

    /// <summary>
    /// Cleanliness score text UI element
    /// </summary>
    [SerializeField] private TextMeshProUGUI cleanlinessScore;

    /// <summary>
    /// Overall score text UI element
    /// </summary>
    [SerializeField] private TextMeshProUGUI overallScore;

    /// <summary>
    /// Sets text of average, cleanliness and overall score
    /// </summary>
    /// <param name="orders">Average order score</param>
    /// <param name="cleanliness">Cleanliness score</param>
    /// <param name="overall">Overall score</param>
    public void SetGameOverScore(float orders, float cleanliness, float overall) {
        this.averageScore.text = Utilities.GetScorePercentage(orders);
        this.cleanlinessScore.text = Utilities.GetScorePercentage(cleanliness);
        this.overallScore.text = Utilities.GetScorePercentage(overall);
    }
}
using TMPro;
using UnityEngine;

/// <summary>
/// Score menu class. Contains relevant UI elements.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class ScoreMenu : MonoBehaviour {
    /// <summary>
    /// Score window reference.
    /// </summary>
    [SerializeField] private GameObject scoreWindow;

    /// <summary>
    /// Count score text UI element.
    /// </summary>
    [SerializeField] private TextMeshProUGUI countScore;

    /// <summary>
    /// Order score text UI element.
    /// </summary>
    [SerializeField] private TextMeshProUGUI orderScore;

    /// <summary>
    /// Ingredient score text UI element.
    /// </summary>
    [SerializeField] private TextMeshProUGUI ingredientScore;

    /// <summary>
    /// Overall score text UI element.
    /// </summary>
    [SerializeField] private TextMeshProUGUI overallScore;


    /// <summary>
    /// Sets all score text UI elements with given scoreData.
    /// </summary>
    /// <param name="scoreData"></param>
    public void SetScore(ScoringSystem.ScoreData scoreData) {
        countScore.text = Utilities.GetScorePercentage(scoreData.countScore);
        orderScore.text = Utilities.GetScorePercentage(scoreData.orderScore);
        ingredientScore.text = Utilities.GetScorePercentage(scoreData.ingredientScore);
        overallScore.text = Utilities.GetScorePercentage(scoreData.overallScore);
    }
}
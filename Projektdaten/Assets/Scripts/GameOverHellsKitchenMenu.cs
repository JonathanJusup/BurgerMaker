using TMPro;
using UnityEngine;

/// <summary>
/// GameOver Hells Kitchen Menu class is a specialization
/// of the base GameOver menu base class. Only the number of
/// fulfilled orders is set here.
/// 
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class GameOverHellsKitchenMenu : GameOverMenu
{
    /// <summary>
    /// Number of orders Text UI element.
    /// </summary>
    [SerializeField] private TextMeshProUGUI numOrders;
    
    /// <summary>
    /// Sets number of fulfilled orders as highscore
    /// </summary>
    /// <param name="orders"></param>
    public void SetGameOverScore(int orders) {
        this.numOrders.text = orders.ToString();
    }
}

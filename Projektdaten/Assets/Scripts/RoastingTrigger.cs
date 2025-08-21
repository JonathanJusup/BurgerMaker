using UnityEngine;

/// <summary>
/// Handles the roasting behaviour on trigger.
/// 
/// @author Stefan Procik (minf104111), Prince Lare-Lantone (cgt104645), Christelle Maaß (minf104420)
/// </summary>
public class RoastingTrigger : MonoBehaviour {

    /// <summary>
	/// the roasting controller
	/// </summary>
    [SerializeField] private RoastingController roastingController;

    /// <summary>
    /// triggers roasting process
    /// </summary>
    /// <param name="other">collider</param>
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Pan")) {
            roastingController.SetIsRoasting(true);
        }
    }

    /// <summary>
	/// ends roasting process
	/// </summary>
    /// <param name="other">collider</param>
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Pan")) {
            roastingController.SetIsRoasting(false);
        }
    }
}

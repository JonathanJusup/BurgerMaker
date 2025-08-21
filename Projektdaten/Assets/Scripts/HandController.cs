using UnityEngine;

/// <summary>
/// Hand controller class for the player hands. If the player touches
/// the bell with his hands, he rings the bell and initializes the
/// burger evaluation process.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class HandController : MonoBehaviour
{
    /// <summary>
    /// On Collision with bell, trigger evaluation process.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Bell")) {
            other.gameObject.GetComponent<BellController>().RingBell();
        }
    }
}

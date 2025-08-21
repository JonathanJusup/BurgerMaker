using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Order container class which contains an order with all its data.
/// Becomes instantiated by the order spawner. If order container is
/// grabbed the first time, make it interact with the environment, by
/// deactivating its kinematic state.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class OrderContainer : MonoBehaviour {
    /// <summary>
    /// Order reference.
    /// </summary>
    public Order Order;
    
    /// <summary>
    /// Grab component, to check if it's beeing grabbed.
    /// </summary>
    private XRGrabInteractable grab;
    
    /// <summary>
    /// Rigidbody reference.
    /// </summary>
    private Rigidbody rb;
    
    /// <summary>
    /// Flag, if it has been grabbed at leastonce.
    /// </summary>
    private bool onceGrabbed = false;
    

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start() {
        this.grab = this.GetComponent<XRGrabInteractable>();
        this.rb = this.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update() {
        if (grab.isSelected) {
            onceGrabbed = true;
        }

        //If once grabbed, deactivate its kinematic state.
        if (!grab.isSelected && onceGrabbed && rb.isKinematic) {
            rb.isKinematic = false;
        }
    }
}

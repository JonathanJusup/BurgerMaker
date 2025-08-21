using System.Collections;
using UnityEngine;

/// <summary>
/// Car Controller class for driving them to their target position over time.
/// Resets them to their initial position and wait randomly.
/// 
/// @author Jonathan El Jusup
/// </summary>
public class CarController : MonoBehaviour {
    /// <summary>
    /// Car to translate it to its target position
    /// </summary>
    [SerializeField] private Transform car;

    /// <summary>
    /// Initial starting position 
    /// </summary>
    private Vector3 startPos;

    /// <summary>
    /// Target Transform position
    /// </summary>
    [SerializeField] private Transform target;

    /// <summary>
    /// Movement speed factor
    /// </summary>
    [SerializeField] private float movementSpeed = 1.0f;

    /// <summary>
    /// Currently lerped factor from 0.0f-1.0f
    /// </summary>
    private float lerpFactor = 0.0f;

    /// <summary>
    /// Flag, if car is paused
    /// </summary>
    private bool isPaused = false;


    // Start is called before the first frame update
    void Start() {
        startPos = car.position;
    }

    // Update is called once per frame
    void Update() {
        //If car is paused don't update
        if (isPaused) {
            return;
        }

        if (lerpFactor < 1.0f) {
            lerpFactor += Time.deltaTime * movementSpeed;
        } else {
            //If car reached its target, reset and pause for a random amount of time
            lerpFactor = 0.0f;
            isPaused = true;
            StartCoroutine(PauseRandomly());
        }

        UpdatePosition();
    }

    /// <summary>
    /// Updates car position based on lerped value. Lerps its starting position to its target.
    /// </summary>
    private void UpdatePosition() {
        Vector3 newPosition = Vector3.Lerp(startPos, target.position, lerpFactor);
        car.position = newPosition;
    }

    /// <summary>
    /// Pauses the car for a random amount of time to prevent percievable patterns.
    /// </summary>
    /// <returns>IEnumerator WaitForSeconds</returns>
    private IEnumerator PauseRandomly() {
        yield return new WaitForSeconds(Random.Range(1.0f, 5.0f));
        isPaused = false;
    }
}
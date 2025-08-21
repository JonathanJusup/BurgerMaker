using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Sauce tube class. Controls the sauce tubes. Allows the player to
/// squeeze the sauce tube to spawn sauce particles based on much he
/// presses the trigger. Differentiates between left and right controller
/// and allows to use multiple sauce tubes at the same time.
///  
/// @author Prince Lare-Lantone (cgt104645), Christelle Maaß (minf104420), Jonathan El Jusup (cgt104707)
/// </summary>
public class SauceTube : MonoBehaviour {
    /// <summary>
    /// Sauce tip transform, where the sauce particles appear.
    /// </summary>
    [SerializeField] private Transform tip;
    
    /// <summary>
    /// Active sauce particles parent.
    /// </summary>
    [SerializeField] private Transform activeIngredients;
    
    /// <summary>
    /// Corresponding sauce pool to get sauce particles from.
    /// </summary>
    [SerializeField] protected SaucePool pool;

    /// <summary>
    /// the sfx to be played when squeezing.
    /// </summary>
    [SerializeField] private AudioClip sauceClip;

    /// <summary>
    /// flag for when the tube is being squeezed to trigger sound.
    /// </summary>
    private bool isSqueezing = false;

    /// <summary>
    /// flag for when the tube is being squeezed and sound already active.
    /// </summary>
    private bool isSqueezed = false;

    /// <summary>
    /// the source for the tube sound.
    /// </summary>
    private AudioSource sauceAudioSource;


    [SerializeField] private ControllerInput controllerInput;
    private XRGrabInteractable grab;
    

    // Start is called before the first frame update
    void Start() {
        this.grab = GetComponent<XRGrabInteractable>();
        this.pool = GetComponent<SaucePool>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// Checks, if sauce tube is being hold by the player. Differentiates
    /// between left and right controller and gets the trigger value to
    /// determine, how much sauce is being squeezed out the tube. 
    /// </summary>
    private void FixedUpdate() {
        if (grab.isSelected) {
            var interactor = grab.firstInteractorSelecting as XRBaseInteractor;
            if (interactor == null) {
                return;
            }

            //Find out, which controller holds the sauce tube, then get trigger value
            Debug.Log(interactor.transform.parent.name);

            //Handle right controller input
            if (interactor.transform.parent.name.Contains("Right")) {
                controllerInput.rightController.TryGetFeatureValue(CommonUsages.trigger, out float sauceFactor);
                SetIsSqueezing(sauceFactor);
                if (sauceFactor > 0) {
                    SpawnSauce(sauceFactor);
                }
            }

            //Handle left controller input
            if (interactor.transform.parent.name.Contains("Left")) {
                controllerInput.leftController.TryGetFeatureValue(CommonUsages.trigger, out float sauceFactor);
                SetIsSqueezing(sauceFactor);
                if (sauceFactor > 0) {
                    SpawnSauce(sauceFactor);
                }
            }
        } else {
            SetIsSqueezing(0.0f);
        }
    }

    /// <summary>
    /// Instantiates sauce drops. Adjusts amount and size based on sauceFactor.
    /// </summary>
    /// <param name="sauceFactor">Trigger value ranging from 0..1</param>
    void SpawnSauce(float sauceFactor) {
        GameObject sauce = pool.GetPooledIngredient();
        if (sauce != null) {
            sauce.transform.position = tip.position;
            sauce.transform.parent = activeIngredients;
            sauce.transform.localScale *= sauceFactor * 2.0f;
        }
    }

    /// <summary>
    /// Checks, if the tube is being squeezed and therefore should play sound
    /// </summary>
    /// <param name="squeezeFactor">Trigger value ranging from 0.0f-1.0f</param>
    public void SetIsSqueezing(float squeezeFactor) {
        this.isSqueezing = grab.isSelected && squeezeFactor > 0.0f;

        if (this.isSqueezing && !isSqueezed) {
            isSqueezed = true;
            PlaySauceSound();
        } else if (!this.isSqueezing) {
            isSqueezed = false;
            StopSauceSound();
        }
    }

    /// <summary>
    /// Plays the squeezing sound during squeezing the tube.
    /// </summary>
    private void PlaySauceSound() {
        if (SoundManager.getInstance != null && sauceClip != null) {
            sauceAudioSource = SoundManager.getInstance.PlaySoundEffectLoop(sauceClip, transform, 0.3f);
        }
    }

    /// <summary>
    /// Stops the squeezing sound during squeezing the tube.
    /// </summary>
    private void StopSauceSound() {
        if (SoundManager.getInstance != null && sauceAudioSource != null) {
            SoundManager.getInstance.StopSoundEffect(sauceAudioSource);
        }
    }
}
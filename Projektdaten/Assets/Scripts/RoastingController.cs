using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

/// <summary>
/// Controls the roasting process of a burger patty.
/// This class manages the timing, color transition and sound effects for the roasting process.
/// It manages the pattys progression through different stages: raw, properly roasted, and burnt.
/// The roasting status is visually represented through color changes and a progress bar.
/// 
/// @author Stefan (Procik minf104111), Prince Lare-Lantone (cgt104645), Christelle Maaß (minf104420), Jonathan El Jusup (cgt104707)
/// </summary>
public class RoastingController : MonoBehaviour {
    /// <summary>
    /// Patty roasting sound
    /// </summary>
    [SerializeField] private AudioClip fryingClip;

    /// <summary>
    /// Sound for when the patty is perfectly roasted
    /// </summary>
    [SerializeField] private AudioClip fryingSuccess;

    /// <summary>
    /// Animator component for controlling animations
    /// </summary>
    [SerializeField] private Animator animator;

    /// <summary>
    /// Defines the lower time limit for the perfect roasting time
    /// </summary>
    [SerializeField] private float roastTimeStart = 4.0f;

    /// <summary>
    /// Defines the upper time limit for the perfect roasting time
    /// </summary>
    [SerializeField] private float roastTimeEnd = 7.0f;

    /// <summary>
    /// Defines the time limit after which the patty is burnt
    /// </summary>
    public float burntTime = 10.0f;

    /// <summary>
    /// Default color of the burger patty
    /// </summary>
    private Color defaultColor;

    /// <summary>
    /// Color of the perfectly roasted patty
    /// </summary>
    private Color roastColor = new Color(0.5566038f, 0.3f, 0.15f);

    /// <summary>
    /// Color of the burned patty
    /// </summary>
    private Color burntColor = new Color(0.2f, 0.2f, 0.2f);

    /// <summary>
    /// Indicates, if the patty is roasting
    /// </summary>
    private bool isRoasting = false;

    /// <summary>
    /// The roasting timer, keeps track of how long the patty is roasting
    /// </summary>
    public float roastingTimer = 0f;

    /// <summary>
    /// Renderer component for the patty 
    /// </summary>
    [SerializeField] private new Renderer renderer;

    /// <summary>
    /// Defines the AudioSource of the frying sound
    /// </summary>
    private AudioSource fryingAudioSource;

    /// <summary>
    /// Indicates whether the success sound has already been played
    /// </summary>
    private bool hasPlayedSuccessSound = false;

    /// <summary>
    /// Reference to the XRGrabInteractable component which handles VR Controller interactions
    /// </summary>
    private XRGrabInteractable grab;

    /// <summary>
    /// Reference to the other half of the patty
    /// </summary>
    [SerializeField] private RoastingController otherHalf;

    /// <summary>
    /// Progress bar for showing the roasting progress
    /// </summary>
    [SerializeField] private Slider progressBar;

    /// <summary>
    /// Image component for changing the color of the progress bar
    /// </summary>
    [SerializeField] private Image progressColor;

    /// <summary>
    /// Parent object containing the progress markers
    /// </summary>
    [SerializeField] private GameObject progressMarkers;

    /// <summary>
    /// Player Transform.
    /// </summary>
    private Transform player;

    /// <summary>
    /// Sets the roasting status and plays roasting sound when a patty is roasting.
    /// </summary>
    /// <param name="isRoasting">Indicates whether a patty is roasting or not</param>
    public void SetIsRoasting(bool isRoasting) {
        this.isRoasting = isRoasting;
        this.animator.SetBool("IsRoasting", isRoasting);

        if (this.isRoasting) {
            PlayFryingSound();
        } else {
            StopFryingSound();
        }
    }

    /// <summary>
    /// Plays roasting sound
    /// </summary>
    private void PlayFryingSound() {
        if (SoundManager.getInstance != null && fryingClip != null) {
            fryingAudioSource = SoundManager.getInstance.PlaySoundEffectLoop(fryingClip, transform, 1f);
        }
    }

    /// <summary>
    /// Stops the roasting sound.
    /// </summary>
    private void StopFryingSound() {
        if (SoundManager.getInstance != null && fryingAudioSource != null) {
            SoundManager.getInstance.StopSoundEffect(fryingAudioSource);
        }
    }

    /// <summary>
    /// Start is called before the first frame update
    /// Initializes the roasting controller, sets the default patty color,
    /// finds the player transform and configures progress markers.
    /// </summary>
    void Start() {
        //this.renderer = this.GetComponent<Renderer>();
        // Store the default color of the material
        defaultColor = renderer.material.color;
        this.player = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
        this.grab = this.transform.parent.GetComponent<XRGrabInteractable>();

        // Set the position of progress markers based on roasting and burnt times
        this.progressMarkers.transform.GetChild(0).GetComponent<RectTransform>().localPosition =
            new Vector3((roastTimeStart / burntTime) * 100.0f, 0.0f, 0.0f);
        this.progressMarkers.transform.GetChild(1).GetComponent<RectTransform>().localPosition =
            new Vector3((roastTimeEnd / burntTime) * 100.0f, 0.0f, 0.0f);
    }

    /// <summary>
    /// Update is called once per frame
    /// Manages the roasting process, updates the roasting timer and progress bar,
    /// calculates the color interpolation for the patty 
    /// </summary>
    void Update() {
        if (isRoasting) {
            roastingTimer += Time.deltaTime;

            // Calculate the percentage of time passed & interpolate color
            float roastPercentage = Mathf.Clamp01(roastingTimer / roastTimeStart);
            Color lerpedColor = Color.Lerp(defaultColor, roastColor, roastPercentage);

            // Play success sound when the patty is perfectly roasted
            if (roastingTimer >= roastTimeStart && !hasPlayedSuccessSound) {
                SoundManager.getInstance.PlaySoundEffect(fryingSuccess, transform, 0.5f);
                hasPlayedSuccessSound = true; // only play once
            }

            //If roastingTime exceeds roastingWindow, start burning process
            if (roastingTimer >= roastTimeEnd) {
                float start = roastingTimer - roastTimeEnd;
                float end = burntTime - roastTimeEnd;
                float burntPercentage = Mathf.Clamp01(start / end);

                //Continue lerping color to burntColor
                lerpedColor = Color.Lerp(lerpedColor, burntColor, burntPercentage);

                //Check if the patty is burnt
                if (roastingTimer >= burntTime) {
                    isRoasting = false; // Stop roasting if the burger is burnt
                }
            }

            UpdateProgressBar(roastingTimer);

            //Set the color of the material
            renderer.material.color = lerpedColor;
        }

        //Show progress bar if the patty is being grabbed or roasted
        progressBar.transform.parent.gameObject.SetActive(grab.isSelected || isRoasting || otherHalf.isRoasting);
        if (progressBar.gameObject.activeSelf) {
            progressBar.transform.parent.parent.position = this.transform.position + Vector3.up * 0.1f;
            progressBar.transform.parent.parent.transform.LookAt(player); //Affordance Container
        }
    }

    /// <summary>
    /// Updates the progress bar based on the roasting progress.
    /// </summary>
    /// <param name="currentTime">Current roasting time</param>
    private void UpdateProgressBar(float currentTime) {
        progressBar.value = currentTime / burntTime;

        if (roastingTimer < roastTimeStart) {
            progressColor.color = Color.red;
        } else if (roastingTimer >= roastTimeStart && roastingTimer < roastTimeEnd) {
            progressColor.color = Color.green;
        } else {
            progressColor.color = Color.red;
        }
    }
}
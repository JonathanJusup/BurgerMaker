using UnityEngine;

/// <summary>
/// Gordon Ramsay controller for controlling the Gordon Ramsay billboard,
/// which approaches a desired target position based on the timer value
/// in the Hell's Kitchen gamemode. Aside from the movement, Gordon is also
/// able to shoot laser beams from his eyes towards a burger, that hasn't
/// met quality standards.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class GordonRamsayController : MonoBehaviour {
    /// <summary>
    /// Transform component of Gordon Ramsay to translate.
    /// </summary>
    [SerializeField] private Transform gordonRamsay;

    /// <summary>
    /// Initial starting position.
    /// </summary>
    private Vector3 startPosition;

    /// <summary>
    /// target transform position.
    /// </summary>
    [SerializeField] private Transform targetPosition;

    /// <summary>
    /// Timer reference for movement interpolation.
    /// </summary>
    [SerializeField] private Timer timer;

    /// <summary>
    /// Player transform for Gordon to always face the player.
    /// </summary>
    private Transform player;

    //________________________________________________________________________________________

    /// <summary>
    /// Laseer audio clip.
    /// </summary>
    [SerializeField] private AudioClip laserClip;

    /// <summary>
    /// Line renderer component for left laser eye.
    /// </summary>
    [SerializeField] private LineRenderer laserLeft;

    /// <summary>
    /// Line renderer component for right laser eye.
    /// </summary>
    [SerializeField] private LineRenderer laserRight;

    /// <summary>
    /// Laser width, which decreases, after its been shot.
    /// </summary>
    private float laserWidth;

    /// <summary>
    /// Laser timer, how long it should be active.
    /// </summary>
    private float laserTimer = 2.0f;

    /// <summary>
    /// Current laser timer, which increases with time.
    /// </summary>
    private float currentLaserTimer = 0.0f;

    /// <summary>
    /// Laser target, which is the burger of poor quality.
    /// </summary>
    private Vector3 laserTarget;

    /// <summary>
    /// Particle system component for explosion vfx.
    /// </summary>
    private ParticleSystem particles;


    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start() {
        this.player = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
        this.currentLaserTimer = laserTimer;
        
        this.startPosition = this.transform.position;
        this.laserWidth = laserLeft.startWidth;
        this.particles = GetComponentInChildren<ParticleSystem>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update() {
        gordonRamsay.LookAt(player);

        //Lerps position based on timer value
        float lerpFactor = 1.0f - timer.GetTimerProgress();
        Vector3 pos = Vector3.Lerp(startPosition, targetPosition.position, lerpFactor);
        pos = new Vector3(pos.x, gordonRamsay.position.y, pos.z);
        gordonRamsay.position = pos;

        //Shoot Laser to failed burger and explode
        if (currentLaserTimer < laserTimer) {
            LaserBurger(laserTarget);
            currentLaserTimer += Time.deltaTime;
        } else {
            //Deactivate lasers
            if (laserLeft.enabled || laserRight.enabled) {
                laserLeft.enabled = false;
                laserRight.enabled = false;
            }
        }
    }

    /// <summary>
    /// Triggers lasers. Laser beams shoot out from Gordon Ramsay's eyes
    /// to the target position.
    /// </summary>
    /// <param name="target">The failed burger target</param>
    public void TriggerLaser(Transform target) {
        this.laserTarget = target.position;
        currentLaserTimer = 0.0f;
        particles.transform.position = target.position;
        particles.Play();
        SoundManager.getInstance.PlaySoundEffect(laserClip, target, 0.2f);
    }

    /// <summary>
    /// Updates the laser beams to always start from Gordon's eyes and
    /// hit the burger position. Gradually decrease laser beam width based
    /// on its lifetime.
    /// </summary>
    /// <param name="targetPos">Target position</param>
    private void LaserBurger(Vector3 targetPos) {
        laserLeft.SetPosition(0, laserLeft.transform.position);
        laserRight.SetPosition(0, laserRight.transform.position);

        laserLeft.SetPosition(1, targetPos);
        laserRight.SetPosition(1, targetPos);

        float timerRatio = currentLaserTimer / laserTimer;
        laserLeft.startWidth = laserWidth * (1.0f - timerRatio);
        laserRight.startWidth = laserWidth * (1.0f - timerRatio);

        if (!laserLeft.enabled || !laserRight.enabled) {
            laserLeft.enabled = true;
            laserRight.enabled = true;
        }
    }
}
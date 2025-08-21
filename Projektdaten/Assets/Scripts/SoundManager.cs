using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Sound manager singleton. Controls everything audio related. Is connected
/// with the options menu for adjusting music and sfx volume. Assigns listeners
/// which react on volume slider changes to also update the volume in logic.
/// 
/// @author Prince Lare-Lantone (cgt104645),  Jonathan El Jusup (cgt104707)
/// </summary>
public class SoundManager : MonoBehaviour {
    /// <summary>
    /// Soundmanager instance,will be a singleton because we only need 1 ever
    /// </summary>
    private static SoundManager instance;

    public static SoundManager getInstance => instance;

    /// <summary>
    /// the sfx to be played
    /// </summary>
    [SerializeField] private AudioSource soundObject;

    /// <summary>
    /// the MusicObject for the bgm to be played
    /// </summary>
    [SerializeField] private AudioSource musicObject;

    /// <summary>
    /// the bgm to be played
    /// </summary>
    private AudioSource musicSource;

    /// <summary>
    /// slider for the sfx UI
    /// </summary>
    [SerializeField] private Slider sfx_slider;

    /// <summary>
    /// text for the sfx UI
    /// </summary>
    [SerializeField] private TMP_Text sfx_value;

    /// <summary>
    /// slider for the music UI
    /// </summary>
    [SerializeField] private Slider music_slider;

    /// <summary>
    /// slider for the music UI
    /// </summary>
    [SerializeField] private TMP_Text music_value;

    /// <summary>
    /// the audioMixer
    /// </summary>
    [SerializeField] private AudioMixer mainMixer;


    /// <summary>
    /// method called on awake
    /// </summary>
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// sets the volumes on start
    /// </summary>
    private void Start() {
        //Add Listeners to Sliders, to change volume
        music_slider.onValueChanged.AddListener(this.SetMusicVolume);
        sfx_slider.onValueChanged.AddListener(SetSFXVolume);

        //Set music volume based on PlayerPrefs
        music_value.text = (PlayerPrefs.GetFloat("Music_Volume") * 100.0f).ToString("0") + "%";
        music_slider.value = PlayerPrefs.GetFloat("Music_Volume");
        mainMixer.SetFloat("Music_Volume", Mathf.Log10(PlayerPrefs.GetFloat("Music_Volume")) * 20);

        //Set sfx volume based on PlayerPrefs
        sfx_value.text = (PlayerPrefs.GetFloat("SFX_Volume") * 100.0f).ToString("0") + "%";
        sfx_slider.value = PlayerPrefs.GetFloat("SFX_Volume");
        mainMixer.SetFloat("SFX_Volume", Mathf.Log10(PlayerPrefs.GetFloat("SFX_Volume")) * 20);
    }


    /// <summary>
    /// Plays a sound effect on Loop
    /// </summary>
    /// <param name="clip">clip to be played</param>
    /// <param name="transform">position to be played from</param>
    /// <param name="volume">volume</param>
    public AudioSource PlaySoundEffectLoop(AudioClip clip, Transform transform, float volume) {
        AudioSource audioSource = Instantiate(soundObject, transform.position, Quaternion.identity, transform);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = true; // Set sound to loop
        audioSource.Play();
        return audioSource;
    }

    /// <summary>
    /// Plays a sound effect once
    /// </summary>
    /// <param name="clip">clip to be played</param>
    /// <param name="transform">position to be played from</param>
    /// <param name="volume">volume</param>
    public void PlaySoundEffect(AudioClip clip, Transform transform, float volume) {
        //instantiates the sound effect to be played
        AudioSource audioSource = Instantiate(soundObject, transform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    /// <summary>
    ///stops a sound effect
    /// </summary>
    public void StopSoundEffect(AudioSource audioSource) {
        if (audioSource != null) {
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }
    }

    /// <summary>
    /// Plays the bgm
    /// </summary>
    /// <param name="clip">clip to be played</param>
    /// <param name="volume">volume</param>
    public void PlayBackgroundMusic(AudioClip clip, float volume) {
        if (musicSource == null) {
            musicSource = Instantiate(musicObject, transform.position, Quaternion.identity, transform);
        }

        if (musicSource != null) {
            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.loop = true; // Loop the music
            musicSource.Play();
        }
    }

    /// <summary>
    /// stops the bgm
    /// </summary>
    public void StopBackgroundMusic() {
        if (musicSource != null) {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// Gets called when the slider value for the volume of the music gets changed.
    /// Sets the volume for the music.
    /// </summary>
    /// <param name="volume"> The volume for the music. </param>
    public void SetMusicVolume(float volume) {
        music_value.text = (volume * 100.0f).ToString("0") + "%";
        // Audiomixer volume changes logarithmically, slider values change linearly
        mainMixer.SetFloat("Music_Volume", Mathf.Log10(volume) * 20);

        // Setting playerprefs for the music with the chosen volume
        PlayerPrefs.SetFloat("Music_Volume", volume);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Gets called when the slider value for the volume of the special effects gets changed.
    /// Sets the volume for the special effects.
    /// </summary>
    /// <param name="volume"> The volume for the special effects. </param>
    public void SetSFXVolume(float volume) {
        sfx_value.text = (volume * 100.0f).ToString("0") + "%";
        // Audiomixer volume changes logarithmically, slider values change linearly
        mainMixer.SetFloat("SFX_Volume", Mathf.Log10(volume) * 20);

        // Setting playerprefs for the special effects with the chosen volume
        PlayerPrefs.SetFloat("SFX_Volume", volume);

        // Saving playerprefs
        PlayerPrefs.Save();
    }
}
using UnityEngine;

/// <summary>
/// This class controls the handles of an analog clock.
/// The clock displays central european summer time (UTC+2).
/// 
/// @Author Christelle Maass (minf104420), Jonathan El Jusup (cgt104707)
/// </summary>
public class Clock : MonoBehaviour
{

    /// <summary>
    /// Seconds Handle Transform
    /// </summary>
    [SerializeField] private Transform secondsHandle;
    
    /// <summary>
    /// Minutes Handle Transform
    /// </summary>
    [SerializeField] private Transform minutesHandle;
    
    /// <summary>
    /// Hours Handle Transform
    /// </summary>
    [SerializeField] private Transform hoursHandle;

    /// <summary>
    /// Flag, if rotation should be inverted
    /// </summary>
    public bool invert = true;


    /// <summary>
    /// Update is called once per frame
    /// Updates the rotation of the clock handles based on the current time
    /// </summary>
    void Update()
    {
        //Get UTC+2 time
        System.DateTime currentTime = System.DateTime.UtcNow.AddHours(2);

        int seconds = currentTime.Second;
        int minutes = currentTime.Minute;
        int hours = currentTime.Hour;
        
        //Update clock handles for seconds, minutes and hours
        float secondsAngle = (seconds / 60f) * 360f * (invert ? -1.0f : 1.0f);
        secondsHandle.rotation = Quaternion.Euler(0, 0, secondsAngle);

        float minutesAngle = (minutes / 60f) * 360f * (invert ? -1.0f : 1.0f);
        minutesHandle.rotation = Quaternion.Euler(0, 0, minutesAngle);

        float hoursAngle = (hours / 12f) * 360f * (invert ? -1.0f : 1.0f);
        hoursHandle.rotation = Quaternion.Euler(0, 0, hoursAngle);
    }
}

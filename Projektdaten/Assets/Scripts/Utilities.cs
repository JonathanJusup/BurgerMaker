using System;
/// <summary>
/// Utility class for useful functions.
/// 
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class Utilities {
    /// <summary>
    /// Helper function for converting floating point numbers to percentage String.
    /// </summary>
    /// <param name="value">value between 0.0f and 1.0f</param>
    /// <returns>String representation of percentage value of value</returns>
    public static String GetScorePercentage(float value) {
        return (int)Math.Ceiling(value * 100.0f) + "%";
    }
}
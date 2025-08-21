using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using InputDevice = UnityEngine.XR.InputDevice;

/// <summary>
/// Initializes left and right controllers to get their input values from.
/// Especially used by sauceTubes, where it's important to know the precise
/// trigger value.
/// 
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class ControllerInput : MonoBehaviour {
    /// <summary>
    /// Left controller
    /// </summary>
    public InputDevice leftController;
    
    /// <summary>
    /// Right controller
    /// </summary>
    public InputDevice rightController;

    /// <summary>
    /// Update is called once per frame. Initializes controllers, if necessary
    /// </summary>
    private void Update() {
        if (!leftController.isValid || !rightController.isValid) {
            InitializeInputDevices();
        }
    }

    /// <summary>
    /// Initializes left and/or right controllers, if not valid
    /// </summary>
    private void InitializeInputDevices() {
        if (!leftController.isValid) {
            InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, ref leftController);
        }

        if (!rightController.isValid) {
            InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, ref rightController);
        }
    }

    /// <summary>
    /// Initializes input device based on given input characteristics.
    /// In this case left or right controllers are initialized here.
    /// </summary>
    /// <param name="inputCharacteristics">Specified input device characteristics</param>
    /// <param name="inputDevice">Reference input device</param>
    private void InitializeInputDevice(InputDeviceCharacteristics inputCharacteristics, ref InputDevice inputDevice) {
        List<InputDevice> devices = new List<InputDevice>();
         InputDevices.GetDevicesWithCharacteristics(inputCharacteristics, devices);

         if (devices.Count > 0) {
             inputDevice = devices[0];
         }
    }
}

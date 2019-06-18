using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

[MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1), // All platforms supported by Unity
        "Custom Input Simulation Service")]
public class CustomInputSimulationService : BaseInputDeviceManager
{
    public CustomInputSimulationService(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, name, priority, profile) { }

    public SimulatedHeadset SimulatedHeadset { get; private set; } = null;
    public Dictionary<Handedness, SimulatedMotionController> SimulatedMotionControllerDictionary { get; private set; } = new Dictionary<Handedness, SimulatedMotionController>();

    /// <inheritdoc />
    public override void Enable()
    {
        if (!m_IsEnabled)
        {
            m_IsEnabled = true;

            SimulatedHeadset = new SimulatedHeadset();

            PlugSimulatedMotionController(Handedness.Right);
        }
    }

    /// <inheritdoc />
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            m_DisableMouse = !m_DisableMouse;

            if (m_DisableMouse)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!SimulatedMotionControllerDictionary.ContainsKey(Handedness.Left))
            {
                PlugSimulatedMotionController(Handedness.Left);
            }
            else
            {
                UnplugSimulatedMotionController(Handedness.Left);
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!SimulatedMotionControllerDictionary.ContainsKey(Handedness.Right))
            {
                PlugSimulatedMotionController(Handedness.Right);
            }
            else
            {
                UnplugSimulatedMotionController(Handedness.Right);
            }
        }

        if (m_DisableMouse)
        {
            SimulatedHeadset.Update();

            foreach (var pair in SimulatedMotionControllerDictionary)
            {
                pair.Value.Update();
            }
        }
    }

    /// <inheritdoc />
    public override void Disable()
    {
        IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

        if (SimulatedMotionControllerDictionary.ContainsKey(Handedness.Left))
        {
            UnplugSimulatedMotionController(Handedness.Left);
        }

        if (SimulatedMotionControllerDictionary.ContainsKey(Handedness.Right))
        {
            UnplugSimulatedMotionController(Handedness.Right);
        }
    }



    bool m_DisableMouse = false;

    void PlugSimulatedMotionController(Handedness controllerHandedness)
    {
        MixedRealityRaycaster.DebugEnabled = true;

        System.Type controllerType = typeof(SimulatedMotionController);

        // Make sure that the handedness declared in the controller attribute matches what we expect
        {
            var controllerAttribute = MixedRealityControllerAttribute.Find(controllerType);
            if (controllerAttribute != null)
            {
                Handedness[] handednesses = controllerAttribute.SupportedHandedness;
                //Debug.Assert(handednesses.Length == 3 && handednesses[0] == Handedness.Any, "Unexpected mouse handedness declared in MixedRealityControllerAttribute");
            }
        }

        IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
        var pointers = RequestPointers(SupportedControllerType.GenericOpenVR, controllerHandedness);
        IMixedRealityInputSource inputSource = inputSystem.RequestNewGenericInputSource("Mouse Input", pointers, InputSourceType.Controller);

        SimulatedMotionController simulatedMotionController = new SimulatedMotionController(TrackingState.Tracked, controllerHandedness, inputSource);

        simulatedMotionController.SetupConfiguration(controllerType);

        if (inputSource != null)
        {
            for (int i = 0; i < inputSource.Pointers.Length; i++)
            {
                inputSource.Pointers[i].Controller = simulatedMotionController;
            }
        }

        SimulatedMotionControllerDictionary.Add(controllerHandedness, simulatedMotionController);

        inputSystem?.RaiseSourceDetected(simulatedMotionController.InputSource, simulatedMotionController);
    }

    void UnplugSimulatedMotionController(Handedness controllerHandedness)
    {
        var motionController = SimulatedMotionControllerDictionary[controllerHandedness];

        IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
        inputSystem?.RaiseSourceLost(motionController.InputSource, motionController);

        SimulatedMotionControllerDictionary.Remove(controllerHandedness);
    }



    bool m_IsEnabled = false;
}

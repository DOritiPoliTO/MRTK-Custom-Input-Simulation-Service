using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

[MixedRealityController(
    SupportedControllerType.SimulatedMotionController,
    new[] { Handedness.Left, Handedness.Right },
    flags: MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings)]
public class SimulatedMotionController : BaseController
{
    public SimulatedMotionController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
    {
        if (controllerHandedness == Handedness.Left)
        {
            m_ControllerActivationKeycode = KeyCode.N;

            m_LocalPosition.x = -0.3f;
        }
        else if (controllerHandedness == Handedness.Right)
        {
            m_ControllerActivationKeycode = KeyCode.M;

            m_LocalPosition.x = 0.3f;
        }
        else
        {
            Debug.Log("Something is wrong with controller handedness!");
        }
    }

    /// <inheritdoc />
    public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
    {
        new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
        new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
        new MixedRealityInteractionMapping(2, "Trigger Press", AxisType.Digital, DeviceInputType.ButtonPress,  KeyCode.Mouse0),
        new MixedRealityInteractionMapping(3, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, ControllerMappingLibrary.AXIS_9),
        new MixedRealityInteractionMapping(4, "Grip Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse1),
        new MixedRealityInteractionMapping(5, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2),
        new MixedRealityInteractionMapping(6, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch,  KeyCode.JoystickButton16),
        new MixedRealityInteractionMapping(7, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress,  KeyCode.JoystickButton8),
        new MixedRealityInteractionMapping(8, "Menu Button", AxisType.Digital, DeviceInputType.Menu,  KeyCode.H),
    };

    /// <inheritdoc />
    public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
    {
        new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
        new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
        new MixedRealityInteractionMapping(2, "Trigger Press", AxisType.Digital, DeviceInputType.ButtonPress,  KeyCode.Mouse0),
        new MixedRealityInteractionMapping(3, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, ControllerMappingLibrary.AXIS_9),
        new MixedRealityInteractionMapping(4, "Grip Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse1),
        new MixedRealityInteractionMapping(5, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2),
        new MixedRealityInteractionMapping(6, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch,  KeyCode.JoystickButton16),
        new MixedRealityInteractionMapping(7, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress,  KeyCode.JoystickButton8),
        new MixedRealityInteractionMapping(8, "Menu Button", AxisType.Digital, DeviceInputType.Menu,  KeyCode.H),
    };

    /// <inheritdoc />
    public override void SetupDefaultInteractions(Handedness controllerHandedness)
    {
        AssignControllerMappings(controllerHandedness == Handedness.Left ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions);
    }

    /// <summary>
    /// Update controller.
    /// </summary>
    public void Update()
    {
        if (UpdatePose())
        {
            InputSystem?.RaiseSourcePoseChanged(InputSource, this, m_Pose);
        }

        for (int i = 0; i < Interactions.Length; i++)
        {
            if (Interactions[i].InputType == DeviceInputType.SpatialPointer)
            {
                Interactions[i].PoseData = m_Pose;

                if (Interactions[i].Changed)
                {
                    InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, Interactions[i].PoseData);

                    foreach (var pointer in InputSource.Pointers)
                    {
                        InputSystem?.RaisePointerDragged(pointer, Interactions[i].MixedRealityInputAction, ControllerHandedness, InputSource);
                    }
                }
            }
            else if (Interactions[i].AxisType == AxisType.Digital)
            {
                Interactions[i].BoolData = Input.GetKey(Interactions[i].KeyCode) && Input.GetKey(m_ControllerActivationKeycode);

                if (Interactions[i].Changed)
                {
                    Debug.Log(Interactions[i].MixedRealityInputAction.Description);
                    if (Interactions[i].BoolData)
                    {
                        InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                    }
                    else
                    {
                        InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                    }
                }
            }
        }
    }



    KeyCode m_ControllerActivationKeycode;
    MixedRealityPose m_Pose = MixedRealityPose.ZeroIdentity;
    Vector3 m_LocalPosition = new Vector3(0.0f, -0.3f, 0.65f);
    Quaternion m_LocalRotation = Quaternion.identity;
    float m_AngleY = 0.0f;
    float m_AngleX = 0.0f;
    float m_Sensitivity = 1.0f;
    float m_Speed = 0.0125f;

    bool UpdatePose()
    {
        bool hasPoseChanged = false;

        if (Input.GetKey(m_ControllerActivationKeycode))
        {
            hasPoseChanged = true;

            // Update rotation.
            m_AngleX -= Input.GetAxis("Mouse Y") * m_Sensitivity;
            m_AngleY += Input.GetAxis("Mouse X") * m_Sensitivity;
            m_LocalRotation.eulerAngles = new Vector3(m_AngleX, m_AngleY, 0.0f);

            // Update position.
            Vector3 localForward = CameraCache.Main.transform.InverseTransformDirection(CameraCache.Main.transform.forward);
            Vector3 localRight = CameraCache.Main.transform.InverseTransformDirection(CameraCache.Main.transform.right);
            if (Input.GetKey(KeyCode.W))
            {
                m_LocalPosition += localForward * m_Speed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                m_LocalPosition -= localForward * m_Speed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                m_LocalPosition -= localRight * m_Speed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                m_LocalPosition += localRight * m_Speed;
            }

            m_Pose.Position = CameraCache.Main.transform.TransformPoint(m_LocalPosition);
            m_Pose.Rotation = CameraCache.Main.transform.rotation * m_LocalRotation;
        }
        else if (CameraCache.Main.transform.hasChanged)
        {
            hasPoseChanged = true;

            m_Pose.Position = CameraCache.Main.transform.TransformPoint(m_LocalPosition);
            m_Pose.Rotation = CameraCache.Main.transform.rotation * m_LocalRotation;
        }

        return hasPoseChanged;
    }
}

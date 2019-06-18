using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

public class SimulatedHeadset
{
    public SimulatedHeadset()
    {
        m_MrtkCameraTransform = CameraCache.Main.transform;
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.N)
            || Input.GetKey(KeyCode.M))
        {
            return;
        }

        UpdatePose();

        if (m_MrtkCameraTransform.localPosition != m_Pose.Position
            || m_MrtkCameraTransform.localRotation != m_Pose.Rotation)
        {
            m_MrtkCameraTransform.hasChanged = true;

            m_MrtkCameraTransform.localPosition = m_Pose.Position;
            m_MrtkCameraTransform.localRotation = m_Pose.Rotation;
        }
        else
        {
            m_MrtkCameraTransform.hasChanged = false;
        }
    }



    MixedRealityPose m_Pose = MixedRealityPose.ZeroIdentity;
    float m_AngleY = 0.0f;
    float m_AngleX = 0.0f;
    float m_Sensitivity = 1.0f;
    float m_Speed = 0.01f;
    Transform m_MrtkCameraTransform = null;

    void UpdatePose()
    {
        // Update rotation.
        m_AngleX -= Input.GetAxis("Mouse Y") * m_Sensitivity;
        m_AngleY += Input.GetAxis("Mouse X") * m_Sensitivity;
        Quaternion rotation = new Quaternion()
        {
            eulerAngles = new Vector3(m_AngleX, m_AngleY, 0.0f),
        };
        m_Pose.Rotation = rotation;

        // Update position.
        Vector3 translation = new Vector3(0.0f, 0.0f, 0.0f);

        if (Input.GetKey(KeyCode.W))
        {
            translation += m_Pose.Forward * m_Speed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            translation -= m_Pose.Forward * m_Speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            translation -= m_Pose.Right * m_Speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            translation += m_Pose.Right * m_Speed;
        }

        m_Pose.Position += translation;
    }
}

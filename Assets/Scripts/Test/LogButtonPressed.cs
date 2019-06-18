using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogButtonPressed : MonoBehaviour
{
    public void SayHello()
    {
        Debug.Log("Hello!");
    }



    [SerializeField]
    Microsoft.MixedReality.Toolkit.UI.Interactable m_Button = null;

    void Start()
    {
        m_Button.OnClick.AddListener(SayHello);
    }
}

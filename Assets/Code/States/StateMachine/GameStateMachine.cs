using SoftLiu.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameStateMachine : StateMachine, ISerializationCallbackReceiver
{

    private bool m_allowBackButton = false;
    public bool allowBackButton
    {
        get { return m_allowBackButton; }
        set
        {
            m_allowBackButton = value;
        }
    }

    private int m_backButtonBufferFrames = 10;
    private int m_backButtonFrameCtr = 0;

    public override void Update()
    {
        base.Update();
        UpdateBackButtonCheck();
    }

    void UpdateBackButtonCheck()
    {
        m_backButtonFrameCtr++;
        m_backButtonFrameCtr = Mathf.Min(m_backButtonFrameCtr, m_backButtonBufferFrames + 5); // so the int doesn't spiral out of control and wrap backwards

        //We disabled this for TVOS problems we had after UpdateManager came in
#if !UNITY_TVOS
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isStateTransitioning && m_allowBackButton)
            {
                if (m_backButtonFrameCtr > m_backButtonBufferFrames)
                {
                    State current = this.GetCurrentState();
                    //Debug.Log("BACKBUTTON ON STATE: " + current.name);
                    current.OnHardwareBackButton();

                    m_backButtonFrameCtr = 0;
                }
            }
        }
#endif
    }
    #region ISerializationCallbackReceiver implementation
    public void OnAfterDeserialize()
    {
        
    }

    public void OnBeforeSerialize()
    {
        
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupStateTransitioner : StateTransitioner
{
    [SerializeField]
    protected GameObject m_parentObj;

    [SerializeField]
    private GameObject[] m_gameObjectArgs = null;

    public override void SwapState()
    {
        if (!CanTransition())
            return;

        if (m_parentObj == null)
        {
            m_parentObj = PopupState.GetParentForPopup();
        }

        if (m_gameObjectArgs != null && m_gameObjectArgs.Length > 0)
        {
            GameStateMachine.Instance.SwapState(m_stateName, m_exitBehaviour, null, m_parentObj, m_gameObjectArgs);
        }
        else
        {
            GameStateMachine.Instance.SwapState(m_stateName, m_exitBehaviour, null, m_parentObj);
        }
    }

    public override void PushState()
    {
        if (!CanTransition())
            return;

        if (m_parentObj == null)
        {
            m_parentObj = PopupState.GetParentForPopup();
        }

        if (m_gameObjectArgs != null && m_gameObjectArgs.Length > 0)
        {
            GameStateMachine.Instance.Push(m_stateName, m_exitBehaviour, null, m_parentObj, m_gameObjectArgs);
        }
        else
        {
            GameStateMachine.Instance.Push(m_stateName, m_exitBehaviour, null, m_parentObj);
        }
    }
}

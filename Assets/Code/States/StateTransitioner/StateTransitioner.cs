using SoftLiu.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransitioner : MonoBehaviour
{
    [SerializeField]
    protected string m_stateName;

    [SerializeField]
    protected State.ExitBehaviour m_exitBehaviour;

    // Properties
    public string stateName { get { return m_stateName; } }

    public virtual void SwapState()
    {
        GameStateMachine.Instance.SwapState(m_stateName, m_exitBehaviour, null);
    }

    public virtual void PushState()
    {
        GameStateMachine.Instance.Push(m_stateName, m_exitBehaviour, null);
    }

    public virtual void PopState()
    {
        GameStateMachine.Instance.Pop(m_exitBehaviour);
    }

    public bool CanTransition()
    {
        return true;
    }
}

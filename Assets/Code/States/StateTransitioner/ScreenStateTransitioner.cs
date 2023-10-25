using SoftLiu.States;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScreenStateTransitioner : StateTransitioner
{
    [SerializeField]
    protected bool m_preload;

    [SerializeField]
    protected ScreenState.EntryBehaviour m_entryBehaviour;

    protected IngameStateJumpInfo m_jumpInfo = null;

    public void SetCustomJumpinfo(IngameStateJumpInfo jumpInfo)
    {
        m_jumpInfo = jumpInfo;
    }

    protected List<object> m_params = new List<object>();

    protected virtual void Awake()
    {
        m_params.Add(m_entryBehaviour);
    }

    public bool PreLoad { get { return m_preload; } }

    override public void PushState()
    {
        if (!CanTransition())
        {
            return;
        }
        object[] parameters = m_params.ToArray();

        PushStateSafe(m_stateName, m_exitBehaviour, parameters);
    }

    override public void PopState()
    {
        if (!CanTransition())
        {
            return;
        }
        object[] parameters = m_params.ToArray();

        PopStateSafe(m_exitBehaviour, parameters);
    }

    public override void SwapState()
    {
        if (!CanTransition())
        {
            return;
        }
        object[] parameters = m_params.ToArray();

        SwapStateSafe(m_stateName, m_exitBehaviour, m_jumpInfo, parameters);
    }

    #region Static methods to check for cyclic loops and perform the state operations
    public static void SwapStateSafe(string stateName, State.ExitBehaviour exitBehaviour, IngameStateJumpInfo jumpInfo, params object[] args)
    {
        // check for cyclic loops... if we're pushing a state that already exists on the stack,put a debug message on screen 
        if (GameStateMachine.Instance.DoesStateExistOnStack(stateName))
        {
            // enforce no cyclic loops
            GameStateMachine.Instance.PopUntilState(stateName, exitBehaviour, args);
        }
        else
        {
            GameStateMachine.Instance.SwapState(stateName, exitBehaviour, jumpInfo, args);
        }
    }

    public static void PushStateSafe(string stateName, State.ExitBehaviour exitBehaviour, params object[] args)
    {
        // check for cyclic loops... if we're pushing a state that already exists on the stack,put a debug message on screen 
        if (GameStateMachine.Instance.DoesStateExistOnStack(stateName))
        {
            // enforce no cyclic loops
            GameStateMachine.Instance.PopUntilState(stateName, exitBehaviour, args);
        }
        else
        {
            GameStateMachine.Instance.Push(stateName, exitBehaviour, null, args);
        }
    }

    public static void PopStateSafe(State.ExitBehaviour exitBehaviour, params object[] args)
    {
        // check for going back to states that don't exist (due to being able to play from any scene)
        if (GameStateMachine.Instance.GetNumStates() > 1)
        {
            GameStateMachine.Instance.Pop(exitBehaviour, args);
        }
        else
        {
#if UNITY_EDITOR
            // EditorUtility.DisplayDialog("CAN'T GO BACK!", "The state you want to go to doesn't exist on the stack!", "OK!");
#endif
        }
    }
    #endregion
}

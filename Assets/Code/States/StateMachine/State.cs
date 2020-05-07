using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.States
{
    public class State
    {
        public enum ExitBehaviour
        {
            None = 0,
            Disable = 1,
            DisableInput = 2,
            Destroy = 3
        }

        #region Settings

        // State machine
        private StateMachine m_stateMachine = null;
        public StateMachine stateMachine { get { return m_stateMachine; } }

        // Name
        private string m_name = "";
        public string name { get { return m_name; } }
        public int nameHash { get; private set; }
        protected GameObject m_currentButton = null;

        private float m_timeInState = 0.0f;
        private float m_totalTimeInState = 0.0f;
        #endregion

        // Register to a state machine
        public void Register(StateMachine sm, string name, params object[] args)
        {
            m_name = name;
            nameHash = m_name.GetHashCode();
            m_stateMachine = sm;
            OnCreate(args);
        }

        #region Interface
        // Called when the state has been registered
        public virtual void OnCreate(params object[] args)
        {
            //Debug.Log( m_name + ":OnCreate" );
        }

        // Called when the state is entered into
        public virtual void OnEnter(State previousState, IngameStateJumpInfo inGameStateJumpInfo, params object[] args)
        {
            //Debug.Log( m_name + ":OnEnter" );
            if (m_currentButton != null && m_currentButton.activeSelf && previousState is PopupState)
            {

            }
            m_totalTimeInState += m_timeInState;
            m_timeInState = 0.0f;
        }
        // Called when the state is left
        public virtual void OnExit(State nextState, ExitBehaviour exitBehaviour)
        {
            //Debug.Log( m_name + ":OnExit" );
        }

        // Called when the state is popped
        public virtual void OnResume(State previousState, params object[] args)
        {
            //Debug.Log( m_name + ":OnResume" );
        }
        // Called when the state is pushed out
        public virtual void OnPause(State nextState, ExitBehaviour exitBehaviour)
        {
            //Debug.Log( m_name + ":OnPause" );
        }

        // Updates
        public virtual void OnUpdate()
        {
            m_timeInState += Time.deltaTime;
            //Debug.Log( m_name + ":OnUpdate" );
        }

        public virtual void OnHardwareBackButton()
        {
            //Debug.Log(m_name + ":OnHardwareBackButton");
        }

        public virtual void OnFixedUpdate()
        {
            //Debug.Log( m_name + ":OnFixedUpdate" );
        }
        public virtual void OnLateUpdate()
        {
            //Debug.Log( m_name + ":OnLateUpdate" );
        }

        // Called when the application is pushed out
        public virtual void OnApplicationPause(bool paused)
        {
            //Debug.Log( m_name + ":OnApplicationPause" );
        }

        protected virtual void OnStateLoaded()
        {
           
        }

        #endregion

        public float GetTimeInState()
        {
            return m_timeInState;
        }
        public float GetTotalTimeInState()
        {
            return m_totalTimeInState + m_timeInState;
        }
    }
}

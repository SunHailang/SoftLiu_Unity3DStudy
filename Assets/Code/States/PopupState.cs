using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftLiu.States;
using SoftLiu.Event;

public class PopupState : State
{
    // Inherited objects
    protected string m_popupPrefabName;
    protected GameObject m_popupInstance;

    // Fields?
    private GameObject m_parent;

    // Properties
    public GameObject parent
    {
        get { return m_parent; }
    }

    public GameObject popupInstance
    {
        get
        {
            return m_popupInstance;
        }
    }

    #region FGOL.State
    // Called when the state has been registered
    public override void OnCreate(params object[] args)
    {
        base.OnCreate(args);

        // PopupState will interpret the argument as popup prefab
        SoftLiu.Assert.Warn(args.Length > 0, this.name + " needs an argument of prefab when being added to the list of states!");
        if (args.Length > 0)
        {
            m_popupPrefabName = args[0] as string;
        }
    }

    // Called when the state is entered into
    public override void OnEnter(State previousState, IngameStateJumpInfo inGameStateJumpInfo, params object[] args)
    {
        base.OnEnter(previousState, inGameStateJumpInfo, args);

        // when entering a popup state, all we need to know is:
        // (a) which prefab (already know)
        // (b) who is the parent (passed in here)
        SoftLiu.Assert.Warn(args.Length > 0, "In order to push a popup, we need to specify 1 argument - the parent game object");
        if (args.Length > 0)
        {
            m_parent = args[0] as GameObject;

            GameObject popupPrefab = Resources.Load<GameObject>(m_popupPrefabName);
            if (popupPrefab == null)
            {
                Debug.LogError("PopupState::OnEnter :: Attempt to load popup failed. Popup prefab is null: " + m_popupPrefabName);
            }
            if (m_parent == null)
            {
                Debug.LogError("PopupState::OnEnter :: Attempt to load popup failed. Parent is null.");
            }
            m_popupInstance = GameObject.Instantiate<GameObject>(popupPrefab, m_parent.transform);
            m_popupInstance.transform.localScale = Vector3.one;
            m_popupInstance.transform.localPosition = Vector3.zero;
            m_popupInstance.transform.localRotation = Quaternion.identity;

            Util.SetUiInteractablesEnabled(m_popupInstance, true);

            // allow the back button
            (GameStateMachine.Instance as GameStateMachine).allowBackButton = true;
        }

        OnStateLoaded();
    }

    public override void OnHardwareBackButton()
    {
        //base.OnHardwareBackButton();

        // for popups, we just do a popState() and pray that that's enough
        GameObject[] backBtns = GameObject.FindGameObjectsWithTag("BackButton");
        for (int i = 0; i < backBtns.Length; i++)
        {
            GameObject backBtn = backBtns[i];
            if (backBtn.activeInHierarchy)
            {
                // see if its parent is our popup instance
                Transform result = null;
                Transform thing = backBtn.transform;
                while (thing.parent != null)
                {
                    if (thing.gameObject == m_popupInstance)
                    {
                        result = thing;
                        break;
                    }

                    thing = thing.parent;
                }

                if (result != null)
                {
                    // this is the back button hierarchy we're interested in
                    // check if it's collider is enabled
                    BoxCollider coll = backBtn.GetComponent<BoxCollider>();
                    if (coll != null && coll.enabled)
                    {
                        backBtn.SendMessage("OnClick");
                    }
                    return;
                }
            }
        }
    }

    // Called when the state is left
    public override void OnExit(State nextState, State.ExitBehaviour exitBehaviour)
    {
        base.OnExit(nextState, exitBehaviour);
        ApplyExitBehaviour(exitBehaviour);
        EventManager<Events>.Instance.TriggerEvent(Events.StateExited, name);
    }

    // Called when the state is popped
    public override void OnResume(State previousState, params object[] args)
    {
        base.OnResume(previousState);

        if (m_popupInstance != null)
        {
            m_popupInstance.SetActive(true);
        }
        else
        {
            GameObject popupPrefab = Resources.Load<GameObject>(m_popupPrefabName);
            m_popupInstance = GameObject.Instantiate<GameObject>(popupPrefab, m_parent.transform);
            m_popupInstance.transform.localScale = Vector3.one;
            m_popupInstance.transform.localPosition = Vector3.zero;
            m_popupInstance.transform.localRotation = Quaternion.identity;
        }

        Util.SetUiInteractablesEnabled(m_popupInstance, true);

        // disallow the back button
        (GameStateMachine.Instance as GameStateMachine).allowBackButton = true;
    }

    // Called when the state is pushed out
    public override void OnPause(State nextState, State.ExitBehaviour exitBehaviour)
    {
        base.OnPause(nextState, exitBehaviour);

        SoftLiu.Assert.Fatal(!(nextState is ScreenState), "Cannot push a screen over a popup!");

        ApplyExitBehaviour(exitBehaviour);
    }

    protected override void OnStateLoaded()
    {
        base.OnStateLoaded();
        //We can't do this because most of the pop ups rely on this not happening
        EventManager<Events>.Instance.TriggerEvent(Events.PopupStateLoaded, name);
    }

    // Updates
    public override void OnUpdate() { }
    public override void OnFixedUpdate() { }
    public override void OnLateUpdate() { }

    #endregion

    void ApplyExitBehaviour(State.ExitBehaviour exitBehaviour)
    {
        if (m_popupInstance != null)
        {
            switch (exitBehaviour)
            {
                case State.ExitBehaviour.Destroy:
                    {
                        GameObject.Destroy(m_popupInstance);
                        m_popupInstance = null;
                    }
                    break;
                case State.ExitBehaviour.Disable:
                    {
                        m_popupInstance.SetActive(false);
                    }
                    break;
                case State.ExitBehaviour.DisableInput:
                    {
                        Util.SetUiInteractablesEnabled(m_popupInstance, false);
                    }
                    break;

                default:
                case State.ExitBehaviour.None:
                    {
                    }
                    break;
            }
        }
    }

    public static GameObject GetParentForPopup()
    {
        GameObject parent = null;

        //ScreenState oldScreen = GameStateMachine.Instance.GetLastState<ScreenState>();
        //if (oldScreen != null)
        //{
        //    oldScreen.FindScreenRootIfNull();

        //    SoftLiu.Assert.Warn(oldScreen != null, "Could not find a screenState on the stack to push the popup on!");
        //    if (oldScreen != null && oldScreen.screenRoot != null)
        //    {
        //        ScreenRoot scrRoot = oldScreen.screenRoot.GetComponent<ScreenRoot>();
        //        SoftLiu.Assert.Warn(scrRoot != null, "Could not find a screen root on the stack to push the popup on!");

        //        if (scrRoot != null)
        //        {
        //            parent = scrRoot.uiRoot.gameObject;
        //        }
        //    }
        //}
        //else
        //{
        //    SoftLiu.Assert.Fatal(false, "oldScreen was null on GetParentPopup");
        //}
        return parent;
    }
}

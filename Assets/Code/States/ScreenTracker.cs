using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using SoftLiu.States;
using UnityEngine.SceneManagement;

public partial class ScreenTracker
{

    // Helper class to abstract away from AsyncOperation so can switch easily between sync/async
    class LoadTracker
    {
        public float Progress;
        public bool IsDone;
    }

    private string m_name;
    private string m_previousStateName;

    // Level-loading trackers
    private LoadTracker m_loadTracker;
    public bool m_wasLevelLoaded = false;

    // Screen root (if it exists)
    public GameObject m_screenRoot;

    // Exit/Entry behaviour
    public Collider[] m_interactables = null;

    Stopwatch m_stopwatch = new Stopwatch();

    public bool IsAudioScene
    {
        get;
        private set;
    }

    public ScreenTracker(string name)
    {
        m_name = name;

        IsAudioScene = name.StartsWith("Audio_");
    }

    public bool Loaded
    {
        get { return m_wasLevelLoaded; }
    }

    public string previousStateName { get { return m_previousStateName; } }

    public string Name
    {
        get { return m_name; }
    }

    public void OnUpdate()
    {
        if (!m_wasLevelLoaded)
        {
            bool wasLevelLoadedThisFrame = (m_loadTracker != null && m_loadTracker.IsDone);
            if (wasLevelLoadedThisFrame)
            {
                m_screenRoot = GameObject.Find(m_name);
                OnScreenLoaded();
            }
        }
    }

    public void OnScreenLoaded()
    {
        m_wasLevelLoaded = true;
        if (m_screenRoot != null)
        {
            
        }
    }

    public void OnEntry(ScreenState.EntryBehaviour entryBehaviour, string previousStateName)
    {
        m_previousStateName = previousStateName;

        // the level has been loaded already.
        if (m_loadTracker != null && m_loadTracker.IsDone)
        {
            Debug.Log("Not loading scene " + m_name + " as it's already loaded.");
            OnScreenLoaded();
            return;
        }

        if (m_screenRoot == null)
        {
            // no screen root in the hierarchy or registry
            // we may be returning to a disabled scene without ScreenRoot (like a spawner scene or FX scene, etc.,), check the level tracker status instead
            if ((m_loadTracker == null) || Mathf.Approximately(m_loadTracker.Progress, 0.0f))
            {
                // scene needs to be loaded.. this is where the entryBehaviour argument comes in
                m_wasLevelLoaded = false;
                m_stopwatch.Reset();
                m_stopwatch.Start();

                switch (entryBehaviour)
                {
                    case ScreenState.EntryBehaviour.Load:
                        LoadScene(LoadSceneMode.Single);
                        break;
                    case ScreenState.EntryBehaviour.LoadAdditive:
                        LoadScene(LoadSceneMode.Additive);
                        break;
                    default:
                        Debug.LogError("Undefined entry behaviour: " + entryBehaviour);
                        break;
                }
            }
        }
        else
        {
            // the screen root already exists in the hierarchy or registry, so finish up here
            OnScreenLoaded();
        }
    }

    private void LoadScene(LoadSceneMode mode)
    {
        m_loadTracker = new LoadTracker();

        SceneManager.LoadScene(m_name, mode);

        Util.StartCoroutineWithoutMonobehaviour("UpdateLoadTracker", UpdateLoadTracker(m_loadTracker));
    }

    IEnumerator UpdateLoadTracker(LoadTracker loadTracker)
    {
        Scene scene = SceneManager.GetSceneByName(m_name);

        // Even when using non async scene load, still takes one frame to actually load...
        while (!scene.isLoaded)
        {
            yield return null;
        }

        loadTracker.Progress = 1f;
        loadTracker.IsDone = true;
    }

    public void OnScreenDestroyed()
    {
        m_loadTracker = null;
    }


    public void OnExit(State nextState, State.ExitBehaviour exitBehaviour)
    {
        // set the level loader to be null if we're destroying the current state
        if (exitBehaviour == State.ExitBehaviour.Destroy)
        {
            m_loadTracker = null;
        }

        FindScreenRootIfNull();
        if (m_screenRoot != null)
        {
            GameObject objToDisable = m_screenRoot;

            // special case - if we're going to a popup, don't destroy/disable the whole thing, just the panel,
            // in case we ever need the 'Disable' ExitBehaviour... most cases we won't need this, as we'll be covering the base screen with an alpha layer etc.,
            bool nextStateIsPopup = (nextState is PopupState);


            switch (exitBehaviour)
            {
                case State.ExitBehaviour.Destroy:
                    {
                        if (nextStateIsPopup)
                        {
                            GameObject.Destroy(objToDisable);
                        }
                        else
                        {
                            if (m_name == "SaveLoader")
                            {
                                GameObject.Destroy(objToDisable);
                            }
#if UNITY_5_3_OR_NEWER
                            else if (SceneManager.GetActiveScene().name != m_name)
                            {
                                SceneManager.UnloadSceneAsync(m_name);
                            }
#endif
                        }
                    }
                    break;
                case State.ExitBehaviour.Disable:
                    {
                        objToDisable.SetActive(false);
                    }
                    break;
                case State.ExitBehaviour.DisableInput:
                    {
                        Util.SetUiInteractablesEnabled(objToDisable, false);
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

    public void FindScreenRootIfNull()
    {
        if (m_screenRoot == null)
        {
            m_screenRoot = GameObject.Find(m_name);
        }
    }
}

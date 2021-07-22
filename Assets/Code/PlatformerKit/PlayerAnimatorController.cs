using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField]
    private PlayerNavMeshAgmentController m_agmentController;

    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_agmentController.onAnimatorIdleEvent += OnIdleEvent;
        m_agmentController.onAnimatorRunningEvent += OnRunningEvent;
    }

    private void OnIdleEvent()
    {
        m_animator.SetFloat("FloatSpeed", -1.0f);
    }

    private void OnRunningEvent()
    {
        m_animator.SetFloat("FloatSpeed", 1.0f);
    }

    private void OnDestroy()
    {
        m_agmentController.onAnimatorIdleEvent -= OnIdleEvent;
        m_agmentController.onAnimatorRunningEvent -= OnRunningEvent;
    }
}

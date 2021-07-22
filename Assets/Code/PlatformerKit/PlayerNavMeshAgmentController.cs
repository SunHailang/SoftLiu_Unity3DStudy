using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMeshAgmentController : MonoBehaviour
{
    private NavMeshAgent m_agent;

    //private CharacterController m_character;

    public event System.Action onAnimatorIdleEvent;
    public event System.Action onAnimatorRunningEvent;

    private float m_moveSpeed = 0.0f;
    private Vector3 m_moveDir;

    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        //m_character = GetComponent<CharacterController>();

        m_moveSpeed = 3.0f;
        m_moveDir = Vector3.forward;
    }

    private void Start()
    {
        PlayerManager.Instance.onNavMeshAgmentSpeedEvent += OnNavMeshAgmentSpeedEvent;
        PlayerManager.Instance.onNavMeshAgmentDestinationEvent += OnNavMeshAgmentDestinationEvent;
    }

    private void Update()
    {
        bool running = m_agent.remainingDistance > m_agent.stoppingDistance;
        //bool running = m_moveSpeed > 0;
        if (!running)
            onAnimatorIdleEvent?.Invoke();
        else
            onAnimatorRunningEvent?.Invoke();
    }


    private void OnNavMeshAgmentSpeedEvent(float speedOffset)
    {
        //m_moveSpeed += speedOffset;
        m_agent.speed += speedOffset;
        if (m_agent.speed < 0.0f) m_agent.speed = 0.0f;
    }

    private void OnNavMeshAgmentDestinationEvent(Vector3 destination)
    {
        m_agent.destination = destination;
    }

    private void FixedUpdate()
    {
        //UnityEngine.Debug.Log($"m_moveDir:{m_moveDir.normalized}");
        //m_character.Move(m_moveDir.normalized * m_moveSpeed * Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.onNavMeshAgmentSpeedEvent -= OnNavMeshAgmentSpeedEvent;
        PlayerManager.Instance.onNavMeshAgmentDestinationEvent -= OnNavMeshAgmentDestinationEvent;
    }
}

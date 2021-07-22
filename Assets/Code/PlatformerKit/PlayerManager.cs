using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField]
    private PlayerAnimatorController m_playerAnimatorController = null;


    public event System.EventHandler onEventHandler;

    public event System.Action<float> onNavMeshAgmentSpeedEvent;
    public event System.Action<Vector3> onNavMeshAgmentDestinationEvent;

    private RaycastHit m_rayHitInfo;

    private void Awake()
    {
        if (Instance != null) DestroyImmediate(Instance);
        Instance = this;
    }

    private void Update()
    {
        //AnimatorStateInfo info = m_playerAnimator.GetCurrentAnimatorStateInfo(0);
        //if (info.normalizedTime >= 1.0f)
        //{
        //    //if (info.IsName("jump")) m_playerAnimator.SetBool("BoolToJump", false);
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    m_Moving = !m_Moving;
        //    m_playerAnimator.SetFloat("FloatSpeed", m_Moving ? 1.0f : -1.0f);
        //}

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out m_rayHitInfo))
            {
                string tag = m_rayHitInfo.collider.tag;
                switch (tag)
                {
                    case "Terrain":
                        onNavMeshAgmentDestinationEvent?.Invoke(m_rayHitInfo.point);
                        break;
                }
            }
        }
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    // forward
        //    onNavMeshAgmentDestinationEvent?.Invoke(Vector3.forward);
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    // back
        //    onNavMeshAgmentDestinationEvent?.Invoke(Vector3.back);
        //}
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    // left
        //    onNavMeshAgmentDestinationEvent?.Invoke(Vector3.left);
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    // right
        //    onNavMeshAgmentDestinationEvent?.Invoke(Vector3.right);
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            onNavMeshAgmentSpeedEvent?.Invoke(-0.5f);
        }
    }

    static float m_triggerTime = 0.0f;
    private void OnTriggerEnter(Collider other)
    {
        m_triggerTime = 0.0f;
    }

    private void OnTriggerStay(Collider other)
    {
        m_triggerTime += Time.fixedDeltaTime;

        UnityEngine.Debug.Log($"Trigger Stay Time:: {m_triggerTime}");
    }

}

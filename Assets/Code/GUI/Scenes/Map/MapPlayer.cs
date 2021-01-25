using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    private List<string> hitTags = new List<string> { "Enemy" };

    public Transform m_spriteRender = null;
    [SerializeField]
    private PingPoint m_pingPoint = null;
    [Space]
    [SerializeField]
    [Range(0, 300)]
    private float m_rotationSpeed = 60f;
    [Space]
    [SerializeField]
    [Range(0, 5)]
    private float m_hitDistance = 2.695f;

    private List<Collider2D> m_alreadyPingList = new List<Collider2D>();

    void Update()
    {
        float preR = (m_spriteRender.eulerAngles.z % 360) - 180;
        m_spriteRender.eulerAngles -= new Vector3(0, 0, m_rotationSpeed * Time.deltaTime);
        float curR = (m_spriteRender.eulerAngles.z % 360) - 180;

        if (preR < 0 && curR >= 0)
        {
            m_alreadyPingList.Clear();
        }
        RaycastHit2D[] raycastHit2DArray = Physics2D.RaycastAll(m_spriteRender.position, GetVectorFromAngle(m_spriteRender.eulerAngles.z), m_hitDistance);
        foreach (RaycastHit2D raycastHit2D in raycastHit2DArray)
        {
            if (raycastHit2D.collider != null)
            {
                // Hit something
                if (!m_alreadyPingList.Contains(raycastHit2D.collider))
                {
                    m_alreadyPingList.Add(raycastHit2D.collider);
                    // TODO
                    Debug.Log("Hit Object: " + raycastHit2D.collider.name);
                    PingPoint ping = null;
                    switch (raycastHit2D.transform.tag)
                    {
                        case "Enemy":
                            ping = Instantiate(m_pingPoint, raycastHit2D.transform);
                            break;
                        case "":
                            break;
                        default:
                            break;
                    }
                    if (ping != null)
                    {
                        ping.transform.position = raycastHit2D.point;
                        ping.SetColor(Color.red);
                    }
                }
            }
        }
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
}

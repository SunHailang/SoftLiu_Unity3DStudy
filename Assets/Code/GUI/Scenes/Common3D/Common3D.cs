using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common3D : MonoBehaviour
{
    public SpriteRenderer m_sprite = null;
    [Range(0, 30)]
    public float m_maxRadius = 5;
    public float m_param = 10;
    public float m_radius = 0;

    private List<Collider2D> alreadyPingList;
    // Start is called before the first frame update
    void Start()
    {
        alreadyPingList = new List<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_radius >= m_maxRadius)
        {
            m_radius = 0;
            alreadyPingList.Clear();
        }
        m_sprite.transform.localScale = new Vector3(m_radius, m_radius);
        m_radius += Time.deltaTime;

        RaycastHit2D[] raycastHit2DArray = Physics2D.CircleCastAll(m_sprite.transform.position, m_radius / 2, Vector2.zero);
        foreach (RaycastHit2D raycastHit2D in raycastHit2DArray)
        {
            if (raycastHit2D.collider != null)
            {
                // Hit something
                if (!alreadyPingList.Contains(raycastHit2D.collider))
                {
                    alreadyPingList.Add(raycastHit2D.collider);
                    // TODO
                    Debug.Log("Hit Object: " + raycastHit2D.collider.name);
                    SoftLiu.SLDebug.TextPopup("Ping", raycastHit2D.transform, raycastHit2D.point, Vector3.one * 0.5f, Color.red);
                }
            }
        }

    }
}

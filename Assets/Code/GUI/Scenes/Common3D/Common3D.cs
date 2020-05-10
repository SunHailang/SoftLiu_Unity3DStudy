using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Common3D : MonoBehaviour
{
    public RectTransform m_sprite = null;
    [Range(0, 300)]
    public float m_maxRadius = 5;

    public float m_param = 10;
    public float m_radius = 0;

    private List<Collider2D> alreadyPingList;

    Vector3 m_baseVec = Vector3.one;

    // Start is called before the first frame update
    void Start()
    {
        alreadyPingList = new List<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!alreadyPingList.Contains(collision))
        {
            alreadyPingList.Add(collision);
            Debug.Log("Hit Object: " + collision.gameObject.name);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (m_radius >= m_maxRadius)
        {
            m_radius = 0;
            alreadyPingList.Clear();
        }

        m_sprite.localScale = Vector2.one * m_radius;
        m_radius += Time.deltaTime * 2;



        //RaycastHit2D[] raycastHit2DArray = Physics2D.CircleCastAll(m_sprite.transform.position, m_radius / 2, Vector2.zero);
        //foreach (RaycastHit2D raycastHit2D in raycastHit2DArray)
        //{
        //    if (raycastHit2D.collider != null)
        //    {
        //        // Hit something
        //        if (!alreadyPingList.Contains(raycastHit2D.collider))
        //        {
        //            alreadyPingList.Add(raycastHit2D.collider);
        //            // TODO
        //            Debug.Log("Hit Object: " + raycastHit2D.collider.name);
        //            //SoftLiu.SLDebug.TextPopup("Ping", raycastHit2D.transform, raycastHit2D.point, Vector3.one * 0.5f, Color.red);
        //        }
        //    }
        //}

    }
}

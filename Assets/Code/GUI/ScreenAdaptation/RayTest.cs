using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    public Camera m_camera;
    public GameObject m_rayTarget;

    private void Update()
    {
        Ray ray = m_camera.ScreenPointToRay(m_rayTarget.transform.position);

        UnityEngine.Debug.DrawLine(ray.origin, m_rayTarget.transform.position, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            UnityEngine.Debug.DrawLine(ray.origin, hit.point, Color.red);
        }
    }
}

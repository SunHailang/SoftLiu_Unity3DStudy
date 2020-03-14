using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_axis = Vector3.zero;

    private void Update()
    {
        if (!m_axis.Equals(Vector3.zero))
        {
            transform.Rotate(m_axis, m_axis.magnitude * 360 * Time.deltaTime);
        }
    }
}

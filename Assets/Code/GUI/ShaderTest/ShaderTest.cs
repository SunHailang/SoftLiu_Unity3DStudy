using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderTest : MonoBehaviour
{
    [SerializeField]
    private Image m_myFristSharderM = null;

    private Material m_myMater = null;
    float speed = 30;
    // Start is called before the first frame update
    void Start()
    {
        m_myMater = m_myFristSharderM.material;
    }

    // Update is called once per frame
    void Update()
    {
        float value = Mathf.PingPong(Time.time * speed, 50);
        m_myMater.SetFloat("_Int", value);
        Debug.Log("value = " + value);
    }
}

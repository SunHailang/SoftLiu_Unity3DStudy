using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaderTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D: " + collision.gameObject.name);
    }


    private void Update()
    {

    }
}

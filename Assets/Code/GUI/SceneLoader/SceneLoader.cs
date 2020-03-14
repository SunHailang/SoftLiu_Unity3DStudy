using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void BtnLoad_OnClick()
    {
        Scene s = SceneManager.GetSceneByName("Comm");
        Scene s1 = SceneManager.GetSceneByName("Common3D");
       
        Debug.Log("BtnLoad_OnClick");
    }

    // Update is called once per frame
    void Update()
    {

    }
}

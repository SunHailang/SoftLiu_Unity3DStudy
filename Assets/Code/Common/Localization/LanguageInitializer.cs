using SoftLiu.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageInitializer : MonoBehaviour
{
    public static LanguageInitializer Instance = null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    
}

using SoftLiu.SceneManagers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquaresGame : MonoBehaviour
{



    #region Button OnClick

    public void BtnBack_OnClick()
    {
        SceneManager.Instance.BackSceneAsync();
    }

    #endregion
}

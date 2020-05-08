using SoftLiu.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class IngameHDU : MonoBehaviour
{
    [SerializeField]
    private Slider m_playerBlood = null;
    [SerializeField]
    private Slider m_playerAnger = null;
    [SerializeField]
    private Button m_btnStartPause = null;
    [SerializeField]
    private GameObject m_btnStartImage = null;
    [SerializeField]
    private GameObject m_btnPauseImage = null;
    [SerializeField]
    private MovePlayer m_movePlayer = null;
    [SerializeField]
    private MoveCamera m_moveCamera = null;

    private Player m_playerController = null;

    private void Awake()
    {
        EventManager<Events>.Instance.RegisterEvent(Events.GameStart, OnGameStart);

        Player activePlayer = App.Instance.GetActivePlayer();
        SoftLiu.Assert.Fatal(activePlayer != null, "Active Player is null.");
        m_playerController = activePlayer;
        m_btnStartPause.interactable = false;
        m_btnPauseImage.SetActive(true);
        m_btnStartImage.SetActive(false);
    }

    private void Update()
    {
        if (m_playerController == null) return;
        // 得到鼠标当前位置
        //float mouseX = Input.GetAxis("Mouse X");
        //float mouseY = Input.GetAxis("Mouse Y") * m_rotationSpeed;
        // 设置照相机和Player的旋转角度，X,Y值需要更具情况变化位置
        //transform.localRotation = transform.localRotation * Quaternion.Euler(-mouseY, 0, 0);
        float mouseX = m_moveCamera.cameraMoveDirection.x;
        m_playerController.PlayerRotation(mouseX);
        m_moveCamera.cameraMoveDirection = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (m_playerController == null) return;
        float horizontal = Input.GetAxis("Horizontal"); //获取垂直轴
        float vertical = Input.GetAxis("Vertical");    //获取水平轴 
        Vector3 move_direction = new Vector3(horizontal, 0, vertical);
        m_playerController.PlayerMove(move_direction);
        //m_character.SimpleMove(move);
        //transform.Translate(Vector3.forward * vertical * m_locationSpeed * Time.deltaTime);//W S
        //transform.Translate(Vector3.right * horizontal * m_locationSpeed * Time.deltaTime);// A D
    }

    private void OnGameStart(Events eventType, object[] mParam)
    {
        App.Instance.InGame = true;
        App.Instance.Pause = false;
        m_btnStartPause.interactable = true;
    }


    public void BtnPause_OnClick()
    {
        App.Instance.Pause = true;
    }

    private void OnDestroy()
    {
        EventManager<Events>.Instance.DeregisterEvent(Events.GameStart, OnGameStart);
    }

}

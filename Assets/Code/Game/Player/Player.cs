using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private CharacterController m_character = null;

    public float m_locationSpeed = 20f;
    public float m_rotationSpeed = 2;

    Vector2 m_mousePosition;

    private void OnEnable()
    {

    }

    private void Update()
    {
        // 得到鼠标当前位置
        float mouseX = Input.GetAxis("Mouse X") * m_rotationSpeed;
        //float mouseY = Input.GetAxis("Mouse Y") * m_rotationSpeed;
        // 设置照相机和Player的旋转角度，X,Y值需要更具情况变化位置
        //transform.localRotation = transform.localRotation * Quaternion.Euler(-mouseY, 0, 0);
        transform.localRotation = transform.localRotation * Quaternion.Euler(0, mouseX, 0);
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal"); //获取垂直轴
        float vertical = Input.GetAxis("Vertical");    //获取水平轴 
        Vector3 move_direction = new Vector3(horizontal, 0, vertical);
        //用以自己为参考的目标点的世界坐标减去自己的世界坐标
        Vector3 move = transform.TransformPoint(move_direction) - transform.position;
        m_character.Move(move * m_locationSpeed * Time.fixedDeltaTime);
        //m_character.SimpleMove(move);
        //transform.Translate(Vector3.forward * vertical * m_locationSpeed * Time.deltaTime);//W S
        //transform.Translate(Vector3.right * horizontal * m_locationSpeed * Time.deltaTime);// A D
    }
}

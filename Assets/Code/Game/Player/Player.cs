using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private CharacterController m_character = null;
    [SerializeField]
    private Camera m_playerCamera = null;
    [SerializeField]
    private GameObject m_rayObject = null;

    public float m_locationSpeed = 7f;
    public float m_rotationSpeed = 1.5f;
    public float m_cameraMoveSpeed = 5f;

    Vector2 m_mousePosition;

    private float m_moveSpeed = 0;
    private Vector3 m_baseCameraVec = Vector3.zero;

    private void Awake()
    {
        App.Instance.SetActivePlayer(this);
    }

    private void OnEnable()
    {
        Vector3 pos = m_playerCamera.transform.localPosition;
        m_baseCameraVec = new Vector3(pos.x, pos.y, pos.z);
    }

    private void Start()
    {
        SceneManager.LoadScene("IngameHDU", LoadSceneMode.Additive);

        //Debug.Log(Vector3.Distance(m_playerCamera.transform.localPosition, m_rayObject.transform.localPosition));
    }

    public void PlayerMove(Vector3 moveDir)
    {
        //用以自己为参考的目标点的世界坐标减去自己的世界坐标        
        Vector3 move = transform.TransformPoint(moveDir) - transform.position;
        Vector3 speed = move * m_locationSpeed * Time.fixedDeltaTime;
        m_character.Move(speed);
        m_moveSpeed = Vector3.Distance(Vector3.zero, speed);
        if (m_moveSpeed != 0) canMoveCamera = true;
    }

    public void PlayerRotation(float rotationDir)
    {
        transform.localRotation = transform.localRotation * Quaternion.Euler(0, rotationDir * m_rotationSpeed, 0);
        if (rotationDir != 0) canMoveCamera = true;
    }
    bool canMoveCamera = false;
    Queue<bool> hitQueue = new Queue<bool>();
    private void Update()
    {
        // 设置 Camera

        Vector3 dir = (m_rayObject.transform.position - m_playerCamera.transform.position).normalized;
        Ray ray = new Ray(m_playerCamera.transform.position, dir);

        bool isHit = false;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f))
        {
            UnityEngine.Debug.DrawLine(ray.origin, hit.point, Color.red);
            if (hit.transform.tag != "Player")
            {
                //Debug.Log(hit.transform.tag + " -> " + hit.transform.name);
                m_playerCamera.transform.position = hit.point;
                isHit = true;
                canMoveCamera = false;
            }
            else
            {
                isHit = false;
            }
            hitQueue.Enqueue(isHit);
            //UnityEngine.Debug.DrawLine(ray.origin, hit.point, Color.red);
        }
        if (hitQueue.Count > 3)
        {
            hitQueue.Dequeue();
        }
        Queue<bool>.Enumerator curQu = hitQueue.GetEnumerator();
        bool reset = true;
        while (curQu.MoveNext())
        {
            if (curQu.Current)
            {
                reset = false;
                break;
            }
        }
        if (reset && canMoveCamera)
        {
            m_playerCamera.transform.localPosition = Vector3.Lerp(m_playerCamera.transform.localPosition, m_baseCameraVec, Time.deltaTime * m_cameraMoveSpeed);
        }
        else
        {
            
        }
        m_playerCamera.transform.LookAt(m_rayObject.transform);

        // 抖动
        if (m_moveSpeed > 0)
        {

        }
        else
        {

        }
        // 得到鼠标当前位置
        //float mouseX = Input.GetAxis("Mouse X") * m_rotationSpeed;
        //float mouseY = Input.GetAxis("Mouse Y") * m_rotationSpeed;
        // 设置照相机和Player的旋转角度，X,Y值需要更具情况变化位置
        //transform.localRotation = transform.localRotation * Quaternion.Euler(-mouseY, 0, 0);
        //transform.localRotation = transform.localRotation * Quaternion.Euler(0, mouseX, 0);
    }

    private void FixedUpdate()
    {
        //float horizontal = Input.GetAxis("Horizontal"); //获取垂直轴
        //float vertical = Input.GetAxis("Vertical");    //获取水平轴 
        //Vector3 move_direction = new Vector3(horizontal, 0, vertical);
        //用以自己为参考的目标点的世界坐标减去自己的世界坐标
        //Vector3 move = transform.TransformPoint(move_direction) - transform.position;
        //m_character.Move(move * m_locationSpeed * Time.fixedDeltaTime);
        //m_character.SimpleMove(move);
        //transform.Translate(Vector3.forward * vertical * m_locationSpeed * Time.deltaTime);//W S
        //transform.Translate(Vector3.right * horizontal * m_locationSpeed * Time.deltaTime);// A D
    }
}

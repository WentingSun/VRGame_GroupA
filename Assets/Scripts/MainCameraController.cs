using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraControllor : MonoBehaviour
{
     public float moveSpeed = 5f;
    public float lookSpeed = 2f;

    float rotationX = 0f;
    float rotationY = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 移动（WSAD / QE）
        float moveX = Input.GetAxis("Horizontal"); // A / D
        float moveZ = Input.GetAxis("Vertical");   // W / S
        float moveY = 0f;

        if (Input.GetKey(KeyCode.E)) moveY = 1f;
        if (Input.GetKey(KeyCode.Q)) moveY = -1f;

        Vector3 move = new Vector3(moveX, moveY, moveZ);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.Self);

        // 鼠标右键按住时旋转视角
        if (Input.GetMouseButton(1)) // 鼠标右键
        {
            rotationX += Input.GetAxis("Mouse X") * lookSpeed;
            rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f); // 防止过头

            transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }
    }
}

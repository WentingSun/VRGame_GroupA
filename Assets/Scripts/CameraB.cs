using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraB : MonoBehaviour
{

    [SerializeField] private Camera mainCamera;

    public float scaleRatio = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera != null)
        {
            transform.position = mainCamera.transform.position * 10;
            transform.LookAt(Vector3.zero);
        }
    }
}

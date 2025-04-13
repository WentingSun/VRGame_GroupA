using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateCameraBPosition()
    {
        transform.position = mainCamera.transform.position * 10;
    }
}

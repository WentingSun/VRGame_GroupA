using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveFootballPlane : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target; // 通常是 MainCamera
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target);
        }
        
    }
}

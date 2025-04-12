using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBall : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector3 velocity;
    [SerializeField] float velocityMagnitude;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = rb.velocity;
        velocityMagnitude = velocity.magnitude;
    }


    public void applyForce()
    {
        rb.AddForce(Vector3.down*3f,ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("SmallBall Enter");
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("CollisionStay");
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("SmallBall Exit");
        Vector3 direction = rb.velocity.normalized;
        
    }

}

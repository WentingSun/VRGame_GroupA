using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBall : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float StayTime;
    [SerializeField] Transform targetPosition;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // velocity = rb.velocity;
        // velocityMagnitude = velocity.magnitude;
    }


    public void applyForce()
    {
        rb.AddForce(Vector3.down * 3f, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("SmallBall Enter");
    }

    void OnCollisionStay(Collision collision)
    {
        // Debug.Log("CollisionStay");
        StayTime += Time.deltaTime;
        if (StayTime > 0.1f)
        {
            rb.velocity = Vector3.zero;
            if(targetPosition != null){
            rb.AddForce((targetPosition.position-transform.position).normalized * 3, ForceMode.VelocityChange);
            }else{
            rb.AddForce((-transform.position).normalized * 3, ForceMode.VelocityChange);
            }
            
        }
        if (rb.velocity.magnitude < 5f)
        {
            Vector3 vector3= rb.velocity;
            rb.velocity = Vector3.zero;
            rb.AddForce(vector3.normalized * 2, ForceMode.VelocityChange);
        }

    }

    void OnCollisionExit(Collision collision)
    {
        StayTime = 0;
        // Debug.Log("SmallBall Exit");
        Vector3 direction = rb.velocity.normalized;

    }

}

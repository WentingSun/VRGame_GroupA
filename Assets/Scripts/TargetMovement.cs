using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    [SerializeField] Transform centrePoint;
    [SerializeField] float radius = 1f;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float pauseTime = 1f;

    private Vector3 nextTargetPosition;

    void Start()
    {
        if (!centrePoint)
        {
            centrePoint = GameManager.Instance.transform;
        }
        PickNewTargetPosition();
    }

    void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, nextTargetPosition, moveSpeed * Time.deltaTime);


        if (Vector3.Distance(transform.position, nextTargetPosition) < 0.1f)
        {
            StartCoroutine(PauseThenPickNew());
        }
    }

    IEnumerator PauseThenPickNew()
    {

        yield return new WaitForSeconds(pauseTime);
        PickNewTargetPosition();
    }

    void PickNewTargetPosition()
    {

        Vector2 randomCircle = Random.insideUnitCircle * radius;
        nextTargetPosition = centrePoint.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    void OnDrawGizmosSelected()
    {
        if (centrePoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(centrePoint.position, radius);
        }
    }
}

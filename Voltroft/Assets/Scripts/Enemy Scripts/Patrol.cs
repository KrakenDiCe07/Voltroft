using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] private WayPoints waypoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float distanceThreshold = 0.1f;

    private Transform currentWayPoint;
    private Transform previousWayPoint;

    private void Start()
    {
        SetNextWaypoint();
    }

    private void Update()
    {
        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        if (currentWayPoint == null) return;

        transform.position = Vector3.MoveTowards(transform.position, currentWayPoint.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, currentWayPoint.position) < distanceThreshold)
        {
            SetNextWaypoint();
        }
    }

    private void SetNextWaypoint()
    {
        Transform nextWayPoint = waypoints.GetNextValidWaypoint(currentWayPoint, previousWayPoint);

        if (nextWayPoint != null && nextWayPoint != currentWayPoint)
        {
            previousWayPoint = currentWayPoint;
            currentWayPoint = nextWayPoint;

            // Adjust rotation towards the next waypoint
            Vector3 direction = (currentWayPoint.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
        else if (nextWayPoint == previousWayPoint)
        {
            // Reverse direction if the only available waypoint is the previous one
            currentWayPoint = previousWayPoint;
            previousWayPoint = null;
            Vector3 direction = (currentWayPoint.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
        else
        {
            // If no valid waypoint found, stop moving
            moveSpeed = 0f;
        }
    }
}
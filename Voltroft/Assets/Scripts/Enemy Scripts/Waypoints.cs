using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // Draw spheres at each waypoint position
        foreach (Transform t in transform)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(t.position, 1f);
        }

        // Draw lines connecting all waypoints
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentWaypoint = transform.GetChild(i);
            for (int j = 0; j < transform.childCount; j++)
            {
                if (i != j)
                {
                    Transform targetWaypoint = transform.GetChild(j);
                    if (IsPathBlocked(currentWaypoint.position, targetWaypoint.position))
                    {
                        Gizmos.color = Color.red; // Path is blocked by a wall
                    }
                    else
                    {
                        Gizmos.color = Color.green; // Path is clear
                    }
                    Gizmos.DrawLine(currentWaypoint.position, targetWaypoint.position);
                }
            }
        }
    }

    public Transform GetNextValidWaypoint(Transform currentWaypoint, Transform previousWaypoint)
    {
        if (currentWaypoint == null)
        {
            return transform.GetChild(0);
        }

        int currentIndex = currentWaypoint.GetSiblingIndex();

        // List to store available waypoints
        List<Transform> availableWaypoints = new List<Transform>();

        // Check all waypoints
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i != currentIndex)
            {
                Transform waypoint = transform.GetChild(i);
                if (!IsPathBlocked(currentWaypoint.position, waypoint.position) && waypoint != previousWaypoint)
                {
                    availableWaypoints.Add(waypoint);
                }
            }
        }

        // Choose a random waypoint from the available waypoints
        if (availableWaypoints.Count > 0)
        {
            return availableWaypoints[Random.Range(0, availableWaypoints.Count)];
        }
        else
        {
            // If no available waypoints found, return to the previous waypoint
            return previousWaypoint;
        }
    }

    // Check if the gizmo line between two waypoints is blocked by a game object with a "Wall" tag
    private bool IsPathBlocked(Vector3 startPoint, Vector3 endPoint)
    {
        RaycastHit2D hit = Physics2D.Linecast(startPoint, endPoint, LayerMask.GetMask("Wall"));
        return hit.collider != null;
    }
}
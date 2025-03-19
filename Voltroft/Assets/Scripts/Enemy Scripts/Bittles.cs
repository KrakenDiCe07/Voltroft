using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class Bittles : MonoBehaviour
{
    public EnemyBaseState currentState;
    
    public PatrolState patrolState;
    public PlayerDetectedState playerDetectedState;
    public Rigidbody2D rb;
    public Transform ledgeDetector;
    public LayerMask groundLayer, obstacleLayer, playerLayer;

    public float raycastDistance, obstacleDistance, playerDetectDistance;
    public float speed;
    public float detectionPauseTime;

    public GameObject alert;

    private bool facingRight = true;
    private bool playerDetected;

    private void Awake()
    {
        patrolState = new PatrolState(this, "patrol");
        playerDetectedState = new PlayerDetectedState(this, "playerDetected");

        currentState = patrolState;
        currentState.Enter();
    }
    private void Update()
    {
        currentState.LogicUpdate();

        CheckForObstacles();
        CheckForPlayer();
    }
    void FixedUpdate()
    {
        currentState.PhysicsUpdate();

        if (!playerDetected)
        {
            if (facingRight)
                rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
            else
               rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
        }
    }
    void CheckForObstacles()
    {
        RaycastHit2D hit = Physics2D.Raycast(ledgeDetector.position, UnityEngine.Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D hitObstacle = Physics2D.Raycast(ledgeDetector.position, Vector2.right, obstacleDistance, obstacleLayer);

        if (hit.collider == null || hitObstacle.collider == true)
            Rotate();
    }
    void CheckForPlayer()
    {
        RaycastHit2D hitPlayer = Physics2D.Raycast(ledgeDetector.position, facingRight ? Vector2.right : Vector2.left, playerDetectDistance, playerLayer);

        if (hitPlayer.collider == true)
            StartCoroutine(PlayerDetected());
        else if (playerDetected)
            StartCoroutine(PlayerNOTDetected());
    }
    IEnumerator PlayerDetected()
    {
        playerDetected = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1);
    }
    IEnumerator PlayerNOTDetected()
    {
        yield return new WaitForSeconds(1);
        playerDetected = false;
    }
    void Rotate()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(ledgeDetector.position, (facingRight ? Vector2.right : Vector2.left) * playerDetectDistance);
    }
}
/*{
    [SerializeField] private WayPoints waypoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float distanceThreshold = 0.1f;

    private Transform currentWayPoint;
    private Transform previousWayPoint;
    public GameObject playerDetector;
    public bool playerDetected = false;

    private void Start()
    {
        SetNextWaypoint();
    }

    private void Update()
    {
        if (playerDetected == false)
        {
            MoveToWaypoint();
        }
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
            /*Vector3 direction = (currentWayPoint.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);*/
        /*}
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

    public void CommenceBattle()
    {

    }
} */
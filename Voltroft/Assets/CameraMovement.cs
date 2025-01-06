using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player; // Reference to the player GameObject
    public float transitionSpeed = 1f; // Speed of camera transition between screens
    public float borderX = 5f; // Distance from center to border in the x direction
    public float borderY = 5f; // Distance from center to border in the y direction
    public float transitionDistanceX = 10f; // Distance the camera moves in the x-direction when transitioning
    public float transitionDistanceY = 10f; // Distance the camera moves in the y-direction when transitioning
    public float forcedMoveSpeedMultiplier = 0.5f; // Multiplier to adjust the forced movement speed of the player

    private Vector3 targetPosition; // Target position for camera transition
    private bool transitioning = false; // Flag to track if camera is currently transitioning

    private GameObject[] enemies; // Array to hold references to enemy GameObjects
    private GameObject[] hazards; // Array to hold references to hazard GameObjects
    private PlayerMovement playerMovementScript;

    void Start()
    {
        // Find all enemies and hazards in the scene
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        hazards = GameObject.FindGameObjectsWithTag("Hazard");
        playerMovementScript = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!transitioning)
        {
            if (player.position.x < transform.position.x - borderX) // Player touches left border
            {
                TransitionToAdjacentScreen(Vector3.left * transitionDistanceX, 0);
            }
            else if (player.position.x > transform.position.x + borderX) // Player touches right border
            {
                TransitionToAdjacentScreen(Vector3.right * transitionDistanceX, 0);
            }
            else if (player.position.y < transform.position.y - borderY) // Player touches bottom border
            {
                TransitionToAdjacentScreen(Vector3.down * transitionDistanceY, 1);
            }
            else if (player.position.y > transform.position.y + borderY) // Player touches top border
            {
                TransitionToAdjacentScreen(Vector3.up * transitionDistanceY, 1);
            }
        }
    }

    void TransitionToAdjacentScreen(Vector3 direction, int axis)
    {
        Vector3 newPosition = transform.position + direction;
        StartCoroutine(TransitionCamera(newPosition, direction, axis));
    }

    IEnumerator TransitionCamera(Vector3 targetPosition, Vector3 direction, int axis)
    {
        transitioning = true;
        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float elapsedTime = 0f;

        // Disable enemies and hazards during the transition
        SetActiveEnemiesAndHazards(false);

        // Adjust the player's forced movement speed
        float forcedMoveSpeed = journeyLength / transitionSpeed * forcedMoveSpeedMultiplier;
        playerMovementScript.ForceMove(direction, forcedMoveSpeed);

        while (elapsedTime < transitionSpeed)
        {
            float fractionOfJourney = elapsedTime / transitionSpeed;
            Vector3 newPosition = startPosition + (targetPosition - startPosition) * fractionOfJourney;

            // Only update the specific axis position
            if (axis == 0)
            {
                newPosition.x = Mathf.Lerp(startPosition.x, targetPosition.x, fractionOfJourney);
            }
            else if (axis == 1)
            {
                newPosition.y = Mathf.Lerp(startPosition.y, targetPosition.y, fractionOfJourney);
            }

            transform.position = newPosition;

            // Increment time based on Time.deltaTime
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPosition;

        // Re-enable enemies and hazards after the transition
        SetActiveEnemiesAndHazards(true);

        playerMovementScript.StopForceMove();

        transitioning = false;
    }

    void SetActiveEnemiesAndHazards(bool isActive)
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(isActive);
        }

        foreach (GameObject hazard in hazards)
        {
            hazard.SetActive(isActive);
        }
    }
}

using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float initialSpeed = 2f;
    public float maxSpeed = 10f;
    public float accelerationTime = 0.5f; // Time in seconds to reach max speed
    public float maxDistance = 10f; // Distance before despawning
    
    private PlayerShit playerScript;
    private float currentSpeed;
    private float accelerationTimer;
    private Vector2 direction;
    private Vector2 startPosition;

    public void Start()
    {
        playerScript = GetComponent<PlayerShit>();
    }
    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        accelerationTimer = 0f;
        startPosition = transform.position;
       /* if(playerScript.shotCharged == true)
        {
            
        }*/
    }

    private void Update()
    {
        // Increase speed over time using Lerp
        if (accelerationTimer < accelerationTime)
        {
            accelerationTimer += Time.deltaTime;
            float t = accelerationTimer / accelerationTime;
            currentSpeed = Mathf.Lerp(initialSpeed, maxSpeed, t);
        }

        // Move the projectile
        transform.position += (Vector3)(direction * currentSpeed * Time.deltaTime);

        // Destroy the projectile if it exceeds max distance
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        if(other.gameObject.CompareTag("BreakableGround"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}

/*{
    public float initialSpeed = 2f;
    public float maxSpeed = 10f;
    public float accelerationTime = 0.5f; // Time in seconds to reach max speed
    public float maxDistance = 10f; // Distance before despawning

    private float currentSpeed;
    private float accelerationTimer;
    private Vector2 direction;
    private Vector2 startPosition;

    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        currentSpeed = initialSpeed;
        accelerationTimer = 0f;
        startPosition = transform.position;
    }

    private void Update()
    {
        // Increase speed over time using Lerp
        if (accelerationTimer < accelerationTime)
        {
            accelerationTimer += Time.deltaTime;
            float t = accelerationTimer / accelerationTime;
            currentSpeed = Mathf.Lerp(initialSpeed, maxSpeed, t);
        }

        // Move the projectile
        transform.position += (Vector3)(direction * currentSpeed * Time.deltaTime);

        // Destroy the projectile if it exceeds max distance
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}*/
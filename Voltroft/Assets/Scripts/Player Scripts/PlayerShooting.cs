using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint; // Set this to an empty GameObject where the projectile spawns
    public float projectileInitialSpeed = 2f;
    public float projectileAccelerationTime = 0.5f;
    public float projectileSpeedCap = 10f;
    public float projectileMaxDistance = 10f;
    public KeyCode fireKey = KeyCode.K; // Change this to your preferred fire key
    private PlayerShit playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerShit>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(fireKey))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (playerController == null) return;

        int facingDirection = playerController.GetFacingDirection();
        Vector2 shootDirection = facingDirection == 1 ? Vector2.right : Vector2.left;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        PlayerProjectile projectileScript = projectile.GetComponent<PlayerProjectile>();
    }
}
using System.Collections;
using UnityEngine;

public class Wranshee2D : MonoBehaviour
{
    public Transform player;
    public float circleRadius = 2f;
    public float spinSpeed = 200f;
    public float maxChargeSpeed = 10f;
    public float maxChargeDistance = 5f;
    public float burstDuration = 0.5f; // Fast charge phase
    public float slowdownDuration = 2f; // Slow charge phase
    public float transparencySpeed = 1f;

    private Vector2 lastPlayerPosition;
    private bool isActive = false;
    private float spinDuration;
    private float disappearTime = 2f;


    private Collider2D wransheeCollider;
    private SpriteRenderer wransheeRenderer;

    private void Start()
    {
        wransheeCollider = GetComponent<Collider2D>();
        wransheeRenderer = GetComponent<SpriteRenderer>();

        SetTransparency(0); // Start fully invisible
        wransheeCollider.enabled = false; // Disable collision initially
    }

    public void Initialize(Transform playerTransform)
    {
        if (isActive) return; // Prevent multiple instances
        isActive = true;

        player = playerTransform;
        spinDuration = Random.Range(1f, 3f);
        StartCoroutine(SpawnSequence());
    }

    private IEnumerator SpawnSequence()
    {

        yield return StartCoroutine(FadeTransparency(1)); // Fade in

        while (true)
        {
            yield return StartCoroutine(SpinAroundPlayer(spinDuration));
            yield return new WaitForSeconds(1f);

            lastPlayerPosition = player.position; // Capture player's position

            wransheeCollider.enabled = true; // Enable collision before charging
            yield return StartCoroutine(ChargeAtLastPosition());

            wransheeCollider.enabled = false; // Disable after charge
            yield return StartCoroutine(FadeTransparency(0));

            yield return new WaitForSeconds(disappearTime);


            yield return StartCoroutine(FadeTransparency(1));
            wransheeCollider.enabled = true; // Enable for the next cycle
        }
    }

    private IEnumerator SpinAroundPlayer(float duration)
    {
        float elapsedTime = 0f;
        float angle = 0f;
        Vector3 orbitCenter = player.position;
        float fixedZ = transform.position.z; // Keep Z constant

        while (elapsedTime < duration)
        {
            orbitCenter = player.position; // Update orbit center to follow player
            angle += 360 * Time.deltaTime;
            float rad = angle * Mathf.Deg2Rad;

            transform.position = new Vector3(
                orbitCenter.x + Mathf.Cos(rad) * circleRadius,
                orbitCenter.y + Mathf.Sin(rad) * circleRadius,
                fixedZ
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    private IEnumerator ChargeAtLastPosition()
    {
        Vector2 startPos = transform.position;
        Vector2 chargeDirection = (lastPlayerPosition - startPos).normalized;
        float elapsedTime = 0f;
        float currentSpeed = maxChargeSpeed;
        float totalChargeTime = burstDuration + slowdownDuration;
        float distanceTraveled = 0f;

        // Burst forward quickly
        while (elapsedTime < burstDuration && distanceTraveled < maxChargeDistance)
        {
            float step = maxChargeSpeed * Time.deltaTime;
            transform.position += (Vector3)(chargeDirection * step);
            distanceTraveled += step;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Slow down gradually
        elapsedTime = 0f;
        while (elapsedTime < slowdownDuration && distanceTraveled < maxChargeDistance)
        {
            float t = elapsedTime / slowdownDuration;
            currentSpeed = Mathf.Lerp(maxChargeSpeed, 0, t);
            float step = currentSpeed * Time.deltaTime;
            transform.position += (Vector3)(chargeDirection * step);
            distanceTraveled += step;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeTransparency(float targetAlpha)
    {
        float startAlpha = wransheeRenderer.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < transparencySpeed)
        {
            float t = elapsedTime / transparencySpeed;
            SetTransparency(Mathf.Lerp(startAlpha, targetAlpha, t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetTransparency(targetAlpha);
    }

    private void SetTransparency(float alpha)
    {
        Color color = wransheeRenderer.color;
        color.a = alpha;
        wransheeRenderer.color = color;
    }

    private void TeleportToRandomPosition()
    {
        Vector3 randomOffset = Random.insideUnitCircle.normalized * circleRadius;
        transform.position = new Vector3(player.position.x + randomOffset.x, player.position.y + randomOffset.y, player.position.z);
    }
}
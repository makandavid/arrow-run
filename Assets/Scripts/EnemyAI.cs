using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private int health = 8;

    [Header("Behaviour")]
    [SerializeField] private float fleeSpeed = 3.5f;
    [SerializeField] private Vector2 fleeTime = new Vector2(3f, 5f);
    [SerializeField] private float windup = 1.5f;
    [SerializeField] private float pauseAfterVolley = 0.5f;

    [Header("Strafe")]
    [SerializeField] private float strafeAmplitude = 3f;
    [SerializeField] private float strafeFrequency = 0.15f;

    [Header("Volley")]
    [SerializeField] private int arrowsPerVolley = 11;
    [SerializeField] private float volleyWidth = 10f;
    [SerializeField] private float maxSpreadAngle = 30f;
    [SerializeField] private float aimHeight = 1f;
    [SerializeField] private float volleyFlightTime = 1.6f;

    private Renderer rend;
    private Color baseColor;

    private float strafeCenter;
    private float strafeTime;
    private Vector3 lastPlayerPosition;
    private Vector3 playerVelocity;

    public int Health => health;

    private void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        baseColor = rend.material.color;

        strafeCenter = transform.position.x;
        lastPlayerPosition = player.position;

        StartCoroutine(Loop());
    }

    private void Update()
    {
        if (GameManager.Instance.IsOver) return;

        // The volley is always aimed at the player
        Vector3 delta = player.position - lastPlayerPosition;
        lastPlayerPosition = player.position;
        if (delta.sqrMagnitude < 1f)
        {
            delta.y = 0f;
            playerVelocity = Vector3.Lerp(playerVelocity, delta / Time.deltaTime, 0.2f);
        }

        // Sweep left-right, the forward flee movement stays in the coroutine.
        strafeTime += Time.deltaTime;
        float x = strafeCenter + Mathf.Sin(strafeTime * strafeFrequency * 2f * Mathf.PI) * strafeAmplitude;
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    private IEnumerator Loop()
    {
        while (!GameManager.Instance.IsOver)
        {
            float t = Random.Range(fleeTime.x, fleeTime.y);
            while (t > 0f && !GameManager.Instance.IsOver)
            {
                t -= Time.deltaTime;
                transform.position += Vector3.forward * fleeSpeed * Time.deltaTime;
                yield return null;
            }

            rend.material.color = Color.red;
            yield return new WaitForSeconds(windup);
            FireVolley();
            rend.material.color = baseColor;
            yield return new WaitForSeconds(pauseAfterVolley);
        }
    }

    private void FireVolley()
    {
        Vector3 origin = transform.position + Vector3.up * 1.2f;

        float gap = transform.position.z - player.position.z;

        float freeLead = playerVelocity.z * volleyFlightTime;
        float chaseLead = fleeSpeed * Mathf.Max(0f, volleyFlightTime - pauseAfterVolley);
        float lead = Mathf.Clamp(Mathf.Max(freeLead, chaseLead), 0f, Mathf.Max(0f, gap));
        float targetZ = player.position.z + lead;

        float distance = Mathf.Abs(targetZ - origin.z);
        float width = Mathf.Min(volleyWidth, 2f * distance * Mathf.Tan(maxSpreadAngle * Mathf.Deg2Rad));

        for (int i = 0; i < arrowsPerVolley; i++)
        {
            float t = arrowsPerVolley > 1 ? i / (float)(arrowsPerVolley - 1) : 0.5f;
            float x = Mathf.Lerp(-width * 0.5f, width * 0.5f, t);

            Vector3 aim = new Vector3(x, aimHeight, targetZ);
            Vector3 velocity = BallisticVelocity(origin, aim);

            var a = Instantiate(arrowPrefab, origin, Quaternion.LookRotation(velocity));
            a.Launch(velocity, false);
        }
    }

    private Vector3 BallisticVelocity(Vector3 from, Vector3 to)
    {
        float t = volleyFlightTime;
        float g = -Physics.gravity.y;
        Vector3 delta = to - from;

        Vector3 horizontal = new Vector3(delta.x, 0f, delta.z) / t;
        float vertical = delta.y / t + 0.5f * g * t;
        return horizontal + Vector3.up * vertical;
    }

    public void TakeHit(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            GameManager.Instance.EndGame(true);
            gameObject.SetActive(false);
        }
    }
}

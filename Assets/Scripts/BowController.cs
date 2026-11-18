using UnityEngine;
using UnityEngine.InputSystem;

public class BowController : MonoBehaviour
{
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerHealth health;

    [Header("Shooting")]
    [SerializeField] private float minSpeed = 20f;
    [SerializeField] private float maxSpeed = 150f;
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float shootCooldown = 0.8f;

    private bool charging;
    private float chargeStart, nextShot, tripleUntil;

    public void GrantTripleShot(float seconds) { tripleUntil = Time.time + seconds; }

    private void Update()
    {
        if (GameManager.Instance.IsOver) return;
        var p = Pointer.current;
        if (p == null) return;

        if (p.press.wasPressedThisFrame)
        {
            charging = true; chargeStart = Time.time; 
        }

        if (p.press.wasReleasedThisFrame)
        {
            if (charging) Shoot(Mathf.Clamp01((Time.time - chargeStart) / maxChargeTime));
            charging = false;
        }
    }

    private void Shoot(float charge)
    {
        if (Time.time < nextShot) return;
        nextShot = Time.time + shootCooldown;

        float speed = Mathf.Lerp(minSpeed, maxSpeed, charge);
        Vector3 origin = transform.position + transform.forward * 0.6f - transform.up * 0.15f;

        Fire(origin, transform.forward, speed);
        if (Time.time < tripleUntil)
        {
            Fire(origin, Quaternion.AngleAxis(-2f, transform.up) * transform.forward, speed);
            Fire(origin, Quaternion.AngleAxis(2f, transform.up) * transform.forward, speed);
        }
    }

    private void Fire(Vector3 origin, Vector3 dir, float speed)
    {
        var a = Instantiate(arrowPrefab, origin, Quaternion.LookRotation(dir));
        a.Launch(dir * speed, true);
    }
}
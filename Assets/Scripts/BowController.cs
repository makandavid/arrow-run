using UnityEngine;
using UnityEngine.InputSystem;

public class BowController : MonoBehaviour
{
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private PlayerHealth health;

    [Header("Shooting")]
    [SerializeField] private float minSpeed = 20f;
    [SerializeField] private float maxSpeed = 150f;
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float shootCooldown = 0.8f;

    [Header("Teleport")]
    [SerializeField] private float lookDownThreshold = -0.3f;  // camera forward.y below this counts as "looking at the ground"
    [SerializeField] private float teleportDistance = 6f;
    [SerializeField] private float teleportCooldown = 5f;
    [SerializeField] private float teleportInvulnerability = 1.5f;
    [SerializeField] private float trackStartZ = 0f;

    private Transform player;
    private bool charging;
    private float chargeStart, nextShot, tripleUntil, nextTeleport;

    public void GrantTripleShot(float seconds) { tripleUntil = Time.time + seconds; }

    private void Awake()
    {
        player = transform.root;
    }

    private void Update()
    {
        if (GameManager.Instance.IsOver) return;
        var p = Pointer.current;
        if (p == null) return;

        if (p.press.wasPressedThisFrame)
        {
            if (transform.forward.y < lookDownThreshold) TryTeleport();
            else { charging = true; chargeStart = Time.time; }
        }

        if (p.press.wasReleasedThisFrame)
        {
            if (charging) Shoot(Mathf.Clamp01((Time.time - chargeStart) / maxChargeTime));
            charging = false;
        }
    }

    private void TryTeleport()
    {
        if (Time.time < nextTeleport) return;
        nextTeleport = Time.time + teleportCooldown;

        Vector3 pos = player.position;
        pos.z = Mathf.Max(trackStartZ, pos.z - teleportDistance);
        player.position = pos;

        health.GrantInvulnerability(teleportInvulnerability);
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
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private bool fromPlayer;
    private Rigidbody rb;

    private void Awake() { rb = GetComponent<Rigidbody>(); }

    public void Launch(Vector3 velocity, bool player)
    {
        fromPlayer = player;
        rb.linearVelocity = velocity;
        transform.forward = velocity;
        Destroy(gameObject, 6f);
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Arrow>() != null) return;

        if (fromPlayer)
        {
            var enemy = other.GetComponentInParent<EnemyAI>();
            if (enemy != null) { enemy.TakeHit(damage); Destroy(gameObject); return; }

            var pickup = other.GetComponentInParent<Pickup>();
            if (pickup != null) { pickup.Collect(); Destroy(gameObject); return; }

            if (other.GetComponentInParent<PlayerHealth>() != null) return;
        }
        else
        {
            var player = other.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                if (player.Invulnerable) return;   // flies straight through while teleport-dodging (not implemented yet)
                player.Hit();
                Destroy(gameObject);
                return;
            }
            if (other.GetComponentInParent<EnemyAI>() != null) return;
        }
        Destroy(gameObject); // ground, walls, anything else
    }
}
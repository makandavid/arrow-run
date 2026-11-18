using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private float duration = 8f;

    public void Collect()
    {
        FindAnyObjectByType<BowController>().GrantTripleShot(duration);
        Destroy(gameObject);
    }
}
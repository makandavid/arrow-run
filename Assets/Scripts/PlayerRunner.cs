using UnityEngine;

public class PlayerRunner : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform enemy;
    [SerializeField] private float minGap = 15f;

    private void Update()
    {
        if (GameManager.Instance.IsOver) return;
        if (enemy != null && enemy.position.z - transform.position.z < minGap) return;
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }
}
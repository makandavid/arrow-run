using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int lives = 3;
    [SerializeField] private float hitGraceTime = 1f;

    private float invulnUntil;

    public int Lives => lives;
    public bool Invulnerable => Time.time < invulnUntil;

    public void GrantInvulnerability(float seconds)
    {
        invulnUntil = Mathf.Max(invulnUntil, Time.time + seconds);
    }

    public void Hit()
    {
        if (Invulnerable || GameManager.Instance.IsOver) return;
        lives--;
        GrantInvulnerability(hitGraceTime);
        if (lives <= 0) GameManager.Instance.EndGame(false);
    }
}
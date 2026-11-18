using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TMP_Text hud;
    [SerializeField] private PlayerHealth player;
    [SerializeField] private EnemyAI enemy;

    public bool IsOver { get; private set; }
    private float endTime;

    private void Awake() { Instance = this; }

    private void Update()
    {
        if (IsOver)
        {
            var p = Pointer.current;
            if (Time.time - endTime > 1.5f && p != null && p.press.wasPressedThisFrame)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }
        hud.text = $"Lives: {player.Lives}   Enemy HP: {enemy.Health}";
    }

    public void EndGame(bool won)
    {
        if (IsOver) return;
        IsOver = true;
        endTime = Time.time;
        hud.text = (won ? "YOU WIN" : "GAME OVER") + "\nTap to restart";
    }
}
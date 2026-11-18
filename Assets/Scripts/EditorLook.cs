using UnityEngine;
using UnityEngine.InputSystem;

// Editor-only testing: hold right mouse button and drag to look around. Put on the Main Camera.
public class EditorLook : MonoBehaviour
{
#if UNITY_EDITOR
    private float yaw, pitch;

    private void Update()
    {
        var m = Mouse.current;
        if (m == null || !m.rightButton.isPressed) return;
        Vector2 d = m.delta.ReadValue() * 0.1f;
        yaw += d.x;
        pitch = Mathf.Clamp(pitch - d.y, -80f, 80f);
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }
#endif
}
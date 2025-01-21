using UnityEngine;

public class MouseController : MonoBehaviour
{
    void Start()
    {
        // マウスを中央に固定し、非表示に設定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Escapeキーでマウスのロックを解除する機能（デバッグ用）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 再度マウスを中央固定する場合（オプション）
        if (Input.GetMouseButtonDown(0)) // 左クリックで再ロック
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
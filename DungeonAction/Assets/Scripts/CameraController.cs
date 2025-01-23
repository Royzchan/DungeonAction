using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GameObject _player; // プレイヤー（ターゲット）となるオブジェクト
    private PlayerController _playerController;
    private GameManager _gm;
    public float _distance = 5.0f; // ターゲットからの距離
    public float _rotationSpeed = 100.0f; // 回転速度
    public Vector2 _pitchLimits = new Vector2(-30, 60); // 上下の角度制限
    public Vector3 _playerOffset = new Vector3(0, 1.5f, 0); // プレイヤー位置調整用のオフセット

    private float _currentYaw = 0.0f; // 現在の水平角度
    private float _currentPitch = 0.0f; // 現在の垂直角度

    [SerializeField]
    private InputAction _cameraRotateAction;

    private void OnEnable()
    {
        _cameraRotateAction?.Enable();
    }

    private void OnDisable()
    {
        _cameraRotateAction?.Disable();
    }

    void Start()
    {
        if (_player == null)
        {
            Debug.LogError("Target is not assigned! Please assign a target for the camera.");
        }
        _playerController = _player.GetComponent<PlayerController>();
        _gm = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (_player == null) return;
        if (!_playerController.Alive) return;
        if (!_gm.GamePlaying) return;

        // マウス入力から角度を計算
        float mouseX = _cameraRotateAction.ReadValue<Vector2>().x * _rotationSpeed * Time.deltaTime;
        float mouseY = _cameraRotateAction.ReadValue<Vector2>().y * _rotationSpeed * Time.deltaTime;

        _currentYaw += mouseX;
        _currentPitch -= mouseY;

        // 垂直角度に制限をかける
        _currentPitch = Mathf.Clamp(_currentPitch, _pitchLimits.x, _pitchLimits.y);

        // カメラの位置と回転を更新
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // プレイヤーの中心位置にオフセットを追加
        Vector3 targetPosition = _player.transform.position + _playerOffset;

        // カメラの理想的な位置を計算
        Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0);
        Vector3 desiredOffset = rotation * new Vector3(0, 0, -_distance);
        Vector3 desiredCameraPosition = targetPosition + desiredOffset;

        // Raycastを使用してカメラとターゲット間の障害物を検出
        RaycastHit hit;
        if (Physics.Raycast(targetPosition, desiredOffset.normalized, out hit, _distance))
        {
            // 障害物がある場合、カメラを障害物の手前に配置
            transform.position = hit.point - desiredOffset.normalized * 0.1f; // 少し手前にずらす
        }
        else
        {
            // 障害物がない場合、理想的な位置に配置
            transform.position = desiredCameraPosition;
        }

        // カメラがターゲットを向くように設定
        transform.LookAt(targetPosition);
    }
}

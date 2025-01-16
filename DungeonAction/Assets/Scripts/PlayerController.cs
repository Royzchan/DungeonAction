using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField]
    private Transform _cameraTransform;

    [SerializeField]
    private float _maxHP = 100f;
    private float _hp;
    [SerializeField]
    private float _maxStamina;
    private float _stamina;
    [SerializeField]
    private float _attackPower = 100f;
    [SerializeField]
    private float _defenses = 100f;
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private InputAction _moveAction;

    private Vector2 _moveDirection = Vector2.zero;

    private void OnEnable()
    {
        _moveAction?.Enable();
    }

    private void OnDisable()
    {
        _moveAction?.Disable();
    }



    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _hp = _maxHP;
        _stamina = _maxStamina;
    }

    void Update()
    {
        _moveDirection = _moveAction.ReadValue<Vector2>();
        Move();
    }

    //移動
    private void Move()
    {
        if (_cameraTransform != null)
        {
            // カメラの前方と右方向を取得
            Vector3 cameraForward = _cameraTransform.forward;
            Vector3 cameraRight = _cameraTransform.right;

            // Y軸方向を無視（水平移動のみ）
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // 移動方向を計算
            Vector3 move = cameraRight * _moveDirection.x + cameraForward * _moveDirection.y;

            // Rigidbodyの速度を設定
            _rb.velocity = move * _speed + new Vector3(0f, _rb.velocity.y, 0f);

            // プレイヤーが進行方向を向く
            if (move.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            }
        }
    }

    //攻撃
    private void Attack()
    {

    }

    //スキル
    private void Skill()
    {

    }

    //必殺技
    private void Special()
    {

    }
}

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
    [SerializeField]
    private InputAction _attackAction;
    [SerializeField]
    private InputAction _skillAction;
    [SerializeField]
    private InputAction _specialAction;

    private Vector2 _moveDirection = Vector2.zero;

    private void OnEnable()
    {
        //移動の入力を有効化
        _moveAction?.Enable();

        //攻撃のアクションのコールバックを追加
        _attackAction.started += OnAttack;
        //攻撃アクションの入力を有効化
        _attackAction?.Enable();

        //スキルのアクションのコールバックを追加
        _skillAction.started += OnSkill;
        //スキルのアクションの入力を有効化
        _skillAction?.Enable();

        //必殺技のアクションのコールバックを追加
        _specialAction.started += OnSpecial;
        //必殺技の入力を有効化
        _specialAction?.Enable();
    }

    private void OnDisable()
    {
        //移動の入力を無効化
        _moveAction?.Disable();

        //攻撃のアクションのコールバックを解除
        _attackAction.started -= OnAttack;
        //攻撃の入力を無効化
        _attackAction?.Disable();

        //スキルのアクションのコールバックを解除
        _skillAction.started -= OnSkill;
        //スキルの入力を無効化
        _skillAction?.Disable();

        //必殺技のアクションのコールバックを解除
        _specialAction.started -= OnSpecial;
        //必殺技の入力を無効化
        _specialAction?.Disable();
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
    }

    private void FixedUpdate()
    {
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
        Debug.Log("攻撃");
    }

    //スキル
    private void Skill()
    {
        Debug.Log("スキル");
    }

    //必殺技
    private void Special()
    {
        Debug.Log("必殺技");
    }

    //Input Systemのコールバック
    private void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("攻撃が押された");
    }

    private void OnSkill(InputAction.CallbackContext context)
    {
        Debug.Log("スキルが押された");
    }

    private void OnSpecial(InputAction.CallbackContext context)
    {
        Debug.Log("必殺技が押された");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private Animator _animator;
    private GameManager _gm;

    [SerializeField]
    private Transform _cameraTransform; // プレイヤーの動きに基づくカメラの向き

    [Header("プレイヤーのステータス")]
    [SerializeField]
    private float _maxHp = 100f; // 最大HP
    private float _hp;
    [SerializeField]
    private float _maxStamina; // 最大スタミナ
    private float _stamina;
    [SerializeField]
    private float _attackPower = 100f; // 攻撃力
    [SerializeField]
    private float _attackRange = 5f; // 攻撃範囲
    [SerializeField]
    private float _defenses = 100f; // 防御力
    [SerializeField]
    private float _speed = 5f; // 通常移動速度
    [SerializeField]
    private float _dashSpeed = 7.5f; // ダッシュ時の移動速度
    [SerializeField]
    private float _invincibilityDuration = 0.5f; // 無敵時間の長さ

    private bool _alive = true; // 生存状態フラグ
    private bool _isInvincible = false; // 無敵状態フラグ
    private bool _isAvoiding = false; // 回避中フラグ
    private Vector2 _avoidDirection = Vector2.zero; // 回避方向

    [Header("Input Actions")]
    [SerializeField]
    private InputAction _moveAction;
    [SerializeField]
    private InputAction _attackAction;
    [SerializeField]
    private InputAction _skillAction;
    [SerializeField]
    private InputAction _specialAction;
    [SerializeField]
    private InputAction _avoidAction;
    [SerializeField]
    private InputAction _dashAction;

    [Header("武器設定")]
    [SerializeField]
    private GameObject _sowrdObj; // プレイヤーの武器オブジェクト
    private Collider _sowrdCollider; // 武器のコライダー

    private HashSet<Collider> _hitEnemies = new HashSet<Collider>(); // 攻撃した敵の記録

    private Vector2 _moveDirection = Vector2.zero; // プレイヤーの移動方向

    private bool _attackNow = false; // 現在攻撃中か

    private string _moveSpeedStr = "MoveSpeed"; // アニメーション用の速度パラメータ
    private string _attackStr = "isAttack"; // 攻撃アニメーショントリガー
    private string _avoidStr = "isAvoid"; // 回避アニメーショントリガー
    private string _dieStr = "Die"; // 死亡アニメーショントリガー

    #region ゲッター
    public float MaxHP { get { return _maxHp; } }
    public float HP { get { return _hp; } }
    public float MaxStamina { get { return _maxStamina; } }
    public float Stamina { get { return _stamina; } }
    public bool Alive { get { return _alive; } }
    #endregion

    // スタミナ回復関連設定
    [SerializeField]
    private float _staminaRecoveryRate = 5f; // スタミナの回復速度
    [SerializeField]
    private float _staminaConsumptionRate = 10f; // ダッシュ中のスタミナ消費速度
    [SerializeField]
    private float _avoidConsumptionAmount = 50f; // 回避時のスタミナ消費量

    private void OnEnable()
    {
        // 入力を有効化
        _moveAction?.Enable();
        _attackAction.started += OnAttack;
        _attackAction?.Enable();
        _skillAction.started += OnSkill;
        _skillAction?.Enable();
        _specialAction.started += OnSpecial;
        _specialAction?.Enable();
        _avoidAction.started += OnAvoid;
        _avoidAction?.Enable();
        _dashAction?.Enable();
    }

    private void OnDisable()
    {
        // 入力を無効化
        _moveAction?.Disable();
        _attackAction.started -= OnAttack;
        _attackAction?.Disable();
        _skillAction.started -= OnSkill;
        _skillAction?.Disable();
        _specialAction.started -= OnSpecial;
        _specialAction?.Disable();
        _avoidAction.started -= OnAvoid;
        _avoidAction?.Disable();
        _dashAction?.Disable();
    }

    private void Awake()
    {
        // 初期化
        _hp = _maxHp;
        _stamina = _maxStamina;
    }

    private void Start()
    {
        // コンポーネントの取得
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _gm = FindAnyObjectByType<GameManager>();

        // 武器のコライダー設定
        if (_sowrdObj != null)
        {
            _sowrdCollider = _sowrdObj.GetComponent<Collider>();
            _sowrdCollider.enabled = false;
        }
    }

    private void Update()
    {
        // デバッグ用ダメージコマンド
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Damage(20);
        }

        if (!_alive || !_gm.GamePlaying)
        {
            _moveDirection = Vector2.zero;
            return; // 生存中でない場合、処理を中止
        }

        // 攻撃中は移動不可
        _moveDirection = _attackNow || _isAvoiding ? Vector2.zero : _moveAction.ReadValue<Vector2>();

        // スタミナ回復（ダッシュ中、回避中では回復しない）
        if (!_dashAction.IsPressed() && _stamina < _maxStamina && !_isAvoiding)
        {
            _stamina += _staminaRecoveryRate * Time.deltaTime;
            if (_stamina > _maxStamina) _stamina = _maxStamina;
        }
    }

    private void FixedUpdate()
    {
        // 移動処理
        Move();
    }

    private void Move()
    {
        if (_cameraTransform != null && !_isAvoiding)
        {
            _animator.SetFloat(_moveSpeedStr, GetCurrentMoveSpeed());
            // 通常の移動処理
            Vector3 cameraForward = _cameraTransform.forward;
            Vector3 cameraRight = _cameraTransform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 move = cameraRight * _moveDirection.x + cameraForward * _moveDirection.y;

            // ダッシュ時の速度変更
            if (_dashAction.IsPressed() && _stamina > 0)
            {
                _rb.velocity = move * _dashSpeed + new Vector3(0f, _rb.velocity.y, 0f);

                // スタミナ消費
                _stamina -= _staminaConsumptionRate * Time.deltaTime;
                if (_stamina < 0) _stamina = 0;
            }
            else
            {
                _rb.velocity = move * _speed + new Vector3(0f, _rb.velocity.y, 0f);
            }

            // 向きの変更
            if (move.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            }
        }
        else if (_isAvoiding)
        {
            // 回避中の移動処理
            Vector3 move = Vector3.zero;

            if (_avoidDirection.magnitude > 0)
            {
                // 方向キーが押されていた場合、その方向に回避
                Vector3 cameraForward = _cameraTransform.forward;
                Vector3 cameraRight = _cameraTransform.right;

                cameraForward.y = 0;
                cameraRight.y = 0;
                cameraForward.Normalize();
                cameraRight.Normalize();

                move = cameraRight * _avoidDirection.x + cameraForward * _avoidDirection.y;
            }
            else
            {
                // 方向キーが押されていない場合は、現在の向きで回避
                move = transform.forward;
            }

            // 一気にスタミナ消費して回避
            if (_stamina >= _avoidConsumptionAmount)
            {
                _rb.velocity = move * _dashSpeed + new Vector3(0f, _rb.velocity.y, 0f);
            }
            else
            {
                // スタミナが足りない場合は回避できない
                EndAvoid();
            }
        }
    }

    private void Attack()
    {
        if (!_attackNow)
        {
            _attackNow = true;

            // 最寄りの敵を向く
            Collider nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                Vector3 directionToEnemy = (nearestEnemy.transform.position - transform.position).normalized;
                directionToEnemy.y = 0;
                if (directionToEnemy.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
                    transform.rotation = targetRotation;
                }
            }

            _animator.SetTrigger(_attackStr);

            // 武器のコライダーを有効化
            if (_sowrdCollider != null)
            {
                _sowrdCollider.enabled = true;
                _hitEnemies.Clear();
            }
        }
    }

    private Collider FindNearestEnemy()
    {
        // 一定範囲内の敵を検索
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _attackRange);
        Collider nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider collider in hitColliders)
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestEnemy = collider;
                    nearestDistance = distance;
                }
            }
        }

        return nearestEnemy;
    }

    public void EndAttack()
    {
        // 攻撃終了時の処理
        _attackNow = false;
        if (_sowrdCollider != null)
        {
            _sowrdCollider.enabled = false;
        }
        _hitEnemies.Clear();
    }

    private void Skill()
    {
        Debug.Log("スキル発動");
    }

    private void Special()
    {
        Debug.Log("必殺技発動");
    }

    private void StartAvoid()
    {
        if (_attackNow)
        {
            _attackNow = false;
        }

        // スタミナが足りていない場合は回避できない
        if (_stamina < _avoidConsumptionAmount)
        {
            return;
        }

        _animator.SetTrigger(_avoidStr);

        // 方向キーの入力を確認
        _avoidDirection = _moveAction.ReadValue<Vector2>();

        if (!_isInvincible)
        {
            StartCoroutine(StartInvincibility());
        }
        _stamina -= _avoidConsumptionAmount;
        _isAvoiding = true;
    }

    public void EndAvoid()
    {
        // 回避アニメーションが終わるのを待つ
        _isAvoiding = false;
    }

    private IEnumerator StartInvincibility()
    {
        // 無敵時間の開始
        _isInvincible = true;
        yield return new WaitForSeconds(_invincibilityDuration);
        _isInvincible = false;
    }

    public void Damage(float attack)
    {
        if (_alive && !_isInvincible)
        {
            _hp -= attack;
            Debug.Log(_hp);

            if (_hp <= 0)
            {
                // 死亡処理
                _alive = false;
                _gm.GameOver();
                _animator.SetTrigger(_dieStr);
            }
        }
    }

    public float GetCurrentMoveSpeed()
    {
        // 現在の移動速度を計算
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        return horizontalVelocity.magnitude;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        Attack();
    }

    private void OnSkill(InputAction.CallbackContext context)
    {
        Skill();
    }

    private void OnSpecial(InputAction.CallbackContext context)
    {
        Special();
    }

    private void OnAvoid(InputAction.CallbackContext context)
    {
        StartAvoid();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 武器の攻撃判定
        bool hitAttack = _sowrdCollider.enabled && !_hitEnemies.Contains(other);
        if (hitAttack)
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Damage(_attackPower);
                _hitEnemies.Add(other);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 攻撃範囲の可視化
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}

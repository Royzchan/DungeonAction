using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private Animator _animator;

    [SerializeField]
    private Transform _cameraTransform;

    [SerializeField]
    private float _maxHp = 100f;
    private float _hp;
    [SerializeField]
    private float _maxStamina;
    private float _stamina;
    [SerializeField]
    private float _attackPower = 100f;
    [SerializeField]
    private float _attackRange = 5f;
    [SerializeField]
    private float _defenses = 100f;
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _dashSpeed = 7.5f;

    private bool _alive = true;

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

    [SerializeField]
    private GameObject _sowrdObj;
    private Collider _sowrdCollider;

    private HashSet<Collider> _hitEnemies = new HashSet<Collider>();

    private Vector2 _moveDirection = Vector2.zero;

    private bool _attackNow = false;
    private bool _avoidNow = false;

    private string _moveSpeedStr = "MoveSpeed";
    private string _attackStr = "isAttack";
    private string _avoidStr = "isAvoid";
    private string _dieStr = "Die";

    #region ゲッター
    public float MaxHP { get { return _maxHp; } }
    public float HP { get { return _hp; } }
    public float MaxStamina { get { return _maxStamina; } }
    public float Stamina { get { return _stamina; } }

    #endregion

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

        //回避のアクションのコールバックを追加
        _avoidAction.started += OnAvoid;
        //回避の入力を有効化
        _avoidAction?.Enable();

        //ダッシュの入力を有効化
        _dashAction?.Enable();
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

        //回避のアクションのコールバックを解除
        _avoidAction.started -= OnAvoid;
        //回避の入力を無効化
        _avoidAction?.Disable();

        //ダッシュの入力を無効化
        _dashAction?.Disable();
    }

    private void Awake()
    {
        _hp = _maxHp;
        _stamina = _maxStamina;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        if (_sowrdObj != null)
        {
            _sowrdCollider = _sowrdObj.GetComponent<Collider>();
            _sowrdCollider.enabled = false;
        }
    }

    void Update()
    {
        //デバックコマンド
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Damage(20);
        }

        if (!_alive) return;

        if (!_attackNow)
        {
            _moveDirection = _moveAction.ReadValue<Vector2>();
        }
        else
        {
            _moveDirection = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        Move();
        _animator.SetFloat(_moveSpeedStr, GetCurrentMoveSpeed());
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
            //ダッシュ中
            if (_dashAction.IsPressed())
            {
                _rb.velocity = move * _dashSpeed + new Vector3(0f, _rb.velocity.y, 0f);
            }
            //通常時
            else
            {
                _rb.velocity = move * _speed + new Vector3(0f, _rb.velocity.y, 0f);
            }

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
        if (!_attackNow)
        {
            _attackNow = true;

            // 敵を検出してターゲットの方向を向く
            Collider nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                Vector3 directionToEnemy = (nearestEnemy.transform.position - transform.position).normalized;
                directionToEnemy.y = 0; // Y方向の回転を無視
                if (directionToEnemy.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
                    transform.rotation = targetRotation;
                }
            }

            _animator.SetTrigger(_attackStr);

            if (_sowrdCollider != null)
            {
                _sowrdCollider.enabled = true;
                _hitEnemies.Clear();
            }
        }
    }

    private Collider FindNearestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _attackRange);
        Collider nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider collider in hitColliders)
        {
            // 敵かどうかを判定
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

        if (nearestEnemy != null)
        {
            Debug.Log("最寄の敵: " + nearestEnemy.name);  // 見つかった敵をログに表示
        }
        else
        {
            Debug.Log("範囲内に敵がいません");
        }

        return nearestEnemy;
    }

    public void EndAttack()
    {
        _attackNow = false;
        if (_sowrdCollider != null)
        {
            _sowrdCollider.enabled = false;
        }
        _hitEnemies.Clear();
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

    //回避
    private void StartAvoid()
    {
        if (_attackNow)
        {
            _attackNow = false;
        }
        _animator.SetTrigger(_avoidStr);
    }

    private void EndAvoid()
    {

    }

    //ダメージ
    public void Damage(float attack)
    {
        if (_alive)
        {
            _hp -= attack;

            if (_hp <= 0)
            {
                _alive = false;
                _animator.SetTrigger(_dieStr);
            }
        }
    }


    //現在の移動速度を取得する関数
    public float GetCurrentMoveSpeed()
    {
        // Rigidbodyの速度を水平面に投影したベクトルを計算
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        // ベクトルの大きさを計算し、移動速度とする
        return horizontalVelocity.magnitude;
    }

    //Input Systemのコールバック
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

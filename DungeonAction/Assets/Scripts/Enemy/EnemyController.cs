using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float _maxHp = 100f;
    private float _hp;
    [SerializeField]
    protected float _attackPower = 100f;
    [SerializeField]
    private float _defenses = 100f;
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _vigilanceRange = 10f;
    [SerializeField]
    private float _attackRange = 2f;
    [SerializeField]
    private float _attackCooldown = 1.5f;

    [SerializeField]
    private GameObject _hpBar;
    private Slider _hpSlider;

    private Transform _player;
    private bool _isChasing = false;
    private float _lastAttackTime = -Mathf.Infinity;
    private Rigidbody _rb;

    protected virtual void Start()
    {
        // Find the player by tag (assumes the player has a "Player" tag)
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody>();

        _hp = _maxHp;

        if (_rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
        }

        _hpSlider = _hpBar.GetComponentInChildren<Slider>();
        if (_hpSlider == null)
        {
            Debug.LogError("HP bar Slider component is missing in child objects!");
        }
        else
        {
            // 初期のSliderの値を設定
            _hpSlider.maxValue = _maxHp;
            _hpSlider.value = _hp;
        }
    }

    protected virtual void Update()
    {
        if (_player == null || _rb == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer <= _vigilanceRange && distanceToPlayer > _attackRange)
        {
            // Player is within vigilance range but outside attack range
            _isChasing = true;
        }
        else if (distanceToPlayer <= _attackRange)
        {
            // Player is within attack range
            _isChasing = false;
            if (Time.time >= _lastAttackTime + _attackCooldown)
            {
                AttackPlayer();
                _lastAttackTime = Time.time;
            }
        }
        else
        {
            // Player is outside vigilance range
            _isChasing = false;
        }

        if (_isChasing)
        {
            ChasePlayer();
        }
        else
        {
            // Stop movement when not chasing
            _rb.velocity = Vector3.zero;
        }

        if (_hp <= 0)
        {
            Die();
        }

        //常にメインのカメラにHPバーを向けて置く
        _hpBar.transform.rotation = Camera.main.transform.rotation;

        if (_hpSlider != null)
        {
            _hpSlider.value = _hp;  // 現在のHPに基づいてSliderを更新
        }
    }

    protected void ChasePlayer()
    {
        // Calculate direction towards the player
        Vector3 direction = (_player.position - transform.position).normalized;

        // Face the player
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Set the velocity towards the player
        _rb.velocity = direction * _speed;
    }

    protected virtual void AttackPlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    /// <summary>
    /// 攻撃判定が始まる時に呼ぶ
    /// 基底クラスに何も書いてないので必ずオーバーライドを行う
    /// </summary>
    public virtual void StartAttack()
    {

    }

    /// <summary>
    /// 攻撃判定が終わる時に呼ぶ
    /// 基底クラスに何も書いてないので必ずオーバーライドを行う
    /// </summary>
    public virtual void EndAttack()
    {

    }

    public virtual void Damage(float attack)
    {
        _hp -= attack;
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }

    protected virtual float GetCurrentMoveSpeed()
    {
        // Rigidbodyの速度を水平面に投影したベクトルを計算
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        // ベクトルの大きさを計算し、移動速度とする
        return horizontalVelocity.magnitude;
    }
}

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

    private bool _alive = true;
    private bool _isAttacking = false;

    [SerializeField]
    private GameObject _hpBar;
    private Slider _hpSlider;

    private Transform _player;
    private bool _isChasing = false;
    private float _lastAttackTime = -Mathf.Infinity;
    private Rigidbody _rb;
    private Collider _collider;

    protected virtual void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

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
            _hpSlider.maxValue = _maxHp;
            _hpSlider.value = _hp;
        }
    }

    protected virtual void Update()
    {
        if (_player == null || _rb == null) return;
        if (!_alive) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (!_isAttacking)
        {
            if (distanceToPlayer <= _vigilanceRange && distanceToPlayer > _attackRange)
            {
                _isChasing = true;
            }
            else if (distanceToPlayer <= _attackRange)
            {
                _isChasing = false;
                if (Time.time >= _lastAttackTime + _attackCooldown)
                {
                    AttackPlayer();
                    _lastAttackTime = Time.time;
                }
            }
            else
            {
                _isChasing = false;
            }

            if (_isChasing)
            {
                ChasePlayer();
            }
            else
            {
                _rb.velocity = Vector3.zero;
            }
        }

        if (_hp <= 0)
        {
            Die();
        }

        _hpBar.transform.rotation = Camera.main.transform.rotation;

        if (_hpSlider != null)
        {
            _hpSlider.value = _hp;
        }
    }

    protected void ChasePlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        _rb.velocity = direction * _speed;
    }

    protected virtual void AttackPlayer()
    {
        _isAttacking = true;
        Vector3 direction = (_player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = lookRotation;
    }

    public virtual void StartAttack()
    {
    }

    public virtual void EndAttack()
    {
    }

    public virtual void EndAttackAnim()
    {
        _isAttacking = false;
    }

    public virtual void Damage(float attack)
    {
        _hp -= attack;
    }

    public virtual void Die()
    {
        _alive = false;
        _hpBar.SetActive(false);
        _collider.enabled = false;
    }

    public virtual void DeleteObj()
    {
        Destroy(this.gameObject);
    }

    protected virtual float GetCurrentMoveSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        return horizontalVelocity.magnitude;
    }
}

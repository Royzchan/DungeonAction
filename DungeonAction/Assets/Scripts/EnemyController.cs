using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float _maxHp;
    private float _hp;
    [SerializeField]
    private float _attack;
    [SerializeField]
    private float _attackPower = 100f;
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

    private Transform _player;
    private bool _isChasing = false;
    private float _lastAttackTime = -Mathf.Infinity;

    protected virtual void Start()
    {
        // Find the player by tag (assumes the player has a "Player" tag)
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        if (_player == null) return;

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
    }

    protected void ChasePlayer()
    {
        // Face the player
        Vector3 direction = (_player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Move towards the player
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    protected virtual void AttackPlayer()
    {
        // Placeholder for attack logic
        Debug.Log("Attacking the player!");
    }
}

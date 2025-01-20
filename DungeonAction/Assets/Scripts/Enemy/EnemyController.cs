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
            // ������Slider�̒l��ݒ�
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

        //��Ƀ��C���̃J������HP�o�[�������Ēu��
        _hpBar.transform.rotation = Camera.main.transform.rotation;

        if (_hpSlider != null)
        {
            _hpSlider.value = _hp;  // ���݂�HP�Ɋ�Â���Slider���X�V
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
    /// �U�����肪�n�܂鎞�ɌĂ�
    /// ���N���X�ɉ��������ĂȂ��̂ŕK���I�[�o�[���C�h���s��
    /// </summary>
    public virtual void StartAttack()
    {

    }

    /// <summary>
    /// �U�����肪�I��鎞�ɌĂ�
    /// ���N���X�ɉ��������ĂȂ��̂ŕK���I�[�o�[���C�h���s��
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
        // Rigidbody�̑��x�𐅕��ʂɓ��e�����x�N�g�����v�Z
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        // �x�N�g���̑傫�����v�Z���A�ړ����x�Ƃ���
        return horizontalVelocity.magnitude;
    }
}

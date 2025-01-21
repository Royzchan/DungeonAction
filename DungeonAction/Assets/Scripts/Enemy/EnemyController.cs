// EnemyController.cs
// �G�L�����N�^�[�̊�{�I�ȋ����𐧌䂷��X�N���v�g

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // �G�̃X�e�[�^�X
    [SerializeField]
    private float _maxHp = 100f; // �ő�HP
    private float _hp; // ���݂�HP
    [SerializeField]
    protected float _attackPower = 100f; // �U����
    [SerializeField]
    private float _defenses = 100f; // �h���
    [SerializeField]
    private float _speed = 5f; // �ړ����x
    [SerializeField]
    private float _vigilanceRange = 10f; // �v���C���[���x������͈�
    [SerializeField]
    private float _attackRange = 2f; // �U���\�Ȕ͈�
    [SerializeField]
    private float _attackCooldown = 1.5f; // �U���̃N�[���_�E������

    private bool _alive = true; // ������ԃt���O
    private bool _isAttacking = false; // �U�����t���O

    [SerializeField]
    private GameObject _hpBar; // HP�o�[�I�u�W�F�N�g
    private Slider _hpSlider; // HP�o�[�̃X���C�_�[�R���|�[�l���g

    private Transform _player; // �v���C���[��Transform
    private bool _isChasing = false; // �v���C���[��ǐՒ��t���O
    private float _lastAttackTime = -Mathf.Infinity; // �Ō�ɍU����������
    private Rigidbody _rb; // �G��Rigidbody
    private Collider _collider; // �G�̃R���C�_�[
    private GameManager _gm;

    // ����������
    protected virtual void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _gm = FindAnyObjectByType<GameManager>();

        _hp = _maxHp; // HP��������

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

    // ���t���[���̏���
    protected virtual void Update()
    {
        if (_player == null || _rb == null) return; // �v���C���[�܂���Rigidbody�����݂��Ȃ��ꍇ�͏������X�L�b�v
        if (!_alive) return; // �G������ł���ꍇ���������X�L�b�v

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position); // �v���C���[�Ƃ̋������v�Z

        if (!_isAttacking)
        {
            if (distanceToPlayer <= _vigilanceRange && distanceToPlayer > _attackRange)
            {
                _isChasing = true; // �x���͈͓��Ƀv���C���[������ꍇ�ǐՊJ�n
            }
            else if (distanceToPlayer <= _attackRange)
            {
                _isChasing = false;
                if (Time.time >= _lastAttackTime + _attackCooldown)
                {
                    AttackPlayer(); // �U�������s
                    _lastAttackTime = Time.time; // �U�����Ԃ��L�^
                }
            }
            else
            {
                _isChasing = false; // �v���C���[���͈͊O�̏ꍇ�ǐՒ�~
            }

            if (_isChasing)
            {
                ChasePlayer(); // �v���C���[��ǐ�
            }
            else
            {
                _rb.velocity = Vector3.zero; // �ړ����~
            }
        }

        if (_hp <= 0)
        {
            Die(); // HP��0�ȉ��̏ꍇ���S���������s
        }

        _hpBar.transform.rotation = Camera.main.transform.rotation; // HP�o�[����ɃJ�����Ɍ�����

        if (_hpSlider != null)
        {
            _hpSlider.value = _hp; // HP�o�[�̒l���X�V
        }
    }

    // �v���C���[��ǐՂ��鏈��
    protected void ChasePlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized; // �v���C���[�ւ̕������v�Z

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // �v���C���[����������
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // ��]����

        _rb.velocity = direction * _speed; // �ړ����x��ݒ�
    }

    // �v���C���[���U�����鏈��
    protected virtual void AttackPlayer()
    {
        _isAttacking = true; // �U�����t���O��ݒ�
        Vector3 direction = (_player.position - transform.position).normalized; // �v���C���[�������v�Z
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // �v���C���[����������
        transform.rotation = lookRotation; // ��]��ݒ�
    }

    // �U���A�j���[�V�����̊J�n
    public virtual void StartAttack()
    {
    }

    // �U���A�j���[�V�����̏I��
    public virtual void EndAttack()
    {
    }

    // �U���I����̏���
    public virtual void EndAttackAnim()
    {
        _isAttacking = false; // �U�����t���O�����Z�b�g
    }

    // �_���[�W���󂯂鏈��
    public virtual void Damage(float attack)
    {
        _hp -= attack; // HP������
    }

    // ���S����
    public virtual void Die()
    {
        _alive = false; // ������Ԃ��X�V
        _hpBar.SetActive(false); // HP�o�[���\��
        _collider.enabled = false; // �R���C�_�[�𖳌���
        if (_gm != null)
        {
            _gm.AddScore(10);
        }
    }

    // �I�u�W�F�N�g���폜���鏈��
    public virtual void DeleteObj()
    {
        Destroy(this.gameObject); // �Q�[���I�u�W�F�N�g���폜
    }

    // ���݂̈ړ����x���擾���鏈��
    protected virtual float GetCurrentMoveSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z); // ���������̑��x���v�Z
        return horizontalVelocity.magnitude; // �ړ����x��Ԃ�
    }
}

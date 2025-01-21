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
    private Transform _cameraTransform; // �v���C���[�̓����Ɋ�Â��J�����̌���

    [Header("�v���C���[�̃X�e�[�^�X")]
    [SerializeField]
    private float _maxHp = 100f; // �ő�HP
    private float _hp;
    [SerializeField]
    private float _maxStamina; // �ő�X�^�~�i
    private float _stamina;
    [SerializeField]
    private float _attackPower = 100f; // �U����
    [SerializeField]
    private float _attackRange = 5f; // �U���͈�
    [SerializeField]
    private float _defenses = 100f; // �h���
    [SerializeField]
    private float _speed = 5f; // �ʏ�ړ����x
    [SerializeField]
    private float _dashSpeed = 7.5f; // �_�b�V�����̈ړ����x
    [SerializeField]
    private float _invincibilityDuration = 0.5f; // ���G���Ԃ̒���

    private bool _alive = true; // ������ԃt���O
    private bool _isInvincible = false; // ���G��ԃt���O
    private bool _isAvoiding = false; // ��𒆃t���O
    private Vector2 _avoidDirection = Vector2.zero; // ������

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

    [Header("����ݒ�")]
    [SerializeField]
    private GameObject _sowrdObj; // �v���C���[�̕���I�u�W�F�N�g
    private Collider _sowrdCollider; // ����̃R���C�_�[

    private HashSet<Collider> _hitEnemies = new HashSet<Collider>(); // �U�������G�̋L�^

    private Vector2 _moveDirection = Vector2.zero; // �v���C���[�̈ړ�����

    private bool _attackNow = false; // ���ݍU������

    private string _moveSpeedStr = "MoveSpeed"; // �A�j���[�V�����p�̑��x�p�����[�^
    private string _attackStr = "isAttack"; // �U���A�j���[�V�����g���K�[
    private string _avoidStr = "isAvoid"; // ����A�j���[�V�����g���K�[
    private string _dieStr = "Die"; // ���S�A�j���[�V�����g���K�[

    #region �Q�b�^�[
    public float MaxHP { get { return _maxHp; } }
    public float HP { get { return _hp; } }
    public float MaxStamina { get { return _maxStamina; } }
    public float Stamina { get { return _stamina; } }
    public bool Alive { get { return _alive; } }
    #endregion

    // �X�^�~�i�񕜊֘A�ݒ�
    [SerializeField]
    private float _staminaRecoveryRate = 5f; // �X�^�~�i�̉񕜑��x
    [SerializeField]
    private float _staminaConsumptionRate = 10f; // �_�b�V�����̃X�^�~�i����x
    [SerializeField]
    private float _avoidConsumptionAmount = 50f; // ������̃X�^�~�i�����

    private void OnEnable()
    {
        // ���͂�L����
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
        // ���͂𖳌���
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
        // ������
        _hp = _maxHp;
        _stamina = _maxStamina;
    }

    private void Start()
    {
        // �R���|�[�l���g�̎擾
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _gm = FindAnyObjectByType<GameManager>();

        // ����̃R���C�_�[�ݒ�
        if (_sowrdObj != null)
        {
            _sowrdCollider = _sowrdObj.GetComponent<Collider>();
            _sowrdCollider.enabled = false;
        }
    }

    private void Update()
    {
        // �f�o�b�O�p�_���[�W�R�}���h
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Damage(20);
        }

        if (!_alive || !_gm.GamePlaying)
        {
            _moveDirection = Vector2.zero;
            return; // �������łȂ��ꍇ�A�����𒆎~
        }

        // �U�����͈ړ��s��
        _moveDirection = _attackNow || _isAvoiding ? Vector2.zero : _moveAction.ReadValue<Vector2>();

        // �X�^�~�i�񕜁i�_�b�V�����A��𒆂ł͉񕜂��Ȃ��j
        if (!_dashAction.IsPressed() && _stamina < _maxStamina && !_isAvoiding)
        {
            _stamina += _staminaRecoveryRate * Time.deltaTime;
            if (_stamina > _maxStamina) _stamina = _maxStamina;
        }
    }

    private void FixedUpdate()
    {
        // �ړ�����
        Move();
    }

    private void Move()
    {
        if (_cameraTransform != null && !_isAvoiding)
        {
            _animator.SetFloat(_moveSpeedStr, GetCurrentMoveSpeed());
            // �ʏ�̈ړ�����
            Vector3 cameraForward = _cameraTransform.forward;
            Vector3 cameraRight = _cameraTransform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 move = cameraRight * _moveDirection.x + cameraForward * _moveDirection.y;

            // �_�b�V�����̑��x�ύX
            if (_dashAction.IsPressed() && _stamina > 0)
            {
                _rb.velocity = move * _dashSpeed + new Vector3(0f, _rb.velocity.y, 0f);

                // �X�^�~�i����
                _stamina -= _staminaConsumptionRate * Time.deltaTime;
                if (_stamina < 0) _stamina = 0;
            }
            else
            {
                _rb.velocity = move * _speed + new Vector3(0f, _rb.velocity.y, 0f);
            }

            // �����̕ύX
            if (move.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            }
        }
        else if (_isAvoiding)
        {
            // ��𒆂̈ړ�����
            Vector3 move = Vector3.zero;

            if (_avoidDirection.magnitude > 0)
            {
                // �����L�[��������Ă����ꍇ�A���̕����ɉ��
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
                // �����L�[��������Ă��Ȃ��ꍇ�́A���݂̌����ŉ��
                move = transform.forward;
            }

            // ��C�ɃX�^�~�i����ĉ��
            if (_stamina >= _avoidConsumptionAmount)
            {
                _rb.velocity = move * _dashSpeed + new Vector3(0f, _rb.velocity.y, 0f);
            }
            else
            {
                // �X�^�~�i������Ȃ��ꍇ�͉���ł��Ȃ�
                EndAvoid();
            }
        }
    }

    private void Attack()
    {
        if (!_attackNow)
        {
            _attackNow = true;

            // �Ŋ��̓G������
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

            // ����̃R���C�_�[��L����
            if (_sowrdCollider != null)
            {
                _sowrdCollider.enabled = true;
                _hitEnemies.Clear();
            }
        }
    }

    private Collider FindNearestEnemy()
    {
        // ���͈͓��̓G������
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
        // �U���I�����̏���
        _attackNow = false;
        if (_sowrdCollider != null)
        {
            _sowrdCollider.enabled = false;
        }
        _hitEnemies.Clear();
    }

    private void Skill()
    {
        Debug.Log("�X�L������");
    }

    private void Special()
    {
        Debug.Log("�K�E�Z����");
    }

    private void StartAvoid()
    {
        if (_attackNow)
        {
            _attackNow = false;
        }

        // �X�^�~�i������Ă��Ȃ��ꍇ�͉���ł��Ȃ�
        if (_stamina < _avoidConsumptionAmount)
        {
            return;
        }

        _animator.SetTrigger(_avoidStr);

        // �����L�[�̓��͂��m�F
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
        // ����A�j���[�V�������I���̂�҂�
        _isAvoiding = false;
    }

    private IEnumerator StartInvincibility()
    {
        // ���G���Ԃ̊J�n
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
                // ���S����
                _alive = false;
                _gm.GameOver();
                _animator.SetTrigger(_dieStr);
            }
        }
    }

    public float GetCurrentMoveSpeed()
    {
        // ���݂̈ړ����x���v�Z
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
        // ����̍U������
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
        // �U���͈͂̉���
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}

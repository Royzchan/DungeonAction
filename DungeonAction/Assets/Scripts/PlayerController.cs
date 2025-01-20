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

    #region �Q�b�^�[
    public float MaxHP { get { return _maxHp; } }
    public float HP { get { return _hp; } }
    public float MaxStamina { get { return _maxStamina; } }
    public float Stamina { get { return _stamina; } }

    #endregion

    private void OnEnable()
    {
        //�ړ��̓��͂�L����
        _moveAction?.Enable();

        //�U���̃A�N�V�����̃R�[���o�b�N��ǉ�
        _attackAction.started += OnAttack;
        //�U���A�N�V�����̓��͂�L����
        _attackAction?.Enable();

        //�X�L���̃A�N�V�����̃R�[���o�b�N��ǉ�
        _skillAction.started += OnSkill;
        //�X�L���̃A�N�V�����̓��͂�L����
        _skillAction?.Enable();

        //�K�E�Z�̃A�N�V�����̃R�[���o�b�N��ǉ�
        _specialAction.started += OnSpecial;
        //�K�E�Z�̓��͂�L����
        _specialAction?.Enable();

        //����̃A�N�V�����̃R�[���o�b�N��ǉ�
        _avoidAction.started += OnAvoid;
        //����̓��͂�L����
        _avoidAction?.Enable();

        //�_�b�V���̓��͂�L����
        _dashAction?.Enable();
    }

    private void OnDisable()
    {
        //�ړ��̓��͂𖳌���
        _moveAction?.Disable();

        //�U���̃A�N�V�����̃R�[���o�b�N������
        _attackAction.started -= OnAttack;
        //�U���̓��͂𖳌���
        _attackAction?.Disable();

        //�X�L���̃A�N�V�����̃R�[���o�b�N������
        _skillAction.started -= OnSkill;
        //�X�L���̓��͂𖳌���
        _skillAction?.Disable();

        //�K�E�Z�̃A�N�V�����̃R�[���o�b�N������
        _specialAction.started -= OnSpecial;
        //�K�E�Z�̓��͂𖳌���
        _specialAction?.Disable();

        //����̃A�N�V�����̃R�[���o�b�N������
        _avoidAction.started -= OnAvoid;
        //����̓��͂𖳌���
        _avoidAction?.Disable();

        //�_�b�V���̓��͂𖳌���
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
        //�f�o�b�N�R�}���h
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

    //�ړ�
    private void Move()
    {
        if (_cameraTransform != null)
        {
            // �J�����̑O���ƉE�������擾
            Vector3 cameraForward = _cameraTransform.forward;
            Vector3 cameraRight = _cameraTransform.right;

            // Y�������𖳎��i�����ړ��̂݁j
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // �ړ��������v�Z
            Vector3 move = cameraRight * _moveDirection.x + cameraForward * _moveDirection.y;

            // Rigidbody�̑��x��ݒ�
            //�_�b�V����
            if (_dashAction.IsPressed())
            {
                _rb.velocity = move * _dashSpeed + new Vector3(0f, _rb.velocity.y, 0f);
            }
            //�ʏ펞
            else
            {
                _rb.velocity = move * _speed + new Vector3(0f, _rb.velocity.y, 0f);
            }

            // �v���C���[���i�s����������
            if (move.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            }
        }
    }

    //�U��
    private void Attack()
    {
        if (!_attackNow)
        {
            _attackNow = true;

            // �G�����o���ă^�[�Q�b�g�̕���������
            Collider nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                Vector3 directionToEnemy = (nearestEnemy.transform.position - transform.position).normalized;
                directionToEnemy.y = 0; // Y�����̉�]�𖳎�
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
            // �G���ǂ����𔻒�
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
            Debug.Log("�Ŋ�̓G: " + nearestEnemy.name);  // ���������G�����O�ɕ\��
        }
        else
        {
            Debug.Log("�͈͓��ɓG�����܂���");
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

    //�X�L��
    private void Skill()
    {
        Debug.Log("�X�L��");
    }

    //�K�E�Z
    private void Special()
    {
        Debug.Log("�K�E�Z");
    }

    //���
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

    //�_���[�W
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


    //���݂̈ړ����x���擾����֐�
    public float GetCurrentMoveSpeed()
    {
        // Rigidbody�̑��x�𐅕��ʂɓ��e�����x�N�g�����v�Z
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        // �x�N�g���̑傫�����v�Z���A�ړ����x�Ƃ���
        return horizontalVelocity.magnitude;
    }

    //Input System�̃R�[���o�b�N
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
        // �U���͈͂̉���
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}

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
    private float _dashSpeed = 7.5f;

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

    private Vector2 _moveDirection = Vector2.zero;

    private bool _attackNow = false;
    private bool _avoidNow = false;

    private string _moveSpeedStr = "MoveSpeed";
    private string _attackStr = "isAttack";
    private string _avoidStr = "isAvoid";

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

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
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
        Debug.Log("�U��");
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
        _animator.SetBool(_avoidStr, true);
    }

    private void EndAvoid()
    {
        _animator.SetBool(_avoidStr, false);
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
}

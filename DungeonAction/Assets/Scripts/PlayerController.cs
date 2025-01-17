using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
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
    private InputAction _moveAction;
    [SerializeField]
    private InputAction _attackAction;
    [SerializeField]
    private InputAction _skillAction;
    [SerializeField]
    private InputAction _specialAction;

    private Vector2 _moveDirection = Vector2.zero;

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
    }



    void Start()
    {
        _rb = GetComponent<Rigidbody>();
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
            _rb.velocity = move * _speed + new Vector3(0f, _rb.velocity.y, 0f);

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

    //Input System�̃R�[���o�b�N
    private void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("�U���������ꂽ");
    }

    private void OnSkill(InputAction.CallbackContext context)
    {
        Debug.Log("�X�L���������ꂽ");
    }

    private void OnSpecial(InputAction.CallbackContext context)
    {
        Debug.Log("�K�E�Z�������ꂽ");
    }
}

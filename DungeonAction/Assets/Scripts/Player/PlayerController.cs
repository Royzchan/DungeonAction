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
    private SettingController _settingController;
    [SerializeField]
    private MapController _mapController;
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
    protected float _attackPower = 100f; // �U����
    [SerializeField]
    protected float _attackRange = 5f; // �U���͈�
    [SerializeField]
    protected float _defenses = 100f; // �h���
    [SerializeField]
    private float _skillCoolTime = 5.0f; //�X�L���̃N�[���^�C��
    private float _skillCoolTimeCounter = 0;
    [SerializeField]
    private float _getSpecialPoin = 15.0f; //�X�L���g�p���Ɋl���ł���K�E�|�C���g
    [SerializeField]
    private float _maxSpecialPoint = 100f;//���߂���K�E�|�C���g�̍ő�l
    [SerializeField]
    private float _specialPoint = 100f; //�K�E�Z��ł̂ɕK�v�ȕK�E�|�C���g
    [SerializeField]
    private float _specialCoolTime = 10.0f; //�K�E�Z�̃N�[���^�C��
    private float _specialCoolTimeCounter = 0f;
    [SerializeField]
    private float _speed = 5f; // �ʏ�ړ����x
    [SerializeField]
    private float _dashSpeed = 7.5f; // �_�b�V�����̈ړ����x
    [SerializeField]
    private float _invincibilityDuration = 0.5f; // ���G���Ԃ̒���

    private bool _alive = true;         // ������ԃt���O
    private bool _isInvincible = false; // ���G��ԃt���O
    private bool _isAvoiding = false;   // ��𒆃t���O
    private bool _skillNow = false;     // �X�L���g�p���t���O
    private bool _canUseSkill = true;   // �X�L���g�p�\�t���O
    private bool _specialNow = false;   // �K�E�Z�g�p���t���O
    private bool _canUseSpecial = true; // �K�E�g�p�\�t���O
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
    [SerializeField]
    private InputAction _mapAction;

    [Header("Setting Input Actions")]
    [SerializeField]
    private InputAction _upAction;
    [SerializeField]
    private InputAction _downAction;
    [SerializeField]
    private InputAction _rightAction;
    [SerializeField]
    private InputAction _leftAction;
    [SerializeField]
    private InputAction _decisionAction;

    [Header("Other Input Action")]
    [SerializeField]
    private InputAction _suspensionAction;

    protected HashSet<Collider> _hitEnemies = new HashSet<Collider>(); // �U�������G�̋L�^

    private Vector2 _moveDirection = Vector2.zero; // �v���C���[�̈ړ�����

    private bool _attackNow = false; // ���ݍU������

    private string _moveSpeedStr = "MoveSpeed"; // �A�j���[�V�����p�̑��x�p�����[�^
    private string _attackStr = "isAttack";     // �U���A�j���[�V�����g���K�[
    private string _skillStr = "isSkill";       // �X�L���A�j���[�V�����g���K�[
    private string _specialStr = "isSpecial";   // �K�E�Z�A�j���[�V�����g���K�[
    private string _avoidStr = "isAvoid";       // ����A�j���[�V�����g���K�[
    private string _dieStr = "Die";             // ���S�A�j���[�V�����g���K�[

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
        //�t�B�[���h�̓��͂�L����
        EnableFieldAction();
        //�ݒ�̓��͂𖳌���
        DisableSettingAction();

        //�t�B�[���h�̃A�N�V������ǉ�
        _attackAction.started += OnAttack;
        _skillAction.started += OnSkill;
        _specialAction.started += OnSpecial;
        _avoidAction.started += OnAvoid;
        _mapAction.started += OnMap;
        //�ݒ�̃A�N�V������ǉ�
        _upAction.started += OnUpButton;
        _downAction.started += OnDownButton;
        _rightAction.started += OnRightButton;
        _leftAction.started += OnLeftButton;
        _decisionAction.started += OnDecisionButton;

        _suspensionAction?.Enable();
        _suspensionAction.started += OnSuspension;
    }

    private void OnDisable()
    {
        //�t�B�[���h�̓��͂𖳌���
        DisableFieldAction();
        //�ݒ�̓��͂𖳌���
        DisableSettingAction();

        _attackAction.started -= OnAttack;
        _skillAction.started -= OnSkill;
        _specialAction.started -= OnSpecial;
        _avoidAction.started -= OnAvoid;
        _mapAction.started -= OnMap;

        _upAction.started -= OnUpButton;
        _downAction.started -= OnDownButton;
        _rightAction.started -= OnRightButton;
        _leftAction.started -= OnLeftButton;
        _decisionAction.started -= OnDecisionButton;

        _suspensionAction?.Disable();
        _suspensionAction.started -= OnSuspension;
    }

    /// <summary>
    /// �t�B�[���h�ŉ�����{�^���̓��̗͂L����
    /// </summary>
    public void EnableFieldAction()
    {
        _moveAction?.Enable();
        _attackAction?.Enable();
        _skillAction?.Enable();
        _specialAction?.Enable();
        _avoidAction?.Enable();
        _dashAction?.Enable();
        _mapAction?.Enable();
    }

    /// <summary>
    /// �t�B�[���h�Ŏg���{�^���̓��͂𖳌���
    /// </summary>
    public void DisableFieldAction()
    {
        _moveAction?.Disable();
        _attackAction?.Disable();
        _skillAction?.Disable();
        _specialAction?.Disable();
        _avoidAction?.Disable();
        _dashAction?.Disable();
        _mapAction?.Disable();
    }

    /// <summary>
    /// �ݒ�Ŏg���{�^���̓��͂�L����
    /// </summary>
    public void EnableSettingAction()
    {
        _upAction?.Enable();
        _downAction?.Enable();
        _rightAction?.Enable();
        _leftAction?.Enable();
        _decisionAction?.Enable();
    }

    /// <summary>
    /// �ݒ�Ŏg���{�^���̓��͂𖳌���
    /// </summary>
    public void DisableSettingAction()
    {
        _upAction?.Disable();
        _downAction?.Disable();
        _rightAction?.Disable();
        _leftAction?.Disable();
        _decisionAction?.Disable();
    }

    protected virtual void Awake()
    {
        // ������
        _hp = _maxHp;
        _stamina = _maxStamina;

        // �R���|�[�l���g�̎擾
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _gm = FindAnyObjectByType<GameManager>();
        if (_gm == null) Debug.LogError("GameManager�����݂��Ă��܂���");
        _settingController = FindAnyObjectByType<SettingController>();
        if (_settingController != null) _settingController.SetPlayer(this);
        _mapController = FindAnyObjectByType<MapController>();
    }

    private void Start()
    {
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
        bool canMove = _attackNow || _skillNow || _specialNow || _isAvoiding;
        _moveDirection = canMove ? Vector2.zero : _moveAction.ReadValue<Vector2>();

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

        // �}�b�v�Ɍ��݂̍��W�ƌ����𑗂�
        UpdatePlayerPositionOnMap();
    }

    /// <summary>
    /// �U������
    /// </summary>
    protected virtual void Attack()
    {
        bool canAttack = !_attackNow && !_skillNow && !_specialNow;
        if (canAttack)
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
            _hitEnemies.Clear();
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

    public virtual void EndAttack()
    {
        // �U���I�����̏���
        _attackNow = false;
        _hitEnemies.Clear();
    }

    /// <summary>
    /// �X�L������
    /// </summary>
    protected virtual void Skill()
    {
        bool canSkill = _canUseSkill && !_skillNow && !_specialNow;
        if (canSkill)
        {
            _canUseSkill = false;
            _skillCoolTimeCounter = _skillCoolTime;
            _skillNow = true;
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
            _animator.SetTrigger(_skillStr);
            _hitEnemies.Clear();
        }
    }

    public virtual void EndSkill()
    {
        _skillNow = false;
        _hitEnemies.Clear();
    }

    /// <summary>
    /// �X�L���̃N�[���^�C�����`�F�b�N
    /// </summary>
    private void CheckSkillCoolTime()
    {
        if (_canUseSkill) return;
        _skillCoolTimeCounter -= Time.deltaTime;
        if (_skillCoolTimeCounter <= 0)
        {
            _skillCoolTimeCounter = 0;
            _canUseSkill = true;
        }
    }

    /// <summary>
    /// �K�E�Z�̃N�[���^�C�����`�F�b�N
    /// </summary>
    private void CheckSpecialCoolTime()
    {
        if (_canUseSpecial) return;
        _specialCoolTimeCounter -= Time.deltaTime;
        if (_specialCoolTimeCounter <= 0)
        {
            _specialCoolTimeCounter = 0;
            _canUseSpecial = true;
        }
    }

    /// <summary>
    /// �K�E�Z����
    /// </summary>
    protected virtual void Special()
    {
        bool canSpecial = _canUseSpecial && !_specialNow;
        if (canSpecial)
        {
            _specialNow = true;
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
            _animator.SetTrigger(_specialStr);
            _hitEnemies.Clear();
        }
    }

    public virtual void EndSpecial()
    {
        _specialNow = false;
        _hitEnemies.Clear();
    }


    private void StartAvoid()
    {
        if (_attackNow)
        {
            _attackNow = false;
        }

        if (_skillNow || _specialNow)
        {
            return;
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

    private void OnSuspension(InputAction.CallbackContext context)
    {
        if (!_gm.OpenSetting)
        {
            _gm.ViewSettingCanvas();
            //�t�B�[���h�̓��͂𖳌���
            DisableFieldAction();
            //�ݒ�̓��͂�L����
            EnableSettingAction();
        }
        else
        {
            _gm.HideSettingCanvas();
            //�t�B�[���h�̓��͂�L����
            EnableFieldAction();
            //�ݒ�̓��͂�L����
            DisableSettingAction();
        }
    }

    private void OnMap(InputAction.CallbackContext context)
    {
        if (!_gm.OpenMap)
        {
            _gm.ViewMapCanvas();
        }
        else
        {
            _gm.HideMapCanvas();
        }
    }

    private void OnUpButton(InputAction.CallbackContext context)
    {
        _settingController.SelectUp();
    }

    private void OnDownButton(InputAction.CallbackContext context)
    {
        _settingController.SelectDown();
    }

    private void OnRightButton(InputAction.CallbackContext context)
    {
        _settingController.SelectRight();
    }

    private void OnLeftButton(InputAction.CallbackContext context)
    {
        _settingController.SelectLeft();
    }

    private void OnDecisionButton(InputAction.CallbackContext context)
    {
        _settingController.Decision();
    }

    private void OnDrawGizmosSelected()
    {
        // �U���͈͂̉���
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    //���W���ړ�(�X�^�[�g���Ɏg��)
    public void SetPos(Vector3 pos)
    {
        this.transform.position = pos;
    }

    /// <summary>
    /// �}�b�v�Ƀv���C���[�̈ʒu�ƌ������X�V
    /// </summary>
    private void UpdatePlayerPositionOnMap()
    {
        if (_mapController != null)
        {
            // ���݂̃v���C���[���W (Grid�P�ʂő��M)
            Vector3 position = transform.position;
            Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(position.x) / 4, Mathf.FloorToInt(position.z) / 4);

            // ���݂̌���
            Vector3 forward = transform.forward;
            Vector2Int facingDirection = new Vector2Int(
                Mathf.RoundToInt(forward.x),
                Mathf.RoundToInt(forward.z)
            );

            // �}�b�v�R���g���[���[���X�V
            _mapController.UpdatePlayerPosition(gridPosition, facingDirection);
        }
    }
}

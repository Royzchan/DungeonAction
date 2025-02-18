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
    private SkillTreeController _skillTreeController;
    [SerializeField]
    private Transform _cameraTransform; // プレイヤーの動きに基づくカメラの向き

    [Header("プレイヤーのステータス")]
    [SerializeField]
    private float _maxHp = 100f; // 最大HP
    private float _hp;
    [SerializeField]
    private float _maxStamina; // 最大スタミナ
    private float _stamina;
    [SerializeField]
    protected float _attack = 100f; // 攻撃力
    [SerializeField]
    protected float _skillPowerUpRatio = 1.5f;　//　スキル時のダメージアップ倍率
    [SerializeField]
    protected float _specialPowerUpRatio = 2.0f; // 必殺技時のダメージアップ倍率
    [SerializeField]
    protected float _attackRange = 5f; // 攻撃範囲
    [SerializeField]
    protected float _defense = 100f; // 防御力
    [SerializeField]
    private float _skillCoolTime = 5.0f; //スキルのクールタイム
    private float _skillCoolTimeCounter = 0;
    protected float _specialPoint = 0f;
    [SerializeField]
    protected float _getSpecialPoint = 15.0f; //スキル使用時に獲得できる必殺ポイント
    [SerializeField]
    protected float _maxSpecialPoint = 100f;//貯められる必殺ポイントの最大値
    [SerializeField]
    private float _useSpecialPoint = 100f; //必殺技を打つのに必要な必殺ポイント
    [SerializeField]
    private float _specialCoolTime = 10.0f; //必殺技のクールタイム
    private float _specialCoolTimeCounter = 0f;
    [SerializeField]
    private float _speed = 5f; // 通常移動速度
    [SerializeField]
    private float _dashSpeed = 7.5f; // ダッシュ時の移動速度
    [SerializeField]
    private float _invincibilityDuration = 0.5f; // 無敵時間の長さ

    private bool _alive = true;         // 生存状態フラグ
    private bool _isInvincible = false; // 無敵状態フラグ
    private bool _isAvoiding = false;   // 回避中フラグ
    protected bool _attackNow = false;  // 攻撃中フラグ
    protected bool _skillNow = false;   // スキル使用中フラグ
    private bool _canUseSkill = true;   // スキル使用可能フラグ
    protected bool _specialNow = false; // 必殺技使用中フラグ
    private bool _canUseSpecial = true; // 必殺使用可能フラグ
    private Vector2 _avoidDirection = Vector2.zero; // 回避方向

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
    [SerializeField]
    private InputAction _skillTreeAction;

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

    protected HashSet<Collider> _hitEnemies = new HashSet<Collider>(); // 攻撃した敵の記録

    private Vector2 _moveDirection = Vector2.zero; // プレイヤーの移動方向

    private string _moveSpeedStr = "MoveSpeed"; // アニメーション用の速度パラメータ
    private string _attackStr = "isAttack";     // 攻撃アニメーショントリガー
    private string _skillStr = "isSkill";       // スキルアニメーショントリガー
    private string _specialStr = "isSpecial";   // 必殺技アニメーショントリガー
    private string _avoidStr = "isAvoid";       // 回避アニメーショントリガー
    private string _dieStr = "Die";             // 死亡アニメーショントリガー

    #region ゲッター
    public float MaxHP { get { return _maxHp; } }
    public float HP { get { return _hp; } }
    public float MaxStamina { get { return _maxStamina; } }
    public float Stamina { get { return _stamina; } }
    public float SpecialPoint { get { return _specialPoint; } }
    public float UseSpecialPoint { get { return _useSpecialPoint; } }
    public float MaxSpecialPoint { get { return _maxSpecialPoint; } }
    public bool Alive { get { return _alive; } }
    #endregion

    // スタミナ回復関連設定
    [SerializeField]
    private float _staminaRecoveryRate = 5f; // スタミナの回復速度
    [SerializeField]
    private float _staminaConsumptionRate = 10f; // ダッシュ中のスタミナ消費速度
    [SerializeField]
    private float _avoidConsumptionAmount = 50f; // 回避時のスタミナ消費量

    private void OnEnable()
    {
        //フィールドの入力を有効化
        EnableFieldAction();
        //設定の入力を無効化
        DisableSettingAction();

        //フィールドのアクションを追加
        _attackAction.started += OnAttack;
        _skillAction.started += OnSkill;
        _specialAction.started += OnSpecial;
        _avoidAction.started += OnAvoid;

        _suspensionAction?.Enable();
        _suspensionAction.started += OnSuspension;
        _mapAction?.Enable();
        _mapAction.started += OnMap;
        _skillTreeAction?.Enable();
        _skillTreeAction.started += OnSkillTree;
    }

    private void OnDisable()
    {
        //フィールドの入力を無効化
        DisableFieldAction();
        //設定の入力を無効化
        DisableSettingAction();

        _attackAction.started -= OnAttack;
        _skillAction.started -= OnSkill;
        _specialAction.started -= OnSpecial;
        _avoidAction.started -= OnAvoid;

        _suspensionAction?.Disable();
        _suspensionAction.started -= OnSuspension;
        _mapAction?.Disable();
        _mapAction.started -= OnMap;
        _skillTreeAction?.Disable();
        _skillTreeAction.started -= OnSkillTree;
    }

    /// <summary>
    /// フィールドで押せるボタンの入力の有効化
    /// </summary>
    public void EnableFieldAction()
    {
        _moveAction?.Enable();
        _attackAction?.Enable();
        _skillAction?.Enable();
        _specialAction?.Enable();
        _avoidAction?.Enable();
        _dashAction?.Enable();
    }

    /// <summary>
    /// フィールドで使うボタンの入力を無効化
    /// </summary>
    public void DisableFieldAction()
    {
        _moveAction?.Disable();
        _attackAction?.Disable();
        _skillAction?.Disable();
        _specialAction?.Disable();
        _avoidAction?.Disable();
        _dashAction?.Disable();
    }

    /// <summary>
    /// 設定で使うボタンの入力を有効化
    /// </summary>
    public void EnableSettingAction()
    {
        _upAction?.Enable();
        _downAction?.Enable();
        _rightAction?.Enable();
        _leftAction?.Enable();
        _decisionAction?.Enable();
        //設定のアクションを追加
        _upAction.started += OnUpButton_Setting;
        _downAction.started += OnDownButton_Setting;
        _rightAction.started += OnRightButton_Setting;
        _leftAction.started += OnLeftButton_Setting;
        _decisionAction.started += OnDecisionButton_Setting;
    }

    /// <summary>
    /// 設定で使うボタンの入力を無効化
    /// </summary>
    public void DisableSettingAction()
    {
        _upAction?.Disable();
        _downAction?.Disable();
        _rightAction?.Disable();
        _leftAction?.Disable();
        _decisionAction?.Disable();

        _upAction.started -= OnUpButton_Setting;
        _downAction.started -= OnDownButton_Setting;
        _rightAction.started -= OnRightButton_Setting;
        _leftAction.started -= OnLeftButton_Setting;
        _decisionAction.started -= OnDecisionButton_Setting;
    }

    /// <summary>
    /// スキルツリーの入力を有効化
    /// </summary>
    public void EnableSkillTreeAction()
    {
        _upAction?.Enable();
        _downAction?.Enable();
        _rightAction?.Enable();
        _leftAction?.Enable();
        _decisionAction?.Enable();
        //スキルツリーのアクションを追加
        _upAction.started += OnUpButton_SkillTree;
        _downAction.started += OnDownButton_SkillTree;
        _rightAction.started += OnRightButton_SkillTree;
        _leftAction.started += OnLeftButton_SkillTree;
        _decisionAction.started += OnDecisionButton_SkillTree;
    }

    /// <summary>
    /// スキルツリーの入力を無効化
    /// </summary>
    public void DisableSkillTreeAction()
    {
        _upAction?.Disable();
        _downAction?.Disable();
        _rightAction?.Disable();
        _leftAction?.Disable();
        _decisionAction?.Disable();

        _upAction.started -= OnUpButton_SkillTree;
        _downAction.started -= OnDownButton_SkillTree;
        _rightAction.started -= OnRightButton_SkillTree;
        _leftAction.started -= OnLeftButton_SkillTree;
        _decisionAction.started -= OnDecisionButton_SkillTree;
    }

    protected virtual void Awake()
    {
        // 初期化
        _hp = _maxHp;
        _stamina = _maxStamina;

        // コンポーネントの取得
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _gm = FindAnyObjectByType<GameManager>();
        if (_gm == null) Debug.LogError("GameManagerが登録されていません。");

        _settingController = FindAnyObjectByType<SettingController>();
        if (_settingController != null)
        {
            _settingController.SetPlayer(this);
        }
        else
        {
            Debug.LogError("SetingControllerが登録されていません。");
        }

        _mapController = FindAnyObjectByType<MapController>();
        if (_mapController == null) Debug.LogError("MapControllerが登録されていません。");

        _skillTreeController = FindAnyObjectByType<SkillTreeController>();
        if (_skillTreeController == null) Debug.LogError("SkillTreeControllerが登録されていません。");
    }

    private void Start()
    {
    }

    private void Update()
    {
        // デバッグ用ダメージコマンド
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Damage(20);
        }

        if (!_alive || !_gm.GamePlaying)
        {
            _moveDirection = Vector2.zero;
            return; // 生存中でない場合、処理を中止
        }

        // 攻撃中は移動不可
        bool canMove = _attackNow || _skillNow || _specialNow || _isAvoiding;
        _moveDirection = canMove ? Vector2.zero : _moveAction.ReadValue<Vector2>();

        // スタミナ回復（ダッシュ中、回避中では回復しない）
        if (!_dashAction.IsPressed() && _stamina < _maxStamina && !_isAvoiding)
        {
            _stamina += _staminaRecoveryRate * Time.deltaTime;
            if (_stamina > _maxStamina) _stamina = _maxStamina;
        }

        //スキル、必殺技のクールタイムチェック
        CheckSkillCoolTime();
        CheckSpecialCoolTime();
    }

    private void FixedUpdate()
    {
        // 移動処理
        Move();
    }

    /// <summary>
    /// プレイヤーのステータスをセット
    /// </summary>
    /// <param name="maxHp">最大HP</param>
    /// <param name="attack">攻撃力</param>
    /// <param name="defense">防御力</param>
    /// <param name="skillPower">スキル効果アップ倍率</param>
    /// <param name="specialPower">必殺技効果アップ倍率</param>
    public void SetStatus(float maxHp, float attack, float defense, float skillPower, float specialPower)
    {
        _maxHp = maxHp;
        _attack = attack;
        _defense = defense;
        _skillPowerUpRatio = skillPower;
        _specialPowerUpRatio = specialPower;
        Debug.Log("最大HP : " + _maxHp + "攻撃力 : " + _attack + "防御力 : " + _defense
            + "スキル効果倍率 : " + _skillPowerUpRatio + "必殺技効果倍率 : " + _specialPowerUpRatio);
    }

    private void Move()
    {
        if (_cameraTransform != null && !_isAvoiding)
        {
            _animator.SetFloat(_moveSpeedStr, GetCurrentMoveSpeed());
            // 通常の移動処理
            Vector3 cameraForward = _cameraTransform.forward;
            Vector3 cameraRight = _cameraTransform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 move = cameraRight * _moveDirection.x + cameraForward * _moveDirection.y;

            // ダッシュ時の速度変更
            if (_dashAction.IsPressed() && _stamina > 0)
            {
                _rb.velocity = move * _dashSpeed + new Vector3(0f, _rb.velocity.y, 0f);

                // スタミナ消費
                _stamina -= _staminaConsumptionRate * Time.deltaTime;
                if (_stamina < 0) _stamina = 0;
            }
            else
            {
                _rb.velocity = move * _speed + new Vector3(0f, _rb.velocity.y, 0f);
            }

            // 向きの変更
            if (move.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            }
        }
        else if (_isAvoiding)
        {
            // 回避中の移動処理
            Vector3 move = Vector3.zero;

            if (_avoidDirection.magnitude > 0)
            {
                // 方向キーが押されていた場合、その方向に回避
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
                // 方向キーが押されていない場合は、現在の向きで回避
                move = transform.forward;
            }

            // 一気にスタミナ消費して回避
            if (_stamina >= _avoidConsumptionAmount)
            {
                _rb.velocity = move * _dashSpeed + new Vector3(0f, _rb.velocity.y, 0f);
            }
            else
            {
                // スタミナが足りない場合は回避できない
                EndAvoid();
            }
        }

        // マップに現在の座標と向きを送る
        UpdatePlayerPositionOnMap();
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    protected virtual void Attack()
    {
        bool canAttack = !_attackNow && !_skillNow && !_specialNow;
        if (canAttack)
        {
            _attackNow = true;

            // 最寄りの敵を向く
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
        else
        {
            return;
        }
    }

    private Collider FindNearestEnemy()
    {
        // 一定範囲内の敵を検索
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
        // 攻撃終了時の処理
        _attackNow = false;
        _hitEnemies.Clear();
    }

    /// <summary>
    /// スキル使用可能な状態かの判定取得関数
    /// </summary>
    /// <returns></returns>
    protected bool CanSkill()
    {
        return _canUseSkill && !_skillNow && !_specialNow;
    }

    /// <summary>
    /// スキル処理
    /// </summary>
    protected virtual void Skill()
    {
        if (!CanSkill()) return;

        _canUseSkill = false;
        _skillCoolTimeCounter = _skillCoolTime;
        _skillNow = true;
        if (_attackNow) _attackNow = false;
        if (_isAvoiding) _isAvoiding = false;
        // 最寄りの敵を向く
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

    public virtual void EndSkill()
    {
        _skillNow = false;
        _hitEnemies.Clear();
    }

    /// <summary>
    /// スキルのクールタイムをチェック
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
    /// 必殺技のクールタイムをチェック
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

    protected bool CanSpecial()
    {
        return _canUseSpecial && !_specialNow && _specialPoint >= _useSpecialPoint;
    }

    /// <summary>
    /// 必殺技処理
    /// </summary>
    protected virtual void Special()
    {
        if (!CanSpecial()) return;

        _specialNow = true;
        _specialPoint -= _useSpecialPoint;
        if (_attackNow) _attackNow = false;
        if (_isAvoiding) _isAvoiding = false;
        // 最寄りの敵を向く
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

        // スタミナが足りていない場合は回避できない
        if (_stamina < _avoidConsumptionAmount)
        {
            return;
        }

        _animator.SetTrigger(_avoidStr);

        // 方向キーの入力を確認
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
        // 回避アニメーションが終わるのを待つ
        _isAvoiding = false;
    }

    private IEnumerator StartInvincibility()
    {
        // 無敵時間の開始
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
                // 死亡処理
                _alive = false;
                _gm.GameOver();
                _animator.SetTrigger(_dieStr);
            }
        }
    }

    public float GetCurrentMoveSpeed()
    {
        // 現在の移動速度を計算
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
        if (_gm.OpenSkillTree) return;
        if (_gm.OpenMap) return;

        if (!_gm.OpenSetting)
        {
            _gm.ViewSettingCanvas();
            //フィールドの入力を無効化
            DisableFieldAction();
            //設定の入力を有効化
            EnableSettingAction();
        }
        else
        {
            _gm.HideSettingCanvas();
            //フィールドの入力を有効化
            EnableFieldAction();
            //設定の入力を有効化
            DisableSettingAction();
        }
    }

    private void OnMap(InputAction.CallbackContext context)
    {
        if (_gm.OpenSetting) return;
        if (_gm.OpenSkillTree) return;

        if (!_gm.OpenMap)
        {
            _gm.ViewMapCanvas();
        }
        else
        {
            _gm.HideMapCanvas();
        }
    }

    private void OnSkillTree(InputAction.CallbackContext context)
    {
        if (_gm.OpenSetting) return;
        if (_gm.OpenMap) return;

        if (!_gm.OpenSkillTree)
        {
            _gm.ViewSkillTreeCanvas();
            //フィールドのアクションを無効化
            DisableFieldAction();
            //スキルツリーのアクションを有効化
            EnableSkillTreeAction();
        }
        else
        {
            _gm.HideSkillTreeCanvas();
            //フィールドのアクションを有効化
            EnableFieldAction();
            //スキルツリーのアクションを無効化
            DisableSkillTreeAction();
        }
    }

    private void OnUpButton_Setting(InputAction.CallbackContext context)
    {
        _settingController.SelectUp();
    }

    private void OnDownButton_Setting(InputAction.CallbackContext context)
    {
        _settingController.SelectDown();
    }

    private void OnRightButton_Setting(InputAction.CallbackContext context)
    {
        _settingController.SelectRight();
    }

    private void OnLeftButton_Setting(InputAction.CallbackContext context)
    {
        _settingController.SelectLeft();
    }

    private void OnDecisionButton_Setting(InputAction.CallbackContext context)
    {
        _settingController.Decision();
    }

    private void OnUpButton_SkillTree(InputAction.CallbackContext context)
    {
        _skillTreeController.SelectUp();
    }

    private void OnDownButton_SkillTree(InputAction.CallbackContext context)
    {
        _skillTreeController.SelectDown();
    }

    private void OnRightButton_SkillTree(InputAction.CallbackContext context)
    {
        _skillTreeController.StatusUp();
    }

    private void OnLeftButton_SkillTree(InputAction.CallbackContext context)
    {
        _skillTreeController.StatusDown();
    }

    private void OnDecisionButton_SkillTree(InputAction.CallbackContext context)
    {
        _skillTreeController.Decision();
    }

    private void OnDrawGizmosSelected()
    {
        // 攻撃範囲の可視化
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    //座標を移動(スタート時に使う)
    public void SetPos(Vector3 pos)
    {
        this.transform.position = pos;
    }

    /// <summary>
    /// マップにプレイヤーの位置と向きを更新
    /// </summary>
    private void UpdatePlayerPositionOnMap()
    {
        if (_mapController != null)
        {
            // 現在のプレイヤー座標 (Grid単位で送信)
            Vector3 position = transform.position;
            Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(position.x) / 4, Mathf.FloorToInt(position.z) / 4);

            // 現在の向き
            Vector3 forward = transform.forward;
            Vector2Int facingDirection = new Vector2Int(
                Mathf.RoundToInt(forward.x),
                Mathf.RoundToInt(forward.z)
            );

            // マップコントローラーを更新
            _mapController.UpdatePlayerPosition(gridPosition, facingDirection);
        }
    }
}

// EnemyController.cs
// 敵キャラクターの基本的な挙動を制御するスクリプト

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // 敵のステータス
    [SerializeField]
    private float _maxHp = 100f; // 最大HP
    private float _hp; // 現在のHP
    [SerializeField]
    protected float _attackPower = 100f; // 攻撃力
    [SerializeField]
    private float _defenses = 100f; // 防御力
    [SerializeField]
    private float _speed = 5f; // 移動速度
    [SerializeField]
    private float _vigilanceRange = 10f; // プレイヤーを警戒する範囲
    [SerializeField]
    private float _attackRange = 2f; // 攻撃可能な範囲
    [SerializeField]
    private float _attackCooldown = 1.5f; // 攻撃のクールダウン時間

    private bool _alive = true; // 生存状態フラグ
    private bool _isAttacking = false; // 攻撃中フラグ

    [SerializeField]
    private GameObject _hpBar; // HPバーオブジェクト
    private Slider _hpSlider; // HPバーのスライダーコンポーネント

    private Transform _player; // プレイヤーのTransform
    private bool _isChasing = false; // プレイヤーを追跡中フラグ
    private float _lastAttackTime = -Mathf.Infinity; // 最後に攻撃した時間
    private Rigidbody _rb; // 敵のRigidbody
    private Collider _collider; // 敵のコライダー
    private GameManager _gm;

    // 初期化処理
    protected virtual void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _gm = FindAnyObjectByType<GameManager>();

        _hp = _maxHp; // HPを初期化

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

    // 毎フレームの処理
    protected virtual void Update()
    {
        if (_player == null || _rb == null) return; // プレイヤーまたはRigidbodyが存在しない場合は処理をスキップ
        if (!_alive) return; // 敵が死んでいる場合も処理をスキップ

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position); // プレイヤーとの距離を計算

        if (!_isAttacking)
        {
            if (distanceToPlayer <= _vigilanceRange && distanceToPlayer > _attackRange)
            {
                _isChasing = true; // 警戒範囲内にプレイヤーがいる場合追跡開始
            }
            else if (distanceToPlayer <= _attackRange)
            {
                _isChasing = false;
                if (Time.time >= _lastAttackTime + _attackCooldown)
                {
                    AttackPlayer(); // 攻撃を実行
                    _lastAttackTime = Time.time; // 攻撃時間を記録
                }
            }
            else
            {
                _isChasing = false; // プレイヤーが範囲外の場合追跡停止
            }

            if (_isChasing)
            {
                ChasePlayer(); // プレイヤーを追跡
            }
            else
            {
                _rb.velocity = Vector3.zero; // 移動を停止
            }
        }

        if (_hp <= 0)
        {
            Die(); // HPが0以下の場合死亡処理を実行
        }

        _hpBar.transform.rotation = Camera.main.transform.rotation; // HPバーを常にカメラに向ける

        if (_hpSlider != null)
        {
            _hpSlider.value = _hp; // HPバーの値を更新
        }
    }

    // プレイヤーを追跡する処理
    protected void ChasePlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized; // プレイヤーへの方向を計算

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // プレイヤー方向を向く
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // 回転を補間

        _rb.velocity = direction * _speed; // 移動速度を設定
    }

    // プレイヤーを攻撃する処理
    protected virtual void AttackPlayer()
    {
        _isAttacking = true; // 攻撃中フラグを設定
        Vector3 direction = (_player.position - transform.position).normalized; // プレイヤー方向を計算
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // プレイヤー方向を向く
        transform.rotation = lookRotation; // 回転を設定
    }

    // 攻撃アニメーションの開始
    public virtual void StartAttack()
    {
    }

    // 攻撃アニメーションの終了
    public virtual void EndAttack()
    {
    }

    // 攻撃終了後の処理
    public virtual void EndAttackAnim()
    {
        _isAttacking = false; // 攻撃中フラグをリセット
    }

    // ダメージを受ける処理
    public virtual void Damage(float attack)
    {
        _hp -= attack; // HPを減少
    }

    // 死亡処理
    public virtual void Die()
    {
        _alive = false; // 生存状態を更新
        _hpBar.SetActive(false); // HPバーを非表示
        _collider.enabled = false; // コライダーを無効化
        if (_gm != null)
        {
            _gm.AddScore(10);
        }
    }

    // オブジェクトを削除する処理
    public virtual void DeleteObj()
    {
        Destroy(this.gameObject); // ゲームオブジェクトを削除
    }

    // 現在の移動速度を取得する処理
    protected virtual float GetCurrentMoveSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z); // 水平方向の速度を計算
        return horizontalVelocity.magnitude; // 移動速度を返す
    }
}

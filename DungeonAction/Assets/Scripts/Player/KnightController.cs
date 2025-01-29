using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightController : PlayerController
{
    [Header("武器設定")]
    [SerializeField]
    private GameObject _sowrdObj; // プレイヤーの武器オブジェクト
    private Collider _sowrdCollider; // 武器のコライダー

    protected override void Awake()
    {
        base.Awake();

        // 武器のコライダー設定
        if (_sowrdObj != null)
        {
            _sowrdCollider = _sowrdObj.GetComponent<Collider>();
            _sowrdCollider.enabled = false;
        }
    }

    protected override void Attack()
    {
        base.Attack();
        // 武器のコライダーを有効化
        if (_sowrdCollider != null)
        {
            _sowrdCollider.enabled = true;
        }
    }

    public override void EndAttack()
    {
        base.EndAttack();
        if (_sowrdCollider != null)
        {
            _sowrdCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 武器の攻撃判定
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
}

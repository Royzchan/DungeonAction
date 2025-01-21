using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonEnemyController : EnemyController
{
    private Animator _animator;
    private string _moveSpeedStr = "MoveSpeed";
    private string _attackStr = "Attack";
    private string _dieStr = "Die";

    [SerializeField]
    private GameObject _weaponObj;
    private Collider _weaponCollider;

    private HashSet<Collider> _hitPlayers = new HashSet<Collider>(); // 攻撃したプレイヤーの記録

    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        if (_weaponObj != null)
        {
            _weaponCollider = _weaponObj.GetComponent<Collider>();
            _weaponCollider.enabled = false; // 初期状態で無効化
        }
    }

    protected override void Update()
    {
        base.Update();
        _animator.SetFloat(_moveSpeedStr, GetCurrentMoveSpeed());
    }

    protected override void AttackPlayer()
    {
        base.AttackPlayer();
        if (_weaponCollider != null)
        {
            _hitPlayers.Clear(); // 攻撃対象の記録をクリア
        }
        _animator.SetTrigger(_attackStr); // 攻撃アニメーションの再生
    }

    public override void StartAttack()
    {
        if (_weaponCollider != null)
        {
            _weaponCollider.enabled = true; // 攻撃時にコライダーを有効化
        }
    }

    public override void EndAttack()
    {
        if (_weaponCollider != null)
        {
            _weaponCollider.enabled = false; // 攻撃終了時にコライダーを無効化
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_weaponCollider != null && _weaponCollider.enabled && !_hitPlayers.Contains(other))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Damage(_attackPower); // プレイヤーにダメージを与える
                _hitPlayers.Add(other); // 攻撃対象として記録
            }
        }
    }

    public override void Die()
    {
        base.Die();
        _animator.SetTrigger(_dieStr);
    }
}

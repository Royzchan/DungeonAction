using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ChestEnemyController : EnemyController
{
    private Animator _animator;
    private string _moveSpeedStr = "MoveSpeed";
    private string _attackStr = "Attack";
    private string _dieStr = "Die";
    [SerializeField]
    Collider _attackHitBox;

    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
    }
    protected override void Update()
    {
        base.Update();
        _animator.SetFloat(_moveSpeedStr, GetCurrentMoveSpeed());
    }

    protected override void AttackPlayer()
    {
        base.AttackPlayer();
        _animator.SetTrigger(_attackStr);
        // Add custom attack logic here
    }

    public override void StartAttack()
    {
        _attackHitBox.enabled = true;
    }

    public override void EndAttack()
    {
        base.EndAttack();
        _attackHitBox.enabled = false;
    }

    public override void Damage(float attack)
    {
        base.Damage(attack);
        Debug.Log("宝箱エネミーに攻撃hit");
    }

    public override void Die()
    {
        base.Die();
        _animator.SetTrigger(_dieStr);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_attackHitBox.enabled)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>().Damage(_attackPower);
            }
        }
    }
}

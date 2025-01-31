using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightController : PlayerController
{
    [Header("����ݒ�")]
    [SerializeField]
    private GameObject _sowrdObj; // �v���C���[�̕���I�u�W�F�N�g
    private Collider _sowrdCollider; // ����̃R���C�_�[
    private bool _firstHitSkill = false;

    protected override void Awake()
    {
        base.Awake();

        // ����̃R���C�_�[�ݒ�
        if (_sowrdObj != null)
        {
            _sowrdCollider = _sowrdObj.GetComponent<Collider>();
            _sowrdCollider.enabled = false;
        }
    }

    protected override void Attack()
    {
        base.Attack();
        // ����̃R���C�_�[��L����
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

    protected override void Skill()
    {
        base.Skill();
        if (_sowrdCollider != null)
        {
            _sowrdCollider.enabled = true;
        }
    }

    public override void EndSkill()
    {
        base.EndSkill();
        if (_sowrdCollider != null)
        {
            _sowrdCollider.enabled = false;
        }
    }

    protected override void Special()
    {
        base.Special();
        if (_sowrdCollider != null)
        {
            _sowrdCollider.enabled = true;
        }
    }

    public override void EndSpecial()
    {
        base.EndSpecial();
        if (_sowrdCollider != null)
        {
            _sowrdCollider.enabled = false;
        }
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
                if (_specialNow)
                {
                    enemy.Damage(_attackPower * _specialPowerUpRatio);
                }
                else if (_skillNow)
                {
                    enemy.Damage(_attackPower * _skillPowerUpRatio);
                }
                else
                {
                    enemy.Damage(_attackPower);
                }
                _hitEnemies.Add(other);
            }
        }
    }
}

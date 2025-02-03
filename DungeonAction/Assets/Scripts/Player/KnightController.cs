using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightController : PlayerController
{
    [Header("����ݒ�")]
    [SerializeField]
    private GameObject _sowrdObj; // �v���C���[�̕���I�u�W�F�N�g
    private Collider _sowrdCollider; // ����̃R���C�_�[
    [SerializeField]
    private Collider _skillRangeCollider;
    [SerializeField]
    private Collider _specialRangeCollider;
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
        if (_skillRangeCollider != null) _skillRangeCollider.enabled = false;
        if (_specialRangeCollider != null) _specialRangeCollider.enabled = false;
    }

    protected override void Attack()
    {
        base.Attack();
        // ����̃R���C�_�[��L����
        if (_sowrdCollider != null) _sowrdCollider.enabled = true;
    }

    public override void EndAttack()
    {
        base.EndAttack();
        if (_sowrdCollider != null) _sowrdCollider.enabled = false;
    }

    protected override void Skill()
    {
        if (!CanSkill()) return;
        _firstHitSkill = true;
        if (_sowrdCollider != null) _sowrdCollider.enabled = true;
        if (_skillRangeCollider != null) _skillRangeCollider.enabled = true;
        base.Skill();
    }

    public override void EndSkill()
    {
        base.EndSkill();
        if (_sowrdCollider != null) _sowrdCollider.enabled = false;
        if (_skillRangeCollider != null) _skillRangeCollider.enabled = false;
    }

    protected override void Special()
    {
        if (!CanSpecial()) return;
        if (_sowrdCollider != null) _sowrdCollider.enabled = true;
        if (_specialRangeCollider != null) _specialRangeCollider.enabled = true;
        base.Special();
    }

    public override void EndSpecial()
    {
        base.EndSpecial();
        if (_sowrdCollider != null) _sowrdCollider.enabled = false;
        if (_specialRangeCollider != null) _specialRangeCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
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
                        if (_firstHitSkill)
                        {
                            _specialPoint += _getSpecialPoint;
                            if (_specialPoint >= _maxSpecialPoint)
                            {
                                _specialPoint = _maxSpecialPoint;
                            }
                            _firstHitSkill = false;
                        }
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
}

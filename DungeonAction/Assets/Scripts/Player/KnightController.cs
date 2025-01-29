using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightController : PlayerController
{
    [Header("����ݒ�")]
    [SerializeField]
    private GameObject _sowrdObj; // �v���C���[�̕���I�u�W�F�N�g
    private Collider _sowrdCollider; // ����̃R���C�_�[

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

    private void OnTriggerEnter(Collider other)
    {
        // ����̍U������
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

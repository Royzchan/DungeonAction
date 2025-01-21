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

    private HashSet<Collider> _hitPlayers = new HashSet<Collider>(); // �U�������v���C���[�̋L�^

    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        if (_weaponObj != null)
        {
            _weaponCollider = _weaponObj.GetComponent<Collider>();
            _weaponCollider.enabled = false; // ������ԂŖ�����
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
            _hitPlayers.Clear(); // �U���Ώۂ̋L�^���N���A
        }
        _animator.SetTrigger(_attackStr); // �U���A�j���[�V�����̍Đ�
    }

    public override void StartAttack()
    {
        if (_weaponCollider != null)
        {
            _weaponCollider.enabled = true; // �U�����ɃR���C�_�[��L����
        }
    }

    public override void EndAttack()
    {
        if (_weaponCollider != null)
        {
            _weaponCollider.enabled = false; // �U���I�����ɃR���C�_�[�𖳌���
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_weaponCollider != null && _weaponCollider.enabled && !_hitPlayers.Contains(other))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Damage(_attackPower); // �v���C���[�Ƀ_���[�W��^����
                _hitPlayers.Add(other); // �U���ΏۂƂ��ċL�^
            }
        }
    }

    public override void Die()
    {
        base.Die();
        _animator.SetTrigger(_dieStr);
    }
}

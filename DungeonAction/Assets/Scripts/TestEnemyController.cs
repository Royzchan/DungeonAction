using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyController : EnemyController
{
    protected override void AttackPlayer()
    {
        Debug.Log("�e�X�g�G�l�~�[�̍U��");
        // Add custom attack logic here
    }

    public override void Damage(float attack)
    {
        base.Damage(attack);
        Debug.Log("�e�X�g�G�l�~�[�ɍU��hit");
    }
}

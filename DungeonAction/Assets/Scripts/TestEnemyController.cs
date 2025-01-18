using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyController : EnemyController
{
    protected override void AttackPlayer()
    {
        Debug.Log("テストエネミーの攻撃");
        // Add custom attack logic here
    }
}

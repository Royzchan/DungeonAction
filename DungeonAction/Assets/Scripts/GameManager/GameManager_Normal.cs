using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Normal : GameManager
{
    [SerializeField]
    private GameObject _bossEnemy;

    protected override void Start()
    {
        base.Start();
        StartTimer(); // �Q�[���J�n�Ɠ����Ƀ^�C�}�[���J�n
        if (_bossEnemy == null)
        {
            Debug.LogError("�{�X���o�^����Ă��܂���B");
        }
    }

    protected override void Update()
    {
        if (!_gamePlaying) return;
        if (_isTimerRunning)
        {
            _timer += Time.deltaTime; // �o�ߎ��Ԃ����Z
            UpdateTimerValues(); // ���E�b�E�~���b���X�V
            UpdateTimerUI(); // UI�̕\�����X�V
        }
        UpdateScoreUI();

        if (!AliveBossEnemy())
        {
            GameClear();
        }
    }

    private bool AliveBossEnemy()
    {
        if (_bossEnemy != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

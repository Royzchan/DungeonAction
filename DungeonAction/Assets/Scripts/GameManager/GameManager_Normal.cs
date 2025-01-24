using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Normal : GameManager
{
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
        int bossCount = _bossEnemy.Count;
        int deadBossNum = 0;
        foreach (GameObject boss in _bossEnemy)
        {
            if (boss == null)
            {
                deadBossNum++;
            }
        }
        if (deadBossNum == bossCount)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void ViewSettingCanvas()
    {
        base.ViewSettingCanvas();
        Time.timeScale = 0;
    }

    public override void HideSettingCanvas()
    {
        base.HideSettingCanvas();
        Time.timeScale = 1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Normal : GameManager
{
    protected override void Start()
    {
        base.Start();
        StartTimer(); // ゲーム開始と同時にタイマーを開始
        if (_bossEnemy == null)
        {
            Debug.LogError("ボスが登録されていません。");
        }
    }

    protected override void Update()
    {
        if (!_gamePlaying) return;
        if (_isTimerRunning)
        {
            _timer += Time.deltaTime; // 経過時間を加算
            UpdateTimerValues(); // 分・秒・ミリ秒を更新
            UpdateTimerUI(); // UIの表示を更新
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

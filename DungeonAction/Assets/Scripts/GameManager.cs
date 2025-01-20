using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UIコンポーネント用

public class GameManager : MonoBehaviour
{
    private float _timer = 0f; // タイマーの総経過時間
    private bool _isTimerRunning = false; // タイマーの開始/停止フラグ

    public int _minutes { get; private set; } // 分
    public int _seconds { get; private set; } // 秒
    public string _milliseconds { get; private set; } // 秒以下2桁（文字列）

    [SerializeField]
    private TMP_Text timerText; // UIのTextコンポーネントを参照
    [SerializeField]
    private GameObject _gameOverUIs;

    protected virtual void Start()
    {
        StartTimer(); // ゲーム開始と同時にタイマーを開始
        _gameOverUIs.SetActive(false);
    }

    protected virtual void Update()
    {
        if (_isTimerRunning)
        {
            _timer += Time.deltaTime; // 経過時間を加算
            UpdateTimerValues(); // 分・秒・ミリ秒を更新
            UpdateTimerUI(); // UIの表示を更新
        }
    }

    // タイマーの計算を行い、分・秒・ミリ秒の値を更新
    private void UpdateTimerValues()
    {
        _minutes = Mathf.FloorToInt(_timer / 60); // 分を計算
        _seconds = Mathf.FloorToInt(_timer % 60); // 秒を計算
        _milliseconds = ((_timer % 1) * 100).ToString("00"); // 小数点以下2桁を文字列で保存
    }

    // タイマーをUIに表示する
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"{_minutes:00}:{_seconds:00}:{_milliseconds}";
        }
    }

    // タイマーを開始する
    public void StartTimer()
    {
        _isTimerRunning = true;
        _timer = 0f; // タイマーをリセット
    }

    // タイマーを停止する
    public void StopTimer()
    {
        _isTimerRunning = false;
    }

    // タイマーの総経過時間を取得する
    public float GetTimer()
    {
        return _timer;
    }

    public void GameOver()
    {
        _gameOverUIs.SetActive(true);
    }
}

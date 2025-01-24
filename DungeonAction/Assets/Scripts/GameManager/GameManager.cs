using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UIコンポーネント用

public class GameManager : MonoBehaviour
{
    protected bool _gamePlaying = true;
    public bool GamePlaying { get { return _gamePlaying; } }
    private bool _openSetting = false;
    public bool OpenSetting { get { return _openSetting; } }
    private bool _openMap = false;
    public bool OpenMap { get { return _openMap; } }
    protected float _timer = 0f; // タイマーの総経過時間
    protected bool _isTimerRunning = false; // タイマーの開始/停止フラグ

    [SerializeField]
    private int _maxScore = 9999;
    protected int _score = 0; // ゲームのスコア

    public int _minutes { get; private set; } // 分
    public int _seconds { get; private set; } // 秒
    public string _milliseconds { get; private set; } // 秒以下2桁（文字列）

    [SerializeField]
    private TMP_Text _timerText; // UIのTextコンポーネントを参照
    [SerializeField]
    private TMP_Text _scoreText; // UIのTextコンポーネントを参照
    [SerializeField]
    private GameObject _gameClearUIs;
    [SerializeField]
    private GameObject _gameOverUIs;
    [SerializeField]
    private GameObject _settingCanvas;
    [SerializeField]
    private GameObject _mapCanvas;

    [SerializeField]
    protected List<GameObject> _bossEnemy;


    protected virtual void Start()
    {
        //フレームレートを固定
        Application.targetFrameRate = 60;
        _gameClearUIs.SetActive(false);
        _gameOverUIs.SetActive(false);
        if (_mapCanvas != null) _mapCanvas.SetActive(false);

        if (_settingCanvas != null) _settingCanvas.SetActive(false);
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
    protected void UpdateTimerValues()
    {
        _minutes = Mathf.FloorToInt(_timer / 60); // 分を計算
        _seconds = Mathf.FloorToInt(_timer % 60); // 秒を計算
        _milliseconds = ((_timer % 1) * 100).ToString("00"); // 小数点以下2桁を文字列で保存
    }

    // タイマーをUIに表示する
    protected void UpdateTimerUI()
    {
        if (_timerText != null)
        {
            _timerText.text = $"{_minutes:00}:{_seconds:00}:{_milliseconds}";
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

    //スコアのテキストを更新
    protected void UpdateScoreUI()
    {
        if (_scoreText != null)
        {
            _scoreText.text = "Score:" + _score.ToString();
        }
    }

    //スコアを追加
    public void AddScore(int score)
    {
        _score += score;
        if (_score >= _maxScore)
        {
            _score = _maxScore;
        }
    }

    public virtual void GameClear()
    {
        _gameClearUIs.SetActive(true);
        _gamePlaying = false;
        StopTimer();
    }

    public virtual void GameOver()
    {
        _gameOverUIs.SetActive(true);
        _gamePlaying = false;
        StopTimer();
    }

    public void AddBossEnemy(GameObject boss)
    {
        _bossEnemy.Add(boss);
    }

    public virtual void ViewSettingCanvas()
    {
        _settingCanvas.SetActive(true);
        _openSetting = true;
    }

    public virtual void HideSettingCanvas()
    {
        _settingCanvas.SetActive(false);
        _openSetting = false;
    }

    public virtual void ViewMapCanvas()
    {
        _mapCanvas.SetActive(true);
        _openMap = true;
    }

    public virtual void HideMapCanvas()
    {
        _mapCanvas.SetActive(false);
        _openMap = false;
    }
}

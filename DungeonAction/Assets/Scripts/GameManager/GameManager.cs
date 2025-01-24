using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI�R���|�[�l���g�p

public class GameManager : MonoBehaviour
{
    protected bool _gamePlaying = true;
    public bool GamePlaying { get { return _gamePlaying; } }
    private bool _openSetting = false;
    public bool OpenSetting { get { return _openSetting; } }
    private bool _openMap = false;
    public bool OpenMap { get { return _openMap; } }
    protected float _timer = 0f; // �^�C�}�[�̑��o�ߎ���
    protected bool _isTimerRunning = false; // �^�C�}�[�̊J�n/��~�t���O

    [SerializeField]
    private int _maxScore = 9999;
    protected int _score = 0; // �Q�[���̃X�R�A

    public int _minutes { get; private set; } // ��
    public int _seconds { get; private set; } // �b
    public string _milliseconds { get; private set; } // �b�ȉ�2���i������j

    [SerializeField]
    private TMP_Text _timerText; // UI��Text�R���|�[�l���g���Q��
    [SerializeField]
    private TMP_Text _scoreText; // UI��Text�R���|�[�l���g���Q��
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
        //�t���[�����[�g���Œ�
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
            _timer += Time.deltaTime; // �o�ߎ��Ԃ����Z
            UpdateTimerValues(); // ���E�b�E�~���b���X�V
            UpdateTimerUI(); // UI�̕\�����X�V
        }
    }

    // �^�C�}�[�̌v�Z���s���A���E�b�E�~���b�̒l���X�V
    protected void UpdateTimerValues()
    {
        _minutes = Mathf.FloorToInt(_timer / 60); // �����v�Z
        _seconds = Mathf.FloorToInt(_timer % 60); // �b���v�Z
        _milliseconds = ((_timer % 1) * 100).ToString("00"); // �����_�ȉ�2���𕶎���ŕۑ�
    }

    // �^�C�}�[��UI�ɕ\������
    protected void UpdateTimerUI()
    {
        if (_timerText != null)
        {
            _timerText.text = $"{_minutes:00}:{_seconds:00}:{_milliseconds}";
        }
    }

    // �^�C�}�[���J�n����
    public void StartTimer()
    {
        _isTimerRunning = true;
        _timer = 0f; // �^�C�}�[�����Z�b�g
    }

    // �^�C�}�[���~����
    public void StopTimer()
    {
        _isTimerRunning = false;
    }

    // �^�C�}�[�̑��o�ߎ��Ԃ��擾����
    public float GetTimer()
    {
        return _timer;
    }

    //�X�R�A�̃e�L�X�g���X�V
    protected void UpdateScoreUI()
    {
        if (_scoreText != null)
        {
            _scoreText.text = "Score:" + _score.ToString();
        }
    }

    //�X�R�A��ǉ�
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

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI�R���|�[�l���g�p

public class GameManager : MonoBehaviour
{
    private float _timer = 0f; // �^�C�}�[�̑��o�ߎ���
    private bool _isTimerRunning = false; // �^�C�}�[�̊J�n/��~�t���O

    public int _minutes { get; private set; } // ��
    public int _seconds { get; private set; } // �b
    public string _milliseconds { get; private set; } // �b�ȉ�2���i������j

    [SerializeField]
    private TMP_Text timerText; // UI��Text�R���|�[�l���g���Q��
    [SerializeField]
    private GameObject _gameOverUIs;

    protected virtual void Start()
    {
        StartTimer(); // �Q�[���J�n�Ɠ����Ƀ^�C�}�[���J�n
        _gameOverUIs.SetActive(false);
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
    private void UpdateTimerValues()
    {
        _minutes = Mathf.FloorToInt(_timer / 60); // �����v�Z
        _seconds = Mathf.FloorToInt(_timer % 60); // �b���v�Z
        _milliseconds = ((_timer % 1) * 100).ToString("00"); // �����_�ȉ�2���𕶎���ŕۑ�
    }

    // �^�C�}�[��UI�ɕ\������
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"{_minutes:00}:{_seconds:00}:{_milliseconds}";
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

    public void GameOver()
    {
        _gameOverUIs.SetActive(true);
    }
}

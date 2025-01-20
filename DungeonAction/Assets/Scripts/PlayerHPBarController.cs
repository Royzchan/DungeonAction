using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBarController : MonoBehaviour
{
    PlayerController _player;
    Slider _hpBar;
    float _currentHP;

    [SerializeField]
    float _baseSpeed = 5.0f; // ��{���x
    [SerializeField]
    float _speedMultiplier = 2.0f; // �����ʂɉ������{��

    [SerializeField]
    Image _fillImage; // �X���C�_�[��Fill������Image

    [SerializeField]
    Color _highHPColor = Color.green; // �̗͂��������̐F
    [SerializeField]
    Color _mediumHPColor = Color.yellow; // �̗͂������炢�̎��̐F
    [SerializeField]
    Color _lowHPColor = Color.red; // �̗͂����Ȃ����̐F

    [SerializeField, Range(0f, 1f)]
    float _mediumHPLimit = 0.5f; // ���F�ɕς�銄��
    [SerializeField, Range(0f, 1f)]
    float _lowHPLimit = 0.2f; // �ԐF�ɕς�銄��

    void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _hpBar = GetComponent<Slider>();
        _hpBar.maxValue = _player.MaxHP;
        _currentHP = _player.HP;
        _hpBar.value = _currentHP;

        // �X���C�_�[��Fill�������擾
        _fillImage = _hpBar.fillRect.GetComponent<Image>();
        UpdateBarColor();
    }

    void Update()
    {
        // HP���������ʂɉ����Č������x���v�Z
        float hpDifference = Mathf.Abs(_currentHP - _player.HP);
        float dynamicSpeed = _baseSpeed + hpDifference * _speedMultiplier;

        // ���݂�HP�Ɍ������ăQ�[�W�����X�ɓ�����
        _currentHP = Mathf.MoveTowards(_currentHP, _player.HP, dynamicSpeed * Time.deltaTime);
        _hpBar.value = _currentHP;

        // �Q�[�W�̐F���X�V
        UpdateBarColor();
    }

    void UpdateBarColor()
    {
        float hpPercentage = _currentHP / _player.MaxHP;

        if (hpPercentage <= _lowHPLimit)
        {
            _fillImage.color = _lowHPColor;
        }
        else if (hpPercentage <= _mediumHPLimit)
        {
            _fillImage.color = _mediumHPColor;
        }
        else
        {
            _fillImage.color = _highHPColor;
        }
    }
}

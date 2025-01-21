using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBarController : MonoBehaviour
{
    PlayerController _player;
    Slider _staminaBar;
    float _currentStamina;

    [SerializeField]
    float _baseSpeed = 5.0f; // ��{���x
    [SerializeField]
    float _speedMultiplier = 2.0f; // �����ʂɉ������{��

    [SerializeField]
    Image _fillImage; // �X���C�_�[��Fill������Image

    [SerializeField]
    Color _highStaminaColor = Color.green; // �X�^�~�i���������̐F
    [SerializeField]
    Color _lowStaminaColor = Color.red; // �X�^�~�i�����Ȃ����̐F

    [SerializeField, Range(0f, 1f)]
    float _lowHPLimit = 0.2f; // �ԐF�ɕς�銄��

    void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _staminaBar = GetComponent<Slider>();
        _staminaBar.maxValue = _player.MaxStamina;
        _currentStamina = _player.Stamina;
        _staminaBar.value = _currentStamina;

        // �X���C�_�[��Fill�������擾
        _fillImage = _staminaBar.fillRect.GetComponent<Image>();
        UpdateBarColor();
    }

    void Update()
    {
        // HP���������ʂɉ����Č������x���v�Z
        float hpDifference = Mathf.Abs(_currentStamina - _player.Stamina);
        float dynamicSpeed = _baseSpeed + hpDifference * _speedMultiplier;

        // ���݂�HP�Ɍ������ăQ�[�W�����X�ɓ�����
        _currentStamina = Mathf.MoveTowards(_currentStamina, _player.Stamina, dynamicSpeed * Time.deltaTime);
        _staminaBar.value = _currentStamina;

        // �Q�[�W�̐F���X�V
        UpdateBarColor();
    }

    void UpdateBarColor()
    {
        float hpPercentage = _currentStamina / _player.MaxStamina;

        if (hpPercentage <= _lowHPLimit)
        {
            _fillImage.color = _lowStaminaColor;
        }
        else
        {
            _fillImage.color = _highStaminaColor;
        }
    }
}

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
    float _baseSpeed = 5.0f; // 基本速度
    [SerializeField]
    float _speedMultiplier = 2.0f; // 減少量に応じた倍率

    [SerializeField]
    Image _fillImage; // スライダーのFill部分のImage

    [SerializeField]
    Color _highStaminaColor = Color.green; // スタミナが多い時の色
    [SerializeField]
    Color _lowStaminaColor = Color.red; // スタミナが少ない時の色

    [SerializeField, Range(0f, 1f)]
    float _lowHPLimit = 0.2f; // 赤色に変わる割合

    void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _staminaBar = GetComponent<Slider>();
        _staminaBar.maxValue = _player.MaxStamina;
        _currentStamina = _player.Stamina;
        _staminaBar.value = _currentStamina;

        // スライダーのFill部分を取得
        _fillImage = _staminaBar.fillRect.GetComponent<Image>();
        UpdateBarColor();
    }

    void Update()
    {
        // HPが減った量に応じて減少速度を計算
        float hpDifference = Mathf.Abs(_currentStamina - _player.Stamina);
        float dynamicSpeed = _baseSpeed + hpDifference * _speedMultiplier;

        // 現在のHPに向かってゲージを徐々に動かす
        _currentStamina = Mathf.MoveTowards(_currentStamina, _player.Stamina, dynamicSpeed * Time.deltaTime);
        _staminaBar.value = _currentStamina;

        // ゲージの色を更新
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

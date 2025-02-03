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
    float _baseSpeed = 5.0f; // 基本速度
    [SerializeField]
    float _speedMultiplier = 2.0f; // 減少量に応じた倍率

    [SerializeField]
    Image _fillImage; // スライダーのFill部分のImage

    [SerializeField]
    Color _highHPColor = Color.green; // 体力が多い時の色
    [SerializeField]
    Color _mediumHPColor = Color.yellow; // 体力が中くらいの時の色
    [SerializeField]
    Color _lowHPColor = Color.red; // 体力が少ない時の色

    [SerializeField, Range(0f, 1f)]
    float _mediumHPLimit = 0.5f; // 黄色に変わる割合
    [SerializeField, Range(0f, 1f)]
    float _lowHPLimit = 0.2f; // 赤色に変わる割合

    void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _hpBar = GetComponent<Slider>();
        _hpBar.maxValue = _player.MaxHP;
        _currentHP = _player.HP;
        _hpBar.value = _currentHP;

        // スライダーのFill部分を取得
        _fillImage = _hpBar.fillRect.GetComponent<Image>();
        UpdateBarColor();
    }

    void Update()
    {
        // HPが減った量に応じて減少速度を計算
        float hpDifference = Mathf.Abs(_currentHP - _player.HP);
        float dynamicSpeed = _baseSpeed + hpDifference * _speedMultiplier;

        // 現在のHPに向かってゲージを徐々に動かす
        _currentHP = Mathf.MoveTowards(_currentHP, _player.HP, dynamicSpeed * Time.deltaTime);
        _hpBar.value = _currentHP;

        // ゲージの色を更新
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpecialGageController : MonoBehaviour
{
    [SerializeField] private PlayerController _player; // プレイヤーの参照
    [SerializeField] private Slider _specialGage; // スライダーの参照
    [SerializeField] private Slider _overfillGage; // オーバーフロー表示用のスライダー
    [SerializeField] private float fillSpeed = 5f; // ゲージ増加速度

    private float _currentSpecialValue;
    private float _currentOverfillValue;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerController>();
    }

    void Start()
    {
        if (_specialGage == null)
        {
            Debug.LogError("スペシャルゲージがセットされていません");
        }
        if (_overfillGage == null)
        {
            Debug.LogError("オーバー分のスペシャルゲージがセットされていません");
        }
        _specialGage.maxValue = _player.UseSpecialPoint;
        _overfillGage.maxValue = _player.UseSpecialPoint;
        _currentSpecialValue = _specialGage.value;
        _currentOverfillValue = _overfillGage.value;
    }

    void Update()
    {
        UpdateGage();
    }

    private void UpdateGage()
    {
        float specialPoint = _player.SpecialPoint;
        float maxSpecialPoint = _player.MaxSpecialPoint;
        float useSpecialPoint = _player.UseSpecialPoint;

        // スライダーの値を滑らかに更新
        _currentSpecialValue = Mathf.Lerp(_currentSpecialValue, Mathf.Min(specialPoint, useSpecialPoint), Time.deltaTime * fillSpeed);
        _specialGage.value = _currentSpecialValue;

        // オーバーフロー部分の処理をスライダーで表示
        if (specialPoint > useSpecialPoint)
        {
            _overfillGage.gameObject.SetActive(true);
            _currentOverfillValue = Mathf.Lerp(_currentOverfillValue, specialPoint - useSpecialPoint, Time.deltaTime * fillSpeed);
            _overfillGage.value = _currentOverfillValue;
        }
        else
        {
            _overfillGage.gameObject.SetActive(false);
        }
    }
}

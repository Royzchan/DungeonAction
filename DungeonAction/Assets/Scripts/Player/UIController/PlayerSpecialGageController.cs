using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpecialGageController : MonoBehaviour
{
    [SerializeField] private PlayerController _player; // �v���C���[�̎Q��
    [SerializeField] private Slider _specialGage; // �X���C�_�[�̎Q��
    [SerializeField] private Slider _overfillGage; // �I�[�o�[�t���[�\���p�̃X���C�_�[
    [SerializeField] private float fillSpeed = 5f; // �Q�[�W�������x

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
            Debug.LogError("�X�y�V�����Q�[�W���Z�b�g����Ă��܂���");
        }
        if (_overfillGage == null)
        {
            Debug.LogError("�I�[�o�[���̃X�y�V�����Q�[�W���Z�b�g����Ă��܂���");
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

        // �X���C�_�[�̒l�����炩�ɍX�V
        _currentSpecialValue = Mathf.Lerp(_currentSpecialValue, Mathf.Min(specialPoint, useSpecialPoint), Time.deltaTime * fillSpeed);
        _specialGage.value = _currentSpecialValue;

        // �I�[�o�[�t���[�����̏������X���C�_�[�ŕ\��
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

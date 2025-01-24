using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    private CameraController _cameraController;
    [SerializeField]
    private float _maxRotateSpeed = 500;
    [SerializeField]
    private float _minRotateSpeed = 10;
    [SerializeField]
    private Slider _setRotateSlider_Yaw;
    [SerializeField]
    private Slider _setRotateSlider_Pitch;

    void Start()
    {
        _cameraController = Camera.main.GetComponent<CameraController>();

        if (_setRotateSlider_Yaw != null)
        {
            _setRotateSlider_Yaw.maxValue = _maxRotateSpeed;
            _setRotateSlider_Yaw.minValue = _minRotateSpeed;
        }
        //���x�ݒ�X���C�_�[�̍ő�l�A�ŏ��l��ݒ�
        if (_setRotateSlider_Pitch != null)
        {
            _setRotateSlider_Pitch.maxValue = _maxRotateSpeed;
            _setRotateSlider_Pitch.minValue = _minRotateSpeed;
        }
        //�X���C�_�[�̐��l�Ɍ��݂̃J�������x��ݒ�
        _setRotateSlider_Yaw.value = _cameraController.GetRotateSpeedYaw();
        _setRotateSlider_Pitch.value = _cameraController.GetRotateSpeedPitch();
    }

    private void Update()
    {
        _cameraController.SetRotateSpeedYaw(_setRotateSlider_Yaw.value);
        _cameraController.SetRotateSpeedPitch(_setRotateSlider_Pitch.value);
    }
}

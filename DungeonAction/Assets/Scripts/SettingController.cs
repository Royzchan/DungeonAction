using DG.Tweening;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    enum SelectList
    {
        Rotate_Yaw, //�J������X�����x�ݒ�
        Rotate_Pith,//�J������Y�����x�ݒ�
        Sound_BGM,  //BGM�̉��ʐݒ�
        Sound_SE,   //SE�̉��ʐݒ�
        ReturnGame, //�Q�[���ɖ߂�
        Gotitle     //�^�C�g���֖߂�
    }

    private CameraController _cameraController;
    [SerializeField]
    private float _maxRotateSpeed = 500;
    [SerializeField]
    private float _minRotateSpeed = 10;
    [SerializeField]
    private Slider _setRotateSlider_Yaw;
    [SerializeField]
    private Slider _setRotateSlider_Pitch;
    [SerializeField]
    private Slider _setSoundSlider_BGM;
    [SerializeField]
    private Slider _setSoundSlider_SE;
    [SerializeField]
    private List<GameObject> _UIs;
    private List<RectTransform> _UIsRect = new List<RectTransform>();
    private List<Vector2> _UIsSize = new List<Vector2>();
    [SerializeField]
    private float _upSizeScale = 1.1f;

    private int _nowSelect = 0;
    private int _maxSelect = 0;

    private void Awake()
    {
        _cameraController = Camera.main.GetComponent<CameraController>();

        //���x�ݒ�X���C�_�[�̍ő�l�A�ŏ��l��ݒ�
        if (_setRotateSlider_Yaw != null)
        {
            _setRotateSlider_Yaw.maxValue = _maxRotateSpeed;
            _setRotateSlider_Yaw.minValue = _minRotateSpeed;
        }
        if (_setRotateSlider_Pitch != null)
        {
            _setRotateSlider_Pitch.maxValue = _maxRotateSpeed;
            _setRotateSlider_Pitch.minValue = _minRotateSpeed;
        }

        foreach (GameObject ui in _UIs)
        {
            var uiRect = ui.GetComponent<RectTransform>();
            _UIsRect.Add(uiRect);
            _UIsSize.Add(uiRect.sizeDelta);
        }

        _maxSelect = _UIs.Count;
    }

    private void OnEnable()
    {
        _nowSelect = 0;
        //�X���C�_�[�̐��l�Ɍ��݂̃J�������x��ݒ�
        _setRotateSlider_Yaw.value = _cameraController.GetRotateSpeedYaw();
        _setRotateSlider_Pitch.value = _cameraController.GetRotateSpeedPitch();
        //�����ɃJ�����̐ݒ�������
        foreach (RectTransform rect in _UIsRect)
        {
            rect.localScale = Vector3.one;
        }
        if (_UIsRect[_nowSelect] != null) _UIsRect[_nowSelect].localScale = _UIsRect[_nowSelect].localScale * _upSizeScale;
    }

    private void OnDisable()
    {
        //�ݒ����鎞�ɐݒ���X�V
        _cameraController.SetRotateSpeedYaw(_setRotateSlider_Yaw.value);
        _cameraController.SetRotateSpeedPitch(_setRotateSlider_Pitch.value);
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
    }

    public void SelectUp()
    {
        _nowSelect--;
        Debug.Log(_nowSelect);
        if (_nowSelect < 0)
        {
            _nowSelect = 0;
            return;
        }
        _UIsRect[_nowSelect].DOScale(_UIsRect[_nowSelect].localScale * _upSizeScale, 0.3f).SetUpdate(true); ;
        _UIsRect[_nowSelect + 1].DOScale(Vector3.one, 0.3f).SetUpdate(true); ;
    }

    public void SelectDown()
    {
        _nowSelect++;
        Debug.Log(_nowSelect);
        Debug.Log(_maxSelect);
        if (_nowSelect >= _maxSelect)
        {
            _nowSelect = _maxSelect - 1;
            return;
        }
        _UIsRect[_nowSelect].DOScale(_UIsRect[_nowSelect].localScale * _upSizeScale, 0.3f).SetUpdate(true); ;
        _UIsRect[_nowSelect - 1].DOScale(Vector3.one, 0.3f).SetUpdate(true); ;
    }

    public void Decision()
    {

    }
}

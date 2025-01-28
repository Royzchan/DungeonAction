using DG.Tweening;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
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
    private GameManager _gm;
    private PlayerController _player;

    [SerializeField]
    private float _sliderMaxValue = 10;
    [SerializeField]
    private float _sliderMinValue = 1;
    [SerializeField]
    private float _rotateSpeedRatio = 50;
    [SerializeField]
    private float _soundRatio = 0.1f;
    [SerializeField]
    private Slider _setRotateSlider_Yaw;
    [SerializeField]
    private Slider _setRotateSlider_Pitch;
    [SerializeField]
    private Slider _setSoundSlider_BGM;
    [SerializeField]
    private Slider _setSoundSlider_SE;
    [SerializeField]
    private TMP_Text _setRotateValueText_Yaw;
    [SerializeField]
    private TMP_Text _setRotateValueText_Pitch;
    [SerializeField]
    private TMP_Text _setSoundValueText_BGM;
    [SerializeField]
    private TMP_Text _setSoundValueText_SE;
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
        _gm = FindAnyObjectByType<GameManager>();
        if (_gm == null) Debug.LogError("GameManager�����݂��Ă��܂���");

        //�X���C�_�[�̍ő�l�A�ŏ��l��ݒ�
        if (_setRotateSlider_Yaw != null)
        {
            _setRotateSlider_Yaw.maxValue = _sliderMaxValue;
            _setRotateSlider_Yaw.minValue = _sliderMinValue;
        }
        if (_setRotateSlider_Pitch != null)
        {
            _setRotateSlider_Pitch.maxValue = _sliderMaxValue;
            _setRotateSlider_Pitch.minValue = _sliderMinValue;
        }
        if (_setSoundSlider_BGM != null)
        {
            _setSoundSlider_BGM.maxValue = _sliderMaxValue;
            _setSoundSlider_BGM.minValue = _sliderMinValue;
        }
        if (_setSoundSlider_SE != null)
        {
            _setSoundSlider_SE.maxValue = _sliderMaxValue;
            _setSoundSlider_SE.minValue = _sliderMinValue;
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
        _setRotateSlider_Yaw.value = _cameraController.GetRotateSpeedYaw() / _rotateSpeedRatio;
        _setRotateValueText_Yaw.text = "< " + _setRotateSlider_Yaw.value + " >";
        _setRotateSlider_Pitch.value = _cameraController.GetRotateSpeedPitch() / _rotateSpeedRatio;
        _setRotateValueText_Pitch.text = "< " + _setRotateSlider_Pitch.value + " >";
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
        _cameraController.SetRotateSpeedYaw(_setRotateSlider_Yaw.value * _rotateSpeedRatio);
        _cameraController.SetRotateSpeedPitch(_setRotateSlider_Pitch.value * _rotateSpeedRatio);
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
        if (_nowSelect >= _maxSelect)
        {
            _nowSelect = _maxSelect - 1;
            return;
        }
        _UIsRect[_nowSelect].DOScale(_UIsRect[_nowSelect].localScale * _upSizeScale, 0.3f).SetUpdate(true); ;
        _UIsRect[_nowSelect - 1].DOScale(Vector3.one, 0.3f).SetUpdate(true); ;
    }

    public void SelectRight()
    {
        switch (_nowSelect)
        {
            case (int)SelectList.Rotate_Yaw:
                _setRotateSlider_Yaw.value += 1;
                _setRotateValueText_Yaw.text = "< " + _setRotateSlider_Yaw.value + " >";
                break;
            case (int)SelectList.Rotate_Pith:
                _setRotateSlider_Pitch.value += 1;
                _setRotateValueText_Pitch.text = "< " + _setRotateSlider_Pitch.value + " >";
                break;
        }
    }

    public void SelectLeft()
    {
        switch (_nowSelect)
        {
            case (int)SelectList.Rotate_Yaw:
                _setRotateSlider_Yaw.value -= 1;
                _setRotateValueText_Yaw.text = "< " + _setRotateSlider_Yaw.value + " >";
                break;
            case (int)SelectList.Rotate_Pith:
                _setRotateSlider_Pitch.value -= 1;
                _setRotateValueText_Pitch.text = "< " + _setRotateSlider_Pitch.value + " >";
                break;
        }
    }

    public void Decision()
    {
        switch (_nowSelect)
        {
            case (int)SelectList.ReturnGame:
                if (_gm != null) _gm.HideSettingCanvas();
                _player.EnableFieldAction();
                _player.DisableSettingAction();
                break;

            case (int)SelectList.Gotitle:
                if (_gm != null) _gm.GoTitle();
                break;
        }
    }

    public void SetPlayer(PlayerController player)
    {
        _player = player;
    }
}

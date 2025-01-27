using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Drawing;

public class TitleUIController : MonoBehaviour
{
    protected TitleManager _tm;
    protected int _nowSelect = 0; //���ݑI��ł���I����
    private int _maxSelect = 0; //�I�����̍ő吔
    [SerializeField]
    private GameObject[] _buttons; //�Ǘ�����{�^��
    private List<RectTransform> _buttonsRect = new List<RectTransform>();
    private List<Vector2> _firstButtonSize = new List<Vector2>();

    void Awake()
    {
        _tm = FindAnyObjectByType<TitleManager>();
        _maxSelect = _buttons.Length;
        foreach (GameObject button in _buttons)
        {
            var buttonRectTransform = button.GetComponent<RectTransform>();
            _buttonsRect.Add(buttonRectTransform);
            _firstButtonSize.Add(buttonRectTransform.sizeDelta);
        }
    }

    protected void OnEnable()
    {
        if (_tm == null)
        {
            _tm = FindAnyObjectByType<TitleManager>();
        }
        _nowSelect = 0;

        for (int i = 0; i < _buttonsRect.Count; i++)
        {
            _buttonsRect[i].sizeDelta = _firstButtonSize[i];
        }
        if (_buttonsRect[_nowSelect] != null) _buttonsRect[_nowSelect].sizeDelta = _firstButtonSize[_nowSelect] * 1.2f;
    }

    /// <summary>
    /// ���̑I������I������֐�
    /// </summary>
    public virtual void SelectUp()
    {
        _nowSelect--;
        if (_nowSelect < 0)
        {
            _nowSelect = 0;
            return;
        }
        if (_buttonsRect[_nowSelect] != null) _buttonsRect[_nowSelect].DOSizeDelta(_firstButtonSize[_nowSelect] * 1.2f, 0.3f);
        if (_buttonsRect[_nowSelect + 1] != null) _buttonsRect[_nowSelect + 1].DOSizeDelta(_firstButtonSize[_nowSelect + 1], 0.3f);
    }

    /// <summary>
    /// ����̑I������I������֐�
    /// </summary>
    public virtual void SelectDown()
    {
        _nowSelect++;
        if (_nowSelect >= _maxSelect)
        {
            //enum�ŊǗ��������̂őI�����̍ő吔�̈����
            _nowSelect = _maxSelect - 1;
            return;
        }
        if (_buttonsRect[_nowSelect] != null) _buttonsRect[_nowSelect].DOSizeDelta(_firstButtonSize[_nowSelect] * 1.2f, 0.3f);
        if (_buttonsRect[_nowSelect - 1] != null) _buttonsRect[_nowSelect - 1].DOSizeDelta(_firstButtonSize[_nowSelect - 1], 0.3f);
    }

    /// <summary>
    /// ���݂̑I�����Ŋm�肳����
    /// </summary>
    public virtual void Decision()
    {

    }
}

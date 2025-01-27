using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Drawing;

public class TitleUIController : MonoBehaviour
{
    protected TitleManager _tm;
    protected int _nowSelect = 0; //現在選んでいる選択肢
    private int _maxSelect = 0; //選択肢の最大数
    [SerializeField]
    private GameObject[] _buttons; //管理するボタン
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
    /// 一個上の選択肢を選択する関数
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
    /// 一個下の選択肢を選択する関数
    /// </summary>
    public virtual void SelectDown()
    {
        _nowSelect++;
        if (_nowSelect >= _maxSelect)
        {
            //enumで管理したいので選択肢の最大数の一個下に
            _nowSelect = _maxSelect - 1;
            return;
        }
        if (_buttonsRect[_nowSelect] != null) _buttonsRect[_nowSelect].DOSizeDelta(_firstButtonSize[_nowSelect] * 1.2f, 0.3f);
        if (_buttonsRect[_nowSelect - 1] != null) _buttonsRect[_nowSelect - 1].DOSizeDelta(_firstButtonSize[_nowSelect - 1], 0.3f);
    }

    /// <summary>
    /// 現在の選択肢で確定させる
    /// </summary>
    public virtual void Decision()
    {

    }
}

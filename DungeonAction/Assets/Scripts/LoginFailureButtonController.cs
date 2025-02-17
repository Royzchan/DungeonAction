using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoginFailureButtonController : MonoBehaviour
{
    enum SelectList
    {
        GamePlay, // ゲームをプレイ
        GameEnd // ゲーム終了
    }

    private SelectList _nowSelect = SelectList.GamePlay;

    [SerializeField]
    private List<RectTransform> _buttonsRect = new List<RectTransform>();
    [SerializeField]
    private Vector3 _selectSize = new Vector3(1.2f, 1.2f, 1.2f);

    [SerializeField]
    InputAction _rightAction;
    [SerializeField]
    InputAction _leftAction;
    [SerializeField]
    InputAction _decisionAction;

    private void OnEnable()
    {
        _nowSelect = SelectList.GamePlay;
        foreach (var button in _buttonsRect)
        {
            button.localScale = Vector3.one;
        }
        _buttonsRect[(int)_nowSelect].localScale = _selectSize;

        _rightAction?.Enable();
        _rightAction.started += SelectNext;
        _leftAction?.Enable();
        _leftAction.started += SelectBefore;
        _decisionAction?.Enable();
        _decisionAction.started += Decision;
    }

    private void OnDisable()
    {
        _rightAction?.Disable();
        _rightAction.started -= SelectNext;
        _leftAction?.Disable();
        _leftAction.started -= SelectBefore;
        _decisionAction?.Disable();
        _decisionAction.started -= Decision;
    }

    private void SelectNext(InputAction.CallbackContext context)
    {
        int selected = (int)_nowSelect;
        _nowSelect++;
        if (_nowSelect > SelectList.GameEnd)
        {
            _nowSelect = SelectList.GamePlay;
        }
        _buttonsRect[selected].DOScale(Vector3.one, 0.3f).SetUpdate(true);
        _buttonsRect[(int)_nowSelect].DOScale(_selectSize, 0.3f).SetUpdate(true);
    }

    private void SelectBefore(InputAction.CallbackContext context)
    {
        int selected = (int)_nowSelect;
        _nowSelect--;
        if (_nowSelect < SelectList.GamePlay)
        {
            _nowSelect = SelectList.GameEnd;
        }
        _buttonsRect[selected].DOScale(Vector3.one, 0.3f).SetUpdate(true);
        _buttonsRect[(int)_nowSelect].DOScale(_selectSize, 0.3f).SetUpdate(true);
    }

    private void Decision(InputAction.CallbackContext context)
    {
        switch (_nowSelect)
        {
            case SelectList.GamePlay:
                this.gameObject.SetActive(false);
                PlayFabController.Instance.GameStart_Offline();
                break;

            case SelectList.GameEnd:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
                break;
        }
    }
}

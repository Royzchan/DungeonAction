using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class TitleManager : MonoBehaviour
{
    enum Scene
    {
        Top,
        ModeSelect,
        Setting,
        Ranking
    }
    private Scene _nowScene = Scene.Top;

    [SerializeField]
    private List<GameObject> _sceneCanvas;
    private List<TitleUIController> _UIController = new List<TitleUIController>();
    private TitleUIController _nowUIController;

    [SerializeField]
    InputAction _upAction;
    [SerializeField]
    InputAction _downAction;
    [SerializeField]
    InputAction _decisionAction;

    private void OnEnable()
    {
        _upAction?.Enable();
        _upAction.started += OnUpButton;
        _downAction?.Enable();
        _downAction.started += OnDownButton;
        _decisionAction?.Enable();
        _decisionAction.started += OnDecisionButton;
    }

    private void OnOnDisable()
    {
        _upAction?.Disable();
        _upAction.started -= OnUpButton;
        _downAction?.Disable();
        _downAction.started -= OnDownButton;
        _decisionAction?.Disable();
        _decisionAction.started -= OnDecisionButton;
    }

    void Start()
    {
        foreach (GameObject canvas in _sceneCanvas)
        {
            var UIController = canvas.GetComponent<TitleUIController>();

            if (UIController != null)
            {
                _UIController.Add(UIController);
            }
            else
            {
                Debug.LogError(canvas.name + "Ç…TitleUIControllerÇ™Ç¬Ç¢ÇƒÇ¢Ç‹ÇπÇÒÅB");
            }
            canvas.SetActive(false);
        }
        if (_UIController[(int)Scene.Top] != null) _UIController[(int)Scene.Top].gameObject.SetActive(true);
        if (_UIController[(int)Scene.Top] != null) _nowUIController = _UIController[(int)Scene.Top];
    }

    void Update()
    {

    }

    public void GoTop()
    {
        _nowScene = Scene.Top;
        foreach (GameObject canvas in _sceneCanvas)
        {
            canvas.SetActive(false);
        }
        if (_sceneCanvas[(int)Scene.Top] != null) _sceneCanvas[(int)Scene.Top].SetActive(true);
        if (_UIController[(int)Scene.Top] != null) _nowUIController = _UIController[(int)Scene.Top];
    }

    public void GoModeSelect()
    {
        _nowScene = Scene.ModeSelect;
        foreach (GameObject canvas in _sceneCanvas)
        {
            canvas.SetActive(false);
        }
        if (_sceneCanvas[(int)Scene.ModeSelect] != null) _sceneCanvas[(int)Scene.ModeSelect].SetActive(true);
        if (_UIController[(int)Scene.ModeSelect] != null) _nowUIController = _UIController[(int)Scene.ModeSelect];
    }

    public void GoSetting()
    {
        _nowScene = Scene.Setting;
        foreach (GameObject canvas in _sceneCanvas)
        {
            canvas.SetActive(false);
        }
        if (_sceneCanvas[(int)Scene.Setting] != null) _sceneCanvas[(int)Scene.Setting].SetActive(true);
        if (_UIController[(int)Scene.Setting] != null) _nowUIController = _UIController[(int)Scene.Setting];
    }

    public void GoRanking()
    {
        _nowScene = Scene.Ranking;
        foreach (GameObject canvas in _sceneCanvas)
        {
            canvas.SetActive(false);
        }
        if (_sceneCanvas[(int)Scene.Ranking] != null) _sceneCanvas[(int)Scene.Ranking].SetActive(true);
        if (_UIController[(int)Scene.Ranking] != null) _nowUIController = _UIController[(int)Scene.Ranking];
    }

    private void OnUpButton(InputAction.CallbackContext context)
    {
        if (_nowUIController != null)
        {
            _nowUIController.SelectUp();
        }
    }

    private void OnDownButton(InputAction.CallbackContext context)
    {
        if (_nowUIController != null)
        {
            _nowUIController.SelectDown();
        }
    }

    private void OnDecisionButton(InputAction.CallbackContext context)
    {
        if (_nowUIController != null)
        {
            _nowUIController.Decision();
        }
    }
}
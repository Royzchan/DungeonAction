using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    enum Scene
    {
        Top,
        ModeSelect,
        Setting,
        SkillTree,
        Ranking
    }
    private Scene _nowScene = Scene.Top;

    [SerializeField]
    private List<GameObject> _sceneCanvas;
    private List<TitleUIController> _UIController = new List<TitleUIController>();
    private TitleUIController _nowUIController;
    [SerializeField]
    private GameObject _skillTreeControllerCanvas;
    private SkillTreeController _skillTreeController;

    [SerializeField]
    InputAction _upAction;
    [SerializeField]
    InputAction _downAction;
    [SerializeField]
    InputAction _decisionAction;

    [SerializeField]
    private string _sceneName_Normal = "TestScene";
    [SerializeField]
    private string _sceneName_Endless = "TestScene";

    private void OnEnable()
    {
        _upAction?.Enable();
        _upAction.started += OnUpButton;
        _downAction?.Enable();
        _downAction.started += OnDownButton;
        _decisionAction?.Enable();
        _decisionAction.started += OnDecisionButton;
    }

    private void OnDisable()
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
        if (_skillTreeControllerCanvas != null)
            _skillTreeController = _skillTreeControllerCanvas.GetComponent<SkillTreeController>();
        _skillTreeControllerCanvas.SetActive(false);
        foreach (GameObject canvas in _sceneCanvas)
        {
            var UIController = canvas.GetComponent<TitleUIController>();

            if (UIController != null)
            {
                _UIController.Add(UIController);
            }
            else
            {
                Debug.LogError(canvas.name + "にTitleUIControllerがついていません。");
            }
            canvas.SetActive(false);
        }
        if (_UIController[(int)Scene.Top] != null) _UIController[(int)Scene.Top].gameObject.SetActive(true);
        if (_UIController[(int)Scene.Top] != null) _nowUIController = _UIController[(int)Scene.Top];
    }

    void Update()
    {

    }

    /// <summary>
    /// タイトルシーンのトップ画面へ移動
    /// </summary>
    public void GoTop()
    {
        _nowScene = Scene.Top;
        foreach (GameObject canvas in _sceneCanvas)
        {
            canvas.SetActive(false);
        }
        _skillTreeControllerCanvas.SetActive(false);
        if (_sceneCanvas[(int)Scene.Top] != null) _sceneCanvas[(int)Scene.Top].SetActive(true);
        if (_UIController[(int)Scene.Top] != null) _nowUIController = _UIController[(int)Scene.Top];
    }

    /// <summary>
    /// タイトルシーンのモードセレクト画面へ移動
    /// </summary>
    public void GoModeSelect()
    {
        _nowScene = Scene.ModeSelect;
        foreach (GameObject canvas in _sceneCanvas)
        {
            canvas.SetActive(false);
        }
        _skillTreeControllerCanvas.SetActive(false);
        if (_sceneCanvas[(int)Scene.ModeSelect] != null) _sceneCanvas[(int)Scene.ModeSelect].SetActive(true);
        if (_UIController[(int)Scene.ModeSelect] != null) _nowUIController = _UIController[(int)Scene.ModeSelect];
    }

    /// <summary>
    /// タイトルシーンの設定画面へ移動
    /// </summary>
    public void GoSetting()
    {
        _nowScene = Scene.Setting;
        foreach (GameObject canvas in _sceneCanvas)
        {
            canvas.SetActive(false);
        }
        _skillTreeControllerCanvas.SetActive(false);
        if (_sceneCanvas[(int)Scene.Setting] != null) _sceneCanvas[(int)Scene.Setting].SetActive(true);
        if (_UIController[(int)Scene.Setting] != null) _nowUIController = _UIController[(int)Scene.Setting];
    }

    /// <summary>
    /// スキルツリーを見る
    /// </summary>
    public void GoSkillTree()
    {
        _nowScene = Scene.SkillTree;
        foreach (GameObject canvas in _sceneCanvas)
        {
            canvas.SetActive(false);
        }
        _skillTreeControllerCanvas.SetActive(true);
        _nowUIController = null;
    }

    /// <summary>
    /// タイトルシーンのランキング画面へ移動
    /// </summary>
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

    /// <summary>
    /// ノーマルモードでゲームスタート
    /// </summary>
    public void GameStart_Normal()
    {
        SceneManager.LoadScene(_sceneName_Normal);
    }

    /// <summary>
    /// エンドレスモードでゲームスタート
    /// </summary>
    public void GameStartEndless()
    {
        SceneManager.LoadScene(_sceneName_Endless);
    }
}
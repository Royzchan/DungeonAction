using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class SkillTreeController : MonoBehaviour
{
    enum GameMode
    {
        Normal,
        Challenge
    }

    enum SkillTreeList
    {
        HP, // HP
        Attack, // 攻撃力
        Defense, // 防御力
        SkillPower, // スキル効果倍率
        SpecialPower, // 必殺技効果倍率
        CloseButton
    }

    private TitleManager _tm;
    [SerializeField]
    private GameMode _mode = GameMode.Normal;
    [SerializeField]
    private TMP_Text _spNum;
    [SerializeField]
    private TMP_Text _hpNum;
    [SerializeField]
    private TMP_Text _attackNum;
    [SerializeField]
    private TMP_Text _defenseNum;
    [SerializeField]
    private TMP_Text _skillPowerNum;
    [SerializeField]
    private TMP_Text _specialPowerNum;

    [SerializeField]
    private GameObject _hpTexts;
    [SerializeField]
    private GameObject _attackTexts;
    [SerializeField]
    private GameObject _defenseTexts;
    [SerializeField]
    private GameObject _skillPowerTexts;
    [SerializeField]
    private GameObject _specialPowerTexts;
    [SerializeField]
    private GameObject _closeButton;

    private int _nowSelect = (int)SkillTreeList.HP;

    private List<RectTransform> _textRects = new List<RectTransform>();
    [SerializeField]
    private float _upSizeScale = 1.1f;

    private void Awake()
    {
        _tm = FindAnyObjectByType<TitleManager>();

        if (_hpTexts != null) _textRects.Add(_hpTexts.GetComponent<RectTransform>());
        if (_attackTexts != null) _textRects.Add(_attackTexts.GetComponent<RectTransform>());
        if (_defenseTexts != null) _textRects.Add(_defenseTexts.GetComponent<RectTransform>());
        if (_skillPowerTexts != null) _textRects.Add(_skillPowerTexts.GetComponent<RectTransform>());
        if (_specialPowerTexts != null) _textRects.Add(_specialPowerTexts.GetComponent<RectTransform>());
        if (_closeButton != null) _textRects.Add(_closeButton.GetComponent<RectTransform>());
    }

    void Start()
    {
        if (PlayerLevelController.Instance != null)
        {
            _spNum.text = PlayerLevelController.Instance.SkillPoint.ToString();
            _hpNum.text = PlayerLevelController.Instance.HP.ToString();
            _attackNum.text = PlayerLevelController.Instance.Attack.ToString();
            _defenseNum.text = PlayerLevelController.Instance.Defense.ToString();
            _skillPowerNum.text = PlayerLevelController.Instance.SkillPowerUpRatio.ToString();
            _specialPowerNum.text = PlayerLevelController.Instance.SpecialPowerUpRatio.ToString();
        }
    }

    private void OnEnable()
    {
        _nowSelect = (int)SkillTreeList.HP;
        if (PlayerLevelController.Instance != null)
        {
            _spNum.text = PlayerLevelController.Instance.SkillPoint.ToString();
            _hpNum.text = PlayerLevelController.Instance.HP.ToString();
            _attackNum.text = PlayerLevelController.Instance.Attack.ToString();
            _defenseNum.text = PlayerLevelController.Instance.Defense.ToString();
            _skillPowerNum.text = PlayerLevelController.Instance.SkillPowerUpRatio.ToString();
            _specialPowerNum.text = PlayerLevelController.Instance.SpecialPowerUpRatio.ToString();
        }
        foreach (RectTransform textsRect in _textRects)
        {
            textsRect.localScale = Vector3.one;
        }
        if (_textRects[_nowSelect] != null) _textRects[_nowSelect].localScale = _textRects[_nowSelect].localScale * _upSizeScale;
    }

    /// <summary>
    /// 選択しているステータスを前に戻す
    /// </summary>
    public void SelectUp()
    {
        _nowSelect--;
        if (_nowSelect < (int)SkillTreeList.HP)
        {
            _nowSelect = (int)SkillTreeList.HP;
            return;
        }
        _textRects[_nowSelect].DOScale(_textRects[_nowSelect].localScale * _upSizeScale, 0.3f).SetUpdate(true);
        _textRects[_nowSelect + 1].DOScale(Vector3.one, 0.3f).SetUpdate(true);
    }

    /// <summary>
    /// 選択しているステータスを次に進める
    /// </summary>
    public void SelectDown()
    {
        _nowSelect++;
        if (_nowSelect > (int)SkillTreeList.CloseButton)
        {
            _nowSelect = (int)SkillTreeList.CloseButton;
            return;
        }
        _textRects[_nowSelect].DOScale(_textRects[_nowSelect].localScale * _upSizeScale, 0.3f).SetUpdate(true);
        _textRects[_nowSelect - 1].DOScale(Vector3.one, 0.3f).SetUpdate(true);
    }

    /// <summary>
    /// 選択しているステータスを上げる
    /// </summary>
    public void StatusUp()
    {

    }

    /// <summary>
    /// 選択しているステータスを下げる
    /// </summary>
    public void StatusDown()
    {

    }

    /// <summary>
    /// ステータスをリセット
    /// </summary>
    public void StatusReset()
    {

    }

    /// <summary>
    /// 決定が押された時の処理
    /// </summary>
    public void Decision()
    {
        if (_nowSelect == (int)SkillTreeList.CloseButton)
        {
            if (_tm != null) _tm.GoTop();
        }
    }
}

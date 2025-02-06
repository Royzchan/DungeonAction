using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class SkillTreeController : MonoBehaviour
{
    enum GameMode
    {
        Normal, //通常モード
        Challenge_TimeAttack, // チャレンジモードのタイムアタック
        Challenge_Endless //チャレンジモードのエンドレス
    }

    enum SkillTreeList
    {
        HP, // HP
        Attack, // 攻撃力
        Defense, // 防御力
        SkillPower, // スキル効果倍率
        SpecialPower, // 必殺技効果倍率
        ResetButton, // リセットボタン
        CloseButton // 閉じるボタン
    }

    private TitleManager _tm;
    private GameManager _gm;

    //各ステータスの初期ステータス
    private float _firstSp = 100;
    private float _firstHp = 100;
    private float _firstAttack = 100;
    private float _firstDefense = 100;
    private float _firstSkillPowerUpRatio = 1.5f;
    private float _firstSpecialPowerUpRatio = 1.5f;

    private float _sp; // スキルポイント
    private int _maxHaveSkillPoint; // 所持できるスキルポイントの最大値
    private float _hp; // HP
    private float _attack; // 攻撃力
    private float _defense; // 防御力
    private float _skillPowerUpRatio; // スキル効果倍率
    private float _specialPowerUpRatio; // 必殺技効果倍率

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
    private GameObject _resetButton;
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
        if (_resetButton != null) _textRects.Add(_resetButton.GetComponent<RectTransform>());
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

            if (_tm != null || _mode == GameMode.Normal)
            {
                //スキルポイント関係をセット
                _sp = PlayerLevelController.Instance.SkillPoint;
                _maxHaveSkillPoint = PlayerLevelController.Instance.MaxHaveSkillPoint;
                //各ステータスの初期値をセット
                _firstHp = PlayerLevelController.Instance.FirstHp;
                _firstAttack = PlayerLevelController.Instance.FirstAttack;
                _firstDefense = PlayerLevelController.Instance.FirstDefense;
                _firstSkillPowerUpRatio = PlayerLevelController.Instance.FirstSkillPowerUpRatio;
                _firstSpecialPowerUpRatio = PlayerLevelController.Instance.FirstSpecialPowerUpRatio;
                //各ステータスをセット
                _hp = PlayerLevelController.Instance.HP;
                _attack = PlayerLevelController.Instance.Attack;
                _defense = PlayerLevelController.Instance.Defense;
                _skillPowerUpRatio = PlayerLevelController.Instance.SkillPowerUpRatio;
                _specialPowerUpRatio = PlayerLevelController.Instance.SpecialPowerUpRatio;
            }
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

            if (_tm != null || _mode == GameMode.Normal)
            {
                //スキルポイント関係をセット
                _sp = PlayerLevelController.Instance.SkillPoint;
                _maxHaveSkillPoint = PlayerLevelController.Instance.MaxHaveSkillPoint;
                //各ステータスの初期値をセット
                _firstHp = PlayerLevelController.Instance.FirstHp;
                _firstAttack = PlayerLevelController.Instance.FirstAttack;
                _firstDefense = PlayerLevelController.Instance.FirstDefense;
                _firstSkillPowerUpRatio = PlayerLevelController.Instance.FirstSkillPowerUpRatio;
                _firstSpecialPowerUpRatio = PlayerLevelController.Instance.FirstSpecialPowerUpRatio;
                //各ステータスをセット
                _hp = PlayerLevelController.Instance.HP;
                _attack = PlayerLevelController.Instance.Attack;
                _defense = PlayerLevelController.Instance.Defense;
                _skillPowerUpRatio = PlayerLevelController.Instance.SkillPowerUpRatio;
                _specialPowerUpRatio = PlayerLevelController.Instance.SpecialPowerUpRatio;
            }
        }
        foreach (RectTransform textsRect in _textRects)
        {
            textsRect.localScale = Vector3.one;
        }
        if (_textRects[_nowSelect] != null) _textRects[_nowSelect].localScale = _textRects[_nowSelect].localScale * _upSizeScale;
    }

    /// <summary>
    /// HPのステータスを上げる
    /// </summary>
    public void HpUp()
    {
        if (_sp > 0)
        {
            _hp += PlayerLevelController.Instance.HPUpValue;
            _sp--;
        }
    }

    /// <summary>
    /// HPのステータスを下げる
    /// </summary>
    public void HpDown()
    {
        _sp++;
        if (_sp > _maxHaveSkillPoint)
        {
            _sp = _maxHaveSkillPoint;
            return;
        }

        var updatedHP = _hp - PlayerLevelController.Instance.HPUpValue;
        if (updatedHP >= _firstHp)
        {
            _hp -= PlayerLevelController.Instance.HPUpValue;
        }
        else
        {
            _sp--;
        }
    }

    /// <summary>
    /// 攻撃力のステータスを上げる
    /// </summary>
    public void AttackUp()
    {
        if (_sp > 0)
        {
            _attack += PlayerLevelController.Instance.AttackUpValue;
            _sp--;
        }
    }

    /// <summary>
    /// 攻撃力のステータスを下げる
    /// </summary>
    public void AttackDown()
    {
        _sp++;
        if (_sp > _maxHaveSkillPoint)
        {
            _sp = _maxHaveSkillPoint;
            return;
        }

        var updatedAttack = _attack - PlayerLevelController.Instance.AttackUpValue;
        if (updatedAttack >= _firstAttack)
        {
            _attack -= PlayerLevelController.Instance.AttackUpValue;
        }
        else
        {
            _sp--;
        }
    }

    /// <summary>
    /// 防御力のステータスを上げる
    /// </summary>
    public void DefenseUp()
    {
        if (_sp > 0)
        {
            _defense += PlayerLevelController.Instance.DefenseUpValue;
            _sp--;
        }
    }

    /// <summary>
    /// 防御力のステータスを下げる
    /// </summary>
    public void DefenseDown()
    {
        _sp++;
        if (_sp > _maxHaveSkillPoint)
        {
            _sp = _maxHaveSkillPoint;
            return;
        }

        var updatedDefense = _defense - PlayerLevelController.Instance.DefenseUpValue;
        if (updatedDefense >= _firstDefense)
        {
            _defense -= PlayerLevelController.Instance.DefenseUpValue;
        }
        else
        {
            _sp--;
        }
    }

    /// <summary>
    /// スキルの効果倍率のステータスを上げる
    /// </summary>
    public void SkillPowerUpRatioUp()
    {
        if (_sp > 0)
        {
            _skillPowerUpRatio += PlayerLevelController.Instance.SkillPowerUpValue;
            _sp--;
        }
    }

    /// <summary>
    /// スキルの効果倍率のステータスを下げる
    /// </summary>
    public void SkillPowerUpRatioDown()
    {
        _sp++;
        if (_sp > _maxHaveSkillPoint)
        {
            _sp = _maxHaveSkillPoint;
            return;
        }

        var updatedSkillPowerUpRatio =
            _skillPowerUpRatio - PlayerLevelController.Instance.SkillPowerUpValue;
        if (updatedSkillPowerUpRatio >= _firstSkillPowerUpRatio)
        {
            _skillPowerUpRatio -= PlayerLevelController.Instance.SkillPowerUpValue;
        }
        else
        {
            _sp--;
        }
    }

    /// <summary>
    /// 必殺技の効果倍率のステータスを上げる
    /// </summary>
    public void SpecialPowerUpRatioUp()
    {
        if (_sp > 0)
        {
            _specialPowerUpRatio += PlayerLevelController.Instance.SpecialPowerUpValue;
            _sp--;
        }
    }

    /// <summary>
    /// 必殺技の効果倍率のステータスを下げる
    /// </summary>
    public void SpecialPowerUpRatioDown()
    {
        _sp++;
        if (_sp > _maxHaveSkillPoint)
        {
            _sp = _maxHaveSkillPoint;
            return;
        }

        var updatedSpecialPowerUpRatio =
            _specialPowerUpRatio - PlayerLevelController.Instance.SpecialPowerUpValue;
        if (updatedSpecialPowerUpRatio >= _firstSpecialPowerUpRatio)
        {
            _specialPowerUpRatio -= PlayerLevelController.Instance.SpecialPowerUpValue;
        }
        else
        {
            _sp--;
        }
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
        switch (_nowSelect)
        {
            case (int)SkillTreeList.HP:
                HpUp();
                _spNum.text = _sp.ToString();
                _hpNum.text = _hp.ToString();
                break;

            case (int)SkillTreeList.Attack:
                AttackUp();
                _spNum.text = _sp.ToString();
                _attackNum.text = _attack.ToString();
                break;

            case (int)SkillTreeList.Defense:
                DefenseUp();
                _spNum.text = _sp.ToString();
                _defenseNum.text = _defense.ToString();
                break;

            case (int)SkillTreeList.SkillPower:
                SkillPowerUpRatioUp();
                _spNum.text = _sp.ToString();
                _skillPowerNum.text = _skillPowerUpRatio.ToString();
                break;

            case (int)SkillTreeList.SpecialPower:
                SpecialPowerUpRatioUp();
                _spNum.text = _sp.ToString();
                _specialPowerNum.text = _specialPowerUpRatio.ToString();
                break;
        }
    }

    /// <summary>
    /// 選択しているステータスを下げる
    /// </summary>
    public void StatusDown()
    {
        switch (_nowSelect)
        {
            case (int)SkillTreeList.HP:
                HpDown();
                _spNum.text = _sp.ToString();
                _hpNum.text = _hp.ToString();
                break;

            case (int)SkillTreeList.Attack:
                AttackDown();
                _spNum.text = _sp.ToString();
                _attackNum.text = _attack.ToString();
                break;

            case (int)SkillTreeList.Defense:
                DefenseDown();
                _spNum.text = _sp.ToString();
                _defenseNum.text = _defense.ToString();
                break;

            case (int)SkillTreeList.SkillPower:
                SkillPowerUpRatioDown();
                _spNum.text = _sp.ToString();
                _skillPowerNum.text = _skillPowerUpRatio.ToString();
                break;

            case (int)SkillTreeList.SpecialPower:
                SpecialPowerUpRatioDown();
                _spNum.text = _sp.ToString();
                _specialPowerNum.text = _specialPowerUpRatio.ToString();
                break;
        }
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
        switch (_nowSelect)
        {
            case (int)SkillTreeList.ResetButton:
                StatusReset();
                break;

            case (int)SkillTreeList.CloseButton:
                if (_tm != null) _tm.GoTop();
                break;
        }
    }
}
